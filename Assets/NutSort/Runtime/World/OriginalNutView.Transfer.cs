using System;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed partial class OriginalNutView
    {
        private enum TransferPhase { None, Waiting, Lifting, Horizontal, Landing }
        private TransferPhase transferPhase;
        private Transform transferSlot;
        private Transform transferReady;
        private bool transferWasReady;
        private float transferWait, transferTime;
        private Vector3 transferStart, transferEnd;
        private Action transferComplete, transferSpark;
        public bool IsTransferring => transferPhase != TransferPhase.None;
        public NutSlot BoundSlot => slot;

        // Called after the model moves the same nut data into its destination
        // slot. Like NutInfo.Move, slot identity changes before the transform's
        // parent; source pose is retained until the staggered callback runs.
        public void PlayTransfer(NutSlot destination, Transform destinationSlot, Transform destinationReady,
            int moveIndex, bool wasReady, Action callback = null, Action requestSpark = null)
        {
            EnsureMotion();
            if (destination == null || destination.Nut == null || destinationSlot == null || destinationReady == null || moveIndex < 0)
                throw new ArgumentException("Original transfer references are incomplete.");
            slot = destination;
            transferSlot = destinationSlot;
            transferReady = destinationReady;
            transferWasReady = wasReady;
            transferComplete = callback;
            transferSpark = requestSpark;
            transferWait = moveIndex * motionSettings.TransferStagger;
            transferTime = 0f;
            transferPhase = TransferPhase.Waiting;
            // Original CancelReady resets the inner wobble pose but does not
            // kill an in-flight ready tween on the outer nut transform.
            CancelReadyPose();
        }

        private void AdvanceTransfer(float deltaTime, float unscaledDeltaTime, TransferPhase phaseAtFrameStart)
        {
            // A callback-created tween starts on the following update rather
            // than consuming an entire elapsed frame a second time.
            if (transferPhase != phaseAtFrameStart) return;
            if (transferPhase == TransferPhase.Waiting)
            {
                transferTime += unscaledDeltaTime;
                if (transferTime < transferWait) return;
                if (transferWasReady) BeginHorizontalTransfer();
                else
                {
                    transferPhase = TransferPhase.Lifting;
                    // The original NutInfo is already the destination's info,
                    // so Ready resolves the DESTINATION ready height here.
                    PlayReady(transferReady, BeginHorizontalTransfer);
                }
            }
            else if (transferPhase == TransferPhase.Horizontal)
            {
                transferTime += deltaTime;
                float t = Mathf.Clamp01(transferTime / motionSettings.TransferDuration);
                body.localPosition = Vector3.LerpUnclamped(transferStart, transferEnd, motionSettings.LinearEase.Evaluate(t));
                if (t < 1f) return;
                transferPhase = TransferPhase.Landing;
                PlayRevert(FinishTransfer, transferSpark);
            }
        }

        private void BeginHorizontalTransfer()
        {
            if (transferSlot == null) throw new InvalidOperationException("Original destination slot no longer exists.");
            body.SetParent(transferSlot, true);
            transferStart = body.localPosition;
            transferEnd = new Vector3(0f, transferStart.y, 0f);
            transferTime = 0f;
            transferPhase = TransferPhase.Horizontal;
        }

        private void FinishTransfer()
        {
            Action completed = transferComplete;
            CancelTransfer();
            completed?.Invoke();
        }

        private void CancelTransfer()
        {
            transferPhase = TransferPhase.None;
            transferSlot = transferReady = null;
            transferComplete = transferSpark = null;
        }
    }
}
