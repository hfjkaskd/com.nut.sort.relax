using UnityEngine;

namespace NutSort.UI
{
    // Source PunchRotation: local Euler startup, per-segment OutQuad and
    // infinite Incremental loop. This closed path has zero loop displacement.
    public sealed class OriginalPunchRotation : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 punch;
        [SerializeField] private float duration;
        [SerializeField] private int vibrato;
        [SerializeField] private float elasticity;
        private OriginalPunchPath path;
        private Vector3 start;
        private float period;
        private double elapsed;
        public Transform Target => target;
        private void Awake() { OriginalUIAnimationDriver.Register(this,Advance); }
        private void OnDestroy() { OriginalUIAnimationDriver.Unregister(this); }
        public void Advance(float delta)
        {
            if(delta==0f)return;
            if(path==null)
            {
                path=new OriginalPunchPath(punch,duration,vibrato,elasticity);
                start=target.localEulerAngles;
                for(int i=0;i<path.Count;i++)period+=path.DurationAt(i);
            }
            elapsed+=delta;
            float position=(float)(elapsed%period);
            float preceding=0f;
            for(int i=0;i<path.Count;i++)
            {
                float length=path.DurationAt(i);
                float endTime=preceding+length;
                if(position<=endTime)
                {
                    float t=(position-preceding)/length;
                    float eased=-t*(t-2f);
                    Vector3 from=i==0?start:start+path.OffsetAt(i-1);
                    Vector3 to=start+path.OffsetAt(i);
                    target.localRotation=Quaternion.Euler(from+(to-from)*eased);
                    return;
                }
                preceding=endTime;
            }
        }
    }
}
