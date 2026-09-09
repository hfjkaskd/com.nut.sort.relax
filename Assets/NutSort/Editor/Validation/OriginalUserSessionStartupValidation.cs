using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUserSessionStartupValidation
    {
        private sealed class Prefs:IOriginalUserPreferences
        {
            public string Value="{\"UserId\":\"saved\",\"ServerConfigData\":{},\"LoginTime\":1,\"TodayChallengeTimes\":5}";
            public string GetString(string key,string fallback)=>Value;
            public void SetString(string key,string value)=>Value=value;
            public void Save(){}
        }
        public static void Validate()
        {
            var root=new GameObject("User session fixture");var session=root.AddComponent<OriginalUserSession>();
            try
            {
                var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");var prefs=new Prefs();
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var country=new OriginalUserCountryState(tables.Countries,s=>throw new Exception(s)){CountryCode="US",Area="USA"};
                OriginalUserStartup startup=null;int saves=0;Action<bool> held=null;bool immediate=true;
                startup=new OriginalUserStartup(defaults,prefs,country,()=>throw new Exception("No fresh identity for saved user"),
                    (cb,flag)=>{Check(ReferenceEquals(session.Store,startup.Store)&&ReferenceEquals(session.Data,startup.Data),"Store visible in synchronous config call");held=cb;if(immediate)cb(false);},
                    ()=>throw new Exception("Unexpected register"),(cb,flag)=>Check(session.IsUserInitDone,"Session readiness reflects callback before reward"),()=>{Check(ReferenceEquals(session.Store,startup.Store),"Shared store for save");saves++;},
                    utcNow:()=>new DateTime(1970,1,1));
                session.StartUser(startup);Check(session.IsUserInitDone&&saves==1,"Synchronous startup completes through session");
                var previous=session.Store;session.Initialize();Check(ReferenceEquals(previous,session.Store),"Audio Initialize does not replace startup store");
                prefs.Value="{";Expect<Newtonsoft.Json.JsonSerializationException>(()=>session.StartUser(startup));Check(ReferenceEquals(previous,session.Store),"Failed reload preserves old shared store");
                prefs.Value="{\"ServerConfigData\":{},\"LoginTime\":1,\"TodayChallengeTimes\":5}";startup.IsInitDone=false;immediate=false;session.StartUser(startup);
                Check(!session.IsUserInitDone&&!ReferenceEquals(previous,session.Store),"New store published while config waits");held(true);Check(session.IsUserInitDone&&saves==2,"Delayed callback updates same session");
                previous=session.Store;Expect<ArgumentNullException>(()=>session.StartUser(null));Check(ReferenceEquals(previous,session.Store),"Null startup does not replace state");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_USER_SESSION_STARTUP_VALIDATION_PASS real MonoBehaviour host, synchronous/delayed callback shared store and readiness, failed reload retention and audio Initialize idempotence; main bootstrap source remains explicit.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
