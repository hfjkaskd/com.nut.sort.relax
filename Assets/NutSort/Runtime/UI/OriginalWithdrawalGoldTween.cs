using System;
using System.Collections.Generic;
using UnityEngine;
namespace NutSort.UI
{
    // Native DOVirtual.Float consumer used by the withdrawal pending-gold block.
    public sealed class OriginalWithdrawalGoldTween : MonoBehaviour
    {
        [SerializeField] private float duration;
        private sealed class Track
        {
            public float Start,End,Duration,Elapsed;
            public Action<float> Update;
        }
        private readonly List<Track> tracks=new List<Track>();
        public float Duration=>duration;
        public int ActiveCount=>tracks.Count;
        public void Play(float from,float to,float seconds,Action<float> update)
        {tracks.Add(new Track{Start=from,End=to,Duration=seconds,Update=update});}
        private void Awake(){OriginalUIAnimationDriver.Register(this,Advance);}
        private void OnDestroy(){OriginalUIAnimationDriver.Unregister(this);}
        public void Advance(float delta)
        {
            if(delta==0)return;
            int count=tracks.Count,index=0;
            for(int i=0;i<count;i++)
            {
                var track=tracks[index];track.Elapsed+=delta;
                float t=Mathf.Clamp01(track.Elapsed/track.Duration);
                float eased=1f-(1f-t)*(1f-t);
                track.Update(Mathf.LerpUnclamped(track.Start,track.End,eased));
                if(t>=1)tracks.RemoveAt(index);else index++;
            }
        }
    }
}
