using System;
using System.IO;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalTransferValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var layout = Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            var world = Resources.Load<OriginalWorldSettings>("Configuration/OriginalWorld");
            var holder = new GameObject("Original transfer validation");
            try
            {
                var pool = holder.AddComponent<OriginalPrefabPool>();
                Transform sourceTile = Point(holder.transform, new Vector3(0, 0, 0));
                Transform targetTile = Point(holder.transform, new Vector3(4, 1.08f, 0));
                Transform ready = Point(holder.transform, new Vector3(0, 5, 0));
                Transform targetReady = Point(holder.transform, new Vector3(4, 8, 0));
                var board = new OriginalBoardState(repository.LoadBoard(false, "4b56d_1_1-1"), layout);
                var operation = new OriginalScrewOperator();
                ScrewState source = board.Screws[1], target = board.Screws[0];
                var view = pool.Rent(world.NutPath(11), sourceTile).GetComponent<OriginalNutView>();
                view.transform.localPosition = Vector3.zero;
                view.BindVisual(source.Slots[0], world, pool);
                operation.Operate(source, null);
                view.PlayReady(ready);
                view.AdvanceMotion(0.2f);
                NutMoveBatch batch = operation.Operate(target, source).Move;
                NutTransfer transfer = batch.Transfers[0];
                int complete = 0, spark = 0;
                view.PlayTransfer(transfer.Destination, targetTile, targetReady, 0, transfer.WasReady,
                    () => { batch.CompleteMovement(0); complete++; }, () => { Check(complete == 1, "Landing completion precedes spark"); spark++; });
                Check(view.BoundSlot == transfer.Destination && view.transform.parent == sourceTile, "Model slot changes before visual parent");
                Vector3 before = view.transform.position;
                view.AdvanceMotion(0f, 0f);
                Check(view.transform.parent == targetTile, "Zero-delay ready transfer reparents at scheduled update");
                Near(view.transform.position, before, "Reparent preserves world pose");
                view.AdvanceMotion(0.1f);
                Near(view.transform.position, new Vector3(2, 5, 0), "Ready transfer skips extra lift and moves horizontally");
                Check(!target.IsCanOperator && complete == 0, "Animation gate remains closed in transit");
                view.AdvanceMotion(0.1f);
                Near(view.transform.position, new Vector3(4, 5, 0), "Transfer reaches destination axis before descent");
                view.AdvanceMotion(0.1f);
                Near(view.transform.localPosition, new Vector3(0, 0.98f, 0), "Landing reuses original eased revert motion");
                view.AdvanceMotion(0.1f);
                Near(view.transform.localPosition, Vector3.zero, "Transfer lands in destination slot");
                Check(complete == 1 && spark == 1 && !view.IsTransferring, "Landing callback and spark each run once");
                Check(batch.RequiresDoneAnimation && !target.IsCanOperator && board.IsSuccess, "Winning move waits for complete-screw animation");
                batch.CompleteDoneAnimation();
                Check(target.IsCanOperator, "Only complete-screw callback releases winning target");
                view.ReleaseVisual();
                pool.Return(view.gameObject);

                // Real first board moved in the other direction produces three
                // transfers. The second nut is unready and has a stagger delay.
                board = new OriginalBoardState(repository.LoadBoard(false, "4b56d_1_1-1"), layout);
                source = board.Screws[0]; target = board.Screws[1];
                view = pool.Rent(world.NutPath(11), sourceTile).GetComponent<OriginalNutView>();
                view.transform.localPosition = Vector3.zero;
                view.BindVisual(source.Slots[1], world, pool);
                operation.Operate(source, null);
                batch = operation.Operate(target, source).Move;
                Check(batch.Transfers.Length == 3 && !batch.Transfers[1].WasReady, "Original group retains per-nut readiness");
                transfer = batch.Transfers[1];
                complete = 0;
                view.PlayTransfer(transfer.Destination, targetTile, targetReady, 1, false, () => complete++);
                view.AdvanceMotion(0f, 0.099f);
                Check(view.transform.parent == sourceTile && !view.IsSelectionAnimating, "Stagger does not start early");
                view.AdvanceMotion(0f, 0.002f);
                Check(view.IsSelectionAnimating && view.transform.parent == sourceTile, "Unscaled delay elapses during pause and schedules lift");
                Near(view.transform.position, Vector3.zero, "Paused scaled animation remains still");
                view.AdvanceMotion(0.1f);
                Near(view.transform.position, new Vector3(0, 6, 0), "Unready nut lifts toward destination ready height");
                view.AdvanceMotion(0.1f);
                Near(view.transform.position, new Vector3(0, 8, 0), "Lift completion preserves position when reparenting");
                Check(view.transform.parent == targetTile && view.IsWobbling, "Original ready completion enables inner wobble during crossing");
                view.AdvanceMotion(0.2f);
                Near(view.transform.position, new Vector3(4, 8, 0), "Unready nut crosses after lifting");
                Check(!view.IsWobbling, "Descent cancels ready wobble");
                view.AdvanceMotion(0.2f);
                Check(complete == 1 && !view.IsTransferring, "Unready transfer completes after descent");

                view.PlayTransfer(transfer.Destination, targetTile, targetReady, 2, false, () => complete++);
                view.ReleaseVisual();
                view.AdvanceMotion(1f, 1f);
                Check(complete == 1 && !view.IsTransferring, "Pool release cancels delayed transfer callback");
            }
            finally { UnityEngine.Object.DestroyImmediate(holder); }
            Debug.Log("NUT_TRANSFER_VALIDATION_PASS original first-board transfers, fixed-slot rebinding, preserve-world reparent, ready/unready paths, unscaled stagger, landing callbacks and operation gates verified.");
        }

        private static Transform Point(Transform parent, Vector3 position)
        {
            var value = new GameObject("Motion reference").transform;
            value.SetParent(parent, false);
            value.localPosition = position;
            return value;
        }
        private static void Near(Vector3 actual, Vector3 expected, string message)
        {
            Check((actual - expected).sqrMagnitude < 0.00001f, message + ": " + actual + " expected " + expected);
        }
        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }
    }
}
