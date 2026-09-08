using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalLoopPosition : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private float duration;
        private Vector3 start;
        private float startedAt;
        private void Awake() { start=target.localPosition; startedAt=Time.time; OriginalUIAnimationDriver.Register(this,Advance); }
        private void OnDestroy() { OriginalUIAnimationDriver.Unregister(this); }
        public void Advance(float delta)
        {
            float t=Mathf.PingPong(Time.time-startedAt,duration)/duration;
            target.localPosition=Vector3.LerpUnclamped(start,endPosition,t);
        }
    }
}
