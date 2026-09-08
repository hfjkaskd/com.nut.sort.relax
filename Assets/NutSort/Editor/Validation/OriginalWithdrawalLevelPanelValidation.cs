using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalLevelPanelValidation
    {
        public sealed class Services:IOriginalWithdrawalLevelUI
        {
            public int Checks;
            public void SetTip(int id,int level)=>throw new InvalidOperationException("View owns tip");
            public void SetGold(string value)=>throw new InvalidOperationException("View owns gold");
            public void PlaySteps()=>throw new InvalidOperationException("View owns steps");
            public void Close()=>throw new InvalidOperationException("View owns closing");
            public bool HasPanel(int id){Check(id==5,"Native auxiliary panel query");Checks++;return false;}
            public void HidePanel(int id)=>throw new InvalidOperationException("No auxiliary panel in fixture");
            public void RefreshGoldItem()=>throw new InvalidOperationException("Held transport must not mutate HUD");
            public void ShowSelf(float amount)=>throw new InvalidOperationException("Held transport must not display payment result");
            public void RefreshGold(bool tip,float add)=>throw new InvalidOperationException("Held transport must not refresh payment result");
        }
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.Level1Gold=5;user.Gold=12;
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXLevelPanel")).GetComponent<OriginalWithdrawalLevelPanel>();
            var services=new Services();bool allowed=false;int sounds=0,closes=0,saves=0;Action<JObject> held=null;
            bool old=OriginalWithdrawalLevelFlow.IsRecordIn;OriginalWithdrawalLevelFlow.IsRecordIn=false;
            try
            {
                panel.Bind(user,tables,"en",v=>new OriginalGoldFormatter(()=>"en-US").Format(v),()=>allowed,s=>sounds++,services,
                    (level,cb)=>{Check(level==1&&panel.Closing,"Actual level request after close starts");held=cb;},()=>saves++,v=>null,()=>closes++,()=>{},(d,cb)=>{},()=>{});
                panel.Init(new object[]{1});panel.Refresh();
                Check(panel.GetComponentsInChildren<Transform>(true).Length==37,"Source thirty-seven-object hierarchy");
                foreach(var c in panel.GetComponentsInChildren<Component>(true))Check(c!=null,"No missing component");
                Check(panel.GetComponentsInChildren<Button>(true).Length==2,"Source two standard Buttons");
                foreach(var b in panel.GetComponentsInChildren<Button>(true))Check(b.onClick.GetPersistentEventCount()==0&&b.GetComponent<OriginalButtonFeedback>()!=null,"Code events and prefab visual feedback");
                Check(panel.GoldLabel.text=="$5"&&panel.GoldLabel.font!=null&&panel.TipLabel.text==tables.Text.GetText(89,"en",1),"Actual source amount/font/tip");
                panel.Advance(.26f);var main=panel.transform.Find("main");main.localScale=Vector3.one*.4f;panel.Advance(.04f);Near(main.localScale.x,.4f,"Completed main tween stops writing; nested Title1 is not a direct Title tween");
                var steps=panel.Steps;var root=steps.StepRoot;var row=root.Find("1");var content=row.Find("Content");
                Check(panel.ConfirmButton.transform.localScale==Vector3.zero&&row.localScale==new Vector3(1,0,1),"Refresh hides by scale without disabling Button");
                steps.Advance(.35f);Near(row.localScale.y,.75f,"Row OutQuad after source delay");Near(content.localScale.y,0,"Content waits for row completion");
                steps.Advance(.051f);Near(row.localScale.y,1,"Row completes");Near(content.localScale.y,0,"Callback-created content waits for next update");
                steps.Advance(.99f);Check(!row.Find("Done").gameObject.activeSelf&&row.Find("Loading").gameObject.activeSelf,"Done remains hidden during content delay");
                steps.Advance(.06f);Near(content.localScale.y,.75f,"Content OutQuad");Check(row.Find("Done").gameObject.activeSelf&&!row.Find("Loading").gameObject.activeSelf,"Markers switch on content start before completion");
                steps.Advance(10);Check(panel.ConfirmButton.transform.localScale==Vector3.zero,"Large update cannot consume new content tracks");
                steps.Advance(1.05f);Check(root.Find("3/Done").gameObject.activeSelf&&panel.ConfirmButton.transform.localScale==Vector3.zero,"Last content start creates button tween for next update");
                steps.Advance(.15f);Near(panel.ConfirmButton.transform.localScale.x,.75f,"Confirmation OutQuad duration .3");steps.Advance(.16f);Check(steps.ActiveTracks==0,"All independent tracks finish");
                steps.Play();steps.Play();Check(steps.ActiveTracks==6,"Repeated refresh retains earlier row tracks");
                row.localScale=new Vector3(2,.4f,3);steps.Advance(0);Near(row.localScale.y,.4f,"Zero scaled delta does not advance");
                steps.Advance(.35f);Near(row.localScale.y,.9625f,"Concurrent tracks capture startup value in dispatch order");Check(row.localScale.x==2&&row.localScale.z==3,"ScaleY preserves live other axes");
                UnityEngine.Object.DestroyImmediate(root.Find("3/Done").gameObject);
                steps.Advance(10);steps.Advance(1.05f);Check(root.Find("3/Loading").gameObject.activeSelf,"Missing Done does not hide Loading");steps.Advance(1);
                steps.Play();var extra=new GameObject("fixture extra child",typeof(RectTransform));extra.transform.SetParent(root,false);
                steps.Advance(10);steps.Advance(1.05f);steps.Advance(1);Check(panel.ConfirmButton.transform.localScale==Vector3.zero,"Last-step callback compares current live child count");UnityEngine.Object.DestroyImmediate(extra);
                panel.ConfirmButton.onClick.Invoke();Check(held==null&&!panel.Closing,"Actual Button click gate");
                allowed=true;panel.ConfirmButton.onClick.Invoke();Check(held!=null&&panel.Closing&&services.Checks==1&&sounds==1&&saves==0&&user.Gold==12,"Actual request remains held without simulated payment or state change");
                panel.Advance(.5f);Check(closes==1,"Panel close finishes through native animation");
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);OriginalWithdrawalLevelFlow.IsRecordIn=old;}
            Debug.Log("NUT_WITHDRAWAL_LEVEL_PANEL_VALIDATION_PASS source prefab/Buttons/fonts, row/content/confirmation timing, OnStart markers, OutQuad, deferred callbacks, repeated refresh, startup capture, missing Done and live child count, held request and closing.");
        }
        private static void Near(float value,float expected,string label)=>Check(Mathf.Abs(value-expected)<.002f,label+" actual="+value+" expected="+expected);
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
