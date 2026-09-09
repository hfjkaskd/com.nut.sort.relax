using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUserCountryStateValidation
    {
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            string log=null;int errors=0;Action onError=null;
            var state=new OriginalUserCountryState(tables.Countries,s=>{log=s;errors++;onError?.Invoke();}){CountryCode="us",Area="USA"};
            state.Initialize();var us=tables.Countries.Get("US");Check(ReferenceEquals(us,state.CountryInfo)&&errors==0,"Assign actual matching row, identical area is quiet");
            state.Area="source-area";state.Initialize();Check(errors==1&&log=="Areasource-area != CountryInfo.Area:USA"&&us.Area=="source-area","Mismatch logs then mutates shared row");
            state.Initialize();Check(errors==1,"Repeated matching initialization is quiet");
            state.CountryCode="GB";state.Area="GBR";state.Initialize();Check(state.CountryInfo.Code=="GB"&&us.Area=="source-area","Next initialization replaces selected row and preserves earlier mutation");
            var gb=state.CountryInfo;state.Area="before";onError=()=>{state.CountryInfo=us;state.Area="after";};state.Initialize();
            Check(log=="Areabefore != CountryInfo.Area:GBR"&&gb.Area=="GBR"&&us.Area=="after","Post-log live target and source reread");
            state.Area="failed";onError=()=>throw new InvalidOperationException();Expect<InvalidOperationException>(state.Initialize);Check(ReferenceEquals(state.CountryInfo,gb)&&gb.Area=="GBR","Country assignment survives log exception; area mutation does not occur");
            onError=null;state.Area=null;state.Initialize();Check(gb.Area==null&&log=="Area != CountryInfo.Area:GBR","Null source area preserved without fallback");
            var prior=state.CountryInfo;state.CountryCode=null;Expect<NullReferenceException>(state.Initialize);Check(ReferenceEquals(prior,state.CountryInfo),"Lookup failure retains previous assignment");
            state.CountryCode="US";state.Area="USA";state.Initialize();
            var clock=new OriginalWithdrawalTime(()=>state.CountryInfo.CountryLanguageCode);long stamp=OriginalWithdrawalTime.SecondsFromLocal(new DateTime(2026,9,8,12,34,0));
            Check(clock.Format(stamp,"g")=="9/8/2026 12:34 PM","Time follows assigned user country");
            state.CountryCode="GB";state.Area="GBR";state.Initialize();Check(clock.Format(stamp,"g")=="08/09/2026 12:34","Reinitialization changes live date culture");
            Debug.Log("NUT_USER_COUNTRY_STATE_VALIDATION_PASS original assignment-before-area-log, shared table row mutation, post-log live fields, reinitialization/null/failure behavior and user-country-driven time; SDK input/bootstrap remains excluded.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
