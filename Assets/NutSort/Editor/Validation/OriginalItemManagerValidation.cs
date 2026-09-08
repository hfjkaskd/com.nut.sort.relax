using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalItemManagerValidation
    {
        private sealed class Preferences : IOriginalUserPreferences
        {
            public string Json;
            public readonly List<string> Trace=new List<string>();
            public string GetString(string key,string fallback)=>Json??fallback;
            public void SetString(string key,string value) { Check(key==OriginalUserStore.Key,"Original save key");Json=value;Trace.Add("write"); }
            public void Save() { Trace.Add("flush"); }
        }
        public static void Validate()
        {
            float[] inputs={.5f,1.5f,2.5f,-.5f,-1.5f,-2.5f,1.4f,-1.6f,float.NaN,float.PositiveInfinity,float.NegativeInfinity,float.MaxValue};
            int[] expected={0,2,2,0,-2,-2,1,-2,0,int.MinValue,int.MinValue,int.MaxValue};
            for(int i=0;i<inputs.Length;i++)Check(new OriginalItemInfo { Count=inputs[i] }.IntCount==expected[i],"Native integer conversion");
            Check(new OriginalItemInfo { Count=1,MoreCount=3.5f,IsMore=true }.IntCount==4,"IsMore selects alternate quantity");
            var prefs=new Preferences();var store=new OriginalUserStore(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"),prefs);
            var user=store.Data;user.RevokeCount=2;user.ExchangeCount=1;user.AddScrewCount=3;user.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":12}");
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelBottom"));
            try
            {
                var bottom=instance.GetComponent<OriginalMainBottomView>();
                var manager=new OriginalItemManager(user,()=>store.SaveData(false,null),()=>
                {
                    Check((int)JObject.Parse(prefs.Json)["RevokeCount"]==user.RevokeCount,"Persist before display refresh");prefs.Trace.Add("refresh");bottom.Refresh();
                },(amount,a,b)=>{Check(amount==17.25f && a && b,"Gold uses CurrentCount and fixed flags");prefs.Trace.Add("gold");},(amount,flag)=>{Check(amount==123456789.25 && flag,"Coin uses DoubleCurrentCount and fixed flag");prefs.Trace.Add("coin");});
                bottom.Bind(user,()=>true,()=>1,()=>{},()=>prefs.Trace.Add("revoke"),manager.AddTool,(active,delay)=>{},(panel,type)=>{},id=>{},()=>true,()=>prefs.Trace.Add("audio"),id=>{});
                bottom.Init();bottom.Refresh();prefs.Trace.Clear();bottom.GetOtherItem(2).Display.Click.onClick.Invoke();
                Check(user.RevokeCount==1 && bottom.GetOtherItem(2).Display.Value.text=="1" && string.Join(",",prefs.Trace)=="revoke,write,flush,refresh,audio","Actual Button to item manager, store and full Bottom refresh");
                prefs.Trace.Clear();manager.Add(new OriginalItemInfo { ItemType=3,Count=2,MoreCount=3.5f,IsMore=true },false);
                Check(user.ExchangeCount==5 && string.Join(",",prefs.Trace)=="write,flush","False refresh still persists selected rounded quantity");
                prefs.Trace.Clear();manager.AddTool(4,-5,true);Check(user.AddScrewCount==-2 && string.Join(",",prefs.Trace)=="write,flush,refresh","No inventory clamp");
                prefs.Trace.Clear();user.RevokeCount=int.MaxValue;manager.AddTool(2,1,false);Check(user.RevokeCount==int.MinValue,"Native integer addition wraps");
                prefs.Trace.Clear();manager.Add(new OriginalItemInfo { ItemType=0,Count=999,CurrentCount=17.25f },false);manager.Add(new OriginalItemInfo { ItemType=1,Count=999,DoubleCurrentCount=123456789.25 },false);
                Check(string.Join(",",prefs.Trace)=="gold,coin","Currency delegates own persistence and ignore item refresh flag");
                prefs.Trace.Clear();manager.AddTool(99,1,true);Check(prefs.Trace.Count==0,"Unknown type does nothing");
                bool failed=false;try{manager.Add(null);}catch(NullReferenceException){failed=true;}Check(failed,"Null item fails");
                user.RevokeCount=7;var failure=new OriginalItemManager(user,()=>{throw new InvalidOperationException("fixture save failure");},()=>prefs.Trace.Add("unexpected"),(v,a,b)=>{},(v,a)=>{});
                failed=false;try{failure.AddTool(2,-1,true);}catch(InvalidOperationException){failed=true;}Check(failed && user.RevokeCount==6 && prefs.Trace.Count==0,"Mutation precedes failed save; refresh is not run");
                Debug.Log("NUT_ITEM_MANAGER_VALIDATION_PASS native rounded quantities and type routing, actual tool Button/store/Bottom integration, save-before-refresh, flags, negative/overflow states and failure ordering.");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
