using System;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed partial class OriginalNutView
    {
        [SerializeField] private OriginalNutMotionSettings motionSettings;
        private Transform body;
        private bool entryActive;
        private bool fallStarted;
        private float entryTime;
        private float entryDelay;
        private Vector3 entryPosition;
        private Quaternion entryRotation;
        private bool selectionActive;
        private bool selectionLifting;
        private float selectionTime;
        private float selectionDuration;
        private float startY, endY, selectionAngle;
        private Quaternion selectionRotation;
        private Action selectionComplete;
        private Action sparkRequested;
        private bool wobbling, xPrimary;
        private float wobbleX, wobbleZ, bobY;
        private int xDirection, zDirection, yDirection;
        public bool IsWobbling => wobbling;
        public bool IsEntryAnimating => entryActive;
        public bool IsSelectionAnimating => selectionActive;

        private void Update() { AdvanceMotion(Time.deltaTime); }

        private void EnsureMotion()
        {
            if (motionSettings == null || Root == null) throw new InvalidOperationException("Nut motion prefab configuration missing.");
            if (body == null) body = transform;
        }

        public void PlayEntry(ScrewState screw, Transform initialPosition, int slotIndex)
        {
            EnsureMotion();
            if (screw == null || initialPosition == null || slotIndex < 0) throw new ArgumentException("Original entry references missing.");
            if (screw.IsColorMask || screw.IsHidden || screw.IsDone) return;
            entryTime = 0f;
            fallStarted = false;
            entryDelay = slotIndex * motionSettings.EntrySlotDelay;
            body.localScale = Vector3.zero;
            Vector3 position = body.position;
            position.y = initialPosition.position.y;
            body.position = position;
            entryPosition = body.localPosition;
            entryRotation = body.localRotation;
            entryActive = true;
        }

        public void PlayReady(Transform readyPosition, Action callback = null)
        {
            EnsureMotion();
            if (readyPosition == null) throw new ArgumentNullException(nameof(readyPosition));
            CancelReadyPose();
            selectionLifting = true;
            startY = body.position.y;
            endY = readyPosition.position.y;
            BeginSelection(-motionSettings.SelectionSpin, callback, null);
        }

        public void PlayRevert(Action callback = null, Action requestSpark = null)
        {
            EnsureMotion();
            CancelReadyPose();
            selectionLifting = false;
            startY = body.localPosition.y;
            endY = 0f;
            BeginSelection(motionSettings.SelectionSpin, callback, requestSpark);
        }

        private void BeginSelection(float spin, Action callback, Action requestSpark)
        {
            selectionActive = true;
            selectionTime = 0f;
            selectionDuration = motionSettings.SelectionDuration;
            selectionRotation = body.localRotation;
            selectionAngle = spin - body.localEulerAngles.y % motionSettings.RotationSnap;
            selectionComplete = callback;
            sparkRequested = requestSpark;
        }

        public void CancelReadyPose()
        {
            wobbling = false;
            Root.localEulerAngles = Vector3.zero;
            Root.localPosition = Vector3.zero;
        }

        public void StopMotion()
        {
            entryActive = selectionActive = false;
            selectionComplete = sparkRequested = null;
            if (Root != null) CancelReadyPose();
        }

        // The runtime and deterministic validation drive exactly the same state
        // transition code. No Editor-specific initialization or animation path.
        public void AdvanceMotion(float deltaTime)
        {
            if (!entryActive && !selectionActive && !wobbling) return;
            EnsureMotion();
            if (deltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            if (wobbling) AdvanceWobble(deltaTime);
            if (entryActive)
            {
                entryTime += deltaTime;
                float scaleTime = entryTime - entryDelay;
                float scaleT = Mathf.Clamp01(scaleTime / motionSettings.EntryScaleDuration);
                body.localScale = Vector3.one * motionSettings.LinearEase.Evaluate(scaleT);
                float fallTime = scaleTime - motionSettings.EntryScaleDuration - motionSettings.EntryFallDelay;
                if (fallTime >= 0f)
                {
                    if (!fallStarted)
                    {
                        entryPosition = body.localPosition;
                        entryRotation = body.localRotation;
                        fallStarted = true;
                    }
                    float t = Mathf.Clamp01(fallTime / motionSettings.EntryFallDuration);
                    float eased = motionSettings.LinearEase.Evaluate(t);
                    body.localPosition = Vector3.LerpUnclamped(entryPosition, Vector3.zero, eased);
                    body.localRotation = entryRotation * Quaternion.AngleAxis(motionSettings.EntrySpin * eased, Vector3.up);
                    if (t >= 1f) entryActive = false;
                }
            }
            if (!selectionActive) return;
            selectionTime += deltaTime;
            float selectionT = Mathf.Clamp01(selectionTime / selectionDuration);
            float y = Mathf.LerpUnclamped(startY, endY, motionSettings.SelectionEase.Evaluate(selectionT));
            Vector3 position = selectionLifting ? body.position : body.localPosition;
            position.y = y;
            if (selectionLifting) body.position = position;
            else body.localPosition = position;
            body.localRotation = selectionRotation * Quaternion.AngleAxis(
                selectionAngle * motionSettings.LinearEase.Evaluate(selectionT), Vector3.up);
            if (selectionT < 1f) return;
            selectionActive = false;
            Action completed = selectionComplete;
            Action spark = sparkRequested;
            selectionComplete = sparkRequested = null;
            if (selectionLifting)
            {
                wobbling = xPrimary = true;
                wobbleX = wobbleZ = bobY = 0f;
                xDirection = zDirection = yDirection = 1;
            }
            completed?.Invoke();
            spark?.Invoke();
        }

        private void AdvanceWobble(float deltaTime)
        {
            float step = deltaTime * motionSettings.WobbleSpeed;
            if (xPrimary)
            {
                wobbleX += step * xDirection;
                if (wobbleX > motionSettings.WobbleAngle) { xDirection = -1; xPrimary = false; }
                else if (wobbleX < -motionSettings.WobbleAngle) { xDirection = 1; xPrimary = false; }
                wobbleZ += step * motionSettings.SecondaryAxisSpeedRatio * zDirection;
            }
            else
            {
                wobbleZ += step * zDirection;
                if (wobbleZ > motionSettings.WobbleAngle) { zDirection = -1; xPrimary = true; }
                else if (wobbleZ < -motionSettings.WobbleAngle) { zDirection = 1; xPrimary = true; }
                wobbleX += step * motionSettings.SecondaryAxisSpeedRatio * xDirection;
            }
            Root.localEulerAngles = new Vector3(wobbleX, 0f, wobbleZ);
            bobY += deltaTime * motionSettings.BobSpeed * yDirection;
            if (bobY < motionSettings.BobMin || bobY > motionSettings.BobMax)
                yDirection = bobY > motionSettings.BobMax ? -1 : 1;
            Root.localPosition = new Vector3(0f, bobY, 0f);
        }
    }
}
