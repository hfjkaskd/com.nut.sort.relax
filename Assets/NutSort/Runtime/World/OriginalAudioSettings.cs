using UnityEngine;

namespace NutSort.World
{
    [CreateAssetMenu(menuName = "Nut Sort/Original audio")]
    public sealed class OriginalAudioSettings : ScriptableObject
    {
        public bool DefaultEnabled;
        public string ResourcePrefix;
        public string BgmName;
        public string SelectName;
        public string MoveNameFormat;
    }
}
