using System;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalLoadingValidation
    {
        public static void Validate()
        {
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXGuideLoadingPanel"));
            try
            {
                Check(root.GetComponentsInChildren<Transform>(true).Length==6,"Original six-object hierarchy");
                foreach(var component in root.GetComponentsInChildren<Component>(true))Check(component!=null,"No missing scripts");
                Check(root.GetComponentsInChildren<Button>(true).Length==0,"Source loading has no button");
                var panel=root.GetComponent<OriginalWithdrawalLoadingPanel>();int claims=0,closed=0,hidden=0,queued=0;
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                panel.Bind(tables,"en",()=>closed++,()=>{Check(panel.Closing&&panel.Fill==1&&closed==0,"Close starts before claim and completes later");claims++;},()=>hidden++,
                    (delay,action)=>{Check(delay==2.5f,"Original hide queue delay");queued++;},()=>{});
                panel.Init();Check(panel.Fill==0&&panel.Main.localScale==Vector3.zero,"Base init then reset fill");
                Check(panel.Label.text==tables.Text.GetText(167,"en")&&panel.Label.font!=null&&panel.Label.fontSharedMaterial!=null,"Original localized label and font");
                panel.Refresh();panel.Advance(0);panel.Advance(.5f);Check(panel.Fill==0&&claims==0,"Initial delay");
                panel.Advance(1);Near(panel.Fill,.4f,"First linear midpoint");
                panel.Advance(1);Near(panel.Fill,.8f,"First endpoint");Check(panel.ActiveCount==1&&claims==0,"Second tween deferred until next advance");
                root.SetActive(false);panel.Advance(1);Near(panel.Fill,.8f,"Second delay while hidden");
                panel.Advance(.5f);Near(panel.Fill,.9f,"Second linear midpoint");panel.Advance(.5f);
                Check(claims==1&&closed==0&&panel.ActiveCount==0,"Claim immediately at completion, no transition to panel 37");
                panel.Advance(.251f);Check(closed==1,"Animated close completion");panel.Hide();Check(hidden==1&&queued==1,"Hide queues next panel");
                panel.Init();panel.Refresh();panel.Advance(1.5f);Near(panel.Fill,.4f,"Restart first track");panel.Refresh();Near(panel.Fill,.4f,"Refresh does not reset current fill");
                panel.Advance(.5f);Near(panel.Fill,0,"New independent refresh writes after previous track");Check(panel.ActiveCount==2,"Refresh retains existing tracks");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_WITHDRAWAL_LOADING_VALIDATION_PASS original hierarchy/font/label, two delayed linear stages, hidden scaled updates, independent refresh, close-before-claim and delayed hide queue; production composition remains pending.");
        }
        private static void Near(float a,float b,string message)=>Check(Mathf.Abs(a-b)<.00001f,message);
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
