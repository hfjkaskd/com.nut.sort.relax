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
        private void Awake() { start = target.localRotation; startedAt = Time.time; }
        private void Update()
        {
            // Source tween continues while the view is hidden. The scaled clock
            // preserves that phase without updating an inactive hierarchy.
            float t = Mathf.Repeat(Time.time - startedAt, duration) / duration;
            target.localRotation = start * Quaternion.Euler(rotation * t);
        }
    }
}
