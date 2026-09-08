using UnityEngine;

namespace NutSort.UI
{
    [CreateAssetMenu(menuName = "Nut Sort/Original target reward banner")]
    public sealed class OriginalTargetRewardSettings : ScriptableObject
    {
        public Vector3 RestPosition;
        public float EntryDuration, HoldDuration, FlightDuration, FlightScale;
    }
}
