using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalFailurePanelValidation
    {
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/FailPanel"));
            try
            {
                var panel=instance.GetComponent<OriginalFailurePanelView>();
                Check(instance.GetComponentsInChildren<Transform>(true).Length==18 && instance.transform.Find("main")==null,"Original topology has no main tween target");
                foreach(var node in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject)==0,"No missing components");
                var user=OriginalMainTopViewValidation.MakeUser();user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_zs_show\":\"1,250\"}");user.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":12}");
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var trace=new List<string>();bool allowed=false;
                panel.Bind(user,tables,"en",()=>allowed,s=>trace.Add(s),done=>{trace.Add("restart");done();},()=>trace.Add("clear"),()=>trace.Add("sdk-boundary"),()=>trace.Add("close"));
                panel.Init();Check(string.Join(",",trace)=="GameLose","Init plays lose sound after binding");
                foreach(var label in instance.GetComponentsInChildren<TextMeshProUGUI>(true))if(label.name.StartsWith("label_"))Check(label.text==tables.Text.GetText(int.Parse(label.name.Substring(6)),"en"),"Original localized label");
                trace.Clear();panel.Restart.onClick.Invoke();panel.Revive.onClick.Invoke();Check(trace.Count==0,"Click gate suppresses both actions");
                user.CurrentLevelAddScrewCount=12;panel.Refresh();Check(panel.CoinValue.text=="1,250" && panel.Revive.interactable,"Gray preserves interactability");
                foreach(var image in panel.Revive.GetComponentsInChildren<Image>())Check(image.material==Resources.Load<Material>("Materials/GrayMaterial"),"Gray covers active revive images");
                allowed=true;panel.Restart.onClick.Invoke();Check(string.Join(",",trace)=="restart,close,Click","Restart close precedes click audio");
                trace.Clear();panel.Revive.onClick.Invoke();Check(string.Join(",",trace)=="clear,sdk-boundary,Click","Revive clears failure before unchanged SDK boundary even when gray");
                trace.Clear();panel.Init();trace.Clear();panel.Restart.onClick.Invoke();Check(string.Join(",",trace)=="restart,close,Click","Repeated initialization replaces listeners");
                user.CurrentLevelAddScrewCount=0;panel.Refresh();Check(panel.Revive.GetComponent<Image>().material!=Resources.Load<Material>("Materials/GrayMaterial"),"Refresh clears gray");
            }
            finally {UnityEngine.Object.DestroyImmediate(instance);}
            Debug.Log("NUT_FAILURE_PANEL_VALIDATION_PASS complete 18-object source prefab, localization, reward/gray refresh, Button gate/action/audio order and listener replacement; SDK boundary is not simulated.");
        }
        private static void Check(bool value,string message) {if(!value)throw new InvalidOperationException(message);}
    }
}
