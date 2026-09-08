using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Content
{
    public sealed class OriginalMarqueeItem
    {
        public float Weight, Minimum, Maximum;
        public bool IsGold;
        public float SampleValue() => UnityEngine.Random.Range(Minimum,Maximum);
        public float SampleValue(Func<float,float,float> range) => range(Minimum,Maximum);
    }

    // Explicit PMDS2CData projection. The caller controls the original success-
    // gated Init; reading an envelope never invents or requests server data.
    public sealed class OriginalMarqueeData
    {
        public string CashDisplayRange;
        public List<OriginalMarqueeItem> CashItems, CoinItems, Items;
        public float CashMinimum { get; private set; }
        public float CashMaximum { get; private set; }

        public static OriginalMarqueeData Read(JObject document)
        {
            if(document==null)throw new ArgumentNullException(nameof(document));
            return new OriginalMarqueeData
            {
                CashDisplayRange=(string)Field(document,"casShowBt"),
                CashItems=ReadItems(Field(document,"popCas")),
                CoinItems=ReadItems(Field(document,"zsRat")),
                Items=ReadItems(Field(document,"Pmds2CDatas"))
            };
        }
        public void Init()
        {
            string[] range=CashDisplayRange.Split('_');
            CashMinimum=OriginalRewardProgress.ToFloat(range[0]);
            CashMaximum=OriginalRewardProgress.ToFloat(range[1]);
            foreach(var item in CashItems)item.IsGold=true;
            Items=new List<OriginalMarqueeItem>();
            Items.AddRange(CashItems);
            Items.AddRange(CoinItems);
        }
        public float SampleCashValue() => UnityEngine.Random.Range(CashMinimum,CashMaximum);
        public float SampleCashValue(Func<float,float,float> range) => range(CashMinimum,CashMaximum);

        private static List<OriginalMarqueeItem> ReadItems(JToken token)
        {
            if(token==null || token.Type==JTokenType.Null)return null;
            var rows=(JArray)token;
            var result=new List<OriginalMarqueeItem>(rows.Count);
            foreach(JToken entry in rows)
            {
                if(entry==null || entry.Type==JTokenType.Null) { result.Add(null);continue; }
                var row=(JObject)entry;
                JToken gold=Field(row,"IsGold");
                result.Add(new OriginalMarqueeItem
                {
                    Weight=Number(row,"wt"), Minimum=Number(row,"lw"), Maximum=Number(row,"hh"),
                    IsGold=gold!=null && (bool)gold
                });
            }
            return result;
        }
        private static float Number(JObject row,string key)
        {
            JToken token=Field(row,key);
            return token==null?0:(float)token;
        }
        private static JToken Field(JObject row,string key)
        {
            JToken result=null;
            foreach(JProperty p in row.Properties())
                if(string.Equals(p.Name,key,StringComparison.OrdinalIgnoreCase))result=p.Value;
            return result;
        }
    }

    public sealed class OriginalMarqueeSelector
    {
        private readonly Func<OriginalMarqueeData> current;
        private readonly Action requestMissing;
        private readonly Func<float,float,float> range;
        public OriginalMarqueeSelector(Func<OriginalMarqueeData> current,Action requestMissing,
            Func<float,float,float> range=null)
        {
            this.current=current ?? throw new ArgumentNullException(nameof(current));
            this.requestMissing=requestMissing ?? throw new ArgumentNullException(nameof(requestMissing));
            this.range=range ?? UnityEngine.Random.Range;
        }
        public OriginalMarqueeItem Next()
        {
            OriginalMarqueeData data=current();
            if(data==null) { requestMissing();return null; }
            float total=0;
            for(int i=0;i<data.Items.Count;i++)total+=data.Items[i].Weight;
            float sample=range(0,total);
            OriginalMarqueeItem selected=data.Items[0];
            float cumulative=0;
            for(int i=0;i<data.Items.Count;i++)
            {
                selected=data.Items[i];
                cumulative+=data.Items[i].Weight;
                // Native FCMP(sample,cumulative); B.GT returns CURRENT row.
                // Equality and unordered comparisons continue to later rows.
                if(sample>cumulative)break;
            }
            return selected;
        }
    }
}
