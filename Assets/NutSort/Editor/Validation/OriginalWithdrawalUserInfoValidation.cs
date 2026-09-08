using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalUserInfoValidation
    {
        private sealed class UI:IOriginalWithdrawalUserInfoUI
        {
            public string Name{get;set;}="";
            public string Email{get;set;}="";
            public string Number{get;set;}="";
            public string AreaNumber{get;set;}
            public int ChannelCount=>3;
            public readonly bool[] On=new bool[3],Visible=new bool[3];
            public readonly string[] Icons=new string[3];
            public readonly List<Action<bool>>[] Listeners={new List<Action<bool>>(),new List<Action<bool>>(),new List<Action<bool>>()};
            public readonly List<string> Trace=new List<string>();
            public bool EmailVisible,AreaVisible,NumberVisible,FailShow,FailClose;
            public int SetInfoCount,Panel,Tip;public object[] Arguments;
            public bool IsChannelOn(int i)=>On[i];
            public void SetChannelOn(int i,bool value){if(On[i]==value)return;On[i]=value;foreach(var cb in Listeners[i])cb(value);}
            public void SetChannelVisible(int i,bool value)=>Visible[i]=value;
            public void SetChannelIcon(int i,string channel)=>Icons[i]=channel;
            public void AddChannelListener(int i,Action<bool> callback)=>Listeners[i].Add(callback);
            public void SetEmailVisible(bool value){EmailVisible=value;SetInfoCount++;}
            public void SetAreaVisible(bool value)=>AreaVisible=value;
            public void SetNumberVisible(bool value)=>NumberVisible=value;
            public void ShowTip(int id){Tip=id;Trace.Add("tip");}
            public void ShowPanel(int id,object[] args){Panel=id;Arguments=args;Trace.Add("show");if(FailShow)throw new InvalidOperationException("fixture show");}
            public void Close(){Trace.Add("close");if(FailClose)throw new InvalidOperationException("fixture close");}
        }
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var user=new OriginalUserLocalData(defaults);var ui=new UI();int saves=0,callbacks=0,clears=0;
            var flow=new OriginalWithdrawalUserInfoFlow(user,tables.PayChannels,tables.ChannelInfos,()=>"US",()=>"1",ui,
                ()=>{saves++;ui.Trace.Add("save");},value=>{Check((int)value==1,"Captured level callback");callbacks++;ui.Trace.Add("guide");},()=>{clears++;ui.Trace.Add("clear");});
            flow.Init(new object[]{1},()=>ui.Trace.Add("base"),()=>ui.Trace.Add("get"),()=>ui.Trace.Add("closeBind"),()=>ui.Trace.Add("channel"));
            Check(string.Join(",",ui.Trace)=="base,get,closeBind,channel"&&flow.Level==1,"Native init/binding order");
            flow.Refresh();Check(flow.GetTypeIndex==0&&ui.On[0]&&ui.AreaNumber=="+1"&&ui.Icons[0]=="Venmo"&&ui.Icons[1]=="Zelle"&&ui.Icons[2]=="PayPal","Source US channel order and default index");
            Check(ReferenceEquals(ui.Listeners[0][0],ui.Listeners[2][0]),"One shared scan callback per refresh");
            var us=tables.PayChannels.GetPayChannel("US",null);
            foreach(var channel in us.Channels){var info=tables.ChannelInfos.GetChannelInfo(channel);Check(info!=null,"Real channel metadata exists");}
            var table=JObject.Parse(@"{""payChannels"":[{""country"":""US"",""channel"":[""Mail"",""Phone""]}],""channelInfos"":[{""channel"":""Mail"",""info"":[""email"",""name01""]},{""channel"":""Phone"",""info"":[""name02""]}]}");
            var channels=new OriginalPayChannels(table);var infos=new OriginalChannelInfos(table);
            var sample=new OriginalChannelInfo{Info=new List<string>{"Email","NAME01"}};Check(!sample.IsEmail&&!sample.IsName,"Source metadata is case-sensitive");
            sample.Info=null;Expect<NullReferenceException>(()=>{var unused=sample.IsEmail;});
            user.UserLssInfo=null;ui=new UI();saves=0;
            flow=new OriginalWithdrawalUserInfoFlow(user,channels,infos,()=>"US",()=>null,ui,()=>{saves++;ui.Trace.Add("save");},v=>{},()=>{});
            flow.Init(new object[]{1},()=>{},()=>{},()=>{},()=>{});flow.Refresh();
            Check(flow.IsName&&flow.IsEmail&&ui.EmailVisible&&!ui.NumberVisible&&!ui.AreaVisible&&!ui.Visible[2]&&ui.AreaNumber=="+","Email mode and null area concatenation");
            ui.Name="x";ui.Email="";flow.Get();Check(ui.Tip==118&&user.UserLssInfo==null&&saves==0,"Name error precedes account validation");
            ui.Name="ab";ui.Email="12345";flow.Get();Check(ui.Tip==120,"Email lower boundary");
            ui.Email="123456";ui.Number="retained";ui.Trace.Clear();flow.Get();
            Check(string.Join(",",ui.Trace)=="show,close,save"&&ui.Panel==21&&(int)ui.Arguments[0]==1&&saves==1,"Confirmation then close then save");
            Check((string)user.UserLssInfo["Email"]=="123456"&&(string)user.UserLssInfo["Number"]=="retained"&&(string)user.UserLssInfo["OtherInfo"]=="","No regex validation, retain hidden inputs, empty other info");
            user.UserLssInfo["IsInit"]=true;user.UserLssInfo["ApplicationTime"]=123L;var original=user.UserLssInfo;
            ui.Name="unsaved edit";ui.Email="unsaved edit";ui.SetChannelOn(0,false);
            Check(flow.GetTypeIndex==-1&&flow.IsEmail,"No selection retains previous requirements");ui.SetChannelOn(1,true);
            Check(flow.GetTypeIndex==1&&!flow.IsEmail&&flow.IsName&&ui.Name=="ab"&&ui.Email=="123456"&&ui.NumberVisible&&ui.AreaVisible,"Channel switch reloads saved fields and phone mode");
            ui.Number="1234";flow.Get();Check(ui.Tip==119,"Phone lower boundary");
            ui.Number="12345";ui.Trace.Clear();flow.Get();Check(ReferenceEquals(original,user.UserLssInfo)&&(long)original["ApplicationTime"]==123&&(bool)original["IsInit"]&&saves==2,"Update existing record in place without clearing unrelated state");
            ui.SetChannelOn(1,false);ui.Trace.Clear();flow.Get();Check(ui.Tip==122&&saves==2&&ui.Trace.Count==1,"Selection check follows valid input checks");
            ui.Number="1";flow.Get();Check(ui.Tip==119,"Account error precedes missing channel");
            ui.SetChannelOn(1,true);ui.Number="12345";int count=ui.Listeners[0].Count;flow.Refresh();
            Check(ui.Listeners[0].Count==count+1&&ui.Listeners[2].Count==0,"Repeated refresh adds listeners only for visible source channels");
            int before=ui.SetInfoCount;ui.On[0]=true;ui.On[1]=true;ui.Listeners[0][0](false);
            Check(flow.GetTypeIndex==1&&ui.SetInfoCount==before+2,"Selection callback ignores event value and scans every selected channel");
            ui.Trace.Clear();flow.ChannelInfo();Check(ui.Panel==22&&(int)ui.Arguments[0]==1&&ui.Trace.Count==1,"Channel help keeps current panel open");
            ui.FailShow=true;ui.Name="Changed";Expect<InvalidOperationException>(flow.Get);
            Check((string)original["Name"]=="Changed"&&saves==2,"Show failure retains mutated record but prevents close/save");
            ui.FailShow=false;ui.FailClose=true;Expect<InvalidOperationException>(flow.Get);Check(saves==2,"Close failure prevents save");ui.FailClose=false;
            ui.Trace.Clear();flow=new OriginalWithdrawalUserInfoFlow(user,channels,infos,()=>"US",()=>"1",ui,()=>{},v=>{ui.Trace.Add("guide");Check((int)v==1,"Close captured payload");},()=>ui.Trace.Add("clear"));
            flow.Init(new object[]{1},()=>{},()=>{},()=>{},()=>{});flow.Close();Check(string.Join(",",ui.Trace)=="close,guide,clear","Close precedes guide invocation and clears hint afterward");
            flow.Init(Array.Empty<object>(),()=>{},()=>{},()=>{},()=>{});Check(flow.Level==1,"Empty repeat init retains captured level");
            bool bound=false;Expect<InvalidCastException>(()=>flow.Init(new object[]{"1"},()=>{},()=>bound=true,()=>{},()=>{}));Check(!bound,"Invalid boxed argument fails before binding");
            Debug.Log("NUT_WITHDRAWAL_USER_INFO_VALIDATION_PASS native US channels, channel requirements, refresh/reentry, input error priorities, original length checks, in-place account mutation, confirmation-close-save order and channel-help/close branches; actual form prefab integration pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action) where T:Exception{try{action();}catch(T){return;}throw new InvalidOperationException("Expected "+typeof(T).Name);}
    }
}
