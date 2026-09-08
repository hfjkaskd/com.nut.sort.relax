using System;
using System.Collections;
using UnityEngine;
namespace NutSort.Gameplay
{
    // InitLevel wait predicate 0x9FFF64 and continuation 0x9FFC1C.
    public static class OriginalInitializationWait
    {
        public static IEnumerator Run(Func<UnityEngine.Object> mainPanel,float delay,Action continuation)
        {
            yield return new WaitUntil(()=>mainPanel()!=null);
            yield return new WaitForSeconds(delay);
            continuation?.Invoke();
        }
    }
}
