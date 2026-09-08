using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalMainBottomValidation
    {
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelBottom"));
            GameObject duplicate=null;
            try
            {
                var bottom=instance.GetComponent<OriginalMainBottomView>();
                Check(instance.GetComponentsInChildren<Transform>(true).Length==20 && instance.transform.childCount==5,"Original Bottom hierarchy");
                string[] names={"Revoke","Exchange","AddTile","SettingBg","ReplayBg"};
                for(int i=0;i<names.Length;i++)Check(instance.transform.GetChild(i).name==names[i],"Source visual order");
                foreach(var node in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject)==0,"No missing scripts");
                foreach(var button in instance.GetComponentsInChildren<Button>(true))Check(button.onClick.GetPersistentEventCount()==0 && button.GetComponent<OriginalButtonFeedback>()!=null,"Standard buttons with prefab feedback and code events");
                Check(bottom.Items.Count==3 && bottom.Items[0].Display.ItemType==4 && bottom.Items[1].Display.ItemType==2 && bottom.Items[2].Display.ItemType==3,"Source list order differs from visual order");
                var add=bottom.Items[0].Display;var revoke=bottom.Items[1].Display;var exchange=bottom.Items[2].Display;
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.AddScrewCount=3;user.RevokeCount=7;user.ExchangeCount=0;user.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":12}");
                bool allow=false;var trace=new List<string>();
                bottom.Bind(user,()=>{trace.Add("gray:"+add.Value.text+","+revoke.Value.text+","+exchange.Value.text);return true;},()=>1,()=>trace.Add("add"),()=>{},(type,amount,refresh)=>{},(active,delay)=>{},(panel,type)=>{},id=>{},()=>allow,()=>trace.Add("audio"),id=>trace.Add("panel:"+id));
                foreach(var item in bottom.Items)item.Display.Value.text="old";
                bottom.Init();bottom.Init();Check(add.Value.text=="old" && revoke.Value.text=="old","Init does not refresh inventory");
                bottom.Setting.onClick.Invoke();bottom.Replay.onClick.Invoke();Check(trace.Count==0,"Common input gate");allow=true;
                bottom.Setting.onClick.Invoke();bottom.Replay.onClick.Invoke();Check(string.Join(",",trace)=="panel:5,audio,panel:17,audio","Exact routes, callback-before-audio and no duplicate listeners");trace.Clear();
                bottom.Refresh();Check(string.Join(";",trace)=="gray:3,old,old;gray:3,7,old" && exchange.Value.text=="","Each item refreshes quantity then gray before next item");trace.Clear();
                user.RevokeCount=9;bottom.RefreshOtherItem(2);Check(revoke.Value.text=="9" && trace.Count==0,"Quantity-only refresh");
                user.RevokeCount=10;bottom.RefreshOtherItemButtonState(2);Check(revoke.Value.text=="9" && trace.Count==1,"State-only refresh");trace.Clear();
                bottom.RefreshOtherItem(999);bottom.RefreshOtherItemButtonState(999);Check(trace.Count==0,"Unmatched local refresh silently does nothing");
                duplicate=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/RevokeItem"));var other=duplicate.GetComponent<OriginalToolItemController>();other.Display.Bind(user,()=>true,()=>0);other.Display.Value.text="duplicate";
                var serialized=new SerializedObject(bottom);var items=serialized.FindProperty("items");items.arraySize=4;items.GetArrayElementAtIndex(3).objectReferenceValue=other;serialized.ApplyModifiedPropertiesWithoutUndo();
                bottom.RefreshOtherItem(2);Check(revoke.Value.text=="10" && other.Display.Value.text=="duplicate" && bottom.GetOtherItem(2)==bottom.Items[1],"Local refresh and lookup stop at first match");
                add.Click.onClick.Invoke();Check(trace[trace.Count-2]=="add" && trace[trace.Count-1]=="audio","Tool controller wired through Bottom");
                Debug.Log("NUT_MAIN_BOTTOM_VALIDATION_PASS original full hierarchy, visual/list orders, settings/replay/tool bindings, paired full refresh and separate first-match updates.");
            }
            finally { if(duplicate!=null)UnityEngine.Object.DestroyImmediate(duplicate);UnityEngine.Object.DestroyImmediate(instance); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
