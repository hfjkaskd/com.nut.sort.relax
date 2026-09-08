using UnityEngine;

namespace NutSort.Content
{
    [CreateAssetMenu(menuName = "Nut Sort/Original content settings")]
    public sealed class OriginalContentSettings : ScriptableObject
    {
        [SerializeField] private string primaryIndexPath;
        [SerializeField] private string loopIndexPath;
        [SerializeField] private string primaryBoardDirectory;
        [SerializeField] private string loopBoardDirectory;
        [SerializeField] private string key;
        [SerializeField] private string iv;
        [SerializeField] private int firstLevelId;
        [SerializeField] private int normalLevelOffset;
        [SerializeField] private int guideContinuationOffset;
        [SerializeField] private int finalPrimaryId;
        [SerializeField] private int loopMinId;
        [SerializeField] private int loopMaxIdExclusive;

        public string PrimaryIndexPath => primaryIndexPath;
        public string LoopIndexPath => loopIndexPath;
        public string PrimaryBoardDirectory => primaryBoardDirectory;
        public string LoopBoardDirectory => loopBoardDirectory;
        public string Key => key;
        public string IV => iv;
        public int FirstLevelId => firstLevelId;
        public int NormalLevelOffset => normalLevelOffset;
        public int GuideContinuationOffset => guideContinuationOffset;
        public int FinalPrimaryId => finalPrimaryId;
        public int LoopMinId => loopMinId;
        public int LoopMaxIdExclusive => loopMaxIdExclusive;
    }
}
