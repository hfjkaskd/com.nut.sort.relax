using System;
using UnityEngine;

namespace NutSort.Content
{
    [CreateAssetMenu(menuName = "Nut Sort/Original world layout")]
    public sealed class OriginalLayoutSettings : ScriptableObject
    {
        [SerializeField] private int twoRowsThreshold;
        [SerializeField] private int threeRowsThreshold;
        [SerializeField] private int wideRowThreshold;
        [SerializeField] private float evenRowsBaseZ;
        [SerializeField] private float oddRowsBaseStepZ;
        [SerializeField] private float rowSpacingZ;
        [SerializeField] private float wideRowSpacingX;
        [SerializeField] private float shortRowSpacingX;
        [SerializeField] private float cameraAspectOffset;
        [SerializeField] private float cameraAspectRange;
        [SerializeField] private float cameraBaseSize;
        [SerializeField] private float cameraAspectSizeDelta;
        [SerializeField] private int cameraBaseColumnCount;

        // ScenePosGroup.Init, RVA 0xA05440. Original layout uses the screw list,
        // not the unused P field carried in some level JSON files.
        public int RowCount(int screwCount)
        {
            if (screwCount <= 0) throw new ArgumentOutOfRangeException(nameof(screwCount));
            return screwCount < twoRowsThreshold ? 1 : screwCount < threeRowsThreshold ? 2 : 3;
        }

        public int MaxColumnCount(int screwCount)
        {
            return Mathf.CeilToInt((float)screwCount / RowCount(screwCount));
        }

        public Vector2Int Coordinate(int screwCount, int screwIndex)
        {
            if (screwIndex < 0 || screwIndex >= screwCount)
                throw new ArgumentOutOfRangeException(nameof(screwIndex));
            int columns = MaxColumnCount(screwCount);
            return new Vector2Int(screwIndex / columns, screwIndex % columns);
        }

        public Vector3 Position(int screwCount, int screwIndex)
        {
            Vector2Int coordinate = Coordinate(screwCount, screwIndex);
            int columns = MaxColumnCount(screwCount);
            int rowColumns = Mathf.Min(columns, screwCount - coordinate.x * columns);
            return Position(RowCount(screwCount), rowColumns, coordinate);
        }

        // Also accepts restored row/column coordinates. The caller supplies each
        // row's actual occupied slot count, as in original ResetScrewPos.
        public Vector3 Position(int rows, int rowColumns, Vector2Int coordinate)
        {
            if (rows <= 0 || rowColumns <= 0 || coordinate.x < 0 || coordinate.x >= rows ||
                coordinate.y < 0 || coordinate.y >= rowColumns)
                throw new ArgumentOutOfRangeException(nameof(coordinate));
            float baseZ = rows % 2 == 0 ? evenRowsBaseZ : (rows / 2) * oddRowsBaseStepZ;
            float z = baseZ + (rows - 1 - coordinate.x) * rowSpacingZ;
            float spacing = rowColumns < wideRowThreshold ? shortRowSpacingX : wideRowSpacingX;
            float x = (coordinate.y - (rowColumns - 1) * 0.5f) * spacing;
            return new Vector3(x, 0f, z);
        }

        // GameScene.FixCameraSize, RVA 0xA05324. Keep the subtraction before
        // division and the fmax minimum verified in ARM64, rather than the
        // decompiler's incorrect unclamped expression.
        public float CameraSize(int pixelWidth, int pixelHeight, int maxColumns)
        {
            if (pixelWidth <= 0 || pixelHeight <= 0 || maxColumns <= 0)
                throw new ArgumentOutOfRangeException(nameof(pixelWidth));
            float aspect = (float)pixelHeight / pixelWidth;
            float t = Mathf.Clamp01((aspect + cameraAspectOffset) / cameraAspectRange);
            float multiplier = 1f + (float)(maxColumns - cameraBaseColumnCount) / cameraBaseColumnCount;
            return (cameraBaseSize + t * cameraAspectSizeDelta) * Mathf.Max(1f, multiplier);
        }
    }
}
