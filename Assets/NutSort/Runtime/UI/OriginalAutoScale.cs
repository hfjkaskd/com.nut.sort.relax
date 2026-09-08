using UnityEngine;
namespace NutSort.UI
{
    // Source prefab auto-play uniform Scale, one linear pass; no restart on visibility changes.
    public sealed class OriginalAutoScale:MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float delay,duration,end;
        private float elapsed;private Vector3 start;private bool started,finished;
        private void Awake()=>OriginalUIAnimationDriver.Register(this,Advance);
        private void OnDestroy()=>OriginalUIAnimationDriver.Unregister(this);
        public void Advance(float delta)
        {
            if(finished||delta==0)return;
            elapsed+=delta;if(elapsed<delay)return;
            if(!started){started=true;start=target.localScale;}
            float t=Mathf.Clamp01((elapsed-delay)/duration);
            target.localScale=Vector3.LerpUnclamped(start,Vector3.one*end,t);finished=t>=1;
        }
    }
}
