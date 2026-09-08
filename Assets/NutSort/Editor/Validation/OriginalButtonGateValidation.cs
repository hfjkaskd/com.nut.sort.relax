using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalButtonGateValidation
    {
        public static void Validate()
        {
            var settings=Resources.Load<OriginalPanelSettings>("Configuration/OriginalPanels");
            Check(settings.ClickMaskDuration==.2f,"Native button delay");
            bool initialized=false,active=false,throws=false;
            var trace=new List<string>();var pending=new List<Action>();
            var mask=new OriginalCountedMask(value=>{active=value;trace.Add(value?"mask-on":"mask-off");},(seconds,callback)=>{Check(seconds==.2f,"Configured source delay");trace.Add("schedule");pending.Add(callback);});
            var gate=new OriginalButtonGate(()=>{trace.Add("init-check");return initialized;},mask,settings);
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/FailPanel"));
            try
            {
                var panel=instance.GetComponent<OriginalFailurePanelView>();var user=OriginalMainTopViewValidation.MakeUser();
                user.ServerConfigData=new JObject();user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_zs_show\":\"250\"}");
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                panel.Bind(user,tables,"en",gate.TryBegin,s=>trace.Add(s),close=>
                {Check(active && mask.Count>0,"Mask already acquired at action entry");trace.Add("action");if(throws)throw new InvalidOperationException("fixture");close();},()=>{},()=>{},()=>trace.Add("close"));
                panel.Init();trace.Clear();panel.Restart.onClick.Invoke();
                Check(string.Join(",",trace)=="init-check" && mask.Count==0 && pending.Count==0,"Uninitialized Button skips mask, action and audio");
                initialized=true;trace.Clear();panel.Restart.onClick.Invoke();
                Check(string.Join(",",trace)=="init-check,mask-on,schedule,action,close,Click","Actual Button ordering matches common native wrapper");
                panel.Restart.onClick.Invoke();Check(mask.Count==2 && pending.Count==2,"Direct invocation does not add mask-active guard; raycast blocking is separate");
                pending[0]();Check(active && mask.Count==1,"First click release retains second acquisition");pending[1]();Check(!active && mask.Count==0,"Both release independently");
                throws=true;trace.Clear();bool caught=false;try{panel.Restart.onClick.Invoke();}catch(InvalidOperationException){caught=true;}
                Check(caught && mask.Count==1 && active && string.Join(",",trace)=="init-check,mask-on,schedule,action","Action failure keeps scheduled mask and skips close/audio");
                initialized=false;trace.Clear();panel.Restart.onClick.Invoke();Check(mask.Count==1 && string.Join(",",trace)=="init-check","Live initialization flag checked on each click");
                pending[2]();Check(mask.Count==0 && !active,"Scheduled release remains after action exception");
            }
            finally{UnityEngine.Object.DestroyImmediate(instance);}
            Debug.Log("NUT_BUTTON_GATE_VALIDATION_PASS real failure Button initialization gate, native 0.2-second mask-before-action/audio ordering, independent releases and exception boundaries.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
