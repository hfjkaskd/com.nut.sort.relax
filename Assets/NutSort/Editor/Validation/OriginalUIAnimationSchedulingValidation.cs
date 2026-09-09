using System;
using System.Collections.Generic;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalUIAnimationSchedulingValidation
    {
        public static void Validate()
        {
            var root=new GameObject("scheduler owner fixture",typeof(RectTransform),typeof(Button));var owner=root.GetComponent<Button>();
            int steps=0,scaled=0,unscaled=0;var trace=new List<string>();
            try
            {
                OriginalUIAnimationDriver.Register(owner,d=>steps++);
                OriginalUIAnimationDriver.Schedule(2,false,()=>scaled++);OriginalUIAnimationDriver.Schedule(2,true,()=>unscaled++);
                OriginalUIAnimationDriver.Advance(0,1);Check(steps==0&&scaled==0&&unscaled==0,"Pause keeps scaled owners and timers still");
                OriginalUIAnimationDriver.Advance(0,1.1f);Check(unscaled==1&&scaled==0&&steps==0,"Unscaled deadline independent from pause");
                OriginalUIAnimationDriver.Advance(1,.1f);Check(scaled==0&&steps==1,"Scaled timer uses only scaled delta");
                OriginalUIAnimationDriver.Advance(1.1f,.1f);Check(scaled==1&&steps==2&&unscaled==1,"Both clocks invoke each timer once");
                OriginalUIAnimationDriver.Unregister(owner);
                bool scheduled=false;
                OriginalUIAnimationDriver.Register(owner,d=>{trace.Add("owner");if(!scheduled){scheduled=true;OriginalUIAnimationDriver.Schedule(0,true,()=>{trace.Add("first");OriginalUIAnimationDriver.Schedule(0,true,()=>trace.Add("second"));});}});
                OriginalUIAnimationDriver.Advance(10,10);Check(string.Join(",",trace)=="owner","New timer cannot execute inside creation frame even with large delta");
                OriginalUIAnimationDriver.Advance(0,.1f);Check(string.Join(",",trace)=="owner,first","First callback executes while owner paused; nested timer waits");
                OriginalUIAnimationDriver.Advance(0,.1f);Check(string.Join(",",trace)=="owner,first,second","Nested callback deferred to next dispatch");
                OriginalUIAnimationDriver.Schedule(.5f,true,()=>trace.Add("survived"));OriginalUIAnimationDriver.Unregister(owner);OriginalUIAnimationDriver.Unregister(null);
                UnityEngine.Object.DestroyImmediate(root);OriginalUIAnimationDriver.Advance(0,.6f);Check(trace[trace.Count-1]=="survived","Owner unregister/destruction cannot remove global timer");
                OriginalUIAnimationDriver.Schedule(0,true,()=>trace.Add("legacy"));OriginalUIAnimationDriver.Advance(0);Check(trace[trace.Count-1]=="survived","Legacy zero delta remains a no-op");OriginalUIAnimationDriver.Advance(.1f);Check(trace[trace.Count-1]=="legacy","Legacy deterministic overload advances both clocks");
            }
            finally{if(root!=null){OriginalUIAnimationDriver.Unregister(owner);UnityEngine.Object.DestroyImmediate(root);}}
            Debug.Log("NUT_UI_ANIMATION_SCHEDULING_VALIDATION_PASS independent scaled/unscaled deadlines, paused owners, single invocation, dispatch snapshots, nested creation deferral, destroyed-owner independence and legacy overload semantics.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
