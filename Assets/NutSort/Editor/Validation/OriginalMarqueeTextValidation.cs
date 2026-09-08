using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalMarqueeTextValidation
    {
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var defaults=ScriptableObject.CreateInstance<OriginalUserDefaults>();
            try
            {
                var user=new OriginalUserLocalData(defaults);
                var trace=new List<string>();float formatted=0;
                var text=new OriginalMarqueeText(user,tables,()=>{trace.Add("name");return "Player_1234";},n=>{trace.Add("format");formatted=n;return "amount";},(a,b)=>{trace.Add("range");Check(a==5 && b==9,"Row range arguments");return 7;});
                var item=new OriginalMarqueeItem { Minimum=5,Maximum=9,IsGold=true };
                string result=text.Build(item,3,"en");
                Check(result==tables.Text.GetText(35,"en","Player_1234","amount") && formatted==7 && string.Join(",",trace)=="name,range,format","Cash branch and native random consumption order");
                item.IsGold=false;trace.Clear();
                Check(text.Build(item,3,"ja")==tables.Text.GetText(153,"ja","Player_1234","amount"),"Noncash uses text 153 with the same currency formatter");
                user.Level=100;user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_list\":[{\"psi_value\":\"10\"},{\"psi_value\":\"20\"}],\"keep\":true}");
                string before=user.GoldRewardTargetS2CData.ToString();
                foreach(int level in new[]{1,2,0,-1})
                {
                    trace.Clear();text.Build(null,level,"en");
                    Check(formatted==(level==1?10:20) && string.Join(",",trace)=="name,format","Early text uses captured level, does not touch PMD item or draw a value");
                }
                Check(before==user.GoldRewardTargetS2CData.ToString(),"Text leaves reward data unchanged");
                user.GoldRewardTargetS2CData=null;trace.Clear();
                bool failed=false;try{text.Build(null,1,"en");}catch(NullReferenceException){failed=true;}
                Check(failed && trace.Count==0,"Missing early reward fails before name/format calls");
                text.Build(item,50,"en");Check(formatted==7,"Later branch requires no early reward document");
                Debug.Log("NUT_MARQUEE_TEXT_VALIDATION_PASS captured-level branches, original text IDs, early reward rows, name/value call order and unchanged user data.");
            }
            finally { UnityEngine.Object.DestroyImmediate(defaults); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
