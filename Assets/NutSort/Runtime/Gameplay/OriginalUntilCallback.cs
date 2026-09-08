using System;
using System.Collections;
using UnityEngine;
namespace NutSort.Gameplay
{
    // TimeLSSUtil.UntilCallbackAsync.MoveNext 0x9BD200.
    public static class OriginalUntilCallback
    {
        public static IEnumerator Run(Func<bool> predicate,Action completed)
        {
            yield return new WaitUntil(predicate);
            completed?.Invoke();
        }
    }
}
