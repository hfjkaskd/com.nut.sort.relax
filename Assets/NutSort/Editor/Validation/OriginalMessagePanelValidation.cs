using System;
using System.Collections.Generic;
using NutSort.UI;
using TMPro;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalMessagePanelValidation
    {
        public static void Validate()
        {
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel")).GetComponent<OriginalMessagePanel>();
            try
            {
                Check(panel.GetComponentsInChildren<Transform>(true).Length==7,"Original seven scene objects");
                foreach(var c in panel.GetComponentsInChildren<Component>(true))Check(c!=null,"No missing components");
                foreach(var label in panel.GetComponentsInChildren<TextMeshProUGUI>(true))Check(label.font!=null,"Original fonts resolve");
                Check(panel.ConfirmButton.onClick.GetPersistentEventCount()==0,"No dragged events");
                var ids=new List<int>();panel.Bind(id=>{ids.Add(id);return "localized "+id;});panel.Init();
                panel.gameObject.SetActive(false);int calls=0;
                panel.Show(120,()=>{Check(!panel.gameObject.activeSelf,"Hide before callback");calls++;},true);
                Check(ids.Count==3&&ids[2]==120&&ids.Contains(73)&&ids.Contains(74),"Localize inactive labels before requested content");
                Check(panel.ContentText.text=="localized 120"&&panel.gameObject.activeSelf,"ID show displays and activates");
                panel.ConfirmButton.onClick.Invoke();Check(calls==1&&!panel.gameObject.activeSelf,"Actual standard button hide and callback");
                panel.ConfirmButton.onClick.Invoke();Check(calls==2,"Callback retained after confirmation");
                ids.Clear();panel.Show("raw",null,true);Check(ids.Count==0&&panel.ContentText.text=="raw","String overload skips localization and ignores close flag");
                panel.ConfirmButton.onClick.Invoke();Check(!panel.gameObject.activeSelf&&calls==2,"Null callback replaces previous");
                panel.Show("reenter",()=>panel.Show("reopened",()=>calls++));panel.ConfirmButton.onClick.Invoke();
                Check(panel.gameObject.activeSelf&&panel.ContentText.text=="reopened","Callback can reopen without subsequent hiding or clearing");
                panel.ConfirmButton.onClick.Invoke();Check(calls==3,"Reentrant replacement retained");
                panel.Init();panel.Show("repeat init",()=>calls++);panel.ConfirmButton.onClick.Invoke();Check(calls==5,"Init preserves duplicate listeners");
                panel.Show("throw",()=>throw new InvalidOperationException("expected callback"));
                try{panel.OkCallback();throw new Exception("Expected callback failure");}catch(InvalidOperationException error){Check(error.Message=="expected callback"&&!panel.gameObject.activeSelf,"Exception propagates after hide");}
                Action retained=panel.OkCallbackAction;panel.Bind(id=>throw new InvalidOperationException("expected text"));
                try{panel.Show(120,null);throw new Exception("Expected localization failure");}catch(InvalidOperationException error){Check(error.Message=="expected text"&&!panel.gameObject.activeSelf&&panel.OkCallbackAction==retained,"Localization failure preserves previous callback and visibility");}
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);}
            Debug.Log("NUT_MESSAGE_PANEL_VALIDATION_PASS scene-derived native prefab, official Button/TMP, ID and raw overload order, inactive label localization, callback retention/reentry/failure, repeated Init listener semantics; production UI manager binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
