using UnityEngine;

namespace NutSort.World
{
    // ScrewPos fields initialized by ScenePosGroup.AddScrewPos 0xA059F0.
    public sealed class OriginalScrewPosition : MonoBehaviour
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int Index { get; private set; }
        public void Bind(int row, int column, int index)
        {
            Row = row; Column = column; Index = index;
        }
    }
}
