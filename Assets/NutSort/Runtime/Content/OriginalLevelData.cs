using System;
using UnityEngine;

namespace NutSort.Content
{
    // Field names/types from Assembly-CSharp TypeDef 7908, 7911-7918.
    // Additional source JSON fields such as ScrewData.P are also ignored by the original schema.
    [Serializable] public sealed class LevelDataConfig { public LevelDataInfo[] LevelDataInfos; }
    [Serializable] public sealed class LevelDataInfo { public int Id; public string[] Seeds; }
    [Serializable] public sealed class LevelData
    {
        public int UCCC;
        public int CC;
        public string LId;
        public ScrewData[] B;
    }
    [Serializable] public sealed class ScrewData
    {
        public int Id;
        public CData[] C;
        public OBIMData[] OBIM;
    }
    [Serializable] public sealed class CData { public LPData LP; public BIMData BIM; }
    [Serializable] public sealed class LPData { public int x; public int y; public int z; }
    [Serializable] public sealed class BIMData { public int Id; public int CI; public bool V; }
    [Serializable] public sealed class OBIMData { public int Id; public int CI; public OBIMObjData Obj; }
    [Serializable] public sealed class OBIMObjData { public int Id; public int TA; public int CI; }
}
