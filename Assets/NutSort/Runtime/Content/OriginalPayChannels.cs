using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Content
{
    public sealed class OriginalPayChannel
    {
        public string Country;
        public List<string> Channels;

        public string RandomChannel() => RandomChannel(UnityEngine.Random.Range);
        public string RandomChannel(Func<int,int,int> range)
        {
            return Channels[range(0,Channels.Count)] ?? string.Empty;
        }
    }

    // PayTable.GetPayChannel; no payment or SDK request is performed here.
    public sealed class OriginalPayChannels
    {
        private readonly List<OriginalPayChannel> rows;
        public OriginalPayChannels(JObject table)
        {
            JToken source = Field(table,"payChannels");
            if(source == null || source.Type == JTokenType.Null) return;
            rows = new List<OriginalPayChannel>();
            foreach(JToken token in (JArray)source)
            {
                if(token.Type == JTokenType.Null) { rows.Add(null); continue; }
                var row = (JObject)token;
                var entry = new OriginalPayChannel { Country = (string)Field(row,"country") };
                JToken channels = Field(row,"channel");
                if(channels != null && channels.Type != JTokenType.Null)
                {
                    entry.Channels = new List<string>();
                    foreach(JToken channel in (JArray)channels) entry.Channels.Add((string)channel);
                }
                rows.Add(entry);
            }
        }

        public OriginalPayChannel GetPayChannel(string country,Func<string> currentCountry)
        {
            if(string.IsNullOrEmpty(country)) country = currentCountry();
            foreach(OriginalPayChannel row in rows)
                if(country.ToUpper() == row.Country) return row;
            Debug.LogError("GetPayChannel not country: " + country);
            return null;
        }

        private static JToken Field(JObject row,string name)
        {
            JToken result = null;
            foreach(JProperty property in row.Properties())
                if(string.Equals(property.Name,name,StringComparison.OrdinalIgnoreCase)) result = property.Value;
            return result;
        }
    }
}
