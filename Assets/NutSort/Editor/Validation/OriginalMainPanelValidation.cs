using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalMainPanelValidation
    {
        public static void BindFixture(OriginalMainPanelView panel,OriginalUserLocalData user,OriginalTables tables,Action<float,Action> delay,Action<bool,float> exchange)
        {
            var formatter=new OriginalGoldFormatter(()=>"en-US");
            panel.Top.Bind(user,tables,"en",formatter,()=>"US",()=>true,done=>done(),done=>done(),id=>{},()=>{},()=>true,()=>null,()=>false,delay);
            panel.Bottom.Bind(user,()=>true,()=>1,()=>{},()=>{},(type,amount,refresh)=>{},exchange,(id,type)=>{},id=>{},()=>true,()=>{},id=>{});
            panel.Bind(tables,"en",()=>true,()=>{},exchange);
        }
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete"));
            try
            {
                var panel=instance.GetComponent<OriginalMainPanelView>();
                Check(instance.GetComponentsInChildren<Transform>(true).Length==105 && instance.transform.childCount==4 && instance.transform.Find("main")==null,"Original full MainPanel without panel tween child");
                foreach(var node in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject)==0,"No missing scripts");
                Check(panel.PlayerHint==panel.Top.PlayerHint,"MainPanel and Top reference the same hint");
                var user=OriginalMainTopViewValidation.MakeUser();var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var trace=new List<string>();BindFixture(panel,user,tables,(seconds,done)=>{},(active,delay)=>{Check(!active && delay==.5f,"Original exchange cancel args");trace.Add("exchange");});
                bool allow=false;panel.Bind(tables,"en",()=>allow,()=>trace.Add("audio"),(active,delay)=>{Check(!active && delay==.5f,"Original exchange cancel args");trace.Add("exchange");});
                var maskLabel=panel.ExchangeMask.GetComponentInChildren<TextMeshProUGUI>(true);Check(maskLabel!=null,"Original mask text");
                string originalMaskText=maskLabel.text;
                TextMeshProUGUI localized=null;
                foreach(var label in instance.GetComponentsInChildren<TextMeshProUGUI>(true))if(label.name.StartsWith("label_")) { localized=label;break; }
                Check(localized!=null,"Original localized label");localized.gameObject.SetActive(false);localized.text="sentinel";
                panel.ExchangeMask.gameObject.SetActive(false);
                panel.PlayerHint.Push();panel.TargetReward.transform.localPosition=new Vector3(123,456,0);
                panel.Init();Check(panel.PlayerHint.LastShowTime==-1 && panel.PlayerHint.transform.localPosition==new Vector3(0,500,0),"Parent owns hint initialization");
                Check(panel.TargetReward.transform.localPosition!=new Vector3(123,456,0),"Parent initializes banner");
                int id=int.Parse(localized.name.Split('_')[1]);Check(localized.text==tables.Text.GetText(id,"en") && maskLabel.text==originalMaskText,"Base pass includes hidden prefixed labels and leaves unprefixed text unchanged");
                Check(!panel.ExchangeMask.gameObject.activeSelf && panel.ExchangeMask.transition==Selectable.Transition.None && panel.ExchangeMask.GetComponent<OriginalButtonFeedback>()==null,"Mask starts hidden with no press scaling");
                Check(panel.ExchangeMask.onClick.GetPersistentEventCount()==0,"Code mask listener");
                panel.ExchangeMask.onClick.Invoke();Check(trace.Count==0,"Mask click gate");allow=true;
                panel.Init();panel.ExchangeMask.onClick.Invoke();Check(string.Join(",",trace)=="exchange,audio","Repeated Init retains one cancel then audio callback");
                panel.PlayerHint.Push();long deadline=panel.PlayerHint.LastShowTime;panel.TargetReward.transform.localPosition=new Vector3(12,34,0);panel.ExchangeMask.gameObject.SetActive(true);
                user.RevokeCount=9;user.Gold=27;panel.Refresh();Check(panel.Bottom.GetOtherItem(2).Display.Value.text=="9" && panel.Top.Gold.Value.text.Contains("27"),"Parent refresh updates both regions");
                Check(panel.PlayerHint.LastShowTime==deadline && panel.TargetReward.transform.localPosition==new Vector3(12,34,0) && panel.ExchangeMask.gameObject.activeSelf,"Refresh does not reset overlays or schedules");
                Debug.Log("NUT_MAIN_PANEL_VALIDATION_PASS full original composition, shared hint identity, hidden label localization, parent Init/Refresh and exchange mask cancellation without scaling or duplicate callbacks.");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
