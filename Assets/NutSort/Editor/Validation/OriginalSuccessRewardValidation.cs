using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalSuccessRewardValidation
    {
        public static void Validate()
        {
            var callbacks=new List<Action<JObject>>();var trace=new List<string>();OriginalItemGetInfo result=null;
            var flow=new OriginalSuccessRewardFlow((more,done)=>{trace.Add(more?"more":"normal");callbacks.Add(done);},
                (id,info)=>{Check(id==8,"Original RewardPanel ID");result=info;trace.Add("show");},()=>trace.Add("close"));
            flow.GetReward(true);flow.GetReward(false);Check(string.Join(",",trace)=="more,normal"&&result==null,"Only request occurs before callback; overlapping requests retain their flags");
            var payload=JObject.Parse(@"{""kinetic_data"":{""hg_amt"":""12.5"",""hg_psi"":""42.25"",""hg_zs_amt"":""0"",""hg_zs_psi"":""16777217.25""}}");
            trace.Clear();callbacks[1](payload);
            Check(string.Join(",",trace)=="show,close"&&!result.IsMore&&result.CallBack==null,"Show precedes close; no invented callback");
            Check(result.ItemInfos.Count==2,"Claim response preserves zero coin entry");
            var gold=result.ItemInfos[0];var coin=result.ItemInfos[1];
            Check(gold.ItemType==0&&gold.Count==12.5f&&gold.MoreCount==12.5f&&gold.CurrentCount==42.25f&&gold.DoubleCurrentCount==0&&!gold.IsMore,"Cash received amount and current balance remain distinct");
            Check(coin.ItemType==1&&coin.Count==0&&coin.MoreCount==0&&coin.CurrentCount==0&&coin.DoubleCurrentCount==16777217.25&&!coin.IsMore,"Coin current balance uses direct double parsing");
            callbacks[0](payload);Check(result.IsMore&&!result.ItemInfos[0].IsMore,"Captured aggregate More flag does not propagate to items");
            var previous=result;callbacks[0](payload);Check(!ReferenceEquals(previous,result),"Repeated response creates a new result; no invented deduplication");
            payload["kinetic_data"]["hg_amt"]="";payload["kinetic_data"]["hg_zs_amt"]=null;trace.Clear();callbacks[0](payload);
            Check(result.ItemInfos.Count==0&&string.Join(",",trace)=="show,close","Empty reward still opens RewardPanel then closes");
            var culture=CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture=new CultureInfo("fr-FR");
                callbacks[0](JObject.Parse(@"{""KINETIC_DATA"":{""HG_AMT"":""1,5"",""HG_PSI"":""2,5"",""HG_ZS_AMT"":""3,5"",""HG_ZS_PSI"":""16777217,25""}}"));
                Check(result.ItemInfos[0].Count==1.5f&&result.ItemInfos[1].DoubleCurrentCount==16777217.25,"Native current-culture conversions and case-insensitive server fields");
            }
            finally{CultureInfo.CurrentCulture=culture;}
            callbacks[0](JObject.Parse(@"{""kinetic_data"":{""hg_amt"":""bad"",""hg_zs_amt"":""bad"",""hg_zs_psi"":""bad""}}"));
            Check(result.ItemInfos.Count==2&&result.ItemInfos[0].Count==0&&result.ItemInfos[1].DoubleCurrentCount==0,"Invalid nonempty values log and remain zero-valued entries");
            trace.Clear();bool failed=false;
            try{callbacks[0](new JObject());}catch(NullReferenceException){failed=true;}
            Check(failed&&trace.Count==0,"Missing data prevents presentation and close");
            Action<JObject> pending=null;bool closed=false;
            var broken=new OriginalSuccessRewardFlow((more,done)=>pending=done,(id,info)=>throw new InvalidOperationException("show"),()=>closed=true);
            broken.GetReward(false);failed=false;
            try{pending(JObject.Parse(@"{""kinetic_data"":{}}"));}catch(InvalidOperationException){failed=true;}
            Check(failed&&!closed,"Show failure does not close the current settlement");
            Debug.Log("NUT_SUCCESS_REWARD_VALIDATION_PASS deferred request/captured More flags, cash/coin amount and balance precision, zero and empty handling, current-culture parsing, panel 8 before close and failure ordering; SDK transport, response InitLss and actual RewardPanel host remain outside this flow.");
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    }
}
