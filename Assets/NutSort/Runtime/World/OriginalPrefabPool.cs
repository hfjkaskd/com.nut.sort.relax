using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalPrefabPool : MonoBehaviour
    {
        private sealed class Entry
        {
            public GameObject Prefab;
            public readonly Stack<GameObject> Available = new Stack<GameObject>();
        }
        private readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>(StringComparer.Ordinal);
        private readonly Dictionary<GameObject, Entry> rented = new Dictionary<GameObject, Entry>();

        public GameObject Rent(string resourcePath, Transform parent)
        {
            if (!entries.TryGetValue(resourcePath, out Entry entry))
            {
                GameObject prefab = Resources.Load<GameObject>(resourcePath);
                if (prefab == null) throw new FileNotFoundException("Original world prefab missing.", resourcePath);
                entry = new Entry { Prefab = prefab };
                entries.Add(resourcePath, entry);
            }
            GameObject instance = null;
            while (entry.Available.Count > 0 && instance == null) instance = entry.Available.Pop();
            if (instance == null) instance = Instantiate(entry.Prefab, parent, false);
            else instance.transform.SetParent(parent, false);
            rented.Add(instance, entry);
            instance.SetActive(true);
            return instance;
        }

        public void Return(GameObject instance)
        {
            if (instance == null) return;
            if (!rented.TryGetValue(instance, out Entry entry)) throw new InvalidOperationException("Prefab is not rented from this pool.");
            rented.Remove(instance);
            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
            entry.Available.Push(instance);
        }
    }
}
