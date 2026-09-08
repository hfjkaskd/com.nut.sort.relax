using UnityEngine;
namespace NutSort.UI
{
    // Original five Get-marker PunchRotation tracks baked into native curves.
    // Curve segment times/values come from DOTween.Punch 0xA0EB1C; each segment
    // uses source OutQuad easing, with one-second forward/reverse (Yoyo) loops.
    public sealed class OriginalRecordPunchRotation : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private AnimationCurve rotation;
        [SerializeField] private float duration;
        private Vector3 initial;
        private float elapsed;
        private void Awake(){initial=target.localEulerAngles;OriginalUIAnimationDriver.Register(this,Advance);}
        private void OnDestroy(){OriginalUIAnimationDriver.Unregister(this);}
        public void Advance(float delta)
        {
            elapsed=Mathf.Repeat(elapsed+delta,duration*2f);
            float position=elapsed>duration?duration*2f-elapsed:elapsed;
            target.localEulerAngles=initial+Vector3.forward*rotation.Evaluate(position);
        }
    }
}
