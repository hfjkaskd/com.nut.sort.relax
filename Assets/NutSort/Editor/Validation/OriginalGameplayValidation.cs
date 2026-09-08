using System.Collections.Generic;
using System.IO;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalGameplayValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var layout = Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            LevelData data = repository.LoadBoard(false, "4b56d_1_1-1");
            var board = new OriginalBoardState(data, layout);
            var operation = new OriginalScrewOperator();
            Check(!board.IsSuccess, "Original first board starts unsolved");
            ScrewState source = board.Screws[1], target = board.Screws[0];
            Check(operation.Operate(source, null).Kind == ScrewOperationKind.Ready, "Select first-board single nut");
            ScrewOperation move = operation.Operate(target, source);
            Check(move.Kind == ScrewOperationKind.Moved && move.Move.Transfers.Length == 1, "First-board winning move");
            Check(source.IsNull && target.IsDone && board.IsSuccess, "Full same-color screw and empty screw win");
            Check(!target.IsCanOperator && !source.Slots[0].IsReady, "Data movement preserves animation gate");
            move.Move.CompleteMovement(0);
            Check(move.Move.RequiresDoneAnimation && !target.IsCanOperator, "Done animation runs before gate release");
            move.Move.CompleteDoneAnimation();
            Check(target.IsCanOperator, "Done animation releases gate");
            Check(data.B[1].C[0].BIM != null && data.B[0].C[3].BIM == null, "Playing does not mutate decoded source data");
            Check(operation.Operate(target, null).Kind == ScrewOperationKind.Ignored, "Completed screw cannot be selected");

            // Top-down same-color collection stops at a hidden nut even when
            // colors match. After moving, reveal the newly exposed group.
            source = Screw(1, 1, 1, 1);
            source.Slots[0].Nut.Type = NutType.Hidden;
            source.Slots[1].Nut.Type = NutType.Hidden;
            source.Slots[0].Nut.Color = 2;
            target = Screw(-1, -1, -1, -1);
            source.Slots[3].IsReady = true;
            move = operation.Operate(target, source);
            Check(move.Move.Transfers.Length == 2, "Move stops before hidden slot");
            Check(move.Move.Transfers[0].WasReady && !move.Move.Transfers[1].WasReady, "Only top nut is lifted");
            Check(source.Slots[1].Nut.Type == NutType.Normal && source.Slots[0].Nut.Type == NutType.Hidden,
                "Reveal exposed group without revealing lower different color");
            Check(ReferenceEquals(move.Move.Transfers[0].Nut, target.Slots[0].Nut), "Transfer preserves nut identity");
            Check(move.Move.Transfers[0].Source.Coordinate.y == 3 && move.Move.Transfers[0].Destination.Coordinate.y == 0,
                "Slot coordinates remain on original screws");
            move.Move.CompleteMovement(0);
            Check(!target.IsCanOperator, "Earlier nut arrival cannot release target");
            move.Move.CompleteMovement(1);
            Check(target.IsCanOperator && !move.Move.RequiresDoneAnimation, "Incomplete screw releases after last movement");

            source = Screw(2, 1, 1, -1);
            target = Screw(2, -1, -1, -1);
            operation.Operate(source, null);
            int attempts = operation.ScrewMoveCount;
            var rejected = operation.Operate(target, source);
            Check(rejected.Kind == ScrewOperationKind.Reverted && rejected.SaveRequested,
                "Mismatch reverts and requests original save");
            Check(!source.IsReadyMove && !target.IsReadyMove && operation.ScrewMoveCount == attempts + 1,
                "Mismatch counts attempt and does not select target");
            operation.Operate(source, null);
            Check(operation.Operate(source, source).Kind == ScrewOperationKind.Reverted && !source.IsReadyMove,
                "Same screw click cancels selection");

            source = Screw(2, 1, 1, 1);
            target = Screw(1, 1, 1, -1);
            operation.Operate(source, null);
            move = operation.Operate(target, source);
            Check(move.Move.Transfers.Length == 1 && source.Slots[2].Nut != null && target.IsDone, "Capacity limits same-color group");
            target = Screw(1, 2, 1, 2);
            operation.Operate(source, null);
            Check(operation.Operate(target, source).Kind == ScrewOperationKind.Reverted, "Full mixed target reverts source");

            target = Screw(-1, -1, -1, -1);
            target.IsLocked = true;
            Check(operation.Operate(target, source).Kind == ScrewOperationKind.AddScrewRequested, "Locked screw routes to add-screw request");
            Check(operation.Operate(target, source, true).Kind == ScrewOperationKind.Ignored, "Exchange ignores locked screw");
            target.IsLocked = false;
            Check(operation.Operate(target, source, true).Kind == ScrewOperationKind.ExchangeRequested, "Exchange request precedes empty selection check");

            var buffer = new List<NutSlot>();
            source = Screw(1, 1, -1, -1);
            source.Slots[0].Nut.Type = source.Slots[1].Nut.Type = NutType.Hidden;
            source.GetTopSame(buffer);
            Check(buffer.Count == 1, "Top hidden nut itself is included");
            source.RevealTopGroup(buffer);
            Check(source.Slots[0].Nut.Type == NutType.Normal && source.Slots[1].Nut.Type == NutType.Normal,
                "Reveal includes entire contiguous same-color hidden group");
            Debug.Log("NUT_GAMEPLAY_VALIDATION_PASS original first-board win, selection/revert, fixed slots, capacity, hidden reveal, operation gates and tool routing verified.");
        }

        private static ScrewState Screw(params int[] colors)
        {
            var data = new ScrewData { C = new CData[colors.Length] };
            for (int i = 0; i < colors.Length; i++)
                data.C[i] = new CData { LP = new LPData { y = i }, BIM = colors[i] < 0 ? null : new BIMData { Id = 1, CI = colors[i], V = true } };
            return new ScrewState(data, 0);
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }
    }
}
