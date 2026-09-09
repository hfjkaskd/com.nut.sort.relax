using System;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalStageZeroAnimationValidation
    {
        public static OriginalWithdrawalStageZeroAnimation.Timing SourceTiming()=>new OriginalWithdrawalStageZeroAnimation.Timing
        {RowDuration=.1f,ContentDuration=.2f,FirstDelay=.3f,FirstSpacing=2,FirstContentDelay=1.5f,HintDelay=2,
         ResumeContentDelay=.5f,ThirdRowDelay=.6f+.3f,ThirdContentDelay=1,LastRowDelay=2*1.1f+.3f,ButtonDuration=.3f};
        public static void Validate()
        {
            // Synthetic UI hierarchy for timeline testing only; production structure must come from source prefab.
            var root=new GameObject("stage animation fixture",typeof(RectTransform));
            var step=new GameObject("Step",typeof(RectTransform)).transform;step.SetParent(root.transform,false);
            var confirm=new GameObject("Sure",typeof(RectTransform),typeof(Button)).transform;confirm.SetParent(root.transform,false);
            for(int number=1;number<=5;number++)
            {
                var row=new GameObject(number.ToString(),typeof(RectTransform)).transform;row.SetParent(step,false);
                foreach(string name in new[]{"Content","Done","Loading"})new GameObject(name,typeof(RectTransform)).transform.SetParent(row,false);
            }
            Action pending=null;int schedules=0,hints=0;
            var animation=new OriginalWithdrawalStageZeroAnimation(step,confirm,SourceTiming(),(seconds,unscaled,callback)=>
                {Check(seconds==2&&unscaled,"Original delayed call ignores time scale");schedules++;pending=callback;},()=>hints++);
            try
            {
                animation.PlayFirst();Check(confirm.localScale==Vector3.zero&&animation.ActiveTracks==2,"Only first two rows scheduled; Sure hidden by scale");
                for(int i=0;i<step.childCount;i++)Check(step.GetChild(i).localScale==new Vector3(1,0,1),"Every existing row initially collapsed");
                Check(step.Find("3/Done").gameObject.activeSelf,"First phase only resets markers of rows one and two");
                animation.Advance(.35f);Near(step.Find("1").localScale.y,.75f,"First row source delay and OutQuad");
                animation.Advance(.051f);Near(step.Find("1/Content").localScale.y,0,"New content track does not consume creation-frame remainder");
                animation.Advance(1.55f);Near(step.Find("1/Content").localScale.y,.4375f,"First content .2 duration after 1.5 delay");Check(step.Find("1/Done").gameObject.activeSelf&&!step.Find("1/Loading").gameObject.activeSelf,"First content OnStart switches markers");
                animation.Advance(.5f);Check(schedules==1&&hints==0&&step.Find("2/Content").localScale.y==0&&!step.Find("2/Done").gameObject.activeSelf,"Second row completion schedules prompt but does not expand content");
                pending();Check(hints==1&&confirm.localScale==Vector3.zero,"Prompt callback does not automatically resume animation");
                animation.PlayProgress();animation.Advance(.6f);Near(step.Find("2/Content").localScale.y,.75f,"Resume second content .5 delay and .2 duration");Check(step.Find("2/Done").gameObject.activeSelf&&!step.Find("2/Loading").gameObject.activeSelf,"Second content switches markers on start");
                animation.Advance(.401f);Near(step.Find("3").localScale.y,1,"Third row .9 delay and .1 duration");Near(step.Find("3/Content").localScale.y,0,"Third content starts next update");
                animation.Advance(1.05f);Near(step.Find("3/Content").localScale.y,.4375f,"Third content one-second delay");Check(step.Find("3/Done").gameObject.activeSelf&&step.Find("3/Loading").gameObject.activeSelf,"Original third callback leaves Loading active");
                animation.Advance(.6f);Check(confirm.localScale==Vector3.zero&&step.Find("4").localScale.y==1,"Fourth row completion defers Sure track");
                animation.Advance(.15f);Near(confirm.localScale.x,.75f,"Sure uniform OutQuad .3 duration");animation.Advance(.16f);Check(animation.ActiveTracks==0&&step.Find("5").localScale.y==0,"No implicit animation of extra rows");
                animation.PlayFirst();animation.PlayFirst();Check(animation.ActiveTracks==4,"Repeated first animation preserves independent tracks");
                animation.Advance(0);Check(animation.ActiveTracks==4,"Paused scaled animation does not advance");
                animation.Advance(100);Check(step.Find("1/Content").localScale.y==0,"Large delta cannot consume callback-created content");
                animation.Advance(2);UnityEngine.Object.DestroyImmediate(step.Find("1/Done").gameObject);
                animation.PlayFirst();animation.Advance(3);animation.Advance(1.55f);Check(!step.Find("1/Loading").gameObject.activeSelf,"Missing Done still hides Loading in this source callback");
                int before=hints;UnityEngine.Object.DestroyImmediate(root);pending();Check(hints==before+1,"Global prompt callback remains independent of destroyed panel targets");
            }
            finally{if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_WITHDRAWAL_STAGE_ZERO_ANIMATION_VALIDATION_PASS native two-phase row/content/Sure timing, deferred callback-created tracks, unscaled prompt scheduling contract, repeated calls, extra/missing nodes and distinct third-step marker behavior; scheduler provider and source prefab host remain pending.");
        }
        private static void Near(float actual,float expected,string text)=>Check(Mathf.Abs(actual-expected)<.002f,text+" actual="+actual);
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
