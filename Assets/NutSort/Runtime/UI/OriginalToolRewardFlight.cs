using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    // UILSSUtil.FlyItem (9BF100), completion (9C06A4). Cash/coin use separate flows.
    public sealed class OriginalToolRewardFlight : MonoBehaviour
    {
        [SerializeField] private float delay,duration;
        private sealed class Track
        {
            public Transform Clone;
            public Vector3 Start,End,StartScale,EndScale;
            public float Elapsed;
            public int Type;
            public bool Started;
        }
        private readonly List<Track> tracks=new List<Track>();
        private Transform topCanvas;
        private Func<OriginalMainBottomView> bottom;
        private Action<float,Action> schedule;
        public int ActiveCount=>tracks.Count;
        public void Bind(Transform topCanvas,Func<OriginalMainBottomView> bottom,Action<float,Action> schedule)
        {this.topCanvas=topCanvas;this.bottom=bottom;this.schedule=schedule;}
        public void Fly(int type,Image source)=>Fly(type,source,null);
        public void Fly(int type,Image source,Action callback)
        {
            var target=bottom().GetOtherItem(type);
            if(target==null)return;
            var clone=Instantiate(source,topCanvas,false);
            clone.transform.position=source.transform.position;
            var icon=target.Display.Icon.transform;
            tracks.Add(new Track{Clone=clone.transform,End=icon.position,EndScale=icon.localScale,Type=type});
            // Native user callback is timed independently of animation completion.
            schedule(delay,callback);
        }
        private void Awake(){OriginalUIAnimationDriver.Register(this,Advance);}
        private void OnDestroy()
        {
            OriginalUIAnimationDriver.Unregister(this);
            foreach(var track in tracks)if(track.Clone!=null)Destroy(track.Clone.gameObject);
            tracks.Clear();
        }
        public void Advance(float delta)
        {
            if(delta<=0)return;
            for(int i=0;i<tracks.Count;)
            {
                var track=tracks[i];track.Elapsed+=delta;
                if(track.Elapsed<=delay){i++;continue;}
                if(track.Clone==null){tracks.RemoveAt(i);continue;}
                if(!track.Started)
                {
                    track.Started=true;track.Start=track.Clone.position;track.StartScale=track.Clone.localScale;
                }
                float t=Mathf.Clamp01((track.Elapsed-delay)/duration);
                float eased=1-(1-t)*(1-t); // Exported DOTweenSettings defaultEaseType 6: OutQuad.
                track.Clone.position=Vector3.LerpUnclamped(track.Start,track.End,eased);
                track.Clone.localScale=Vector3.LerpUnclamped(track.StartScale,track.EndScale,eased);
                if(t<1){i++;continue;}
                Destroy(track.Clone.gameObject);tracks.RemoveAt(i);
                bottom().RefreshOtherItem(track.Type);
            }
        }
    }
}
