using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalCountryInfosValidation
    {
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var source=(JArray)tables.ReadTable("config.json")["CountryInfos"];
            Check(tables.Countries.Countries.Count==46,"Original country row count");
            for(int i=0;i<source.Count;i++)
            {
                var row=tables.Countries.Countries[i];var expected=source[i];
                Check(row.Code==(string)expected["Code"]&&row.Area==(string)expected["Area"]&&row.LanguageCode==(string)expected["LanguageCode"]&&row.PhoneAreaNumber==(string)expected["PhoneAreaNumber"]&&row.Name==(string)expected["Name"],"Every source country field and row order");
                Check(ReferenceEquals(row,tables.Countries.Get(row.Code))&&tables.Countries.Contains(row.Code),"Original row reference lookup");
            }
            var us=tables.Countries.Get("us");Check(us.Code=="US"&&us.Area=="USA"&&us.LanguageCode=="en"&&us.PhoneAreaNumber=="1"&&us.CountryLanguageCode=="en-US","US from actual configuration");
            int errors=0;string last=null;
            var countries=new OriginalCountryInfos(tables.ReadTable("config.json"),s=>{errors++;last=s;});
            Check(!countries.Contains("unknown")&&errors==0,"Existence check is quiet and has no fallback");
            Check(countries.Get("unknown").Code=="US"&&errors==1&&last=="CountryInfos not find countryCode:unknown","Missing country logs exact source message then recursively returns US");
            Check(countries.Get(" US ").Code=="US"&&errors==2,"Source does not trim request");
            Expect<NullReferenceException>(()=>countries.Get(null));Expect<NullReferenceException>(()=>countries.Contains(null));
            var duplicate=new OriginalCountryInfo{Code="US",LanguageCode="fake"};countries.Countries.Add(duplicate);Check(!ReferenceEquals(countries.Get("US"),duplicate),"First match preserves source list semantics");
            var oldCulture=CultureInfo.CurrentCulture;
            try{CultureInfo.CurrentCulture=new CultureInfo("tr-TR");Check(!countries.Contains("in")&&countries.Contains("IN"),"Current-culture uppercase is not invariant");}
            finally{CultureInfo.CurrentCulture=oldCulture;}
            var missing=new OriginalCountryInfos(new JObject{{"countryinfos",new JArray()}},s=>throw new InvalidOperationException());
            Check(!missing.Contains(null),"Empty list never dereferences query");Expect<InvalidOperationException>(()=>missing.Get("missing"));
            string selected="US";var time=new OriginalWithdrawalTime(()=>tables.Countries.Get(selected).CountryLanguageCode);
            long stamp=OriginalWithdrawalTime.SecondsFromLocal(new DateTime(2026,9,8,12,34,0));
            Check(time.Format(stamp,"g")=="9/8/2026 12:34 PM","Time uses original configured country");selected="GB";Check(time.Format(stamp,"g")=="08/09/2026 12:34","Live country selection changes time formatting");
            Debug.Log("NUT_COUNTRY_INFOS_VALIDATION_PASS original encrypted configuration fields/order, first-row reference, current-culture uppercase, missing-country US recursion, quiet existence check, null/error ordering and live country-driven date formatting; GM/startup composition pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
