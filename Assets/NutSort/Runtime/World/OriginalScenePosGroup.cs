using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalScenePosGroup : MonoBehaviour
    {
        private readonly List<Transform> positions = new List<Transform>();
        private OriginalPrefabPool pool;
        public int MaxColumnCount { get; private set; }
        public void Bind(OriginalPrefabPool prefabPool, OriginalLayoutSettings layout, OriginalScrewSettings settings, int count)
        {
            Clear(); pool = prefabPool;
            MaxColumnCount = layout.MaxColumnCount(count);
            for (int i = 0; i < count; i++)
            {
                Transform position = pool.Rent(settings.ScrewPositionPath, transform).transform;
                position.localPosition = layout.Position(count, i);
                position.localRotation = Quaternion.identity;
                position.localScale = Vector3.one;
                positions.Add(position);
            }
        }
        public void Bind(OriginalPrefabPool prefabPool, OriginalLayoutSettings layout, OriginalScrewSettings settings, OriginalBoardState board)
        {
            Clear(); pool = prefabPool;
            // Original Init(false) derives bounds from saved coordinates and
            // ResetScrewPos centers each row using that row's actual child count.
            var rowCounts = new Dictionary<int, int>();
            int rows = 1;
            MaxColumnCount = 1;
            for (int i = 0; i < board.Screws.Length; i++)
            {
                Vector2Int coordinate = board.Screws[i].Coordinate;
                rows = Mathf.Max(rows, coordinate.x + 1);
                MaxColumnCount = Mathf.Max(MaxColumnCount, coordinate.y + 1);
                rowCounts.TryGetValue(coordinate.x, out int count);
                rowCounts[coordinate.x] = count + 1;
            }
            for (int i = 0; i < board.Screws.Length; i++)
            {
                Vector2Int coordinate = board.Screws[i].Coordinate;
                Transform position = pool.Rent(settings.ScrewPositionPath, transform).transform;
                position.localPosition = layout.Position(rows, rowCounts[coordinate.x], coordinate);
                position.localRotation = Quaternion.identity;
                position.localScale = Vector3.one;
                positions.Add(position);
            }
        }
        public Transform GetPosition(int index) => positions[index];
        public void Clear()
        {
            for (int i = 0; i < positions.Count; i++) pool.Return(positions[i].gameObject);
            positions.Clear(); pool = null; MaxColumnCount = 0;
        }
    }
}
