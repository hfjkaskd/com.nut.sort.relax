using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalMarqueeDataValidation
    {
        public static void Validate()
        {
            CultureInfo previous=CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture=new CultureInfo("en-US");
                JObject document=JObject.Parse("{\"casShowBt\":\"1.5_9.5_ignored\",\"unknown\":{\"keep\":true},\"popCas\":[{\"wt\":2,\"lw\":3,\"hh\":4},{\"wt\":3,\"lw\":5,\"hh\":6}],\"zsRat\":[{\"wt\":5,\"lw\":7,\"hh\":8,\"IsGold\":true}],\"Pmds2CDatas\":[{\"wt\":99}]}");
                string before=document.ToString();var data=OriginalMarqueeData.Read(document);
                Check(data.Items.Count==1 && !data.CashItems[0].IsGold,"Read does not run success-gated Init");
                data.Init();
                Check(data.CashMinimum==1.5f && data.CashMaximum==9.5f && data.Items.Count==3,"Range split and merged item count");
                Check(ReferenceEquals(data.Items[0],data.CashItems[0]) && ReferenceEquals(data.Items[2],data.CoinItems[0]),"Init combines original row objects in cash/coin order");
                Check(data.CashItems[0].IsGold && data.CashItems[1].IsGold && data.CoinItems[0].IsGold,"Cash marks true; coin retains its incoming flag");
                Check(before==document.ToString(),"Projection preserves the opaque original JSON");
                int requests=0,calls=0;float sample=0;
                OriginalMarqueeData available=data;
                var selector=new OriginalMarqueeSelector(()=>available,()=>requests++,(a,b)=>{Check(a==0 && b==10,"One native float draw spans total raw weight");calls++;return sample;});
                foreach(float n in new[]{0f,1.99f,2f,float.NaN})
                { sample=n;Check(ReferenceEquals(selector.Next(),data.Items[2]),"At/below first cumulative weight, including equality/NaN, reaches the last row"); }
                foreach(float n in new[]{2.01f,5f,10f,float.PositiveInfinity})
                { sample=n;Check(ReferenceEquals(selector.Next(),data.Items[0]),"Strictly greater exits at first row, preserving original branch direction"); }
                Check(calls==8 && requests==0,"No extra random draw or request for available data");
                available=null;Check(selector.Next()==null && selector.Next()==null && requests==2 && calls==8,"Missing data requests every query and returns null without sampling");
                var synchronous=new OriginalMarqueeSelector(()=>available,()=>available=data,(a,b)=>0);
                Check(synchronous.Next()==null && available==data,"A synchronous request completion does not change the original null return");
                Check(ReferenceEquals(synchronous.Next(),data.Items[2]),"Next query observes completed data");
                int ranges=0;
                Check(data.SampleCashValue((a,b)=>{ranges++;Check(a==1.5f && b==9.5f,"Cash range");return 7;})==7,"Cash value sampled on demand");
                Check(data.Items[0].SampleValue((a,b)=>{ranges++;Check(a==3 && b==4,"Item range");return 3.5f;})==3.5f && ranges==2,"Item value uses its own bounds");
                data.Init();Check(data.Items.Count==3,"Repeated init replaces merged list rather than appending duplicates");
                var empty=OriginalMarqueeData.Read(JObject.Parse("{\"casShowBt\":\"0_0\",\"popCas\":[],\"zsRat\":[]}"));empty.Init();
                int emptyCalls=0;var emptySelector=new OriginalMarqueeSelector(()=>empty,()=>{},(a,b)=>{emptyCalls++;return 0;});
                Expect<ArgumentOutOfRangeException>(()=>emptySelector.Next());Check(emptyCalls==1,"Empty data samples zero range before original first-item failure");
                var malformed=OriginalMarqueeData.Read(JObject.Parse("{\"casShowBt\":\"2\",\"popCas\":[],\"zsRat\":[]}"));
                Expect<IndexOutOfRangeException>(()=>malformed.Init());Check(malformed.CashMinimum==2,"Malformed range preserves original partial initialization order");
                var missing=OriginalMarqueeData.Read(new JObject());Expect<NullReferenceException>(()=>missing.Init());
                var nullRow=OriginalMarqueeData.Read(JObject.Parse("{\"casShowBt\":\"0_1\",\"popCas\":[null],\"zsRat\":[]}"));Expect<NullReferenceException>(()=>nullRow.Init());
                CultureInfo.CurrentCulture=new CultureInfo("de-DE");
                var localized=OriginalMarqueeData.Read(JObject.Parse("{\"CASSHOWBT\":\"1,5_2,5\",\"popCas\":[],\"zsRat\":[]}"));localized.Init();
                Check(localized.CashMinimum==1.5f && localized.CashMaximum==2.5f,"Range parsing follows current culture");
                Debug.Log("NUT_MARQUEE_DATA_VALIDATION_PASS source projection/init order, cash/coin merge, unusual selection boundary, missing-data request, random draw count, malformed/empty data and preserved JSON.");
            }
            finally { CultureInfo.CurrentCulture=previous; }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
        private static void Expect<T>(Action action) where T:Exception
        { try { action(); } catch(T) { return; } throw new InvalidOperationException("Expected "+typeof(T).Name); }
    }
}
