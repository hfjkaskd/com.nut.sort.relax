using System;
using System.IO;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalNutMotionValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var world = Resources.Load<OriginalWorldSettings>("Configuration/OriginalWorld");
            var layout = Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            var board = new OriginalBoardState(repository.LoadBoard(false, "4b56d_1_1-1"), layout);
            var holder = new GameObject("Native nut motion validation");
            try
            {
                var pool = holder.AddComponent<OriginalPrefabPool>();
                var instance = pool.Rent(world.NutPath(11), holder.transform);
                var view = instance.GetComponent<OriginalNutView>();
                view.BindVisual(board.Screws[0].Slots[0], world, pool);
                Transform body = instance.transform;
                var ready = new GameObject("Ready position").transform;
                ready.SetParent(holder.transform, false);
                ready.position = new Vector3(10, 5, 10);
                body.localPosition = new Vector3(2, 1, 3);
                body.localRotation = Quaternion.Euler(0, 17, 0);
                int callbackCount = 0;
                view.PlayReady(ready, () => callbackCount++);
                view.AdvanceMotion(0.1f);
                Near(body.position, new Vector3(2, 4, 3), "Ready midpoint uses OutQuad world Y only");
                Check(Quaternion.Angle(body.localRotation, Quaternion.Euler(0, 128.5f, 0)) < 0.01f,
                    "Ready rotation is linear, negative local-axis additive, including snap remainder");
                Check(!view.IsWobbling && callbackCount == 0, "Ready callback waits for full duration");
                view.AdvanceMotion(0.1f);
                Check(view.IsWobbling && callbackCount == 1, "Ready completion enables wobble once");
                Near(body.position, new Vector3(2, 5, 3), "Ready finishes at reference height");
                view.AdvanceMotion(0.1f);
                Near(view.ColorRoot.localEulerAngles, new Vector3(1.5f, 0, 0.75f), "Original two-axis wobble rates");
                Near(view.ColorRoot.localPosition, new Vector3(0, 0.005f, 0), "Original vertical bob speed");

                int completionOrder = 0;
                view.PlayRevert(() => completionOrder = 1, () => { Check(completionOrder == 1, "Callback precedes spark request"); completionOrder = 2; });
                Check(!view.IsWobbling, "Revert cancels wobble immediately");
                Near(view.ColorRoot.localPosition, Vector3.zero, "CancelReady restores root position");
                view.AdvanceMotion(0.1f);
                Near(body.localPosition, new Vector3(2, 1.25f, 3), "Revert midpoint uses OutQuad local Y only");
                view.AdvanceMotion(0.1f);
                Near(body.localPosition, new Vector3(2, 0, 3), "Revert returns to slot plane");
                Check(completionOrder == 2, "Revert requests spark after completion");

                view.PlayReady(ready, () => callbackCount++);
                view.AdvanceMotion(0.05f);
                view.PlayRevert();
                view.AdvanceMotion(0.3f);
                Check(callbackCount == 1 && !view.IsWobbling, "Interrupting ready suppresses its obsolete callback");

                body.localPosition = Vector3.zero;
                body.localRotation = Quaternion.identity;
                ready.position = new Vector3(0, 4, 0);
                view.PlayEntry(board.Screws[0], ready, 1);
                Near(body.localScale, Vector3.zero, "Entry begins invisible");
                view.AdvanceMotion(0.25f);
                Near(body.localScale, Vector3.zero, "Entry slot staggering");
                view.AdvanceMotion(0.075f);
                Near(body.localScale, Vector3.one * 0.5f, "Entry scale midpoint");
                view.AdvanceMotion(0.075f);
                Near(body.localScale, Vector3.one, "Entry scale completion");
                view.AdvanceMotion(0.08f);
                Near(body.localPosition, new Vector3(0, 4, 0), "Entry fall delay");
                view.AdvanceMotion(0.25f);
                Near(body.localPosition, new Vector3(0, 2, 0), "Entry linear fall midpoint");
                Check(Quaternion.Angle(body.localRotation, Quaternion.Euler(0, 180, 0)) < 0.01f, "Entry spins during fall");
                // Cross the nominal endpoint rather than assuming decimal
                // phase durations have an exact binary floating-point sum.
                view.AdvanceMotion(0.251f);
                Near(body.localPosition, Vector3.zero, "Entry lands in slot coordinates");
                Check(!view.IsEntryAnimating, "Entry timeline completes");

                view.PlayReady(ready, () => callbackCount++);
                view.ReleaseVisual();
                view.AdvanceMotion(1f);
                Check(callbackCount == 1 && !view.IsSelectionAnimating && !view.IsWobbling, "Pool release cancels pending animation and callbacks");
            }
            finally { UnityEngine.Object.DestroyImmediate(holder); }
            Debug.Log("NUT_MOTION_VALIDATION_PASS ready/revert coordinates, direction/easing, delayed entry, wobble, interrupt callbacks and pooled release verified on native prefab transforms.");
        }

        private static void Near(Vector3 actual, Vector3 expected, string message)
        {
            Check((actual - expected).sqrMagnitude < 0.000001f, message + ": " + actual + " expected " + expected);
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }
    }
}
