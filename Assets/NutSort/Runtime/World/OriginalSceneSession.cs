using UnityEngine;

namespace NutSort.World
{
    [CreateAssetMenu(menuName = "Nut Sort/Scene session configuration")]
    public sealed class OriginalSceneSession : ScriptableObject
    {
        public string LevelPrefabPath;
        public int LockedScrewStartLevel;
        public bool LongEntryDelay;
        public float RestartDelay;
        public float FailPanelDelay;
        public int FailPanelId;
        public string StageStartSound;
        public float FirstStageSoundDelay, RestartStageSoundDelay;
        public bool LSS260820;
        public int LSSSHSLV;
        public bool LSSAB;
        public float RaycastDistance;
        public LayerMask GameplayLayers;
    }
}
