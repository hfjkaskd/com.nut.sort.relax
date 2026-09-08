using System;
using Newtonsoft.Json.Linq;

namespace NutSort.Content
{
    public sealed class OriginalPlayerGoldHintText
    {
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly Func<string> name;
        private readonly Func<float,string> currency;
        private readonly Func<float,float,float> range;
        public OriginalPlayerGoldHintText(OriginalUserLocalData user,OriginalTables tables,
            Func<float,string> currency,Func<string> name=null,Func<float,float,float> range=null)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.tables=tables ?? throw new ArgumentNullException(nameof(tables));
            this.currency=currency ?? throw new ArgumentNullException(nameof(currency));
            this.name=name ?? OriginalMarqueeName.Generate;
            this.range=range ?? UnityEngine.Random.Range;
        }
        public string PushName(string language) => tables.Text.GetText(95,language,name());
        public string PushInfo(OriginalMarqueeItem item,string language)
        {
            if(user.Level>2)
            {
                int id=item.IsGold?96:152;
                return tables.Text.GetText(id,language,currency(item.SampleValue(range)));
            }
            int index=user.Level==1?0:1;
            JObject document=user.GoldRewardTargetS2CData ?? throw new NullReferenceException("GoldRewardTargetS2CData");
            JToken list=Field(document,"bear_list");
            if(list==null || list.Type==JTokenType.Null)throw new NullReferenceException("bear_list");
            JToken row=((JArray)list)[index];
            if(row==null || row.Type==JTokenType.Null)throw new NullReferenceException("bear_list item");
            float value=OriginalRewardProgress.ToFloat((string)Field((JObject)row,"psi_value"));
            return tables.Text.GetText(97,language,currency(value));
        }
        public string SelfName(string language) => tables.Text.GetText(94,language);
        public string SelfInfo(float gold,string language) => tables.Text.GetText(96,language,currency(gold));
        private static JToken Field(JObject row,string key)
        {
            JToken value=null;
            foreach(JProperty property in row.Properties())
                if(string.Equals(property.Name,key,StringComparison.OrdinalIgnoreCase))value=property.Value;
            return value;
        }
    }
}
