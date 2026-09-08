using UnityEngine;

namespace NutSort.UI
{
    public sealed class OriginalLoopRotation : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float duration;
        [SerializeField] private Vector3 rotation;
        private Quaternion start;
        private float startedAt;
        private void Awake() { start = target.localRotation; startedAt = Time.time; OriginalUIAnimationDriver.Register(this,Advance); }
        private void OnDestroy() { OriginalUIAnimationDriver.Unregister(this); }
        public void Advance(float delta)
        {
            // Preserve the source scaled-clock phase while updating hidden targets too.
            float t = Mathf.Repeat(Time.time - startedAt, duration) / duration;
            target.localRotation = start * Quaternion.Euler(rotation * t);
        }
    }
}
