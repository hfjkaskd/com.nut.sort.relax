using System;

namespace NutSort.Content
{
    public sealed class OriginalItemInfo
    {
        public int ItemType;
        public float Count,MoreCount,CurrentCount;
        public double DoubleCurrentCount;
        public bool IsMore;
        public int CountInt => RoundNative(Count);
        public int MoreCountInt => RoundNative(MoreCount);
        public int IntCount => IsMore?MoreCountInt:CountInt;
        private static int RoundNative(float value)
        {
            double rounded=Math.Round((double)value,MidpointRounding.ToEven);
            // Preserve the source ARM64 conversion, including its explicit +infinity branch.
            if(double.IsPositiveInfinity(rounded))return int.MinValue;
            if(double.IsNaN(rounded))return 0;
            if(rounded>=int.MaxValue)return int.MaxValue;
            if(rounded<=int.MinValue)return int.MinValue;
            return (int)rounded;
        }
    }
}
