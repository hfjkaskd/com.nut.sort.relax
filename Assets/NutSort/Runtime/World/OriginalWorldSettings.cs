using System;
using UnityEngine;

namespace NutSort.World
{
    [CreateAssetMenu(menuName = "Nut Sort/Original world prefabs")]
    public sealed class OriginalWorldSettings : ScriptableObject
    {
        [SerializeField] private float slotHeight;
        [SerializeField] private int fullScrewCapacity;
        [SerializeField] private string screwTilePath;
        [SerializeField] private string hiddenNutPath;
        [SerializeField] private string[] nutPaths;
        public float SlotHeight => slotHeight;
        public int FullScrewCapacity => fullScrewCapacity;
        public string ScrewTilePath => screwTilePath;
        public string HiddenNutPath => hiddenNutPath;
        public string NutPath(int color)
        {
            if (color < 0 || color >= nutPaths.Length) throw new ArgumentOutOfRangeException(nameof(color));
            return nutPaths[color];
        }
    }
}
