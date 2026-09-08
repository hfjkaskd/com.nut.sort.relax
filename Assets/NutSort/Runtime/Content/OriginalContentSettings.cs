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

        public string PrimaryIndexPath => primaryIndexPath;
        public string LoopIndexPath => loopIndexPath;
        public string PrimaryBoardDirectory => primaryBoardDirectory;
        public string LoopBoardDirectory => loopBoardDirectory;
        public string Key => key;
        public string IV => iv;
    }
}
