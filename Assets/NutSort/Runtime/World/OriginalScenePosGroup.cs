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
        // AddScrewPos 0xA06038: last row with minimum child count wins ties.
        // Existing position/nut objects stay alive; only the selected row recenters.
        public Transform Append(OriginalBoardState board,OriginalLayoutSettings layout,OriginalScrewSettings settings)
        {
            var rows=new List<int>();var counts=new List<int>();
            int rowExtent=1;
            for(int i=0;i<positions.Count;i++)
            {
                int row=board.Screws[i].Coordinate.x;rowExtent=Mathf.Max(rowExtent,row+1);
                int index=rows.IndexOf(row);
                if(index<0){rows.Add(row);counts.Add(1);}else counts[index]++;
            }
            int selected=-1,minimum=int.MaxValue;
            for(int i=0;i<rows.Count;i++)if(counts[i]<=minimum){selected=rows[i];minimum=counts[i];}
            var coordinate=new Vector2Int(selected,minimum);
            board.Screws[board.Screws.Length-1].Coordinate=coordinate;
            Transform added=pool.Rent(settings.ScrewPositionPath,transform).transform;
            added.localRotation=Quaternion.identity;added.localScale=Vector3.one;
            positions.Add(added);
            for(int i=0;i<positions.Count;i++)
                if(board.Screws[i].Coordinate.x==selected)
                    positions[i].localPosition=layout.Position(rowExtent,minimum+1,board.Screws[i].Coordinate);
            MaxColumnCount=minimum+1;
            return added;
        }

        public Transform GetPosition(int index) => positions[index];
        public void Clear()
        {
            for (int i = 0; i < positions.Count; i++) pool.Return(positions[i].gameObject);
            positions.Clear(); pool = null; MaxColumnCount = 0;
        }
    }
}
