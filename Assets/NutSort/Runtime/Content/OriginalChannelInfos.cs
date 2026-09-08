using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Content
{
    public sealed class OriginalChannelInfo
    {
        public string Channel;
        public List<string> Info;
        public bool IsEmail=>Info.Contains("email");
        public bool IsName=>Info.Contains("name01")||Info.Contains("name02");
    }
    public sealed class OriginalChannelInfos
    {
        private readonly List<OriginalChannelInfo> rows;
        public OriginalChannelInfos(JObject table)
        {
            JToken source=Field(table,"channelInfos");
            if(source==null || source.Type==JTokenType.Null)return;
            rows=new List<OriginalChannelInfo>();
            foreach(JToken token in (JArray)source)
            {
                if(token.Type==JTokenType.Null) { rows.Add(null);continue; }
                var value=(JObject)token;
                var row=new OriginalChannelInfo { Channel=(string)Field(value,"channel") };
                JToken info=Field(value,"info");
                if(info!=null && info.Type!=JTokenType.Null)
                {
                    row.Info=new List<string>();
                    foreach(JToken item in (JArray)info)row.Info.Add((string)item);
                }
                rows.Add(row);
            }
        }
        public OriginalChannelInfo GetChannelInfo(string channel)
        {
            foreach(var row in rows)if(channel==row.Channel)return row;
            Debug.LogError("GetChannelInfo not channel: "+channel);
            return null;
        }
        public static string IconPath(string channel,bool full=false) => "Atlas/"+(full?"Pay":"PaySimple")+"/"+channel;
        internal static JToken Field(JObject document,string name)
        {
            JToken result=null;
            foreach(JProperty field in document.Properties())
                if(string.Equals(field.Name,name,StringComparison.OrdinalIgnoreCase))result=field.Value;
            return result;
        }
        public OriginalChannelInfo GetSelfChannel(OriginalPayChannels channels,string country,JObject userInfo)
        {
            OriginalPayChannel countryRow=channels.GetPayChannel(country,null);
            if(userInfo==null)throw new NullReferenceException("UserLssInfo");
            JToken index=Field(userInfo,"GetType");
            int selected=index==null?0:(int)index;
            return GetChannelInfo(countryRow.Channels[selected]);
        }
    }
}
