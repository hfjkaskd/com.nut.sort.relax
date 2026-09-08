using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalSuccessSettlementValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){Level=5,Gold=10,Coin=20};
            user.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[{""psi_value"":""12""},{""psi_value"":""34""}]}");
            var trace=new List<string>();var queue=new OriginalPanelActionQueue();bool modal=false;OriginalItemGetInfo shown=null;
            var flow=new OriginalSuccessSettlementFlow(user,()=>trace.Add("save"),id=>{Check(id==11,"Native modal ID");trace.Add("check");return modal;},
                callback=>{trace.Add("queue");queue.Add(callback);},()=>trace.Add("close"),(id,info)=>{Check(id==9,"Native success panel ID");trace.Add("show");shown=info;});
            var response=JObject.Parse(@"{""kinetic_data"":{""hg_amt"":""2.5"",""bear_video"":""3"",""hg_zs_amt"":""7""}}");
            flow.Run(response);Check(string.Join(",",trace)=="check,close,show","Direct branch closes before show without save");
            Check(shown.ItemInfos.Count==2&&shown.ItemInfos[0].Count==2.5f&&shown.ItemInfos[0].MoreCount==7.5f&&shown.ItemInfos[1].ItemType==1&&shown.ItemInfos[1].MoreCount==0,"Gold then coin and float multiplier");
            Check(!shown.IsMore&&shown.CallBack==null&&user.Gold==10&&user.Coin==20,"Presentation defaults and unchanged balances");
            user.Level=2;response["kinetic_data"]["hg_amt"]="";response["kinetic_data"]["hg_zs_amt"]="0";trace.Clear();flow.Run(response);
            Check(user.Level1Gold==12&&shown.ItemInfos.Count==1&&shown.ItemInfos[0].Count==12&&shown.ItemInfos[0].MoreCount==12&&string.Join(",",trace)=="save,check,close,show","Level two target reward and save-before-routing");
            user.Level=3;trace.Clear();modal=true;shown=null;flow.Run(response);Check(user.Level2Gold==34&&queue.Count==1&&shown==null&&string.Join(",",trace)=="save,check,queue","Level three second target reward and deferred display");
            response["kinetic_data"]["hg_amt"]="999";trace.Clear();queue.Dequeue();Check(shown.ItemInfos[0].Count==34&&string.Join(",",trace)=="close,show","Queue retains built payload without rereading response");
            modal=false;user.Level=5;response["kinetic_data"]["hg_amt"]="";response["kinetic_data"]["hg_zs_amt"]="0.0";flow.Run(response);
            Check(shown.ItemInfos.Count==1&&shown.ItemInfos[0].ItemType==1&&shown.ItemInfos[0].Count==0,"Only exact string zero is skipped; no general numeric zero filter");
            response["kinetic_data"]["hg_zs_amt"]="";flow.Run(response);Check(shown.ItemInfos.Count==0,"Empty reward list still reaches settlement");
            Debug.Log("NUT_SUCCESS_SETTLEMENT_VALIDATION_PASS gold/multiplier/coin mapping, ordered items, unchanged balances, early level target fields and save, modal queue with captured payload, exact zero string and empty list behavior.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
