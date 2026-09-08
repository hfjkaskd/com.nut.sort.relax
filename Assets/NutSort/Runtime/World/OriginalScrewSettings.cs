using System;
using UnityEngine;

namespace NutSort.World
{
    [CreateAssetMenu(menuName = "Nut Sort/Original screw configuration")]
    public sealed class OriginalScrewSettings : ScriptableObject
    {
        public string PrefabPath;
        public Vector3 ColliderCenter;
        public Vector3 ColliderSize;
        public Vector3 CapPosition;
        public Vector3 ReadyPosition;
        public Vector3 InitialPosition;
        [SerializeField] private string[] maskNutPaths;
        public string MaskNutPath(int color)
        {
            if (color < 0 || color >= maskNutPaths.Length) throw new ArgumentOutOfRangeException(nameof(color));
            return maskNutPaths[color];
        }
    }
}
