using System;
using Newtonsoft.Json.Linq;

namespace NutSort.Content
{
    // PMDItem.Init's text branch; the supplied name provider belongs to the
    // original display-name generator, not a real player's account identity.
    public sealed class OriginalMarqueeText
    {
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly Func<string> playerName;
        private readonly Func<float,string> formatGold;
        private readonly Func<float,float,float> range;
        public OriginalMarqueeText(OriginalUserLocalData user, OriginalTables tables,
            Func<string> playerName, Func<float,string> formatGold, Func<float,float,float> range=null)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.tables=tables ?? throw new ArgumentNullException(nameof(tables));
            this.playerName=playerName ?? throw new ArgumentNullException(nameof(playerName));
            this.formatGold=formatGold ?? throw new ArgumentNullException(nameof(formatGold));
            this.range=range ?? UnityEngine.Random.Range;
        }
        public string Build(OriginalMarqueeItem item,int capturedLevel,string language)
        {
            if(capturedLevel>2)
            {
                int textId=item.IsGold?35:153;
                string name=playerName();
                string amount=formatGold(item.SampleValue(range));
                return tables.Text.GetText(textId,language,name,amount);
            }
            JObject document=user.GoldRewardTargetS2CData ?? throw new NullReferenceException("GoldRewardTargetS2CData");
            JToken list=Field(document,"bear_list");
            if(list==null || list.Type==JTokenType.Null)throw new NullReferenceException("bear_list");
            JToken row=((JArray)list)[capturedLevel==1?0:1];
            if(row==null || row.Type==JTokenType.Null)throw new NullReferenceException("bear_list item");
            float value=OriginalRewardProgress.ToFloat((string)Field((JObject)row,"psi_value"));
            string earlyName=playerName();
            return tables.Text.GetText(35,language,earlyName,formatGold(value));
        }
        private static JToken Field(JObject row,string name)
        {
            JToken result=null;
            foreach(JProperty property in row.Properties())
                if(string.Equals(property.Name,name,StringComparison.OrdinalIgnoreCase))result=property.Value;
            return result;
        }
    }
}
