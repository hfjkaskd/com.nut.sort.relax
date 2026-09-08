using System;
using UnityEngine;

namespace NutSort.World
{
    public sealed partial class OriginalScrewView
    {
        private int donePhase;
        private float doneTime;
        private Vector3 doneStartScale;
        private float doneStartY;
        private Action doneComplete, doneEffectRequested;
        public bool IsDoneAnimating => donePhase != 0;
        private void Update() { AdvanceDone(Time.deltaTime); }

        public void PlayDone(Action callback, Action requestEffect = null)
        {
            CancelDone();
            doneComplete = callback;
            doneEffectRequested = requestEffect;
            doneStartScale = Cap.localScale;
            doneStartY = Cap.localPosition.y;
            donePhase = 1;
        }

        public void AdvanceDone(float deltaTime)
        {
            if (donePhase == 0) return;
            if (deltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            doneTime += deltaTime;
            float t = Mathf.Clamp01(doneTime / (donePhase == 1 ? settings.DoneRiseDuration : settings.DoneReturnDuration));
            // Exact OutCirc / InCirc used by original Done and its callbacks.
            // A sampled AnimationCurve would approximate the vertical tangent.
            float ease = donePhase == 1 ? Mathf.Sqrt(1f - (t - 1f) * (t - 1f)) : 1f - Mathf.Sqrt(1f - t * t);
            Cap.localScale = Vector3.LerpUnclamped(doneStartScale,
                Vector3.one * (donePhase == 1 ? settings.DonePeakScale : 1f), ease);
            Vector3 position = Cap.localPosition;
            position.y = Mathf.LerpUnclamped(doneStartY,
                settings.CapPosition.y + (donePhase == 1 ? settings.DoneLiftHeight : 0f), ease);
            Cap.localPosition = position;
            if (t < 1f) return;
            if (donePhase == 1)
            {
                donePhase = 2; doneTime = 0f;
                // The original Y completion refreshes the cap immediately.
                // Its newly scheduled scale tween initializes on the next frame,
                // after this refresh has reset the visible scale to one.
                RefreshCap();
                doneStartScale = Cap.localScale;
                doneStartY = Cap.localPosition.y;
                Action effect = doneEffectRequested;
                doneEffectRequested = null;
                effect?.Invoke();
            }
            else
            {
                Action completed = doneComplete;
                CancelDone();
                completed?.Invoke();
            }
        }

        private void CancelDone()
        {
            donePhase = 0; doneTime = 0f;
            doneComplete = doneEffectRequested = null;
        }
    }
}
