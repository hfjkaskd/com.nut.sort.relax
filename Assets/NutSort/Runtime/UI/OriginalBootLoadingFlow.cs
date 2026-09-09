using System;
using System.Collections.Generic;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalBootLoadingFlow
    {
        [Serializable] public sealed class Timing
        {public float Rate,Cap,CompleteDuration,HideDelay;}
        private sealed class Track{public float From,Elapsed;}
        private readonly Timing timing;
        private readonly Func<bool> active;
        private readonly Action<bool> setActive;
        private readonly Action<float> display;
        private readonly Action<float,Action> delay;
        private readonly List<Track> tracks=new List<Track>();
        public bool IsShow{get;private set;}
        public float Value{get;private set;}
        public OriginalBootLoadingFlow(Timing timing,Func<bool> active,Action<bool> setActive,Action<float> display,Action<float,Action> delay)
        {this.timing=timing;this.active=active;this.setActive=setActive;this.display=display;this.delay=delay;}
        public void Start(Action resizeLogo){Value=0;resizeLogo();}
        public void SetState(bool show)
        {
            IsShow=show;
            if(show){Value=0;display(Value);setActive(true);}
            else tracks.Add(new Track{From=Value});
        }
        // Source MonoBehaviour.Update. The owner invokes only while Unity runs its Update.
        public void Update(float delta)
        {
            if(!IsShow||!active())return;
            Value=Mathf.Min(Value+delta*timing.Rate,timing.Cap);display(Value);
        }
        // Source globally owned DOVirtual tracks continue independently of the panel's active state.
        public void AdvanceTweens(float delta)
        {
            if(delta==0)return;
            int count=tracks.Count,index=0;
            for(int i=0;i<count;i++)
            {
                var track=tracks[index];track.Elapsed+=delta;
                float t=Mathf.Clamp01(track.Elapsed/timing.CompleteDuration);
                Value=Mathf.LerpUnclamped(track.From,1,t*(2-t));display(Value);
                if(t>=1)
                {
                    tracks.RemoveAt(index);Value=1;display(Value);
                    delay(timing.HideDelay,()=>setActive(false));
                }
                else index++;
            }
        }
    }
}
