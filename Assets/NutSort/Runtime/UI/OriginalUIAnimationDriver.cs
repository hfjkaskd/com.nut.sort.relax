using System;
using System.Collections.Generic;
using UnityEngine;

namespace NutSort.UI
{
    // Scene-owned replacement for the source's global normal-update tween pump.
    // Target visibility and enabled state never determine whether a track runs.
    public sealed class OriginalUIAnimationDriver : MonoBehaviour
    {
        private struct Entry
        {
            public MonoBehaviour Owner;public Action<float> Step;
            public Action Callback;public float Remaining;public bool IgnoreTimeScale;
        }
        private static readonly List<Entry> entries = new List<Entry>(32);
        private static bool dispatching;
        private static OriginalUIAnimationDriver driver;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntime() { entries.Clear(); driver=null; dispatching=false; }

        private void Awake()
        {
            if(driver!=null && driver!=this)throw new InvalidOperationException("Multiple UI animation drivers in the scene.");
            driver=this;
        }
        private void OnDestroy() { if(driver==this)driver=null; }
        private void Update() { Advance(Time.deltaTime,Time.unscaledDeltaTime); }

        public static void Register(MonoBehaviour owner, Action<float> step)
        {
            if(owner==null)throw new ArgumentNullException(nameof(owner));
            if(step==null)throw new ArgumentNullException(nameof(step));
            for(int i=0;i<entries.Count;i++)if(entries[i].Owner==owner)return;
            entries.Add(new Entry { Owner=owner,Step=step });
        }
        // DOVirtual delayed calls belong to the global pump, not the panel that requested them.
        public static void Schedule(float duration,bool ignoreTimeScale,Action callback)
        {
            if(duration<0)throw new ArgumentOutOfRangeException(nameof(duration));
            if(callback==null)throw new ArgumentNullException(nameof(callback));
            entries.Add(new Entry{Callback=callback,Remaining=duration,IgnoreTimeScale=ignoreTimeScale});
        }
        public static void Unregister(MonoBehaviour owner)
        {
            for(int i=0;i<entries.Count;i++)
                if(entries[i].Callback==null&&ReferenceEquals(entries[i].Owner,owner))entries[i]=default;
            if(!dispatching)Compact();
        }
        public static void Advance(float delta)=>Advance(delta,delta);
        public static void Advance(float delta,float unscaledDelta)
        {
            if(delta<0||unscaledDelta<0)throw new ArgumentOutOfRangeException(nameof(delta));
            if(delta==0&&unscaledDelta==0)return;
            if(dispatching)throw new InvalidOperationException("Recursive UI animation update.");
            dispatching=true;
            try
            {
                // Snapshot the active range like TweenManager.Update. A callback
                // can remove a later owner or append work for the following frame.
                int count=entries.Count;
                for(int i=0;i<count;i++)
                {
                    Entry entry=entries[i];
                    if(entry.Callback!=null)
                    {
                        float elapsed=entry.IgnoreTimeScale?unscaledDelta:delta;
                        if(elapsed==0)continue;
                        entry.Remaining-=elapsed;
                        if(entry.Remaining>0){entries[i]=entry;continue;}
                        entries[i]=default;entry.Callback();
                    }
                    else if(entry.Owner!=null&&delta!=0)entry.Step(delta);
                }
            }
            finally { dispatching=false; Compact(); }
        }
        private static void Compact()
        {
            int alive=0;
            for(int i=0;i<entries.Count;i++)if(entries[i].Owner!=null||entries[i].Callback!=null)entries[alive++]=entries[i];
            if(alive<entries.Count)entries.RemoveRange(alive,entries.Count-alive);
        }
    }
}
