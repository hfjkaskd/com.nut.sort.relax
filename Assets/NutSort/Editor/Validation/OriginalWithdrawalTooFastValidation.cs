using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalTooFastValidation
    {
        private sealed class View:IOriginalWithdrawalTooFastUI
        {
            public readonly List<string> Trace=new List<string>();
            public Action OnLevelTip,OnVideoFill,OnClose;
            public float Fill;public string LevelValue;
            public void SetLevelTip(int id,int level){Trace.Add("level:"+id+":"+level);OnLevelTip?.Invoke();}
            public void SetVideoTip(int id,int count){Trace.Add("video:"+id+":"+count);}
            public void SetLevelFill(float value){Fill=value;Trace.Add("fill");}
            public void SetVideoFill(float value){Check(value==1,"Literal full video fill");Trace.Add("videoFill");OnVideoFill?.Invoke();}
            public void SetLevelValue(string value){LevelValue=value;Trace.Add("value");}
            public void SetVideoValue(string value){Check(value=="10/10","Literal video count");Trace.Add("videoValue");}
            public void Close(){Trace.Add("close");OnClose?.Invoke();}
        }
        public static void Validate()
        {
            bool oldFlag=OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint;
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                var row=new JObject{{"Stage2StartShowLevel",12}};
                user.GoldRewardTargetS2CData=new JObject{{"bear_list",new JArray(new JObject(),new JObject(),row)}};
                // JArray keeps this unattached row as the same reference.
                var ui=new View();int level=7,reads=0;
                Action current=()=>ui.Trace.Add("oldStage");
                var flow=new OriginalWithdrawalTooFastFlow(user,ui,()=>{reads++;return level;},()=>{ui.Trace.Add("find23");return current;});
                Action click=null;flow.Init(()=>ui.Trace.Add("base"),a=>{ui.Trace.Add("bind");click=a;});
                Check(string.Join(",",ui.Trace)=="base,bind"&&click!=null,"Base before code-bound Sure");ui.Trace.Clear();
                flow.Refresh();
                Check(OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint&&reads==2&&ui.Fill==.5f&&ui.LevelValue=="6/12","Live level read separately for fill and text");
                Check(string.Join(",",ui.Trace)=="level:165:12,video:166:10,fill,videoFill,value,videoValue","Native UI mutation order");
                ui.OnLevelTip=()=>{row["Stage2StartShowLevel"]=20;user.GoldRewardTargetS2CData=new JObject();};
                ui.OnVideoFill=()=>{level=10;row["Stage2StartShowLevel"]=30;};
                flow.Refresh();Check(ui.Fill==.3f&&ui.LevelValue=="9/30","Captured row retained across replacement, fields reread after UI callbacks");
                ui.OnLevelTip=null;ui.OnVideoFill=null;
                user.GoldRewardTargetS2CData=new JObject{{"bear_list",new JArray(new JObject(),new JObject(),new JObject{{"Stage2StartShowLevel",0}})}};
                level=1;flow.Refresh();Check(float.IsNaN(ui.Fill),"Controller preserves source zero/zero; Image owns clamping");
                level=2;flow.Refresh();Check(float.IsPositiveInfinity(ui.Fill),"No invented divide-by-zero guard");
                level=int.MinValue;flow.Refresh();Check(ui.LevelValue=="2147483647/0","Native unchecked subtract");
                ui.Trace.Clear();ui.OnClose=()=>current=()=>ui.Trace.Add("liveStage");click();
                Check(string.Join(",",ui.Trace)=="close,find23,liveStage","Resolve live stage after close, resume immediately");
                ui.Trace.Clear();ui.OnClose=null;current=null;Expect<NullReferenceException>(flow.Sure);
                Check(string.Join(",",ui.Trace)=="close,find23","Missing stage fails after close, no implicit open");
                ui.Trace.Clear();ui.OnClose=()=>throw new InvalidOperationException();Expect<InvalidOperationException>(flow.Sure);
                Check(string.Join(",",ui.Trace)=="close","Close failure prevents lookup");
                OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint=false;user.GoldRewardTargetS2CData=null;
                Expect<NullReferenceException>(flow.Refresh);Check(OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint,"Flag written before missing-data failure");
                Debug.Log("NUT_WITHDRAWAL_TOO_FAST_VALIDATION_PASS source display order, captured reward row/live values, static flag, numeric boundaries, close-before-live-stage resume; controller only, no SDK or production host claim.");
            }
            finally{OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint=oldFlag;}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
