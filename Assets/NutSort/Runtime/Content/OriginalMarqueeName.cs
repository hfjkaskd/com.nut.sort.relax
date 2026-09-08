using System;

namespace NutSort.Content
{
    // UserMgr.GetPlayerName (0x9B9674), with the A-Z array from .cctor.
    public static class OriginalMarqueeName
    {
        private static readonly string[] Codes =
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };

        public static string Generate() => Generate(UnityEngine.Random.Range);

        public static string Generate(Func<int, int, int> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            string first = Codes[range(0, Codes.Length)];
            string second = Codes[range(0, Codes.Length)];
            int firstDigit = range(0, 10);
            int secondDigit = range(0, 10);
            int variant = range(0, 4);
            switch (variant)
            {
                case 0: return string.Format("Player_{0}{1}{2}{3}", first, firstDigit, second, secondDigit);
                case 1: return string.Format("Player_{0}{1}{2}{3}", first, second, firstDigit, secondDigit);
                case 2: return string.Format("Player_{0}{1}{2}{3}", first, second.ToLower(), firstDigit, secondDigit);
                default: return string.Format("Player_{0}{1}{2}{3}", first, firstDigit, second.ToLower(), secondDigit);
            }
        }
    }
}
