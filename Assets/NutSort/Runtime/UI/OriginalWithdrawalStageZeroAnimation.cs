using System;
using System.Collections.Generic;
using UnityEngine;
namespace NutSort.UI
{
    // TXProgress0Panel's two animation entry points. Structure/settings come from the owning prefab.
    public sealed class OriginalWithdrawalStageZeroAnimation
    {
        [Serializable]
        public sealed class Timing
        {
            public float RowDuration,ContentDuration,FirstDelay,FirstSpacing,FirstContentDelay,HintDelay;
            public float ResumeContentDelay,ThirdRowDelay,ThirdContentDelay,LastRowDelay,ButtonDuration;
        }
        private enum Kind{FirstRow,FirstContent,SecondRow,SecondContent,ThirdRow,ThirdContent,LastRow,Confirm}
        private sealed class Track
        {
            public Transform Target,Content,Done,Loading;public Kind Kind;
            public float Elapsed,Delay,Duration;public bool Started,Finished;public Vector3 Start;
        }
        private readonly Transform step,confirm;
        private readonly Timing timing;
        private readonly Action<float,bool,Action> delay;
        private readonly Action tooFast;
        private readonly List<Track> tracks=new List<Track>(12);
        public int ActiveTracks=>tracks.Count;
        public OriginalWithdrawalStageZeroAnimation(Transform step,Transform confirm,Timing timing,Action<float,bool,Action> delay,Action tooFast)
        {this.step=step;this.confirm=confirm;this.timing=timing;this.delay=delay;this.tooFast=tooFast;}
        public void PlayFirst()
        {
            confirm.localScale=Vector3.zero;
            for(int i=0;i<step.childCount;i++)step.GetChild(i).localScale=new Vector3(1,0,1);
            for(int number=1;number<=2;number++)
            {
                Transform row=step.Find(number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                var t=Capture(row);t.Content.localScale=new Vector3(1,0,1);ResetMarkers(t);
                t.Kind=number==1?Kind.FirstRow:Kind.SecondRow;t.Target=row;t.Delay=timing.FirstDelay+(number-1)*timing.FirstSpacing;t.Duration=timing.RowDuration;tracks.Add(t);
            }
        }
        public void PlayProgress()
        {
            for(int number=2;number<=4;number++)
            {
                Transform row=step.Find(number.ToString(System.Globalization.CultureInfo.InvariantCulture));var t=Capture(row);
                if(t.Content!=null)t.Content.localScale=new Vector3(1,0,1);ResetMarkers(t);
                if(number==2){t.Kind=Kind.SecondContent;t.Target=t.Content;t.Delay=timing.ResumeContentDelay;t.Duration=timing.ContentDuration;}
                else{t.Kind=number==3?Kind.ThirdRow:Kind.LastRow;t.Target=row;t.Delay=number==3?timing.ThirdRowDelay:timing.LastRowDelay;t.Duration=timing.RowDuration;}
                tracks.Add(t);
            }
        }
        private static Track Capture(Transform row)=>new Track{Content=row.Find("Content"),Done=row.Find("Done"),Loading=row.Find("Loading")};
        private static void ResetMarkers(Track t){if(t.Done!=null)t.Done.gameObject.SetActive(false);t.Loading.gameObject.SetActive(true);}
        private static void CompleteMarkers(Track t){if(t.Done!=null)t.Done.gameObject.SetActive(true);t.Loading.gameObject.SetActive(false);}
        public void Advance(float delta)
        {
            if(delta<0)throw new ArgumentOutOfRangeException(nameof(delta));if(delta==0)return;
            int count=tracks.Count;
            for(int i=0;i<count;i++)
            {
                var t=tracks[i];if(t.Target==null){t.Finished=true;continue;}
                t.Elapsed+=delta;if(t.Elapsed<t.Delay)continue;
                if(!t.Started)
                {
                    t.Started=true;t.Start=t.Target.localScale;
                    if(t.Kind==Kind.FirstContent||t.Kind==Kind.SecondContent)CompleteMarkers(t);
                    else if(t.Kind==Kind.ThirdContent&&t.Done!=null)t.Done.gameObject.SetActive(true);
                }
                float p=Mathf.Clamp01((t.Elapsed-t.Delay)/t.Duration),ease=p*(2-p);
                if(t.Kind==Kind.Confirm)t.Target.localScale=Vector3.LerpUnclamped(t.Start,Vector3.one,ease);
                else{var scale=t.Target.localScale;scale.y=Mathf.LerpUnclamped(t.Start.y,1,ease);t.Target.localScale=scale;}
                if(p<1)continue;t.Finished=true;
                if(t.Kind==Kind.FirstRow||t.Kind==Kind.ThirdRow)
                    tracks.Add(new Track{Kind=t.Kind==Kind.FirstRow?Kind.FirstContent:Kind.ThirdContent,Target=t.Content,Done=t.Done,Loading=t.Loading,Duration=timing.ContentDuration,Delay=t.Kind==Kind.FirstRow?timing.FirstContentDelay:timing.ThirdContentDelay});
                else if(t.Kind==Kind.SecondRow)delay(timing.HintDelay,true,tooFast);
                else if(t.Kind==Kind.LastRow)tracks.Add(new Track{Kind=Kind.Confirm,Target=confirm,Duration=timing.ButtonDuration});
            }
            int alive=0;for(int i=0;i<tracks.Count;i++)if(!tracks[i].Finished)tracks[alive++]=tracks[i];
            if(alive<tracks.Count)tracks.RemoveRange(alive,tracks.Count-alive);
        }
    }
}
