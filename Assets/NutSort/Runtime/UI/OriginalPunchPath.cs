using UnityEngine;

namespace NutSort.UI
{
    // DOTween.Punch (0xA0EB1C): generated once when the animation is created.
    // This is trajectory data only; target startup and looping belong to the view.
    public sealed class OriginalPunchPath
    {
        private readonly Vector3[] offsets;
        private readonly float[] durations;
        public int Count => offsets.Length;
        public Vector3 OffsetAt(int index) => offsets[index];
        public float DurationAt(int index) => durations[index];

        public OriginalPunchPath(Vector3 direction, float duration, int vibrato, float elasticity)
        {
            elasticity = Mathf.Clamp01(elasticity);
            int count = Mathf.Max(2, (int)(vibrato * duration));
            offsets = new Vector3[count];
            durations = new float[count];
            float magnitude = direction.magnitude;
            float decrement = magnitude / count;
            float total = 0f;
            for (int i = 0; i < count; i++)
            {
                float segment = ((float)(i + 1) / count) * duration;
                durations[i] = segment;
                total += segment;
            }
            float normalization = duration / total;
            for (int i = 0; i < count; i++) durations[i] *= normalization;
            for (int i = 0; i < count; i++)
            {
                if (i == count - 1) offsets[i] = Vector3.zero;
                else
                {
                    if (i == 0) offsets[i] = direction;
                    else if ((i & 1) != 0) offsets[i] = -Vector3.ClampMagnitude(direction, magnitude * elasticity);
                    else offsets[i] = Vector3.ClampMagnitude(direction, magnitude);
                    magnitude -= decrement;
                }
            }
        }
    }
}
