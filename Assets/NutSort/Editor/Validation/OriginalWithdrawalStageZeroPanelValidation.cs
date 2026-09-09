using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalStageZeroPanelValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.UserLssInfo=null;
            user.GoldRewardTargetS2CData=new JObject{{"bear_list",new JArray(new JObject(),new JObject(),new JObject{{"Stage2StartShowLevel",12}})}};
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXProgress0Panel")).GetComponent<OriginalWithdrawalStageZeroPanel>();
            int saves=0,hints=0,returns=0,closed=0,sounds=0;bool allowed=false;
            try
            {
                panel.Bind(user,tables,"en",new object[]{true},()=>100,v=>throw new Exception("Source Gold is null"),(t,f)=>{Check(f=="g","Source time format");return "fixture:"+t;},
                    (a,b)=>a,()=>false,()=>7,()=>saves++,v=>throw new Exception("Guide mode must route UI18"),()=>allowed,s=>sounds++,
                    (id,args)=>{Check(args.Length==0,"Empty routed args");if(id==33)hints++;else{Check(id==18&&panel.Closing,"Close before UI18");returns++;}},()=>closed++,()=>{},(d,cb)=>{},()=>{});
                panel.Init();panel.Refresh();
                // Edit-mode fixture does not run MonoBehaviour Awake; Play validation uses actual registration.
                OriginalUIAnimationDriver.Register(panel,panel.Advance);
                Check(panel.GetComponentsInChildren<Transform>(true).Length==39,"Source hierarchy");foreach(var c in panel.GetComponentsInChildren<Component>(true))Check(c!=null,"No missing scripts");
                var buttons=panel.GetComponentsInChildren<Button>(true);Check(buttons.Length==2,"Source two Buttons");foreach(var b in buttons)Check(b.onClick.GetPersistentEventCount()==0&&b.GetComponent<OriginalButtonFeedback>()!=null,"Code-only buttons and feedback");
                Check(!panel.HasGold&&panel.StepCount==4&&panel.HasStepTime(1)&&panel.HasStepTime(2)&&!panel.HasStepTime(3)&&!panel.HasStepTime(4),"Optional source gold and time nodes");
                Check(panel.Steps.Find("1/Content/Time").GetComponent<TMPro.TMP_Text>().text=="fixture:100"&&panel.Steps.Find("2/Content/Time").GetComponent<TMPro.TMP_Text>().text=="fixture:103","Actual time labels");
                Check(saves==2&&panel.SureButton.transform.localScale==Vector3.zero,"First opening flag/save and hidden Sure");
                for(int i=0;i<120;i++)OriginalUIAnimationDriver.Advance(.05f,.05f);
                Check(hints==1&&panel.Steps.Find("1/Done").gameObject.activeSelf&&!panel.Steps.Find("1/Loading").gameObject.activeSelf&&panel.Steps.Find("3").localScale.y==0,"Actual first animation waits for hint response");
                panel.PlayProgress();for(int i=0;i<80;i++)OriginalUIAnimationDriver.Advance(.05f,.05f);
                Check(panel.SureButton.transform.localScale==Vector3.one&&panel.Steps.Find("2/Done").gameObject.activeSelf&&!panel.Steps.Find("2/Loading").gameObject.activeSelf&&panel.Steps.Find("3/Loading").gameObject.activeSelf,"Actual resume keeps original third loading and reveals Sure");
                panel.Refresh();Check(saves==3&&hints==1&&panel.SureButton.transform.localScale==Vector3.one,"Repeat refresh does not replay first animation");
                panel.SureButton.onClick.Invoke();Check(returns==0&&sounds==0,"Button gate");allowed=true;panel.SureButton.onClick.Invoke();Check(returns==1&&sounds==1&&closed==0,"Guide confirmation before animated removal");panel.Advance(.5f);Check(closed==1,"Animated close");
            }
            finally{OriginalUIAnimationDriver.Unregister(panel);UnityEngine.Object.DestroyImmediate(panel.gameObject);}
            Debug.Log("NUT_WITHDRAWAL_STAGE_ZERO_PANEL_VALIDATION_PASS actual source prefab, optional gold/time, localized text, first/resume animation, unscaled hint scheduling, repeated refresh and code-bound Sure; production clock/entry pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
