using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalPanelFlowValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var previous=OriginalNewbieGuideView.CallbackAction;var trace=new List<string>();bool online=false,target=false;
            var flow=new OriginalWithdrawalPanelFlow(user,()=>{trace.Add("online");return online;},v=>{online=v;trace.Add("clear");},()=>{trace.Add("target");return target;},id=>trace.Add("panel:"+id),()=>trace.Add("close"));
            try
            {
                user.Level=2;flow.Init(Array.Empty<object>(),()=>{trace.Add("base");user.Level=3;},()=>trace.Add("withdraw"),()=>trace.Add("bindClose"));
                Equal(trace,"base,withdraw,bindClose");Check(flow.Level==3,"No argument reads user after base initialization");
                flow.Init(new object[]{1,999},()=>{},()=>{},()=>{});user.Level=10;Check(flow.Level==1,"First integer argument captured; extra arguments ignored");
                foreach(object[] input in new object[][]{null,new object[]{null},new object[]{1L},new object[]{"1"}})
                {
                    trace.Clear();bool failed=false;try{flow.Init(input,()=>trace.Add("base"),()=>trace.Add("withdraw"),()=>trace.Add("bindClose"));}catch(Exception e)when(e is NullReferenceException||e is InvalidCastException){failed=true;}
                    Check(failed&&flow.Level==1&&string.Join(",",trace)=="base","Native invalid argument fails after base but before level/bind mutation");
                }
                foreach(int index in new[]{int.MinValue,-1,0,1,2,3,10,11,12,13,14,15,16,int.MaxValue})
                {
                    user.GuideIndex=index;trace.Clear();flow.TweenEnd();
                    Equal(trace,index==2||index==11||index==13||index==15?"panel:7":"");
                    Check(user.GuideIndex==index,"Tween end does not change guide index");
                }
                foreach(bool a in new[]{false,true})foreach(bool b in new[]{false,true})
                {
                    online=a;target=b;trace.Clear();OriginalNewbieGuideView.CallbackAction=value=>{Check(value==null,"Native null callback argument");trace.Add("callback");};
                    flow.CloseCallback();Equal(trace,a?"online,clear,panel:30,close":b?"online,target,panel:31,close":"online,target,callback,close");
                    Check(!online,"Online flag cleared only when active");Check((OriginalNewbieGuideView.CallbackAction==null)==(!a&&!b),"Hint branches retain global callback");
                }
                online=false;target=false;trace.Clear();OriginalNewbieGuideView.CallbackAction=null;flow.CloseCallback();Equal(trace,"online,target,close");
                Action<object> failing=value=>throw new InvalidOperationException("callback");OriginalNewbieGuideView.CallbackAction=failing;trace.Clear();bool failure=false;
                try{flow.CloseCallback();}catch(InvalidOperationException){failure=true;}
                Check(failure&&OriginalNewbieGuideView.CallbackAction==failing&&string.Join(",",trace)=="online,target","Callback failure prevents close and retains global registration");
                online=true;bool closed=false;var failingShow=new OriginalWithdrawalPanelFlow(user,()=>online,v=>online=v,()=>false,id=>throw new InvalidOperationException("panel"),()=>closed=true);
                try{failingShow.CloseCallback();}catch(InvalidOperationException){}
                Check(!online&&!closed,"Hint flag reset survives panel dispatch failure; close does not run");
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;}
            Debug.Log("NUT_WITHDRAWAL_PANEL_FLOW_VALIDATION_PASS native argument capture/binding order, exact live guide indices, online/target/global callback priority, clear-before-show and failure boundaries; TXPanel prefab/refresh/withdrawal branch pending.");
        }
        private static void Equal(List<string> trace,string expected){Check(string.Join(",",trace)==expected,"Expected "+expected+" got "+string.Join(",",trace));}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
