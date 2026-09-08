using System;
using System.Collections.Generic;

namespace NutSort.Gameplay
{
    public enum ScrewOperationKind { Ignored, Ready, Reverted, Moved, AddScrewRequested, ExchangeRequested }

    public readonly struct NutTransfer
    {
        public readonly NutSlot Source;
        public readonly NutSlot Destination;
        public readonly NutState Nut;
        public readonly bool WasReady;
        public NutTransfer(NutSlot source, NutSlot destination)
        {
            Source = source;
            Destination = destination;
            Nut = source.Nut;
            WasReady = source.IsReady;
        }
    }

    public sealed class NutMoveBatch
    {
        public readonly ScrewState Source;
        public readonly ScrewState Destination;
        public readonly NutTransfer[] Transfers;
        public bool DestinationDoneAfterTransfer { get; private set; }
        public bool RequiresDoneAnimation { get; private set; }
        private bool lastMovementCompleted;

        internal NutMoveBatch(ScrewState source, ScrewState destination, int count)
        {
            Source = source;
            Destination = destination;
            Transfers = new NutTransfer[count];
        }

        internal void MarkTransferred() { DestinationDoneAfterTransfer = Destination.IsDone; }

        // Original b__1 (0xA0ADF4) only acts on the last staggered movement.
        // Completing the data transfer must not release the visual operation gate.
        public void CompleteMovement(int index)
        {
            if (index < 0 || index >= Transfers.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (index != Transfers.Length - 1 || lastMovementCompleted) return;
            lastMovementCompleted = true;
            RequiresDoneAnimation = Destination.IsDone;
            if (!RequiresDoneAnimation) Destination.IsCanOperator = true;
        }

        public void CompleteDoneAnimation()
        {
            if (!RequiresDoneAnimation) throw new InvalidOperationException("Done animation has not started.");
            RequiresDoneAnimation = false;
            Destination.IsCanOperator = true;
        }
    }

    public readonly struct ScrewOperation
    {
        public readonly ScrewOperationKind Kind;
        public readonly NutSlot AnimatedSlot;
        public readonly NutMoveBatch Move;
        public readonly bool SaveRequested;
        public bool OriginalReturnValue => Kind == ScrewOperationKind.Ready || Kind == ScrewOperationKind.Reverted || Kind == ScrewOperationKind.Moved;

        internal ScrewOperation(ScrewOperationKind kind, NutSlot slot = null, NutMoveBatch move = null, bool save = false)
        {
            Kind = kind;
            AnimatedSlot = slot;
            Move = move;
            SaveRequested = save;
        }
    }

    // Runtime state portion of ScrewInfo.Operator (0xA09430). The scene controller
    // consumes explicit animation/tool requests; this layer does not fake SDK
    // grants, complete animations early, or claim to implement reward panels.
    public sealed class OriginalScrewOperator
    {
        private readonly List<NutSlot> targetGroup = new List<NutSlot>();
        private readonly List<NutSlot> sourceGroup = new List<NutSlot>();
        public readonly List<OriginalMoveRecord> MoveHistory = new List<OriginalMoveRecord>();
        public int ScrewMoveCount;
        public event Action MoveAttempted;
        public event Action SelectionSoundRequested;
        public event Action<int> MoveSoundRequested;

        public ScrewOperation Operate(ScrewState target, ScrewState source, bool isExchanging = false)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.IsColorMask || target.IsHidden || target.IsDone) return default;
            if (isExchanging)
                return target.IsLocked ? default : new ScrewOperation(ScrewOperationKind.ExchangeRequested);
            if (target.IsLocked) return new ScrewOperation(ScrewOperationKind.AddScrewRequested);

            target.GetTopSame(targetGroup);
            if (targetGroup.Count > 0) SelectionSoundRequested?.Invoke();
            if (targetGroup.Count == 0 && (source == null || source.IsDone)) return default;
            if (source == null || !source.IsReadyMove)
            {
                if (targetGroup.Count == 0 || !target.IsCanOperator || target.IsDontMove) return default;
                targetGroup[0].IsReady = true;
                return new ScrewOperation(ScrewOperationKind.Ready, targetGroup[0]);
            }
            if (ReferenceEquals(source, target))
            {
                targetGroup[0].IsReady = false;
                return new ScrewOperation(ScrewOperationKind.Reverted, targetGroup[0]);
            }

            // The original counts attempts, including full/mismatched targets.
            ScrewMoveCount++;
            MoveAttempted?.Invoke();
            source.GetTopSame(sourceGroup);
            if (sourceGroup.Count == 0) throw new InvalidOperationException("Selected screw has no nut.");
            if (target.IsFull || (targetGroup.Count > 0 && targetGroup[0].Nut.Color != sourceGroup[0].Nut.Color))
            {
                sourceGroup[0].IsReady = false;
                return new ScrewOperation(ScrewOperationKind.Reverted, sourceGroup[0], save: true);
            }

            int room = targetGroup.Count == 0 ? target.Capacity : target.Capacity - targetGroup[0].Coordinate.y - 1;
            if (room <= 0) throw new InvalidOperationException("Original target has no coordinate capacity.");
            int count = Math.Min(room, sourceGroup.Count);
            MoveSoundRequested?.Invoke(count);
            var batch = new NutMoveBatch(source, target, count);
            MoveHistory.Add(new OriginalMoveRecord { FromScrewIndex = source.Index, ToScrewIndex = target.Index, NutCount = count });
            target.IsCanOperator = false;
            for (int i = 0; i < count; i++)
            {
                NutSlot from = sourceGroup[i];
                NutSlot to = target.FirstEmpty();
                if (to == null) throw new InvalidOperationException("Original target has no empty slot.");
                batch.Transfers[i] = new NutTransfer(from, to);
                to.Nut = from.Nut;
                to.IsReady = false;
                from.Nut = null;
                from.IsReady = false;
            }
            batch.MarkTransferred();
            source.RevealTopGroup(sourceGroup);
            return new ScrewOperation(ScrewOperationKind.Moved, move: batch, save: true);
        }
    }
}
