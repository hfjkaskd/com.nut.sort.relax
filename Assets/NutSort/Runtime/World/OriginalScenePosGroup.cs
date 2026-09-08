using System.Collections.Generic;
using System.Globalization;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalScenePosGroup : MonoBehaviour
    {
        private readonly List<Transform> positions = new List<Transform>();
        private readonly Dictionary<int, Transform> rows = new Dictionary<int, Transform>();
        private OriginalPrefabPool pool;
        public int MaxColumnCount { get; private set; }

        public void Bind(OriginalPrefabPool prefabPool, OriginalLayoutSettings layout, OriginalScrewSettings settings, int count)
        {
            Clear(); pool = prefabPool;
            MaxColumnCount = layout.MaxColumnCount(count);
            for (int i = 0; i < count; i++)
                AddPosition(layout.Coordinate(count, i), i, layout.RowCount(count), layout, settings);
            ResetRows(layout);
        }

        public void Bind(OriginalPrefabPool prefabPool, OriginalLayoutSettings layout, OriginalScrewSettings settings, OriginalBoardState board)
        {
            Clear(); pool = prefabPool;
            int rowCount = 1;
            MaxColumnCount = 1;
            foreach (var screw in board.Screws)
            {
                rowCount = Mathf.Max(rowCount, screw.Coordinate.x + 1);
                MaxColumnCount = Mathf.Max(MaxColumnCount, screw.Coordinate.y + 1);
            }
            foreach (var screw in board.Screws) AddPosition(screw.Coordinate, screw.Index, rowCount, layout, settings);
            ResetRows(layout);
        }

        // AddScrewPos 0xA059F0: rows carry Z; named position children carry X.
        // Source creates unparented identity objects then uses SetParent(parent, true).
        private Transform AddPosition(Vector2Int coordinate, int index, int rowCount, OriginalLayoutSettings layout, OriginalScrewSettings settings)
        {
            if (!rows.TryGetValue(coordinate.x, out Transform row))
            {
                row = RentIdentity(settings.ScrewRowPath);
                row.name = "Row_" + coordinate.x.ToString(CultureInfo.InvariantCulture);
                row.position = layout.RowPosition(rowCount, coordinate.x);
                row.SetParent(transform, true);
                rows.Add(coordinate.x, row);
            }
            Transform position = RentIdentity(settings.ScrewPositionPath);
            position.name = "Pos_" + coordinate.y.ToString(CultureInfo.InvariantCulture);
            position.GetComponent<OriginalScrewPosition>().Bind(coordinate.x, coordinate.y, index);
            position.SetParent(row, true);
            positions.Add(position);
            return position;
        }

        private Transform RentIdentity(string path)
        {
            Transform value = pool.Rent(path, null).transform;
            value.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            value.localScale = Vector3.one;
            return value;
        }

        private void ResetRows(OriginalLayoutSettings layout)
        {
            foreach (var row in rows.Values) ResetRow(row, layout);
        }

        // ResetScrewPos 0xA05D2C looks up column names, not sibling indices.
        private static void ResetRow(Transform row, OriginalLayoutSettings layout)
        {
            int count = row.childCount;
            for (int column = 0; column < count; column++)
            {
                Transform position = row.Find("Pos_" + column.ToString(CultureInfo.InvariantCulture));
                if (position == null)
                {
                    Debug.LogError("posTransform == null rowIndex:" + row.name.Substring(4) + " pos:" + column);
                    continue;
                }
                position.localPosition = layout.ColumnPosition(count, column);
            }
        }

        // AddScrewPos 0xA06038: actual sibling order resolves equal child counts.
        public Transform Append(OriginalBoardState board, OriginalLayoutSettings layout, OriginalScrewSettings settings)
        {
            int selected = -1, minimum = int.MaxValue;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform row = transform.GetChild(i);
                if (row.childCount > minimum) continue;
                selected = int.Parse(row.name.Substring(4), CultureInfo.InvariantCulture);
                minimum = row.childCount;
            }
            var coordinate = new Vector2Int(selected, minimum);
            board.Screws[board.Screws.Length - 1].Coordinate = coordinate;
            // The row already exists; append never computes a new row layout.
            Transform added = AddPosition(coordinate, positions.Count, rows.Count, layout, settings);
            ResetRow(rows[selected], layout);
            MaxColumnCount = minimum + 1;
            return added;
        }

        public Transform GetPosition(int index) => positions[index];
        public void Clear()
        {
            foreach (var position in positions) pool.Return(position.gameObject);
            positions.Clear();
            foreach (var row in rows.Values) pool.Return(row.gameObject);
            rows.Clear(); pool = null; MaxColumnCount = 0;
        }
    }
}
