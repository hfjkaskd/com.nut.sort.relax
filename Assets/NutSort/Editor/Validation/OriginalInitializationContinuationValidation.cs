using System;
using NutSort.Content;
using NutSort.Gameplay;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalInitializationContinuationValidation
    {
        public static void Validate()
        {
            var defaults = ScriptableObject.CreateInstance<OriginalUserDefaults>();
            try
            {
                var user = new OriginalUserLocalData(defaults);
                var tables = new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var progress = new OriginalRewardProgress(user, tables, value => "unused");
                int reads = 0, calls = 0;
                string flags = "";
                Action pending = null;
                var flow = new OriginalInitializationContinuation(user,
                    () => { reads++; return progress.IsCompletePassStage2Level(); },
                    callback => pending = callback,
                    (banner, first) => { calls++; flags += (banner ? "1" : "0") + (first ? "1" : "0") + ";"; });
                user.ComeOnGold = " ";
                flow.Run(false, true);
                Check(calls == 1 && reads == 0 && pending == null && flags == "01;", "Nonempty including whitespace skips reward data entirely");
                user.ComeOnGold = null;
                bool missingFailed = false;
                try { flow.Run(true, false); } catch (NullReferenceException) { missingFailed = true; }
                Check(missingFailed && calls == 1 && pending == null, "Missing reward data cannot report initialization success");
                user.GoldRewardTargetS2CData = JObject.Parse("{\"bear_list\":[null,null,{\"Stage2RealLevel\":10}]}");
                user.Level = 9; flow.Run(true, false);
                user.Level = 10; flow.Run(false, false);
                Check(calls == 3 && pending == null && flags == "01;10;00;", "Below and equal boundary initialize immediately");
                user.Level = 11; user.ComeOnGold = ""; flow.Run(true, true);
                Check(calls == 3 && pending != null, "Beyond boundary waits for supplied synchronization callback");
                Action firstPending = pending;
                flow.Run(false, true);
                Check(calls == 3 && pending != firstPending, "Independent invocations retain independent continuations");
                user.ComeOnGold = "updated"; user.Level = 1;
                pending(); firstPending();
                Check(calls == 5 && flags == "01;10;00;01;11;", "Callbacks preserve each invocation flags without rechecking changed state");
                user.ComeOnGold = null; user.Level = 11;
                user.GoldRewardTargetS2CData = JObject.Parse("{\"bear_list\":[null,null,{\"Stage2RealLevel\":20}]}");
                pending = null; flow.Run(true, false);
                Check(calls == 6 && pending == null, "Decision reads replacement reward document");
                var failing = new OriginalInitializationContinuation(user, () => true,
                    callback => throw new InvalidOperationException("fixture sync"), (a,b) => calls++);
                bool syncFailed = false;
                try { failing.Run(true, false); } catch (InvalidOperationException) { syncFailed = true; }
                Check(syncFailed && calls == 6, "Synchronization failure never substitutes a success callback");
            }
            finally { UnityEngine.Object.DestroyImmediate(defaults); }
            Debug.Log("NUT_INITIALIZATION_CONTINUATION_VALIDATION_PASS lazy ComeOnGold branch, strict Stage2RealLevel boundary, current reward data, deferred independent captured flags and failure propagation.");
        }
        private static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    }
}
