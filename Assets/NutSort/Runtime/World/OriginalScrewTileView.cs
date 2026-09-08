using System;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalScrewTileView : MonoBehaviour
    {
        [SerializeField] private int Index;
        [SerializeField] private GameObject Tite1;
        [SerializeField] private GameObject Tite2;
        [SerializeField] private GameObject Hint;
        public int SlotIndex => Index;
        public GameObject Shaft => Tite1;
        public GameObject Tip => Tite2;
        public GameObject AddHint => Hint;

        // Original ScrewTile.Init, RVA 0xA06D90.
        public void Configure(int index, int capacity, OriginalWorldSettings settings)
        {
            if (index < 0 || capacity <= 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (Tite1 == null || Tite2 == null || Hint == null || settings == null)
                throw new InvalidOperationException("Original screw tile references are incomplete.");
            Index = index;
            transform.localPosition = new Vector3(0f, index * settings.SlotHeight, 0f);
            transform.localScale = Vector3.one;
            Tite1.SetActive(index < capacity - 1);
            Tite2.SetActive(index == capacity - 1);
            Hint.SetActive(Tite2.activeSelf && capacity < settings.FullScrewCapacity);
        }
    }
}
