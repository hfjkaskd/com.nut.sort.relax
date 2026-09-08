using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalToolItemDisplayValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var gray=Resources.Load<Material>("Materials/GrayMaterial");Check(gray!=null && !ShaderUtil.ShaderHasError(gray.shader),"Recovered gray material");
            foreach(string prefab in new[]{"RevokeItem","ExchangeItem","AddScrewItem"})
            {
                GameObject instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/"+prefab));
                try
                {
                    var view=instance.GetComponent<OriginalToolItemDisplay>();
                    Check(instance.GetComponentsInChildren<Transform>(true).Length==5 && view.Icon.sprite!=null && view.Value.font!=null,"Original hierarchy, icon and font");
                    foreach(var node in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject)==0,"No missing scripts");
                    bool level=false;int history=0,historyReads=0,levelReads=0;
                    view.Bind(user,()=>{levelReads++;return level;},()=>{historyReads++;return history;});
                    foreach(int count in new[]{7,0,-3})
                    {
                        user.RevokeCount=count;user.ExchangeCount=count;user.AddScrewCount=count;
                        view.Refresh();Check(view.Value.text==(count>0?"7":""),"Positive count only");
                        Check(view.CountIcon.sprite==Resources.Load<Sprite>("Atlas/MainPanel/"+(count>0?"item_count_bg":"item_add")) && view.CountIcon.sprite!=null,"Count background or plus resource");
                    }
                    user.ServerConfigData=null;view.RefreshButtonState();Check(historyReads==0 && view.Click.interactable,"No level bypasses history/config without disabling click");
                    if(view.ItemType==3)
                    {
                        view.Icon.material=gray;level=true;view.RefreshButtonState();Check(levelReads==0 && view.Icon.material==gray,"Exchange gray refresh is a no-op");continue;
                    }
                    level=true;
                    if(view.ItemType==4)
                    {
                        bool failed=false;try{view.RefreshButtonState();}catch(NullReferenceException){failed=true;}Check(failed,"Missing live config fails");
                        user.ServerConfigData=new JObject();user.CurrentLevelAddScrewCount=11;view.RefreshButtonState();Check(view.Icon.material!=gray,"Native default limit is 12");
                        user.CurrentLevelAddScrewCount=12;
                    }
                    view.RefreshButtonState();Check(view.Icon.material==gray && view.CountIcon.material==gray && view.Click.interactable,"Gray all active images, preserve click");
                    var fontMaterial=view.Value.fontSharedMaterial;view.CountIcon.gameObject.SetActive(false);
                    if(view.ItemType==2)history=1;
                    else {user.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":0,\"lsslsmac\":13}");}
                    string configBefore=user.ServerConfigData?.ToString();view.RefreshButtonState();
                    Check(view.Icon.material!=gray && view.CountIcon.material==gray && view.Value.fontSharedMaterial==fontMaterial,"Clearing excludes inactive Image and leaves TMP material");
                    Check(user.ServerConfigData?.ToString()==configBefore,"Config unchanged");
                    view.CountIcon.gameObject.SetActive(true);view.RefreshButtonState();Check(view.CountIcon.material!=gray,"Reactivated image clears on next refresh");
                }
                finally { UnityEngine.Object.DestroyImmediate(instance); }
            }
            Debug.Log("NUT_TOOL_ITEM_DISPLAY_VALIDATION_PASS three original prefabs, count/plus visuals, history/config gray rules, default limit 12, active Image filtering, untouched TMP and preserved button interactivity.");
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
