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
        public float MainPanelReadyDelay;
        public float FailPanelDelay;
        public float ScrewDoneEventDelay, SuccessDoneEventDelay;
        public int FailPanelId;
        public string StageStartSound;
        public string StageCompleteSound;
        public float SuccessRequestDelay;
        public float FirstStageSoundDelay, RestartStageSoundDelay;
        public bool LSS260820;
        public int LSSSHSLV;
        public bool LSSAB;
        public int[] GameplayUnlockLevels;
        public float RaycastDistance;
        public LayerMask GameplayLayers;
    }
}
