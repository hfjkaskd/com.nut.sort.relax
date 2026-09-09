using System;
using System.Globalization;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalTimeValidation
    {
        public static void Validate()
        {
            var epoch=new DateTime(1970,1,1);
            Check(OriginalWithdrawalTime.SecondsFromLocal(epoch.AddTicks(9999999))==0&&OriginalWithdrawalTime.SecondsFromLocal(epoch.AddSeconds(1))==1,"Positive tick truncation");
            Check(OriginalWithdrawalTime.SecondsFromLocal(epoch.AddTicks(-9999999))==0&&OriginalWithdrawalTime.SecondsFromLocal(epoch.AddSeconds(-1))==-1,"Negative truncates toward zero, not Unix floor");
            var wall=new DateTime(2026,9,8,12,34,56);
            Check(OriginalWithdrawalTime.SecondsFromLocal(DateTime.SpecifyKind(wall,DateTimeKind.Local))==OriginalWithdrawalTime.SecondsFromLocal(DateTime.SpecifyKind(wall,DateTimeKind.Utc)),"Raw ticks do not normalize DateTime Kind");
            string country="US",language="en";int reads=0;
            var time=new OriginalWithdrawalTime(()=>{reads++;return OriginalWithdrawalTime.CountryLanguageCode(language,country);});
            long seconds=OriginalWithdrawalTime.SecondsFromLocal(wall);
            Check(time.Format(seconds,"yyyy-MM-dd HH:mm:ss")=="2026-09-08 12:34:56"&&reads==1,"No extra local conversion of encoded wall time");
            Check(time.Format(seconds,"g")=="9/8/2026 12:34 PM","Source US short date/time");
            country="DE";language="de";Check(time.Format(seconds,"g")=="08.09.2026 12:34","Country is reread and changes culture");
            country="GB";language="en";Check(time.Format(seconds,"g")=="08/09/2026 12:34","Country-specific date order for same language");
            int before=reads;Expect<ArgumentOutOfRangeException>(()=>time.Format(long.MaxValue,"g"));Check(reads==before,"Timestamp failure precedes country read");
            var missing=new OriginalWithdrawalTime(()=>null);Expect<ArgumentNullException>(()=>missing.Format(0,"g"));
            var failed=new OriginalWithdrawalTime(()=>throw new InvalidOperationException());Expect<InvalidOperationException>(()=>failed.Format(0,"g"));
            country="US";language="en";Expect<FormatException>(()=>time.Format(0,"Q"));
            Check(OriginalWithdrawalTime.CountryLanguageCode(null,"US")=="-US"&&OriginalWithdrawalTime.CountryLanguageCode("en",null)=="en-","Source concatenation has no fallback");
            Check(time.Format(-1,"yyyy-MM-dd HH:mm:ss")=="1969-12-31 23:59:59","Negative display timestamp");
            long low=OriginalWithdrawalTime.SecondsFromLocal(DateTime.Now),actual=OriginalWithdrawalTime.LocalSeconds(),high=OriginalWithdrawalTime.SecondsFromLocal(DateTime.Now);
            Check(actual>=low&&actual<=high,"Live local clock bounds");
            Debug.Log("NUT_WITHDRAWAL_TIME_VALIDATION_PASS native raw local ticks, truncation, unspecified date display, live country-language culture, conversion-before-country and exception semantics; no UTC normalization or fallback.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
