using System.Collections.Generic;
using NutSort.Content;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalScenePosGroup : MonoBehaviour
    {
        private readonly List<Transform> positions = new List<Transform>();
        private OriginalPrefabPool pool;
        public void Bind(OriginalPrefabPool prefabPool, OriginalLayoutSettings layout, OriginalScrewSettings settings, int count)
        {
            Clear(); pool = prefabPool;
            for (int i = 0; i < count; i++)
            {
                Transform position = pool.Rent(settings.ScrewPositionPath, transform).transform;
                position.localPosition = layout.Position(count, i);
                position.localRotation = Quaternion.identity;
                position.localScale = Vector3.one;
                positions.Add(position);
            }
        }
        public Transform GetPosition(int index) => positions[index];
        public void Clear()
        {
            for (int i = 0; i < positions.Count; i++) pool.Return(positions[i].gameObject);
            positions.Clear(); pool = null;
        }
    }
}
