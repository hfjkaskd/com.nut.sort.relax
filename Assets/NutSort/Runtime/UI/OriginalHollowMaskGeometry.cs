using UnityEngine;

namespace NutSort.UI
{
    // UIHollowOutImage geometry shared by its eventual Graphic implementation.
    // Bounds are cached: a missing/destroyed target preserves all four values.
    public sealed class OriginalHollowMaskGeometry
    {
        public Vector2 InnerMax { get; private set; }
        public Vector2 InnerMin { get; private set; }
        public Vector2 OuterMax { get; private set; }
        public Vector2 OuterMin { get; private set; }

        // 0x9B5604 includes descendant RectTransforms in the relative bounds.
        public void CalculateBounds(RectTransform outer, RectTransform inner)
        {
            if (inner == null) return;
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(outer, inner);
            InnerMax = bounds.max;
            InnerMin = bounds.min;
            OuterMax = outer.rect.max;
            OuterMin = outer.rect.min;
        }

        // 0x9B5840: Radius is a width divisor. Preserve comparison/NaN behavior
        // and mutations of invalid settings; no height clamp is present.
        public float PrepareCornerRadius(ref float radius, ref int triangleCount)
        {
            float width = Mathf.Abs(InnerMin.x - InnerMax.x);
            if (radius < 0f) radius = 0f;
            float divided = width / radius;
            float half = width * .5f;
            float result = divided > half ? half : divided;
            if (triangleCount <= 0) triangleCount = 1;
            return result;
        }

        // 0x9B6DE4, per vertex. Compute directly instead of allocating the
        // original one-element input/output arrays in every AddVert call.
        public static Vector2 GetUV(Vector2 position, float width, float height)
        {
            return new Vector2(position.x / width + .5f, position.y / height + .5f);
        }
    }
}
