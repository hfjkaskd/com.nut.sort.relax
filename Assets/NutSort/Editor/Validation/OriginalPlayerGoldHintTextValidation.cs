using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalPlayerGoldHintTextValidation
    {
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var trace=new List<string>();float amount=0;
            var text=new OriginalPlayerGoldHintText(user,tables,n=>{amount=n;trace.Add("currency");return "amount";},()=>{trace.Add("name");return "Player_AI29";},(a,b)=>{Check(a==5 && b==9,"Original row range");trace.Add("range");return 7;});
            var item=new OriginalMarqueeItem { IsGold=true,Minimum=5,Maximum=9 };
            Check(text.PushName("en")==tables.Text.GetText(95,"en","Player_AI29"),"Separate push name text 95");
            user.Level=3;
            Check(text.PushInfo(item,"en")==tables.Text.GetText(96,"en","amount") && amount==7 && string.Join(",",trace)=="name,range,currency","Cash push info and name/value order");
            item.IsGold=false;trace.Clear();
            Check(text.PushInfo(item,"ja")==tables.Text.GetText(152,"ja","amount") && amount==7,"Coin push uses 152 and cash formatter");
            user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_list\":[{\"psi_value\":\"10\"},{\"psi_value\":\"20\"}],\"keep\":true}");
            string before=user.GoldRewardTargetS2CData.ToString();
            foreach(int level in new[]{1,2,0,-1})
            {
                user.Level=level;trace.Clear();
                Check(text.PushInfo(null,"en")==tables.Text.GetText(97,"en","amount") && amount==(level==1?10:20) && string.Join(",",trace)=="currency","Early live-level reward row without PMD value sampling");
            }
            Check(before==user.GoldRewardTargetS2CData.ToString(),"Reward document unchanged");
            trace.Clear();Check(text.SelfName("en")==tables.Text.GetText(94,"en") && trace.Count==0,"Self label has no generated name");
            Check(text.SelfInfo(12.5f,"en")==tables.Text.GetText(96,"en","amount") && amount==12.5f && string.Join(",",trace)=="currency","Self uses supplied amount and text 96");
            user.Level=3;user.GoldRewardTargetS2CData=null;text.PushInfo(item,"en");
            user.Level=1;trace.Clear();bool failed=false;
            try { text.PushInfo(null,"en"); } catch(NullReferenceException) { failed=true; }
            Check(failed && trace.Count==0,"Missing early reward fails before currency formatting");
            Debug.Log("NUT_PLAYER_GOLD_HINT_TEXT_VALIDATION_PASS distinct name/info IDs, live-level early reward rows, cash formatting for both types and self amount without generated name.");
        }
        private static void Check(bool condition,string message) { if(!condition)throw new InvalidOperationException(message); }
    }
}
