using System.Collections.Generic;

namespace NutSort.Gameplay
{
    // Original LevelInfo envelope. Unity objects, IsReady, IsCanOperator and
    // computed properties are excluded in the source serialization contract.
    public sealed partial class OriginalBoardSnapshot
    {
        public List<SavedScrew> ScrewInfos = new List<SavedScrew>();
        public List<OriginalMoveRecord> OperatorInfos = new List<OriginalMoveRecord>();
    }

    public sealed class SavedScrew
    {
        public int Id;
        public int Index;
        public int NutMaxCount;
        public SavedCoordinate Coordinate;
        public List<SavedNut> NutInfos = new List<SavedNut>();
        public List<SavedMask> ScrewMaskDatas = new List<SavedMask>();
        public bool IsLocked;
    }

    public sealed class SavedCoordinate
    {
        public int x;
        public int y;
    }

    public sealed class SavedNut
    {
        public SavedNutPosition NutPos;
        public SavedNutData NutData;
    }

    public sealed class SavedNutPosition
    {
        public int x;
        public int y;
        public int z;
    }

    public sealed class SavedNutData
    {
        public int NutType;
        public int NutColor;
        public bool V;
    }

    public sealed class SavedMask
    {
        public int ScrewType;
        public int NutColor;
        public SavedMaskObject Obj;
        public bool IsShow = true;
    }

    public sealed class SavedMaskObject
    {
        public int Id;
        public int TA;
        public int NutColor;
        public bool IsShow = true;
    }

    public sealed class OriginalMoveRecord
    {
        public int FromScrewIndex;
        public int ToScrewIndex;
        public int NutCount;
    }

}
