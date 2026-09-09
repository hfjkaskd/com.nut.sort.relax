using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
namespace NutSort.Content
{
    [Serializable]
    public sealed class OriginalRegistrationData
    {
        public string scrap_mark,copp_uuid;
    }
    [Serializable]
    public sealed class OriginalRegistrationResponse
    {
        public string message_status,kinetic_gap;
        public long Time;
        public OriginalRegistrationData kinetic_data;
        public bool IsSuccess=>kinetic_gap=="NO-0";
        public void Init(){_ = IsSuccess;} // Native InitLss only evaluates IsSuccess.
        public static OriginalRegistrationResponse Read(string json)
        {
            using(var reader=new JsonTextReader(new StringReader(json)))
            {
                reader.DateParseHandling=DateParseHandling.None;
                Next(reader);
                var result=ReadEnvelope(reader);
                while(reader.Read())if(reader.TokenType!=JsonToken.Comment)throw new JsonSerializationException("Additional registration content.");
                return result;
            }
        }
        private static OriginalRegistrationResponse ReadEnvelope(JsonTextReader reader)
        {
            if(reader.TokenType==JsonToken.Null||reader.TokenType==JsonToken.None)return null;
            if(reader.TokenType!=JsonToken.StartObject)throw new JsonSerializationException("Registration must be an object.");
            var result=new OriginalRegistrationResponse();
            while(Next(reader))
            {
                if(reader.TokenType==JsonToken.EndObject)return result;
                if(reader.TokenType!=JsonToken.PropertyName)throw new JsonSerializationException("Expected registration field.");
                switch(((string)reader.Value).ToUpperInvariant())
                {
                    case "MESSAGE_STATUS":result.message_status=reader.ReadAsString();break;
                    case "KINETIC_GAP":result.kinetic_gap=reader.ReadAsString();break;
                    case "TIME":
                        if(!Next(reader)||reader.TokenType==JsonToken.Null)throw new JsonSerializationException("Null registration time.");
                        result.Time=Convert.ToInt64(reader.Value,CultureInfo.InvariantCulture);break;
                    case "KINETIC_DATA":Next(reader);result.kinetic_data=ReadData(reader,result.kinetic_data);break;
                    default:Next(reader);reader.Skip();break;
                }
            }
            throw new JsonSerializationException("Incomplete registration object.");
        }
        private static OriginalRegistrationData ReadData(JsonTextReader reader,OriginalRegistrationData current)
        {
            if(reader.TokenType==JsonToken.Null)return null;
            if(reader.TokenType!=JsonToken.StartObject)throw new JsonSerializationException("Registration data must be an object.");
            // Json.NET's default ObjectCreationHandling reuses an existing nested object.
            var result=current??new OriginalRegistrationData();
            while(Next(reader))
            {
                if(reader.TokenType==JsonToken.EndObject)return result;
                if(reader.TokenType!=JsonToken.PropertyName)throw new JsonSerializationException("Expected registration data field.");
                switch(((string)reader.Value).ToUpperInvariant())
                {
                    case "SCRAP_MARK":result.scrap_mark=reader.ReadAsString();break;
                    case "COPP_UUID":result.copp_uuid=reader.ReadAsString();break;
                    default:Next(reader);reader.Skip();break;
                }
            }
            throw new JsonSerializationException("Incomplete registration data.");
        }
        private static bool Next(JsonTextReader reader)
        {while(reader.Read())if(reader.TokenType!=JsonToken.Comment)return true;return false;}
    }
}
