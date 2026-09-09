using System;
using System.Collections.Generic;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalMarqueeRequestsValidation
    {
        public const string Success="{\"message_status\":\"fixture\",\"kinetic_gap\":\"NO-0\",\"Time\":42,\"kinetic_data\":{\"casShowBt\":\"5_8\",\"popCas\":[{\"wt\":2,\"lw\":7,\"hh\":7}],\"zsRat\":[{\"wt\":3,\"lw\":4,\"hh\":4}]}}";
        public static void Validate()
        {
            var prior=OriginalMarqueeRequests.Cached;
            try
            {
                OriginalMarqueeRequests.Cached=null;var pending=new List<Action<string,byte[]>>();int callbacks=0;
                var requests=new OriginalMarqueeRequests((version,done)=>{Check(version=="-1","Native version_value");pending.Add(done);},(a,b)=>3);
                requests.Initialize();Check(pending.Count==1&&OriginalMarqueeRequests.Cached==null,"Startup requests without fabricated response");
                Check(requests.Next()==null&&pending.Count==2,"Missing data requests each time without in-flight throttle");
                pending[0](null,new byte[]{1});Check(OriginalMarqueeRequests.Cached==null,"Null string ignores bytes and leaves cache");
                pending[1](Success,null);var first=OriginalMarqueeRequests.Cached;
                Check(first.IsSuccess&&first.Time==42&&first.kinetic_data.Items.Count==2&&first.kinetic_data.CashMinimum==5&&first.kinetic_data.CashMaximum==8,"Unity deserialization and success initialization");
                Check(ReferenceEquals(first.kinetic_data.Items[0],first.kinetic_data.CashItems[0])&&first.kinetic_data.Items[0].IsGold,"Initialized row aliases");
                Check(ReferenceEquals(requests.Next(),first.kinetic_data.Items[0]),"Actual selector reads response cache");
                var shared=new OriginalMarqueeRequests((v,d)=>throw new Exception("Cached request must not send"));
                shared.Request(value=>{Check(ReferenceEquals(value,first),"Shared synchronous cache identity");callbacks++;});
                Expect<InvalidCastException>(()=>shared.PMD(row=>callbacks++));Check(callbacks==1,"Source manager invalid cast before supplied callback");shared.Initialize();
                OriginalMarqueeRequests.Cached=null;
                requests.Request(value=>{Check(value==null,"Empty receive callback is null");callbacks++;});pending[pending.Count-1]("",null);Check(callbacks==2&&OriginalMarqueeRequests.Cached==null,"Empty receive doesn't cache");
                requests.Request(value=>{Check(value==null,"Failed receive callback is null");callbacks++;});pending[pending.Count-1]("{\"kinetic_gap\":\"NO-1\",\"kinetic_data\":{\"casShowBt\":\"bad\"}}",null);
                var failed=OriginalMarqueeRequests.Cached;Check(!failed.IsSuccess&&failed.kinetic_data.CashMinimum==0&&callbacks==3,"Failed envelope cached without Init");
                shared.Request(value=>Check(ReferenceEquals(value,failed),"Cached failed envelope bypasses success check"));
                OriginalMarqueeRequests.Cached=null;requests.Request(value=>callbacks++);var malformed=pending[pending.Count-1];
                Expect<IndexOutOfRangeException>(()=>malformed(Success.Replace("5_8","5"),null));Check(OriginalMarqueeRequests.Cached!=null&&OriginalMarqueeRequests.Cached.kinetic_data.CashMinimum==5&&callbacks==3,"Cache published before failing Init, callback skipped");
                var published=OriginalMarqueeRequests.Cached;Expect<ArgumentException>(()=>malformed("{",null));Check(ReferenceEquals(published,OriginalMarqueeRequests.Cached),"Parse failure retains previous cache");
                OriginalMarqueeRequests.Cached=null;requests.Request(value=>{Check(OriginalMarqueeRequests.Cached.kinetic_data.Items.Count==2,"Init before callback");throw new InvalidOperationException();});Expect<InvalidOperationException>(()=>pending[pending.Count-1](Success,null));
                Check(OriginalMarqueeRequests.Cached.IsSuccess,"Callback failure retains initialized cache");
                OriginalMarqueeRequests.Cached=null;var synchronous=new OriginalMarqueeRequests((v,d)=>d(Success,null));Check(synchronous.Next()==null&&OriginalMarqueeRequests.Cached!=null,"First missing selection returns null despite synchronous completion");
                OriginalMarqueeRequests.Cached=null;requests.Request();requests.Request();var older=pending[pending.Count-2];var newer=pending[pending.Count-1];newer(Success,null);older(Success.Replace("5_8","9_10"),null);Check(OriginalMarqueeRequests.Cached.kinetic_data.CashMinimum==9,"Native arrival order overwrites cache without stale-request suppression");
                var wrongCase=JsonUtility.FromJson<OriginalMarqueeResponse>(Success.Replace("kinetic_gap","KINETIC_GAP"));Check(!wrongCase.IsSuccess,"Native Unity field name matching");
            }
            finally{OriginalMarqueeRequests.Cached=prior;}
            Debug.Log("NUT_MARQUEE_REQUESTS_VALIDATION_PASS native JsonUtility envelope, success-gated Init and shared cache publication, startup/missing selection, cached failure, empty/parse/init/callback failure order, arrival order and source manager cast; transport remains explicit and no response is fabricated.");
        }
        private static void Expect<T>(Action action) where T:Exception {try{action();}catch(T){return;}throw new InvalidOperationException("Expected "+typeof(T).Name);}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
