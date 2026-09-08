using System;
using System.Collections.Generic;
using UnityEngine;

namespace NutSort.UI
{
    // Scene-owned replacement for the source's global normal-update tween pump.
    // Target visibility and enabled state never determine whether a track runs.
    public sealed class OriginalUIAnimationDriver : MonoBehaviour
    {
        private struct Entry { public MonoBehaviour Owner; public Action<float> Step; }
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
        private void Update() { Advance(Time.deltaTime); }

        public static void Register(MonoBehaviour owner, Action<float> step)
        {
            if(owner==null)throw new ArgumentNullException(nameof(owner));
            if(step==null)throw new ArgumentNullException(nameof(step));
            for(int i=0;i<entries.Count;i++)if(entries[i].Owner==owner)return;
            entries.Add(new Entry { Owner=owner,Step=step });
        }
        public static void Unregister(MonoBehaviour owner)
        {
            for(int i=0;i<entries.Count;i++)
                if(ReferenceEquals(entries[i].Owner,owner))entries[i]=default;
            if(!dispatching)Compact();
        }
        public static void Advance(float delta)
        {
            if(delta<0)throw new ArgumentOutOfRangeException(nameof(delta));
            if(delta==0)return;
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
                    if(entry.Owner!=null)entry.Step(delta);
                }
            }
            finally { dispatching=false; Compact(); }
        }
        private static void Compact()
        {
            int alive=0;
            for(int i=0;i<entries.Count;i++)if(entries[i].Owner!=null)entries[alive++]=entries[i];
            if(alive<entries.Count)entries.RemoveRange(alive,entries.Count-alive);
        }
    }
}
