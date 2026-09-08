using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalChannelValidation
    {
        private sealed class UI:IOriginalWithdrawalChannelUI
        {
            public string Info{get;set;}
            public readonly List<string> Trace=new List<string>();public object[] Args;public bool FailShow,FailClose;
            public void ShowTip(int id){Check(id==120,"Source short-text tip");Trace.Add("tip");}
            public void ShowPanel(int id,object[] args){Check(id==21,"Source confirmation");Args=args;Trace.Add("show");if(FailShow)throw new InvalidOperationException();}
            public void Close(){Trace.Add("close");if(FailClose)throw new InvalidOperationException();}
            public void HidePanel(int id){Check(id==20,"Source account form hide");Trace.Add("hide");}
        }
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.UserLssInfo=null;
            var ui=new UI();var flow=new OriginalWithdrawalChannelFlow(user,ui);
            flow.Init(new object[]{7},()=>ui.Trace.Add("base"),()=>ui.Trace.Add("sure"),()=>ui.Trace.Add("closeBinding"));
            Check(flow.Level==7&&string.Join(",",ui.Trace)=="base,sure,closeBinding","Required captured argument and native binding order");
            flow.Refresh();Check(ui.Info==null,"Absent account supplies null to input setter");
            foreach(string value in new[]{"","a","1234"}){ui.Trace.Clear();ui.Info=value;flow.Sure();Check(user.UserLssInfo==null&&string.Join(",",ui.Trace)=="tip","Short input cannot mutate or close");}
            ui.Trace.Clear();ui.Info="     ";flow.Sure();Check((string)user.UserLssInfo["OtherInfo"]=="     "&&(int)ui.Args[0]==7&&string.Join(",",ui.Trace)=="show,close,hide","Five untrimmed characters accepted with exact display/close/hide sequence");
            var firstArgs=ui.Args;var account=user.UserLssInfo;account["Name"]="Keep";account["GetType"]=2;account["unknown"]=123;
            ui.Info="A\nB\nC";flow.Sure();Check(ReferenceEquals(account,user.UserLssInfo)&&!ReferenceEquals(firstArgs,ui.Args)&&(string)account["Name"]=="Keep"&&(int)account["GetType"]==2&&(int)account["unknown"]==123,"Update only OtherInfo in place and allocate fresh args");
            user.UserLssInfo=JObject.Parse(@"{""OtherInfo"":""first"",""OTHERINFO"":""last""}");flow.Refresh();Check(ui.Info=="last","Live account and existing JSON projection convention");ui.Info="new value";flow.Sure();Check((string)user.UserLssInfo["OtherInfo"]=="first"&&(string)user.UserLssInfo["OTHERINFO"]=="new value","Update last matching property preserving key case");
            ui.Trace.Clear();ui.FailShow=true;ui.Info="retained change";Expect<InvalidOperationException>(flow.Sure);Check((string)user.UserLssInfo["OTHERINFO"]==ui.Info&&string.Join(",",ui.Trace)=="show","Show failure retains mutation and prevents closing/hide");ui.FailShow=false;
            ui.Trace.Clear();ui.FailClose=true;Expect<InvalidOperationException>(flow.Sure);Check(string.Join(",",ui.Trace)=="show,close","Close failure prevents hiding account form");ui.FailClose=false;
            ui.Trace.Clear();flow.Close();Check(string.Join(",",ui.Trace)=="close","Close has no guide or hide side effect");
            ui.Trace.Clear();Expect<IndexOutOfRangeException>(()=>flow.Init(Array.Empty<object>(),()=>ui.Trace.Add("base"),()=>ui.Trace.Add("bind"),()=>{}));Check(string.Join(",",ui.Trace)=="base"&&flow.Level==7,"Required empty argument fails after base and before bindings");
            Expect<InvalidCastException>(()=>flow.Init(new object[]{"7"},()=>{},()=>{},()=>{}));
            ValidatePanel(user);
            Debug.Log("NUT_WITHDRAWAL_CHANNEL_VALIDATION_PASS required captured level, live OtherInfo, raw length boundary, in-place mutation, exact show/close/hide ordering and failures, no save/guide API, actual prefab input and code-bound Buttons.");
        }
        private static void ValidatePanel(OriginalUserLocalData user)
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));user.UserLssInfo=null;
            var p=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXChannelPanel")).GetComponent<OriginalWithdrawalChannelPanel>();
            bool allowed=false;int tips=0,sounds=0,shows=0,closes=0,hides=0;
            try
            {
                p.Bind(user,tables,"en",()=>allowed,s=>sounds++,id=>{Check(id==120,"Actual tip");tips++;},
                    (id,args)=>{Check(id==21&&(int)args[0]==1&&!p.Closing&&user.UserLssInfo!=null,"Actual Show before Close after mutation");shows++;},
                    id=>{Check(id==20&&p.Closing,"Actual Hide after close starts");hides++;},()=>closes++,()=>{},(d,cb)=>{},()=>{});
                p.Init(new object[]{1});p.Refresh();Check(p.Info==""&&p.Input.textComponent.font!=null,"Actual TMP normalizes null, source font");
                Check(p.GetComponentsInChildren<Transform>(true).Length==13,"Thirteen source objects");
                foreach(var c in p.GetComponentsInChildren<Component>(true))Check(c!=null,"No missing scripts");
                Check(p.GetComponentsInChildren<Button>(true).Length==2,"Two standard Buttons");foreach(var b in p.GetComponentsInChildren<Button>(true))Check(b.onClick.GetPersistentEventCount()==0&&b.GetComponent<OriginalButtonFeedback>()!=null,"Code events and prefab feedback");
                p.SureButton.onClick.Invoke();Check(tips==0&&sounds==0,"Actual gate");allowed=true;p.Info="1234";p.SureButton.onClick.Invoke();Check(tips==1&&!p.Closing&&sounds==1,"Actual short-input branch");
                p.Info="fixture channel\nfixture account";p.SureButton.onClick.Invoke();Check(shows==1&&hides==1&&closes==0&&sounds==2,"Actual success order before animation completes");p.Advance(.5f);Check(closes==1,"Animated close callback");
            }
            finally{UnityEngine.Object.DestroyImmediate(p.gameObject);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new InvalidOperationException("Expected "+typeof(T).Name);}
    }
}
