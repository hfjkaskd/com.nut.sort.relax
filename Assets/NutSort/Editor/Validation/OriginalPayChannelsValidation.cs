using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalPayChannelsValidation
    {
        public static void Validate()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var tables = new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                JArray rows = (JArray)tables.ReadTable("pay.json")["payChannels"];
                Check(rows.Count == 46,"Current original country count");
                foreach(JToken row in rows)
                {
                    string code = (string)row["country"];
                    OriginalPayChannel result = tables.PayChannels.GetPayChannel(code.ToLower(),()=>throw new Exception("Unexpected fallback"));
                    JArray channels = (JArray)row["channel"];
                    Check(result.Country == code && result.Channels.Count == channels.Count,"Country channel count");
                    for(int i=0;i<channels.Count;i++) Check(result.Channels[i] == (string)channels[i],"Original channel order");
                }
                int calls=0;
                Func<string> live=()=>{calls++;return "US";};
                var us=tables.PayChannels.GetPayChannel(null,live);
                Check(ReferenceEquals(us,tables.PayChannels.GetPayChannel("",live)) && calls==2,"Live fallback and shared row identity");
                Check(us.Channels[0]=="Venmo" && us.Channels[1]=="Zelle" && us.Channels[2]=="PayPal","US source channels");
                Check(us.RandomChannel((min,max)=>{Check(min==0 && max==3,"Integer channel random bounds");return 2;})=="PayPal","Channel selection index");
                var custom=new OriginalPayChannels(JObject.Parse("{\"payChannels\":[{\"country\":\"US\",\"channel\":[null]},{\"country\":\"US\",\"channel\":[\"later\"]},{\"country\":\"EMPTY\",\"channel\":[]}]}"));
                Check(custom.GetPayChannel("US",null).RandomChannel((a,b)=>0)==string.Empty,"First match and null channel becomes empty string");
                bool drew=false,failed=false;
                try { custom.GetPayChannel("EMPTY",null).RandomChannel((a,b)=>{drew=true;Check(a==0 && b==0,"Empty draw bounds");return 0;}); }
                catch(ArgumentOutOfRangeException) { failed=true; }
                Check(drew && failed,"Empty channel still draws before original index failure");
                CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("tr-TR");
                // Dotless lowercase i maps to source I under the original current-culture conversion.
                Check(tables.PayChannels.GetPayChannel("ın",null).Country=="IN","Current-culture country uppercase");
                Debug.Log("NUT_PAY_CHANNELS_VALIDATION_PASS 46 original countries, channel order, lazy live country, shared first match, culture conversion and random/empty selection behavior.");
            }
            finally { CultureInfo.CurrentCulture=culture; }
        }
        private static void Check(bool condition,string message) { if(!condition)throw new InvalidOperationException(message); }
    }
}
