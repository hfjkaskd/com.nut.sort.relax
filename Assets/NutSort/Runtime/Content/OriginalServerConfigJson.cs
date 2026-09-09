using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    // Explicit Json.NET field projection avoids reflection-based field assignment.
    public static class OriginalServerConfigJson
    {
        public static JObject Read(string json,OriginalServerConfigDefaults defaults)
        {
            using(var reader=new JsonTextReader(new StringReader(json)))
            {
                reader.DateParseHandling=DateParseHandling.None;
                while(reader.Read()&&reader.TokenType==JsonToken.Comment){}
                var result=ReadValue(reader,defaults);End(reader);return result;
            }
        }
        // Leaves the reader at this value's end, allowing the user save reader to
        // retain duplicate field order instead of first collapsing it to JObject.
        internal static JObject ReadValue(JsonTextReader reader,OriginalServerConfigDefaults defaults)
        {
            if(reader.TokenType==JsonToken.None||reader.TokenType==JsonToken.Null)return null;
            if(reader.TokenType!=JsonToken.StartObject)throw new JsonSerializationException("ServerConfigData must be an object.");
            var result=defaults.CreateDocument();
            while(reader.Read())
            {
                if(reader.TokenType==JsonToken.Comment)continue;
                if(reader.TokenType==JsonToken.EndObject)return result;
                if(reader.TokenType!=JsonToken.PropertyName)throw new JsonSerializationException("Expected config field.");
                string name=(string)reader.Value;
                if(!reader.Read())throw new JsonSerializationException("Missing config field value.");
                while(reader.TokenType==JsonToken.Comment)if(!reader.Read())throw new JsonSerializationException("Missing config field value.");
                Apply(result,name,JToken.ReadFrom(reader));
            }
            throw new JsonSerializationException("Incomplete config object.");
        }
        private static void End(JsonTextReader reader)
        {while(reader.Read())if(reader.TokenType!=JsonToken.Comment)throw new JsonSerializationException("Additional config content.");}
        private static void Apply(JObject result,string name,JToken value)
        {
            string key=name.ToUpperInvariant();
            switch(key)
            {
                case "LSSLR1":case "LSSLR2":case "LSSLRFP":case "LSSLRFV":case "LSSLD1":case "LSSLD2":
                case "LSSEDGC":case "LSSR1":case "LSSR2":case "LSSSHSLV":case "LSSLSMAC":result[key]=Integer(value);break;
                case "LSSNTY":case "LSS260820":case "LSSAB":
                    if(value.Type==JTokenType.Null)throw new JsonSerializationException("Null config boolean.");result[key]=(bool)value;break;
                case "LSSLDR":case "LSSUPT":case "LSSGPUL":
                    if(value.Type==JTokenType.Null){result[key]=JValue.CreateNull();break;}
                    var list=new JArray();foreach(var item in (JArray)value)list.Add(Integer(item));result[key]=list;break;
                case "KSCOUNTRY":result["ksCountry"]=value.Type==JTokenType.Null?JValue.CreateNull():new JValue((string)value);break;
            }
        }
        private static int Integer(JToken value)
        {
            if(value.Type==JTokenType.Null)throw new JsonSerializationException("Null config integer.");
            return Convert.ToInt32(((JValue)value).Value,CultureInfo.InvariantCulture);
        }
    }
}
