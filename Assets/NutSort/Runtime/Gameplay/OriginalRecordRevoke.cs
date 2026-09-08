using System;
using UnityEngine;

namespace NutSort.Gameplay
{
    // OperatorInfo.Revoke (0xA0498C), NutInfo.Move (0xA0458C).
    // Visual callbacks run at the original mutation boundaries.
    public sealed class OriginalRecordRevoke
    {
        private readonly Func<int, ScrewState> find;
        private readonly Action<ScrewState> revert;
        private readonly Action<ScrewState, NutTransfer, int, Action> move;
        private readonly Action<ScrewState> refreshCap;

        public OriginalRecordRevoke(Func<int, ScrewState> find, Action<ScrewState> revert,
            Action<ScrewState, NutTransfer, int, Action> move, Action<ScrewState> refreshCap)
        {
            this.find = find ?? throw new ArgumentNullException(nameof(find));
            this.revert = revert ?? throw new ArgumentNullException(nameof(revert));
            this.move = move ?? throw new ArgumentNullException(nameof(move));
            this.refreshCap = refreshCap ?? throw new ArgumentNullException(nameof(refreshCap));
        }

        public bool Revoke(OriginalMoveRecord record)
        {
            ScrewState from = find(record.FromScrewIndex);
            if (from == null) { Debug.LogError("fromScrewInfo == null FromScrewIndex:" + record.FromScrewIndex); return false; }
            ScrewState to = find(record.ToScrewIndex);
            if (to == null) { Debug.LogError("toScrewInfo == null ToScrewIndex:" + record.ToScrewIndex); return false; }
            if (!from.IsCanOperator || !to.IsCanOperator)
            {
                Debug.LogWarning(string.Format("fromScrewInfo.IsCanOperator:{0} || toScrewInfo.IsCanOperator:{1}", from.IsCanOperator, to.IsCanOperator));
                return false;
            }
            if (from.IsReadyMove) revert(from);
            to.IsCanOperator = false;
            int initialCount = record.NutCount;
            for (int i = unchecked(initialCount - 1); i >= 0; i--)
            {
                NutSlot source = null;
                for (int j = to.Slots.Length - 1; j >= 0; j--)
                    if (to.Slots[j].Nut != null) { source = to.Slots[j]; break; }
                if (source == null) { Debug.LogError("toNutInfo == null"); continue; }
                int remaining = i;
                Action completed = () => { if (remaining == 0) to.IsCanOperator = true; };
                NutSlot destination = from.FirstEmpty();
                if (destination == null)
                {
                    Debug.LogError("nullNutInfo == null");
                    completed();
                    continue;
                }
                var transfer = new NutTransfer(source, destination);
                destination.Nut = source.Nut;
                move(from, transfer, unchecked(record.NutCount - initialCount + initialCount - 1 - i), completed);
                destination.IsReady = false;
                source.IsReady = false;
                source.Nut = null;
            }
            refreshCap(from);
            refreshCap(to);
            return true;
        }
    }
}
