using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalTooFastPanelValidation
    {
        public static void Validate()
        {
            bool old=OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint;
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.GoldRewardTargetS2CData=new JObject{{"bear_list",new JArray(new JObject(),new JObject(),new JObject{{"Stage2StartShowLevel",12}})}};
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXTooFastHintPanel")).GetComponent<OriginalWithdrawalTooFastPanel>();
            bool allowed=false;int resumes=0,sounds=0,closes=0,hides=0,schedules=0;
            try
            {
                panel.Bind(user,tables,"en",()=>allowed,s=>sounds++,()=>7,()=>{Check(panel.Closing&&closes==0,"Resolve stage during close animation");return ()=>resumes++;},()=>closes++,()=>hides++,
                    (d,cb)=>{Check(d==2.5f&&hides==1,"Hide before source queue scheduling");schedules++;},()=>{});
                panel.Init();panel.Refresh();
                Check(panel.GetComponentsInChildren<Transform>(true).Length==19,"Source nineteen objects");
                foreach(var c in panel.GetComponentsInChildren<Component>(true))Check(c!=null,"No missing scripts");
                var buttons=panel.GetComponentsInChildren<Button>(true);Check(buttons.Length==1&&buttons[0]==panel.SureButton,"One source Button");
                Check(buttons[0].onClick.GetPersistentEventCount()==0&&buttons[0].GetComponent<OriginalButtonFeedback>()!=null,"Code-only binding and source feedback");
                Check(panel.LevelValue.text=="6/12"&&panel.VideoValue.text=="10/10"&&panel.LevelFill.fillAmount==.5f&&panel.VideoFill.fillAmount==1,"Actual TMP and Image display");
                Check(panel.LevelLabel.font!=null&&panel.VideoLabel.font!=null&&panel.LevelFill.sprite!=null&&panel.VideoFill.sprite!=null,"Source font and sprites resolve");
                Check(panel.LevelLabel.text==tables.Text.GetText(165,"en",12)&&panel.VideoLabel.text==tables.Text.GetText(166,"en",10),"Actual localized labels");
                panel.SetLevelFill(2);Check(panel.LevelFill.fillAmount==1,"Image clamps upper fill");panel.SetLevelFill(-1);Check(panel.LevelFill.fillAmount==0,"Image clamps lower fill");panel.Refresh();
                panel.Advance(1);Check(panel.transform.Find("main").localScale==Vector3.one,"Main opening completes");
                panel.SureButton.onClick.Invoke();Check(resumes==0&&sounds==0&&!panel.Closing,"Initialization click gate");
                allowed=true;panel.SureButton.onClick.Invoke();Check(resumes==1&&sounds==1&&panel.Closing&&closes==0,"Resume before animated close completes and click sound");
                panel.Advance(.5f);Check(closes==1,"Animated close completion");panel.Hide();Check(hides==1&&schedules==1,"Original hide queue ordering");
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint=old;}
            Debug.Log("NUT_WITHDRAWAL_TOO_FAST_PANEL_VALIDATION_PASS actual source hierarchy, official Button/TMP/Image, localized display, native fill clamping, gate, immediate stage callback and animated close; stage23 production binding remains pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
