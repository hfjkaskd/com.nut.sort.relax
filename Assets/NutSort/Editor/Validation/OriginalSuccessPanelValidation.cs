using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalSuccessPanelValidation
    {
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>("Prefabs/Panels/SuccessPanel");Check(prefab!=null,"SuccessPanel loads");
            Check(prefab.GetComponentsInChildren<RectTransform>(true).Length==11,"Original eleven-object hierarchy");
            foreach(var t in prefab.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
            foreach(var image in prefab.GetComponentsInChildren<Image>(true))
                if(image.transform!=prefab.transform)Check(image.sprite!=null,"Original sprite resolved: "+image.name);
            foreach(var text in prefab.GetComponentsInChildren<TMP_Text>(true))Check(text.font!=null&&text.fontSharedMaterial!=null,"Original font and material resolve");
            foreach(var b in prefab.GetComponentsInChildren<Button>(true))Check(b.onClick.GetPersistentEventCount()==0&&b.GetComponent<OriginalButtonFeedback>()!=null,"Standard Buttons bind in code with configured feedback");
            var obj=UnityEngine.Object.Instantiate(prefab);var panel=obj.GetComponent<OriginalSuccessPanel>();
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                user.Level=2;
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var factory=new OriginalRewardItemFactory(Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem"),new OriginalGoldFormatter(()=>"en-US"),()=>"US");
                int guides=0,closed=0,sounds=0,claims=0,queued=0,hidden=0;bool gate=false;Action queuedAction=null;
                var lifecycle=new OriginalSuccessPanelFlow(user,()=>123,()=>sounds++,id=>{Check(id==7,"Native guide panel");guides++;});
                var claim=new OriginalRewardGetFlow(user,()=>100,()=>1,cb=>{},(cb,b)=>{},()=>0,v=>{},b=>claims++);
                var queue=new OriginalPanelActionQueue();queue.Add(()=>queued++);
                panel.Bind(lifecycle,claim,factory,tables,"en",()=>gate,s=>sounds++,()=>closed++,
                    baseHide=>lifecycle.Hide(baseHide,()=>false,()=>false,()=>{},()=>{}),
                    (delay,action)=>{Check(delay==2.5f,"Base hide delay");queuedAction=action;},queue,()=>hidden++);
                var info=new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=0,Count=5,MoreCount=10}}};
                panel.Init(info);panel.Refresh();Check(sounds==1&&user.Level1TXTime==123,"Base init and timestamp before panel audio");
                Check(panel.Views.Count==1&&panel.Views[0].name=="Item_Big(Clone)","Actual big reward prefab");
                Check(!panel.Ad.activeSelf&&!panel.GetButton.gameObject.activeSelf&&panel.MoreLabel.rectTransform.anchoredPosition==Vector2.zero&&panel.MoreLabel.text==tables.Text.GetText(1,"en"),"Early continue layout");
                panel.Advance(.249f);Check(guides==0,"No premature guide");panel.Advance(.002f);
                var settings=Resources.Load<OriginalPanelSettings>("Configuration/OriginalPanels");
                Check(guides==1&&Mathf.Approximately(panel.Title.localScale.x,settings.TitleCurve.Evaluate((.251f-settings.TitleDelayTime)/settings.TitleDurationTime)),"Guide follows main tween while original title curve is still progressing");
                panel.MoreButton.onClick.Invoke();Check(sounds==1&&claims==0,"Initialization gate blocks button callback and audio");
                gate=true;panel.GetCallback();panel.Advance(.251f);Check(closed==1&&claims==0,"Guide direct GetCallback closes without SDK claim");
                panel.Hide();Check(hidden==1&&queued==0&&queuedAction!=null,"Hide action precedes deferred queue continuation");queuedAction();Check(queued==1,"Actual queue action");
                user.Level=4;panel.Refresh();Check(panel.Views.Count==1&&panel.transform.Find("main/Items").childCount==2&&!panel.GetButton.gameObject.activeSelf,"Refresh appends rewards and retains previous early-level visibility");
                panel.GetButton.onClick.Invoke();Check(claims==1&&sounds==2,"Live later-level dispatch and post-action click audio");
            }
            finally{UnityEngine.Object.DestroyImmediate(obj);}
            Debug.Log("NUT_SUCCESS_PANEL_VALIDATION_PASS original hierarchy/resources/fonts/buttons, actual big item generation, early presentation, main/title timing, guide close, gated native claim and deferred Hide queue; production host remains pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
