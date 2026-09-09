using System;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalBootLoadingPanelValidation
    {
        public static void Validate()
        {
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/BootLoadingPanel")).GetComponent<OriginalBootLoadingPanel>();
            try
            {
                panel.Initialize();OriginalUIAnimationDriver.Register(panel,panel.AdvanceTweens);panel.StartDisplay();
                Check(panel.GetComponentsInChildren<Transform>(true).Length==8,"Eight original scene objects");foreach(var c in panel.GetComponentsInChildren<Component>(true))Check(c!=null,"No missing scripts");
                Check(panel.Percent.font!=null&&panel.Progress.sprite!=null&&panel.LogoImage.sprite!=null,"Source font and sprites");
                panel.SetState(true);Check(panel.Percent.text=="0%"&&panel.Progress.fillAmount==0&&panel.Marker.anchoredPosition==Vector2.zero,"Original zero display");
                panel.AdvanceProgress(1);Check(panel.Percent.text=="50%"&&panel.Progress.fillAmount==.5f&&panel.Marker.anchoredPosition==new Vector2(375,0),"Actual marker from fill times 750");
                panel.AdvanceProgress(10);Check(panel.Percent.text=="90%"&&panel.Progress.fillAmount==.9f&&panel.Marker.anchoredPosition==new Vector2(675,0),"Actual cap layout");
                panel.SetState(false);OriginalUIAnimationDriver.Advance(.25f,.25f);Check(panel.gameObject.activeSelf&&panel.Percent.text==string.Format("{0:F0}%",panel.Flow.Value*100),"Exact F0 midpoint display");
                OriginalUIAnimationDriver.Advance(.25f,.25f);Check(panel.Percent.text=="100%"&&panel.Marker.anchoredPosition==new Vector2(750,0)&&panel.gameObject.activeSelf,"Complete before delayed hide");
                OriginalUIAnimationDriver.Advance(0,1);Check(panel.gameObject.activeSelf,"Hide uses scaled clock");OriginalUIAnimationDriver.Advance(.21f,.21f);Check(!panel.gameObject.activeSelf,"Delayed actual GameObject hide");
                panel.SetState(true);Check(panel.gameObject.activeSelf&&panel.Percent.text=="0%","Reopen actual view");
            }
            finally{OriginalUIAnimationDriver.Unregister(panel);UnityEngine.Object.DestroyImmediate(panel.gameObject);}
            Debug.Log("NUT_BOOT_LOADING_PANEL_VALIDATION_PASS actual scene-derived prefab, official Image/TMP, source fill/marker/F0, 90-percent cap, globally driven completion and scaled delayed hide; full bootstrap binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
