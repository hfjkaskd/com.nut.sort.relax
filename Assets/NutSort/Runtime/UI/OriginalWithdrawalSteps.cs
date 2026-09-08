using System;
using System.Collections.Generic;
using UnityEngine;
namespace NutSort.UI
{
    // TXLevelPanel.PlayStepTween: callback-created tracks enter the next normal update.
    public sealed class OriginalWithdrawalSteps:MonoBehaviour
    {
        [SerializeField] private Transform step,confirm;
        [SerializeField] private float firstDelay,rowSpacing,rowDuration,contentDelay,contentDuration,confirmDuration;
        private sealed class Track
        {
            public Transform Target,Content,Done,Loading;
            public int Kind,Index;
            public float Time,Delay,Duration;
            public bool Started,Finished;
            public Vector3 Start;
        }
        private readonly List<Track> tracks=new List<Track>(12);
        public int ActiveTracks=>tracks.Count;
        public Transform StepRoot=>step;
        public void Play()
        {
            confirm.localScale=Vector3.zero;
            for(int i=0;i<step.childCount;i++)
            {
                Transform row=step.Find((i+1).ToString(System.Globalization.CultureInfo.InvariantCulture));
                Transform content=row.Find("Content"),done=row.Find("Done"),loading=row.Find("Loading");
                row.localScale=new Vector3(1,0,1);content.localScale=new Vector3(1,0,1);
                if(done!=null)done.gameObject.SetActive(false);
                loading.gameObject.SetActive(true);
                tracks.Add(new Track{Target=row,Content=content,Done=done,Loading=loading,Index=i,Delay=firstDelay+i*rowSpacing,Duration=rowDuration});
            }
        }
        private void Awake()=>OriginalUIAnimationDriver.Register(this,Advance);
        private void OnDestroy()=>OriginalUIAnimationDriver.Unregister(this);
        public void Advance(float delta)
        {
            if(delta<0)throw new ArgumentOutOfRangeException(nameof(delta));
            if(delta==0)return;
            int count=tracks.Count;
            for(int i=0;i<count;i++)
            {
                Track t=tracks[i];
                if(t.Target==null){t.Finished=true;continue;}
                t.Time+=delta;if(t.Time<t.Delay)continue;
                if(!t.Started)
                {
                    t.Started=true;t.Start=t.Target.localScale;
                    if(t.Kind==1)
                    {
                        if(t.Done!=null){t.Done.gameObject.SetActive(true);t.Loading.gameObject.SetActive(false);}
                        if(t.Index==step.childCount-1)tracks.Add(new Track{Kind=2,Target=confirm,Duration=confirmDuration});
                    }
                }
                float p=Mathf.Clamp01((t.Time-t.Delay)/t.Duration),ease=p*(2-p);
                if(t.Kind==2)t.Target.localScale=Vector3.LerpUnclamped(t.Start,Vector3.one,ease);
                else{Vector3 scale=t.Target.localScale;scale.y=Mathf.LerpUnclamped(t.Start.y,1,ease);t.Target.localScale=scale;}
                if(p<1)continue;
                t.Finished=true;
                if(t.Kind==0)tracks.Add(new Track{Kind=1,Target=t.Content,Done=t.Done,Loading=t.Loading,Index=t.Index,Delay=contentDelay,Duration=contentDuration});
            }
            int alive=0;
            for(int i=0;i<tracks.Count;i++)if(!tracks[i].Finished)tracks[alive++]=tracks[i];
            if(alive<tracks.Count)tracks.RemoveRange(alive,tracks.Count-alive);
        }
    }
}
