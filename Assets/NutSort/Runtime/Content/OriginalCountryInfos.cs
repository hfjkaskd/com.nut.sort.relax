using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    public sealed class OriginalCountryInfo
    {
        public string Code,Area,LanguageCode,PhoneAreaNumber,Name;
        public string CountryLanguageCode=>string.Concat(LanguageCode,"-",Code);
    }
    public sealed class OriginalCountryInfos
    {
        public readonly List<OriginalCountryInfo> Countries=new List<OriginalCountryInfo>();
        private readonly Action<string> error;
        public OriginalCountryInfos(JObject document,Action<string> error)
        {
            this.error=error;
            var rows=(JArray)Field(document,"CountryInfos");
            foreach(var token in rows)
            {
                if(token.Type==JTokenType.Null){Countries.Add(null);continue;}
                var row=(JObject)token;
                Countries.Add(new OriginalCountryInfo{Code=(string)Field(row,"Code"),Area=(string)Field(row,"Area"),LanguageCode=(string)Field(row,"LanguageCode"),PhoneAreaNumber=(string)Field(row,"PhoneAreaNumber"),Name=(string)Field(row,"Name")});
            }
        }
        public OriginalCountryInfo Get(string code)
        {
            foreach(var row in Countries)if(row.Code==code.ToUpper())return row;
            error("CountryInfos not find countryCode:"+code);
            // Original recursive US lookup: shipped configuration contains US.
            return Get("US");
        }
        public bool Contains(string code)
        {
            foreach(var row in Countries)if(row.Code==code.ToUpper())return true;
            return false;
        }
        private static JToken Field(JObject row,string name)
        {JToken value=null;foreach(var field in row.Properties())if(string.Equals(field.Name,name,StringComparison.OrdinalIgnoreCase))value=field.Value;return value;}
    }
}
