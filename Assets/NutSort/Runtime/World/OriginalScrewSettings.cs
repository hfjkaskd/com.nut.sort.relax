using System;
using UnityEngine;

namespace NutSort.World
{
    [CreateAssetMenu(menuName = "Nut Sort/Original screw configuration")]
    public sealed class OriginalScrewSettings : ScriptableObject
    {
        public string PrefabPath;
        public string[] GuidePaths;
        public float GuideHeightOffset;
        public Vector3 ColliderCenter;
        public Vector3 ColliderSize;
        public Vector3 CapPosition;
        public Vector3 ReadyPosition;
        public Vector3 InitialPosition;
        public string ScrewPositionPath;
        public string ScrewRowPath;
        public float NutInitDelay;
        public float NutInitLongDelay;
        public float DoneRiseDuration;
        public float DoneReturnDuration;
        public float DonePeakScale;
        public float DoneLiftHeight;
        public float HiddenBreakHideDelay;
        public float MaskDoneScaleDuration, MaskBreakHideDelay, DontMoveBreakHideDelay, MaskBackOvershoot;
        [SerializeField] private string[] maskNutPaths;
        public string MaskNutPath(int color)
        {
            if (color < 0 || color >= maskNutPaths.Length) throw new ArgumentOutOfRangeException(nameof(color));
            return maskNutPaths[color];
        }
    }
}
