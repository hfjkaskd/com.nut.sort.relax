using UnityEngine;

namespace NutSort.UI
{
    [CreateAssetMenu(menuName = "Nut Sort/Original loading animation")]
    public sealed class OriginalLoadingSettings : ScriptableObject
    {
        public float AutomaticRate;
        public float AutomaticLimit;
        public float MarkerTravel;
        public float CompletionDuration;
        public float CompletedHoldDuration;
    }
}
