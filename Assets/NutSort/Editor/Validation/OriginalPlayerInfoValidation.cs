using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalPlayerInfoValidation
    {
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>("Prefabs/PlayerInfo/PlayerInfo");Check(prefab!=null,"PlayerInfo prefab loads");
            Check(prefab.GetComponentsInChildren<RectTransform>(true).Length==13,"Original thirteen-object hierarchy");
            foreach(var t in prefab.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
            foreach(var i in prefab.GetComponentsInChildren<Image>(true))Check(i.sprite!=null,"Original image resolves: "+i.name);
            foreach(var t in prefab.GetComponentsInChildren<TMP_Text>(true))Check(t.font!=null&&t.fontSharedMaterial!=null,"Original fonts/materials resolve");
            var obj=UnityEngine.Object.Instantiate(prefab);var view=obj.GetComponent<OriginalPlayerInfo>();
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var formatter=new OriginalGoldFormatter(()=>"en-US");bool data=false;int reads=0,created=0;
                user.ServerConfigData=JObject.Parse(@"{""LSS260820"":true}");
                Action<int> init=level=>view.Init(level,user,tables,"en",()=>{reads++;return data;},()=>"9/8/2026",()=>46,()=>17.5f,v=>formatter.Format(v),(parent,l)=>{Check(parent==view.MarqueeParent&&l==level&&obj.activeSelf,"PMD init follows activation and receives captured level");created++;});
                obj.transform.localPosition=Vector3.one;view.DateLabel.text="unchanged";init(1);
                Check(!obj.activeSelf&&obj.transform.localPosition==Vector3.zero&&reads==0&&view.DateLabel.text=="unchanged","New mode hides and resets position before any data or text access");
                user.ServerConfigData["LSS260820"]=false;init(1);
                Check(!obj.activeSelf&&reads==1&&created==0&&view.DateLabel.text=="unchanged","No PMD response leaves block hidden after static localization");
                data=true;user.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[{""psi_value"":""5""},{""psi_value"":""10""}]}");
                init(1);Check(obj.activeSelf&&view.DateLabel.text==tables.Text.GetText(31,"en","9/8/2026")&&view.PeopleLabel.text=="46"&&view.ChallengeLabel.text=="1"&&view.GoldLabel.text==formatter.Format(5)&&created==1,"First level renders actual labels and first reward row");
                init(2);Check(view.GoldLabel.text==formatter.Format(10)&&created==2,"Second level uses second reward row and requests another PMD instance");
                init(0);Check(view.GoldLabel.text==formatter.Format(10),"Other levels below three use second reward row");
                user.GoldRewardTargetS2CData=null;user.TodayChallengeTimes=7;init(3);
                Check(view.GoldLabel.text==formatter.Format(17.5f)&&view.ChallengeLabel.text=="7"&&created==4,"Later level reads live challenge count and PMD random value without reward document");
                var lifecycle=new OriginalWithdrawalPanelFlow(user,()=>false,b=>{},()=>false,id=>{},()=>{});
                lifecycle.Init(new object[]{3},()=>{},()=>{},()=>{});
                bool goldTween=true;int tip=0;user.IsGuideGold=false;
                lifecycle.FinishRefresh(level=>{Check(goldTween,"PlayerInfo precedes tween reset");init(level);},
                    value=>{goldTween=value;user.IsGuideGold=true;},id=>tip=id);
                Check(!goldTween&&tip==159&&created==5,"Actual PlayerInfo initialized before reset and live guide flag read");
                goldTween=true;tip=0;bool tailFailed=false;
                try{lifecycle.FinishRefresh(level=>throw new InvalidOperationException("PlayerInfo fixture"),value=>goldTween=value,id=>tip=id);}
                catch(InvalidOperationException){tailFailed=true;}
                Check(tailFailed&&goldTween&&tip==0,"PlayerInfo failure preserves tween flag and skips final hint");
                user.IsGuideGold=false;tip=0;lifecycle.FinishRefresh(init,value=>goldTween=value,id=>tip=id);
                Check(tip==0&&created==6,"Each refresh initializes another PlayerInfo; false guide flag retains tip");
                bool failed=false;user.ServerConfigData=null;obj.transform.localPosition=Vector3.one;
                try{init(3);}catch(NullReferenceException){failed=true;}
                Check(failed&&!obj.activeSelf&&obj.transform.localPosition==Vector3.zero,"Missing config fails after hide and position reset");
                user.ServerConfigData=JObject.Parse(@"{""LSS260820"":false}");
                failed=false;
                try{view.Init(3,user,tables,"en",()=>true,()=>null,()=>46,()=>9,v=>formatter.Format(v),(parent,l)=>throw new InvalidOperationException("PMD fixture"));}
                catch(InvalidOperationException){failed=true;}
                Check(failed&&obj.activeSelf&&view.GoldLabel.text==formatter.Format(9)&&view.DateLabel.text==tables.Text.GetText(31,"en",""),"Child initialization failure preserves displayed parent; null date becomes empty string");
            }
            finally{UnityEngine.Object.DestroyImmediate(obj);}
            Debug.Log("NUT_PLAYER_INFO_VALIDATION_PASS original prefab hierarchy/images/fonts, mode and response gates, date/people/challenge/gold labels, first/second/later reward branches, repeat child request, shared withdrawal tail and failure order; actual PMDSimple and TXPanel binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
