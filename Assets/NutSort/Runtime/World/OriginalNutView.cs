using System;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed partial class OriginalNutView : MonoBehaviour
    {
        // Original prefab field names retained so geometry references survive.
        [SerializeField] private MeshRenderer MeshRenderer;
        [SerializeField] private Transform Root;
        private NutSlot slot;
        private OriginalWorldSettings settings;
        private OriginalPrefabPool pool;
        private GameObject hiddenInstance;
        public MeshRenderer Renderer => MeshRenderer;
        public Transform ColorRoot => Root;
        public GameObject HiddenInstance => hiddenInstance;

        public void BindVisual(NutSlot value, OriginalWorldSettings configuration, OriginalPrefabPool prefabPool)
        {
            if (value == null || value.Nut == null) throw new ArgumentException("Cannot bind an empty nut slot.");
            if (Root == null || MeshRenderer == null || configuration == null || prefabPool == null)
                throw new InvalidOperationException("Original nut prefab references are incomplete.");
            slot = value;
            settings = configuration;
            pool = prefabPool;
            transform.localScale = Vector3.one;
            RefreshVisual();
        }

        // NutInfo.Refresh 0xA04334: reparent with world position preserved,
        // then write local zero and refresh visuals; no new movement tween.
        public void RefreshPosition(NutSlot value,Transform tile)
        {
            slot=value;
            transform.SetParent(tile);
            transform.localPosition=Vector3.zero;
            RefreshVisual();
        }

        // Nut.Refresh (0xA02704): keep the colored mesh/material intact and rent
        // the original hidden prefab as a child. Revealing returns that child.
        public void RefreshVisual()
        {
            if (slot == null || slot.Nut == null) return;
            bool hidden = slot.Nut.Type == NutType.Hidden;
            if (hidden && hiddenInstance == null)
            {
                hiddenInstance = pool.Rent(settings.HiddenNutPath, transform);
                hiddenInstance.transform.localPosition = Vector3.zero;
                hiddenInstance.transform.localScale = Vector3.one;
            }
            else if (!hidden && hiddenInstance != null)
            {
                pool.Return(hiddenInstance);
                hiddenInstance = null;
            }
            Root.gameObject.SetActive(!hidden);
        }

        public void ReleaseVisual()
        {
            StopMotion();
            if (hiddenInstance != null) pool.Return(hiddenInstance);
            hiddenInstance = null;
            slot = null;
            settings = null;
            pool = null;
        }
    }
}
