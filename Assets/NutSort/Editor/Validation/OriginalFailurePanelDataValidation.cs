using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalFailurePanelDataValidation
    {
        public static void Validate()
        {
            var user = new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_zs_show\":\"  $001,200.00  \"}");
            user.ServerConfigData=new JObject();
            var trace=new List<string>();string shown=null;bool gray=false;
            var data=new OriginalFailurePanelData(user,text=>{shown=text;trace.Add("value");},value=>{gray=value;trace.Add("gray");},12);
            user.CurrentLevelAddScrewCount=11;data.Refresh();
            Check(shown=="  $001,200.00  " && !gray && string.Join(",",trace)=="value,gray","Verbatim text precedes gray with source constructor default");
            user.CurrentLevelAddScrewCount=12;data.Refresh();Check(gray,"Equality reaches maximum");
            user.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":99,\"lsslsmac\":3}");
            user.CurrentLevelAddScrewCount=3;data.Refresh();Check(gray,"Latest case-insensitive configuration field wins");
            user.GoldRewardTargetS2CData=new JObject();data.Refresh();Check(shown==null,"Missing source string is null");
            user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_zs_show\":\"\"}");data.Refresh();Check(shown==string.Empty,"Empty string stays empty");
            user.GoldRewardTargetS2CData=null;trace.Clear();ExpectNull(data.Refresh);
            Check(trace.Count==0,"Missing reward fails before both presentation writes");
            user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_zs_show\":\"next\"}");user.ServerConfigData=null;
            ExpectNull(data.Refresh);Check(shown=="next" && string.Join(",",trace)=="value","Missing config fails after text write");
            user.ServerConfigData=new JObject();user.CurrentLevelAddScrewCount=0;
            var changing=new OriginalFailurePanelData(user,text=>user.CurrentLevelAddScrewCount=12,value=>gray=value,12);
            changing.Refresh();Check(gray,"Live count is read after text callback");
            Debug.Log("NUT_FAILURE_PANEL_DATA_VALIDATION_PASS raw reward display, revive gray threshold, source default, live refresh order, null/empty values and missing data failure boundaries.");
        }
        private static void ExpectNull(Action action) {bool caught=false;try{action();}catch(NullReferenceException){caught=true;}Check(caught,"Expected missing source data failure");}
        private static void Check(bool value,string message) {if(!value)throw new InvalidOperationException(message);}
    }
}
