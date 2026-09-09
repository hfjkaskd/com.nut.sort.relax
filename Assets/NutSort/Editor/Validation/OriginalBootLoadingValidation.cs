using System;
using System.Collections.Generic;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalBootLoadingValidation
    {
        public static void Validate()
        {
            var timing=new OriginalBootLoadingFlow.Timing{Rate=.5f,Cap=.9f,CompleteDuration=.5f,HideDelay=.2f};
            bool active=false;var values=new List<float>();var trace=new List<string>();var delayed=new List<Action>();bool failDisplay=false;
            var flow=new OriginalBootLoadingFlow(timing,()=>active,v=>{active=v;trace.Add(v?"on":"off");},v=>{values.Add(v);trace.Add("display");if(failDisplay)throw new InvalidOperationException();},
                (d,cb)=>{Check(d==.2f,"Source scaled hide delay");delayed.Add(cb);trace.Add("delay");});
            flow.Start(()=>trace.Add("resize"));Check(flow.Value==0&&!flow.IsShow&&string.Join(",",trace)=="resize","Start resets only value and resizes logo, no display or activation");
            trace.Clear();flow.SetState(true);Check(flow.IsShow&&active&&flow.Value==0&&string.Join(",",trace)=="display,on","Show flag/reset/display before active");
            flow.Update(1);Check(flow.Value==.5f,"Source progress rate");flow.Update(5);Check(flow.Value==.9f,"Cap while waiting");active=false;flow.Update(1);Check(flow.Value==.9f,"Inactive source Update does not advance");active=true;
            flow.SetState(false);Check(!flow.IsShow&&active&&delayed.Count==0,"Hide starts tween, not deactivation");flow.Update(1);Check(flow.Value==.9f,"Progress loop stops immediately when hide requested");
            flow.AdvanceTweens(.25f);Check(Mathf.Abs(flow.Value-.975f)<.00001f&&active,"Source default OutQuad midpoint");
            int before=values.Count;flow.AdvanceTweens(.25f);Check(values.Count==before+2&&flow.Value==1&&active&&delayed.Count==1,"Final tween value and explicit completion value before delayed hide");
            flow.SetState(true);delayed[0]();Check(!active&&flow.IsShow,"Reopen does not cancel old hide callback");
            delayed.Clear();flow.SetState(true);flow.SetState(false);flow.SetState(false);flow.AdvanceTweens(.5f);Check(delayed.Count==2,"Repeated false requests retain independent completions");
            foreach(var cb in delayed)cb();Check(!active,"Independent callbacks deactivate");
            failDisplay=true;trace.Clear();Expect<InvalidOperationException>(()=>flow.SetState(true));Check(flow.IsShow&&flow.Value==0&&!active&&string.Join(",",trace)=="display","Show mutation retained when display fails before activation");failDisplay=false;
            flow.SetState(true);flow.Update(1);flow.Start(()=>{});Check(flow.Value==0&&flow.IsShow&&active,"Late Start resets value without clearing state or repainting");
            delayed.Clear();flow.SetState(false);active=false;flow.AdvanceTweens(.5f);Check(delayed.Count==1&&flow.Value==1,"Global tween advances while panel inactive");
            Debug.Log("NUT_BOOT_LOADING_VALIDATION_PASS source show/reset/display order, 0.5-rate 0.9-cap progress, OutQuad completion, duplicate final display, 0.2 hide delay, repeated/reopened uncancelled tracks and failure order; actual source scene view pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
