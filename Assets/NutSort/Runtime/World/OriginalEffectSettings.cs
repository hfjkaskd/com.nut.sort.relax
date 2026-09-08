using UnityEngine;

namespace NutSort.World
{
    [CreateAssetMenu(menuName = "Nut Sort/Original gameplay effects")]
    public sealed class OriginalEffectSettings : ScriptableObject
    {
        public string SparkPath;
        public string DonePath;
        public string UnlockPath;
        public float UnlockLifetime;
        public float SparkLifetime;
        public float DoneLifetime;
        public Color[] NutColors;

        public Color GetColor(int index) => NutColors[index >= 0 && index < NutColors.Length ? index : 0];
    }
}
