using UnityEngine;

namespace NutSort.UI
{
    [CreateAssetMenu(menuName = "Nut Sort/Original main level display")]
    public sealed class OriginalMainLevelSettings : ScriptableObject
    {
        public string PrefabPath;
        public string LanguageCode;
        public int MinimumLevel;
        public int TextId;
        public float NormalX;
        public float WithdrawalProgressX;
    }
}
