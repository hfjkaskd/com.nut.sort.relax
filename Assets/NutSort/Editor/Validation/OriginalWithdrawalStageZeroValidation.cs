using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalStageZeroValidation
    {
        private sealed class UI:IOriginalWithdrawalStageZeroUI
        {
            public bool HasGold=>true;public int StepCount=>0;public string Description{get;set;}
            public readonly List<string> Trace=new List<string>();public int Service,TipLevel,DesId;public object[] DesArgs;
            public Action OnPlay,OnDescription,OnClose;
            public void SetGold(string value)=>Trace.Add("gold");
            public bool HasStepTime(int number)=>throw new InvalidOperationException();
            public void SetStepTime(int number,string value)=>throw new InvalidOperationException();
            public void Close(){Trace.Add("close");OnClose?.Invoke();}
            public void SetService(int id,int number){Check(id==44,"Service label");Service=number;Trace.Add("service");}
            public void SetDescription(int id,params object[] args){DesId=id;DesArgs=args;Description="line1\nline2\r\nline3";Trace.Add("description");OnDescription?.Invoke();}
            public void SetTip(int id,int level){Check(id==40,"Tip label");TipLevel=level;Trace.Add("tip");}
            public void PlayFirstProgress(){Trace.Add("play");OnPlay?.Invoke();}
            public void ShowPanel(int id,object[] args){Check(id==18&&args.Length==0,"Guide returns to withdrawal with empty args");Trace.Add("show18");}
        }
        public static void Validate()
        {
            ValidateQueue();
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.UserLssInfo=null;user.IsCompleteGuidePassStage2Level=false;
            user.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[{},{},{""Stage2StartShowLevel"":12}]}");
            var ui=new UI();int clocks=0,saves=0,plays=0;bool complete=false,failSave=false;OriginalWithdrawalStageZeroFlow flow=null;
            flow=new OriginalWithdrawalStageZeroFlow(user,ui,new object[]{"nonempty only"},()=>100+(++clocks),v=>"gold",(t,f)=>"time",(min,max)=>min,
                ()=>{Check(user.IsCompleteGuidePassStage2Level,"Completion query after guide flag");return complete;},()=>7,
                ()=>{saves++;ui.Trace.Add("save");if(failSave)throw new InvalidOperationException("fixture save");},v=>{Check(v==null,"Original null guide payload");ui.Trace.Add("guide");});
            ui.OnPlay=()=>{plays++;Check(!flow.IsGuide&&!user.IsCompleteGuidePassStage2Level&&user.UserLssInfo["GoldGetTime"]==null,"First animation occurs during base refresh before guide state and GoldGetTime assignment");};
            flow.Refresh();Check(flow.IsGuide&&clocks==3&&saves==2&&plays==1&&ui.Service==6000000&&ui.DesId==49&&(int)ui.DesArgs[0]==6&&(int)ui.DesArgs[1]==12&&ui.TipLevel==12,"First-entry controller values");
            Check((long)user.UserLssInfo["GoldGetTime"]==103&&(int)user.UserLssInfo["QueueCount"]==8000&&(int)user.UserLssInfo["PassRate"]==90&&string.Join(",",ui.Trace)=="gold,save,play,service,description,tip,save","Original base-save-animation then stage mutation/display/final-save sequence");
            ui.Trace.Clear();ui.OnPlay=null;complete=true;ui.OnDescription=()=>((JObject)((JArray)user.GoldRewardTargetS2CData["bear_list"])[2])["Stage2StartShowLevel"]=33;
            flow.Refresh();Check(clocks==3&&saves==3&&plays==1&&(int)user.UserLssInfo["QueueCount"]==7950&&ui.DesId==6&&ui.Description=="line1 line2\r line3"&&ui.TipLevel==33,"Repeat preserves times/service, advances queue, normalizes LF only and reads captured live row for tip");
            ui.Trace.Clear();flow.Sure();Check(string.Join(",",ui.Trace)=="close,show18","Guide Sure closes before showing withdrawal");
            flow.IsGuide=false;ui.Trace.Clear();flow.Sure();Check(string.Join(",",ui.Trace)=="close,guide","Non-guide Sure closes then calls guide null");
            ui.OnClose=()=>flow.IsGuide=true;ui.Trace.Clear();flow.Sure();Check(string.Join(",",ui.Trace)=="close,show18","Sure rereads IsGuide after Close");ui.OnClose=null;
            user.UserLssInfo["TX0IsOpened"]=false;ui.Trace.Clear();failSave=true;Expect(flow.RefreshSteps);Check((bool)user.UserLssInfo["TX0IsOpened"]&&string.Join(",",ui.Trace)=="save","First-open flag survives save failure; animation does not run");failSave=false;ui.Trace.Clear();flow.RefreshSteps();Check(ui.Trace.Count==0,"Repeated refresh does not retry first animation after flag set");
            var invalid=new OriginalWithdrawalStageZeroFlow(user,ui,null,()=>0,v=>"",(t,f)=>"",(a,b)=>a,()=>false,()=>1,()=>{},v=>{});
            bool threw=false;try{invalid.Refresh();}catch(NullReferenceException){threw=true;}Check(threw,"Null argument array fails after base refresh; no invented empty fallback");
            Debug.Log("NUT_WITHDRAWAL_STAGE_ZERO_VALIDATION_PASS original first-open/save/animation ordering, timestamps, queue and service initialization, guide flag, completed/incomplete text branches, live row reads and close/show/guide behavior; stage animations and prefab host remain pending, SDK event skipped.");
        }
        private static void ValidateQueue()
        {
            var account=new JObject();var calls=new List<string>();Func<int,int,int> random=(min,max)=>{calls.Add(min+":"+max);return min;};
            OriginalWithdrawalQueue.Refresh(account,random);Check((int)account["QueueCount"]==8000&&(int)account["PassRate"]==90&&string.Join(",",calls)=="8000:9000,90:96","Initial queue and pass-rate ranges");
            foreach(var test in new[]{new[]{150,100,1},new[]{101,90,2},new[]{100,90,1},new[]{20,10,1},new[]{11,5,2},new[]{10,10,0},new[]{1,1,0}})
            {account["QueueCount"]=test[0];calls.Clear();OriginalWithdrawalQueue.Refresh(account,random);Check((int)account["QueueCount"]==test[1]&&calls.Count==test[2]&&(int)account["PassRate"]==90,"Exact queue thresholds and unchanged pass rate");}
            account["QueueCount"]=-1;int n=0;Expect(()=>OriginalWithdrawalQueue.Refresh(account,(a,b)=>{if(++n==2)throw new InvalidOperationException();return 8500;}));Check((int)account["QueueCount"]==8500&&(int)account["PassRate"]==90,"Queue write precedes second random call failure");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect(Action action){try{action();}catch(InvalidOperationException){return;}throw new Exception("Expected failure");}
    }
}
