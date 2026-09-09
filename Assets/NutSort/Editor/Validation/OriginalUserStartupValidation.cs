using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUserStartupValidation
    {
        private sealed class Preferences:IOriginalUserPreferences
        {
            public string Value="";public int Writes;
            public string GetString(string key,string fallback){Check(key=="UserLocalData","Native key");return Value;}
            public void SetString(string key,string value){Value=value;Writes++;}
            public void Save(){}
        }
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var country=new OriginalUserCountryState(tables.Countries,s=>throw new Exception(s)){CountryCode="US",Area="USA"};
            var prefs=new Preferences();var trace=new List<string>();long ticks=100;Action<bool> held=null;bool failName=false,failIdentifier=false;
            OriginalUserStartup startup=null;
            startup=new OriginalUserStartup(defaults,prefs,country,()=>{trace.Add("id");if(failIdentifier)throw new InvalidOperationException();return "fixture-id";},
                (cb,flag)=>{Check(!flag&&startup.Data!=null&&country.CountryInfo.Code=="US","Loaded user and country before config");trace.Add("config");held=cb;},
                ()=>{Check(startup.Data.UserId=="fixture-id"&&country.CountryInfo.Code=="US","Fresh profile published and country resolved before register");trace.Add("register");},
                (cb,flag)=>{Check(startup.IsInitDone&&!flag,"Ready before reward");trace.Add("reward");},()=>{trace.Add("save");startup.Store.SaveData(false,null);},
                ()=>{trace.Add("name");if(failName)throw new InvalidOperationException();return "Player_A1B2";},()=>{trace.Add("clock");return ++ticks;},()=>new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc),(a,b)=>a);
            startup.Initialize();Check(string.Join(",",trace)=="id,name,clock,clock,register"&&startup.Data.RegisterTime==101&&startup.Data.LastGetEveryDayGift==102&&startup.Data.LoginTime==0&&!startup.IsInitDone&&prefs.Writes==0,"Fresh native identity/time order with no automatic save/readiness");
            var prior=startup.Store;failName=true;trace.Clear();Expect<InvalidOperationException>(startup.Initialize);Check(ReferenceEquals(prior,startup.Store)&&string.Join(",",trace)=="id,name","Failure before fresh publication preserves previous store");failName=false;
            failIdentifier=true;trace.Clear();Expect<InvalidOperationException>(startup.Initialize);Check(string.Join(",",trace)=="id"&&ReferenceEquals(prior,startup.Store),"Identity failure prevents name/clock/publication");failIdentifier=false;
            prefs.Value="{\"UserId\":\"saved\",\"ServerConfigData\":{},\"LoginTime\":1,\"TodayChallengeTimes\":8}";trace.Clear();startup.Initialize();
            Check(string.Join(",",trace)=="config"&&startup.Data.UserId=="saved"&&held!=null&&!startup.IsInitDone,"Existing profile skips all fresh initializers and waits for request");
            held(false);Check(startup.IsInitDone&&startup.Data.TodayChallengeTimes==8&&string.Join(",",trace)=="config,reward,save"&&prefs.Writes==1,"Actual saved-user config initialization reaches ready then reward/save");
            prefs.Value="null";trace.Clear();Expect<NullReferenceException>(startup.Initialize);Check(startup.Data==null&&trace.Count==0,"Literal null saved user is published, then original tail fails without new-user fallback");
            prefs.Value="{";var nullStore=startup.Store;Expect<Newtonsoft.Json.JsonSerializationException>(startup.Initialize);Check(ReferenceEquals(nullStore,startup.Store),"Malformed save preserves previously published store");
            Debug.Log("NUT_USER_STARTUP_VALIDATION_PASS actual store/country/config/tail composition, fresh identity/name/two UTC reads before publication, existing save skips fresh calls, held response readiness/save order and failure semantics; host SDK ports remain explicit, production scene not bound.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
