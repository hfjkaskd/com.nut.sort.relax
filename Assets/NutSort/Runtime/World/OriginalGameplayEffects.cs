using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalGameplayEffects : MonoBehaviour
    {
        [SerializeField] private OriginalEffectSettings settings;
        private OriginalLevelView level;
        private OriginalPrefabPool pool;
        private GameObject donePrefab;
        private struct ActiveEffect
        {
            public GameObject Instance;
            public float Remaining;
            public bool Pooled;
        }
        private readonly List<ActiveEffect> active = new List<ActiveEffect>();
        public int ActiveCount => active.Count;

        public void Bind(OriginalLevelView value, OriginalPrefabPool prefabPool)
        {
            Clear();
            if (settings == null || value == null || prefabPool == null)
                throw new InvalidOperationException("Original gameplay effect references are incomplete.");
            level = value; pool = prefabPool;
            level.SparkRequested += ShowSpark;
            level.DoneEffectRequested += ShowDone;
            level.Clearing += ClearActive;
        }

        private void ShowSpark(OriginalNutView nut) { PlaySpark(nut.ColorRoot); }
        private void ShowDone(OriginalScrewView screw)
        {
            PlayDone(screw.CompletionEffectRoot, screw.State.Slots[0].Nut.Color);
        }

        public GameObject PlaySpark(Transform parent)
        {
            var instance = pool.Rent(settings.SparkPath, parent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
            active.Add(new ActiveEffect { Instance = instance, Remaining = settings.SparkLifetime, Pooled = true });
            return instance;
        }

        public GameObject PlayDone(Transform parent, int color)
        {
            if (donePrefab == null) donePrefab = Resources.Load<GameObject>(settings.DonePath);
            if (donePrefab == null) throw new FileNotFoundException("Original completion effect missing.", settings.DonePath);
            // Source uses an instance per completed rod, unlike pooled landing
            // sparks. This is a bounded, low-frequency completion event.
            var instance = Instantiate(donePrefab, parent, false);
            instance.transform.localPosition = Vector3.zero;
            instance.GetComponent<OriginalParticleEffect>().PlayDone(settings.GetColor(color));
            active.Add(new ActiveEffect { Instance = instance, Remaining = settings.DoneLifetime });
            return instance;
        }

        private void Update() { Advance(Time.deltaTime); }

        public void Advance(float deltaTime)
        {
            if (deltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            for (int i = active.Count - 1; i >= 0; i--)
            {
                ActiveEffect effect = active[i];
                effect.Remaining -= deltaTime;
                if (effect.Remaining > 0f) { active[i] = effect; continue; }
                Release(effect);
                active.RemoveAt(i);
            }
        }

        private void Release(ActiveEffect effect)
        {
            if (effect.Instance == null) return;
            if (effect.Pooled) pool.Return(effect.Instance);
            else Destroy(effect.Instance);
        }

        public void Clear()
        {
            if (level != null)
            {
                level.SparkRequested -= ShowSpark;
                level.DoneEffectRequested -= ShowDone;
                level.Clearing -= ClearActive;
            }
            ClearActive(); level = null; pool = null;
        }

        private void ClearActive()
        {
            for (int i = active.Count - 1; i >= 0; i--) Release(active[i]);
            active.Clear();
        }
    }
}
