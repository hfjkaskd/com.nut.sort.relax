using UnityEngine;

namespace NutSort.World
{
    [CreateAssetMenu(menuName = "Nut Sort/Original nut motion")]
    public sealed class OriginalNutMotionSettings : ScriptableObject
    {
        public float SelectionDuration;
        public float SelectionSpin;
        public float RotationSnap;
        public AnimationCurve SelectionEase;
        public float EntryScaleDuration;
        public float EntrySlotDelay;
        public float EntryFallDelay;
        public float EntryFallDuration;
        public float EntrySpin;
        public AnimationCurve LinearEase;
        public float WobbleSpeed;
        public float WobbleAngle;
        public float SecondaryAxisSpeedRatio;
        public float BobSpeed;
        public float BobMin;
        public float BobMax;
        public float TransferStagger;
        public float TransferDuration;
    }
}
