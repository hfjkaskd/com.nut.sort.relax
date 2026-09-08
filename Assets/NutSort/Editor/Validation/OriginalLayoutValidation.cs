using System;
using System.IO;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalLayoutValidation
    {
        public static void Validate()
        {
            var layout = Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            if (layout == null) throw new InvalidDataException("Original layout asset missing.");
            Check(layout.RowCount(5) == 1 && layout.RowCount(6) == 2 && layout.RowCount(10) == 2 &&
                layout.RowCount(11) == 3, "Original row thresholds");
            Check(layout.MaxColumnCount(7) == 4 && layout.MaxColumnCount(11) == 4,
                "ARM64 frintp rounds column counts upward");
            Near(layout.Position(2, 0), new Vector3(-1.1f, 0, 0));
            Near(layout.Position(2, 1), new Vector3(1.1f, 0, 0));
            // Current source device board: five screws above, four below.
            Near(layout.Position(9, 0), new Vector3(-3.3f, 0, 3.3f));
            Near(layout.Position(9, 4), new Vector3(3.3f, 0, 3.3f));
            Near(layout.Position(9, 5), new Vector3(-2.475f, 0, -3.3f));
            Near(layout.Position(9, 8), new Vector3(2.475f, 0, -3.3f));
            Near(layout.Position(11, 0), new Vector3(-2.475f, 0, 6.6f));
            Near(layout.Position(11, 4), new Vector3(-2.475f, 0, 0));
            Near(layout.Position(11, 8), new Vector3(-2.2f, 0, -6.6f));
            Check(layout.Coordinate(11, 10) == new Vector2Int(2, 2), "Row-major coordinates");
            Near(layout.CameraSize(1000, 1000, 2), 7.5f);
            Near(layout.CameraSize(1000, 3000, 5), 10.5f);
            Near(layout.CameraSize(1000, 3000, 6), 12.6f);
            Near(layout.CameraSize(1440, 2560, 5), 8.025013f);
            Near(layout.CameraSize(1440, 2560, 2), layout.CameraSize(1440, 2560, 5));
            Debug.Log("NUT_LAYOUT_VALIDATION_PASS row thresholds, ceil columns, original world positions, camera aspect clamps and minimum scale verified.");
        }

        private static void Near(Vector3 actual, Vector3 expected)
        {
            Check((actual - expected).sqrMagnitude < 0.00000001f, "World position " + actual + " expected " + expected);
        }

        private static void Near(float actual, float expected)
        {
            Check(Math.Abs(actual - expected) < 0.0001f, "Camera size " + actual + " expected " + expected);
        }

        private static void Check(bool condition, string detail)
        {
            if (!condition) throw new InvalidDataException(detail);
        }
    }
}
