using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGoldRewardInfoFlowValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){GoldRewardTargetS2CData=new JObject{{"fixture",1}}};
            var trace=new List<string>();var pending=new List<Action<OriginalGoldRewardInfoResponse>>();var flags=new List<bool>();var retries=new List<Action>();
            var incoming=new OriginalGoldRewardInfoResponse{kinetic_gap="NO-0",message_status="incoming",Time=9,kinetic_data=new JObject{{"incoming",true}}};OriginalGoldRewardInfoResponse received=null;bool main=true;int reads=0;
            var flow=new OriginalGoldRewardInfoFlow(()=>{reads++;return user;},(flag,done)=>{flags.Add(flag);pending.Add(done);trace.Add("request");},
                (id,retry,close)=>{Check(id==75&&!close,"Source message args");retries.Add(retry);trace.Add("message");},()=>{trace.Add("main");return main;},()=>trace.Add("hint"));
            flow.GoldRewardInfo(value=>{received=value;trace.Add("callback");},true);pending[0](null);
            Check(string.Join(",",trace)=="request,message"&&reads==0&&received==null,"Forced null failure holds callback and hint");pending[0](new OriginalGoldRewardInfoResponse{kinetic_gap="NO-1"});Check(ReferenceEquals(retries[0],retries[1]),"Failure closure caches retry action");
            retries[0]();Check(flags.Count==2&&flags[0]&&flags[1],"Retry preserves force flag");pending[1](incoming);
            Check(!ReferenceEquals(received,incoming)&&ReferenceEquals(received.kinetic_data,user.GoldRewardTargetS2CData)&&received.message_status==null&&received.kinetic_gap==null&&received.Time==0&&!received.IsSuccess,"Fresh callback envelope holds live user data alias, no copied status/time");
            Check(trace[trace.Count-3]=="callback"&&trace[trace.Count-2]=="main"&&trace[trace.Count-1]=="hint","Callback before main lookup and hint");received.kinetic_data["fixture"]=2;Check((int)user.GoldRewardTargetS2CData["fixture"]==2,"No cloned reward document");
            trace.Clear();flow.GoldRewardInfo(value=>{Check(ReferenceEquals(value.kinetic_data,user.GoldRewardTargetS2CData),"Cached user data on failure");trace.Add("callback");},false);pending[2](null);Check(string.Join(",",trace)=="request,callback,main,hint"&&!flags[2],"Nonforced empty failure still calls back and refreshes");
            trace.Clear();flow.GoldRewardInfo(value=>{main=false;trace.Add("callback");},true);pending[3](incoming);Check(string.Join(",",trace)=="request,callback,main","Callback can remove main before existence check");
            int before=reads;main=true;flow.GoldRewardInfo(null,false);pending[4](incoming);Check(reads==before&&trace[trace.Count-1]=="hint","Null callback skips user read but still refreshes hint");
            flow.GoldRewardInfo(value=>throw new InvalidOperationException(),false);trace.Clear();Expect<InvalidOperationException>(()=>pending[5](incoming));Check(trace.Count==0,"Callback exception prevents main lookup and refresh");
            user.GoldRewardTargetS2CData=null;flow.GoldRewardInfo(value=>Check(value!=null&&value.kinetic_data==null,"Null user data stays null in new envelope"),false);pending[6](null);
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel")).GetComponent<OriginalMessagePanel>();
            try
            {
                panel.Bind(id=>"localized "+id);panel.Init();panel.gameObject.SetActive(false);pending.Clear();int callbacks=0;
                flow=new OriginalGoldRewardInfoFlow(()=>user,(flag,done)=>{Check(flag&&!panel.gameObject.activeSelf,"Actual message hides before forced retry request");pending.Add(done);},panel.Show,()=>false,()=>throw new Exception());
                flow.GoldRewardInfo(value=>callbacks++,true);pending[0](null);Check(panel.gameObject.activeSelf&&callbacks==0,"Actual forced failure message");panel.ConfirmButton.onClick.Invoke();pending[1](incoming);Check(callbacks==1&&!panel.gameObject.activeSelf,"Standard confirmation retries and releases original callback");
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);}
            Debug.Log("NUT_GOLD_REWARD_INFO_FLOW_VALIDATION_PASS force/nonforce failures, original MessagePanel retry with captured flag, new callback envelope sharing live user reward document, default headers, callback-before-main-hint ordering and exception/null semantics; lower response Init remains a separate port.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected exception");}
    }
}
