using UnityEngine;
namespace NutSort.UI
{
    [CreateAssetMenu(menuName = "Nut Sort/Original gold item")]
    public sealed class OriginalGoldItemSettings : ScriptableObject
    {
        public float PulseScale, PulseDuration, PulseDelay;
        public int PulseLoops;
        public float IconOffsetX, TextOffsetX, FloatTargetY, FloatDuration, FloatScale;
        public Color FloatColor;
    }
}
