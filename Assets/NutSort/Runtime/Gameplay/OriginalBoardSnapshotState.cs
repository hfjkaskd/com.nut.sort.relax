using System;
using System.Collections.Generic;
using System.IO;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Gameplay
{
    public sealed partial class OriginalBoardSnapshot
    {
        public static OriginalBoardSnapshot Capture(OriginalBoardState board, IReadOnlyList<OriginalMoveRecord> history)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (history == null) throw new ArgumentNullException(nameof(history));
            var snapshot = new OriginalBoardSnapshot();
            for (int i = 0; i < board.Screws.Length; i++)
            {
                ScrewState state = board.Screws[i];
                var screw = new SavedScrew
                {
                    Id = state.Id, Index = state.Index, NutMaxCount = state.Capacity,
                    Coordinate = new SavedCoordinate { x = state.Coordinate.x, y = state.Coordinate.y },
                    IsLocked = state.IsLocked
                };
                for (int j = 0; j < state.Slots.Length; j++)
                {
                    NutSlot slot = state.Slots[j];
                    screw.NutInfos.Add(new SavedNut
                    {
                        NutPos = new SavedNutPosition { x = slot.Coordinate.x, y = slot.Coordinate.y, z = slot.Coordinate.z },
                        NutData = slot.Nut == null ? null : new SavedNutData
                        { NutType = (int)slot.Nut.Type, NutColor = slot.Nut.Color, V = slot.Nut.V }
                    });
                }
                for (int j = 0; j < state.Masks.Length; j++)
                {
                    ScrewMaskState mask = state.Masks[j];
                    screw.ScrewMaskDatas.Add(new SavedMask
                    {
                        ScrewType = (int)mask.Type, NutColor = mask.Color, IsShow = mask.IsShow,
                        Obj = mask.Object == null ? null : new SavedMaskObject
                        { Id = mask.Object.Id, TA = mask.Object.TA, NutColor = mask.Object.Color, IsShow = mask.Object.IsShow }
                    });
                }
                snapshot.ScrewInfos.Add(screw);
            }
            for (int i = 0; i < history.Count; i++) snapshot.OperatorInfos.Add(CopyMove(history[i]));
            return snapshot;
        }

        public OriginalBoardState Restore(OriginalLayoutSettings layout)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            if (ScrewInfos == null || OperatorInfos == null) throw new InvalidDataException("Original saved board lists are null.");
            var screws = new ScrewState[ScrewInfos.Count];
            bool resetCoordinates = false;
            for (int i = 0; i < ScrewInfos.Count; i++)
                if (ScrewInfos[i] != null && ScrewInfos[i].Coordinate == null) resetCoordinates = true;
            for (int i = 0; i < screws.Length; i++)
            {
                SavedScrew saved = ScrewInfos[i];
                if (saved == null || saved.NutInfos == null || saved.ScrewMaskDatas == null)
                    throw new InvalidDataException("Original saved screw is incomplete.");
                var slots = new NutSlot[saved.NutInfos.Count];
                for (int j = 0; j < slots.Length; j++)
                {
                    SavedNut nut = saved.NutInfos[j];
                    if (nut == null || nut.NutPos == null) throw new InvalidDataException("Original saved nut position is missing.");
                    var slot = new NutSlot(new Vector3Int(nut.NutPos.x, nut.NutPos.y, nut.NutPos.z));
                    if (nut.NutData != null) slot.Nut = new NutState
                    { Type = (NutType)nut.NutData.NutType, Color = nut.NutData.NutColor, V = nut.NutData.V };
                    slots[j] = slot;
                }
                var masks = new ScrewMaskState[saved.ScrewMaskDatas.Count];
                for (int j = 0; j < masks.Length; j++)
                {
                    SavedMask mask = saved.ScrewMaskDatas[j];
                    if (mask == null) throw new InvalidDataException("Original saved mask is null.");
                    masks[j] = new ScrewMaskState
                    {
                        Type = (ScrewType)mask.ScrewType, Color = mask.NutColor, IsShow = mask.IsShow,
                        Object = mask.Obj == null ? null : new MaskObjectState
                        { Id = mask.Obj.Id, TA = mask.Obj.TA, Color = mask.Obj.NutColor, IsShow = mask.Obj.IsShow }
                    };
                }
                screws[i] = new ScrewState(saved.Id, saved.Index, saved.NutMaxCount, slots, masks)
                {
                    IsLocked = saved.IsLocked,
                    // ScenePosGroup.Init(false) first creates positions from saved
                    // row/column values. Any missing coordinate resets the WHOLE
                    // layout; ScrewInfo.Init then copies those position coordinates.
                    Coordinate = resetCoordinates ? layout.Coordinate(screws.Length, saved.Index) :
                        new Vector2Int(saved.Coordinate.x, saved.Coordinate.y)
                };
            }
            return new OriginalBoardState(screws);
        }

        internal static OriginalMoveRecord CopyMove(OriginalMoveRecord move)
        {
            if (move == null) return null;
            return new OriginalMoveRecord
            { FromScrewIndex = move.FromScrewIndex, ToScrewIndex = move.ToScrewIndex, NutCount = move.NutCount };
        }
    }
}
