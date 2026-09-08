using System;
using System.Collections.Generic;

namespace NutSort.Gameplay
{
    // Original CheckDie (0x9FDB20), IsOnlyDontMove (0x9FE304), and
    // IsCannotMove (0x9FC404). Invoke at lifecycle checkpoints, not every frame.
    public sealed class OriginalDeadlockRules
    {
        private readonly Action<int> playHiddenBreak;
        private readonly Action save;
        private readonly Dictionary<int, int> colors = new Dictionary<int, int>();
        private readonly List<NutSlot> sourceGroup = new List<NutSlot>();
        private readonly List<NutSlot> destinationGroup = new List<NutSlot>();

        public OriginalDeadlockRules(Action<int> playHiddenBreak, Action save)
        {
            this.playHiddenBreak = playHiddenBreak ?? throw new ArgumentNullException(nameof(playHiddenBreak));
            this.save = save ?? throw new ArgumentNullException(nameof(save));
        }

        public bool CheckDie(OriginalBoardState board)
        {
            if (board == null || board.Screws == null) return false;
            bool changed = false;
            for (int i = 0; i < board.Screws.Length; i++)
            {
                ScrewState hidden = board.Screws[i];
                if (!hidden.IsHidden) continue;
                colors.Clear();
                foreach (ScrewState screw in board.Screws)
                {
                    // Completed, empty and masked rods do not contribute.
                    // Locked/DontMove rods and hidden nuts DO contribute.
                    if (screw.IsDone || screw.IsNull || screw.IsColorMask || screw.IsHidden) continue;
                    foreach (NutSlot slot in screw.Slots)
                    {
                        if (slot.Nut == null) continue;
                        colors.TryGetValue(slot.Nut.Color, out int count);
                        colors[slot.Nut.Color] = count + 1;
                    }
                }
                bool hasFour = false;
                foreach (int count in colors.Values)
                    if (count >= 4) { hasFour = true; break; }
                if (hasFour) continue;

                // The source clears every mask entry on this hidden rod, and
                // plays/saves once PER ENTRY, even if a later entry was hidden.
                for (int mask = 0; mask < hidden.Masks.Length; mask++)
                {
                    hidden.Masks[mask].IsShow = false;
                    playHiddenBreak(i);
                    save();
                }
                changed = true;
                // Source recursively restarts CheckDie. Iteration preserves its
                // reveal/save order without stack growth or new dictionaries.
                i = -1;
            }
            return changed;
        }

        public bool IsOnlyDontMove(OriginalBoardState board)
        {
            bool found = false;
            foreach (ScrewState screw in board.Screws)
            {
                if (screw.IsNull || screw.IsDone) continue;
                if (!screw.IsDontMove) return false;
                found = true;
            }
            return found;
        }

        public bool IsCannotMove(OriginalBoardState board)
        {
            CheckDie(board);
            if (board == null || board.Screws == null) return false;
            if (IsOnlyDontMove(board)) return true;
            for (int i = 0; i < board.Screws.Length; i++)
            {
                ScrewState source = board.Screws[i];
                if (source.IsColorMask || source.IsHidden || source.Capacity <= 0 || source.IsLocked) continue;
                if (source.IsNull) return false;
                if (source.IsDontMove) continue;
                // No IsDone/IsCanOperator gate here: retain the source predicate,
                // which is intentionally different from an actual input attempt.
                for (int j = 0; j < board.Screws.Length; j++)
                {
                    if (i == j) continue;
                    ScrewState destination = board.Screws[j];
                    if (destination.IsColorMask || destination.IsHidden || destination.IsLocked || destination.Capacity <= 0) continue;
                    if (destination.IsNull) return false;
                    source.GetTopSame(sourceGroup);
                    destination.GetTopSame(destinationGroup);
                    if (sourceGroup.Count == 0 || destinationGroup.Count == 0) continue;
                    if (sourceGroup[0].Nut.Color != destinationGroup[0].Nut.Color) continue;
                    int occupied = 0;
                    foreach (NutSlot slot in destination.Slots) if (slot.Nut != null) occupied++;
                    if (sourceGroup.Count <= destination.Capacity - occupied) return false;
                }
            }
            return true;
        }
    }
}
