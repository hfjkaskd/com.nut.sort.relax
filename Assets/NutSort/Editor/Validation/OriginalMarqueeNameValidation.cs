using System;
using System.Globalization;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalMarqueeNameValidation
    {
        public static void Validate()
        {
            CultureInfo savedCulture = CultureInfo.CurrentCulture;
            UnityEngine.Random.State savedRandom = UnityEngine.Random.state;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                string[] expected = { "Player_A2I9", "Player_AI29", "Player_Ai29", "Player_A2i9" };
                for (int variant = 0; variant < 4; variant++)
                {
                    int[] values = { 0, 8, 2, 9, variant };
                    int[] bounds = { 26, 26, 10, 10, 4 };
                    int call = 0;
                    string result = OriginalMarqueeName.Generate((min, max) =>
                    {
                        Check(call < 5 && min == 0 && max == bounds[call], "Native name draw bounds/order");
                        return values[call++];
                    });
                    Check(result == expected[variant] && call == 5, "Native name permutation " + variant);
                }
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
                int step = 0;
                int[] turkish = { 25, 8, 0, 9, 2 };
                Check(OriginalMarqueeName.Generate((min, max) => turkish[step++]) == "Player_Zı09", "Original current-culture lowercase, not invariant lowercase");
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                // Independent Unity draws verify the production overload shares Unity's global stream.
                for (int seed = 0; seed < 32; seed++)
                {
                    UnityEngine.Random.InitState(seed);
                    int a = UnityEngine.Random.Range(0,26), b = UnityEngine.Random.Range(0,26);
                    int c = UnityEngine.Random.Range(0,10), d = UnityEngine.Random.Range(0,10);
                    int v = UnityEngine.Random.Range(0,4);
                    int next = UnityEngine.Random.Range(0,100000);
                    UnityEngine.Random.InitState(seed);
                    string actual = OriginalMarqueeName.Generate();
                    string first = ((char)('A'+a)).ToString();
                    string second = ((char)('A'+b)).ToString();
                    if(v >= 2) second = second.ToLower();
                    string body = v == 1 || v == 2 ? first+second+c+d : first+c+second+d;
                    Check(actual == "Player_"+body && UnityEngine.Random.Range(0,100000) == next, "Production name and global random position");
                }
                Debug.Log("NUT_MARQUEE_NAME_VALIDATION_PASS original alphabet, five integer draws, four layouts, culture-sensitive lowercase and Unity random stream.");
            }
            finally
            {
                CultureInfo.CurrentCulture = savedCulture;
                UnityEngine.Random.state = savedRandom;
            }
        }
        private static void Check(bool value,string message) { if(!value) throw new InvalidOperationException(message); }
    }
}
