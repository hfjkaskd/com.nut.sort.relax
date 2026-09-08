using System;
using System.Collections.Generic;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Gameplay
{
    public enum NutType { Normal = 1, Hidden = 2, Type3 = 3, Type4 = 4, Type5 = 5 }
    public enum ScrewType { Normal = 1, Type2 = 2, Type3 = 3, Mask = 4, Type5 = 5, DontMove = 6, Hidden = 7 }

    public sealed class NutState
    {
        public NutType Type;
        public int Color;
        public bool V;
    }

    // Slots stay on their screw. A move transfers NutState, not the slot or its
    // coordinate: NutInfo.Move (0xA0458C) and GetNullNutInfo (0xA04744).
    public sealed class NutSlot
    {
        public readonly Vector3Int Coordinate;
        public NutState Nut;
        public bool IsReady;
        public NutSlot(Vector3Int coordinate) { Coordinate = coordinate; }
    }

    public sealed class ScrewMaskState
    {
        public ScrewType Type;
        public int Color;
        public bool IsShow = true;
        public MaskObjectState Object;
    }

    public sealed class MaskObjectState
    {
        public int Id;
        public int TA;
        public int Color;
        public bool IsShow = true;
    }

    public sealed class ScrewState
    {
        public readonly int Id;
        public readonly int Index;
        public readonly NutSlot[] Slots;
        public readonly ScrewMaskState[] Masks;
        public Vector2Int Coordinate;
        public bool IsLocked;
        public bool IsCanOperator = true;
        public int Capacity { get; }
        public bool IsColorMask => Masks.Length > 0 && Masks[0].Type == ScrewType.Mask && Masks[0].Object.IsShow;
        public bool IsHidden => Masks.Length > 0 && Masks[0].Type == ScrewType.Hidden && Masks[0].IsShow;
        public bool IsDontMove => Masks.Length > 0 && Masks[0].Type == ScrewType.DontMove;

        public ScrewState(ScrewData data, int index)
        {
            if (data == null || data.C == null) throw new ArgumentException("Original screw cells missing.");
            Id = data.Id;
            Index = index;
            Capacity = data.C.Length;
            Slots = new NutSlot[data.C.Length];
            for (int i = 0; i < Slots.Length; i++)
            {
                CData cell = data.C[i];
                if (cell == null || cell.LP == null) throw new ArgumentException("Original cell coordinate missing.");
                var slot = new NutSlot(new Vector3Int(cell.LP.x, cell.LP.y, cell.LP.z));
                if (cell.BIM != null)
                    slot.Nut = new NutState { Type = (NutType)cell.BIM.Id, Color = cell.BIM.CI, V = cell.BIM.V };
                Slots[i] = slot;
            }
            Masks = new ScrewMaskState[data.OBIM == null ? 0 : data.OBIM.Length];
            for (int i = 0; i < Masks.Length; i++)
            {
                OBIMData mask = data.OBIM[i];
                var state = new ScrewMaskState { Type = (ScrewType)mask.Id, Color = mask.CI };
                if (mask.Obj != null)
                    state.Object = new MaskObjectState { Id = mask.Obj.Id, TA = mask.Obj.TA, Color = mask.Obj.CI };
                Masks[i] = state;
            }
        }

        internal ScrewState(int id, int index, int capacity, NutSlot[] slots, ScrewMaskState[] masks)
        {
            Id = id; Index = index; Capacity = capacity; Slots = slots; Masks = masks;
        }

        public bool IsNull
        {
            get { for (int i = 0; i < Slots.Length; i++) if (Slots[i].Nut != null) return false; return true; }
        }

        public bool IsFull
        {
            get { for (int i = 0; i < Slots.Length; i++) if (Slots[i].Nut == null) return false; return true; }
        }

        public bool IsDone
        {
            get
            {
                // Preserve the original loop beginning at 1, including its
                // zero/one-slot edge case (0xA03054), without a new capacity rule.
                for (int i = 1; i < Slots.Length; i++)
                {
                    if (Slots[i].Nut == null) return false;
                    if (Slots[i].Nut.Color != Slots[0].Nut.Color) return false;
                }
                return true;
            }
        }

        public bool IsReadyMove
        {
            get { for (int i = 0; i < Slots.Length; i++) if (Slots[i].IsReady) return true; return false; }
        }

        public NutSlot FirstEmpty()
        {
            for (int i = 0; i < Slots.Length; i++) if (Slots[i].Nut == null) return Slots[i];
            return null;
        }

        // Destination list belongs to the caller and is reused per interaction.
        public void GetTopSame(List<NutSlot> result, bool skipHidden = true)
        {
            result.Clear();
            for (int i = Slots.Length - 1; i >= 0; i--)
            {
                NutSlot slot = Slots[i];
                if (slot.Nut == null) continue;
                if (result.Count > 0 && (slot.Nut.Color != result[0].Nut.Color ||
                    (skipHidden && slot.Nut.Type == NutType.Hidden))) break;
                result.Add(slot);
            }
        }

        public void RevealTopGroup(List<NutSlot> buffer)
        {
            GetTopSame(buffer, false);
            for (int i = 0; i < buffer.Count; i++) buffer[i].Nut.Type = NutType.Normal;
        }
    }

    public sealed class OriginalBoardState
    {
        public readonly string LevelId;
        public readonly ScrewState[] Screws;
        public bool IsSkipLevel;

        public OriginalBoardState(LevelData data, OriginalLayoutSettings layout, bool addLockedScrew = false)
        {
            if (data == null || data.B == null || data.B.Length == 0) throw new ArgumentException("Original board missing.");
            LevelId = data.LId;
            Screws = new ScrewState[data.B.Length + (addLockedScrew ? 1 : 0)];
            for (int i = 0; i < data.B.Length; i++)
            {
                Screws[i] = new ScrewState(data.B[i], i);
            }
            if (addLockedScrew)
            {
                // Original AddNullScrew(true), 0x9FA0A4: repeat the last Id,
                // increment Index, zero usable capacity, but retain as many
                // empty NutInfos as the previous screw's NutMaxCount.
                ScrewState last = Screws[data.B.Length - 1];
                var slots = new NutSlot[last.Capacity];
                for (int i = 0; i < slots.Length; i++) slots[i] = new NutSlot(new Vector3Int(0, i, 0));
                Screws[data.B.Length] = new ScrewState(last.Id, last.Index + 1, 0, slots, Array.Empty<ScrewMaskState>())
                { IsLocked = true };
            }
            for (int i = 0; i < Screws.Length; i++) Screws[i].Coordinate = layout.Coordinate(Screws.Length, i);
        }

        internal OriginalBoardState(ScrewState[] screws)
        {
            // Source LevelInfo has no serialized level identifier.
            LevelId = null;
            Screws = screws;
        }

        public bool IsSuccess
        {
            get
            {
                if (IsSkipLevel) return true;
                for (int i = 0; i < Screws.Length; i++)
                    if (!Screws[i].IsNull && !Screws[i].IsDone) return false;
                return true;
            }
        }
    }
}
