using System;
using System.Collections.Generic;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalScrewView : MonoBehaviour
    {
        [SerializeField] private List<OriginalScrewTileView> ScrewTiles = new List<OriginalScrewTileView>();
        [SerializeField] private Transform Tile;
        [SerializeField] private GameObject ScrewCap;
        [SerializeField] private Transform ReadyPos;
        [SerializeField] private Transform InitPos;
        [SerializeField] private BoxCollider BoxCollider;
        [SerializeField] private OriginalScrewTypeView ScrewTypeObj;
        [SerializeField] private GameObject DoneEffect;
        private OriginalPrefabPool pool;
        private OriginalScrewSettings settings;
        private OriginalWorldSettings world;
        public ScrewState State { get; private set; }
        public Transform ReadyPosition => ReadyPos;
        public Transform InitialPosition => InitPos;
        public Transform Cap => ScrewCap.transform;
        public BoxCollider Bounds => BoxCollider;
        public OriginalScrewTypeView TypeView => ScrewTypeObj;
        public int TileCount => ScrewTiles.Count;

        public void Bind(ScrewState state, OriginalPrefabPool prefabPool, OriginalWorldSettings worldSettings,
            OriginalScrewSettings screwSettings, bool lssab)
        {
            if (state == null || prefabPool == null || worldSettings == null || screwSettings == null)
                throw new ArgumentException("Original screw binding is incomplete.");
            State = state; pool = prefabPool; world = worldSettings; settings = screwSettings;
            RefreshCap();
            Refresh(lssab);
        }

        // Original Refresh grows the tile list from the configured tile prefab;
        // heights are fixed constants, not derived from current capacity.
        public void Refresh(bool lssab)
        {
            if (State == null) throw new InvalidOperationException("Screw is not bound.");
            for (int i = 0; i < ScrewTiles.Count; i++)
                ScrewTiles[i].Configure(ScrewTiles[i].SlotIndex, State.Capacity, world);
            while (ScrewTiles.Count < State.Capacity)
            {
                GameObject instance = pool.Rent(world.ScrewTilePath, Tile);
                OriginalScrewTileView view = instance.GetComponent<OriginalScrewTileView>();
                if (view == null) throw new InvalidOperationException("Configured screw tile has no native view.");
                view.Configure(ScrewTiles.Count, State.Capacity, world);
                ScrewTiles.Add(view);
            }
            BoxCollider.center = settings.ColliderCenter;
            BoxCollider.size = settings.ColliderSize;
            ScrewCap.transform.localPosition = settings.CapPosition;
            ReadyPos.localPosition = settings.ReadyPosition;
            // Keep this write after ReadyPos: both fields alias in the source prefab.
            InitPos.localPosition = settings.InitialPosition;
            RefreshType(lssab);
        }

        public void RefreshType(bool lssab)
        {
            if (State.Masks.Length == 0) ScrewTypeObj.Configure(null, State, settings, lssab);
            else for (int i = 0; i < State.Masks.Length; i++)
                ScrewTypeObj.Configure(State.Masks[i], State, settings, lssab);
        }

        public void RefreshCap()
        {
            bool done = State.IsDone;
            ScrewCap.transform.localScale = done ? Vector3.one : Vector3.zero;
            DoneEffect.SetActive(done);
        }

        public OriginalScrewTileView GetTile(int index) => ScrewTiles[index];

        public void Release()
        {
            for (int i = 0; i < ScrewTiles.Count; i++) pool.Return(ScrewTiles[i].gameObject);
            ScrewTiles.Clear();
            ScrewTypeObj.Release();
            DoneEffect.SetActive(false);
            State = null; pool = null; world = null; settings = null;
        }
    }
}
