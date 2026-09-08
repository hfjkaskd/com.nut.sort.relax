using UnityEngine;
namespace NutSort.UI
{
    // Source guide hand DOLocalRotate and arrow DOLocalMove: Restart, OutQuad.
    public sealed class OriginalGuideLoopMotion : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float duration;
        [SerializeField] private bool rotate;
        [SerializeField] private Vector3 end;
        private Vector3 initial,delta;
        private float elapsed;
        private void Awake()
        {
            initial=rotate?target.localEulerAngles:target.localPosition;
            delta=rotate?new Vector3(Mathf.DeltaAngle(initial.x,end.x),Mathf.DeltaAngle(initial.y,end.y),Mathf.DeltaAngle(initial.z,end.z)):end-initial;
            OriginalUIAnimationDriver.Register(this,Advance);
        }
        public void Advance(float time)
        {
            elapsed=Mathf.Repeat(elapsed+time,duration);
            float t=elapsed/duration;t=1f-(1f-t)*(1f-t);
            if(rotate)target.localEulerAngles=initial+delta*t;else target.localPosition=initial+delta*t;
        }
        private void OnDestroy(){OriginalUIAnimationDriver.Unregister(this);}
    }
}
