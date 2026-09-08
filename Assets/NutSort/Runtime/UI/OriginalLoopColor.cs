using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    // Source DOTweenAnimation Color, Linear, infinite Yoyo, normal update.
    public sealed class OriginalLoopColor : MonoBehaviour
    {
        [SerializeField] private Image target;
        [SerializeField] private float duration;
        [SerializeField] private Color endColor;
        private Color startColor;
        private double elapsed;
        private bool started;
        public Image Target => target;

        private void Awake() { OriginalUIAnimationDriver.Register(this, Advance); }
        private void OnDestroy() { OriginalUIAnimationDriver.Unregister(this); }

        public void Advance(float delta)
        {
            if (delta == 0f) return;
            // DOTween captures the current color when the tween first updates.
            if (!started) { startColor = target.color; started = true; }
            elapsed += delta;
            double cycle = elapsed % (duration * 2.0);
            float t = (float)(cycle <= duration ? cycle / duration : 2.0 - cycle / duration);
            target.color = Color.LerpUnclamped(startColor, endColor, t);
        }
    }
}
