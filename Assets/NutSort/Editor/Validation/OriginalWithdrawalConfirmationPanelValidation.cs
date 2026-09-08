using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalConfirmationPanelValidation
    {
        public sealed class Services
        {
            public bool Allowed=true;public int Gets,Closes,Panel;public object[] Args;
            public readonly List<string> Trace=new List<string>();
        }
        public static OriginalWithdrawalConfirmationPanel Create(Transform parent,OriginalUserLocalData user,OriginalTables tables,Services services)
        {
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXUserInfoSurePanel"),parent,false).GetComponent<OriginalWithdrawalConfirmationPanel>();
            panel.Bind(user,tables,"en",()=>"US",()=>"1",()=>services.Allowed,s=>services.Trace.Add("sound"),
                (id,args)=>{services.Panel=id;services.Args=args;services.Trace.Add("show");},
                (level,args)=>{Check(level==1&&args&&panel.Closing,"GoldGet called with captured level after close begins");services.Gets++;services.Trace.Add("get");},
                v=>services.Trace.Add("guide"),()=>services.Closes++,()=>{},(d,cb)=>Check(d==2.5f,"Native queue delay"),()=>{});
            panel.Init(new object[]{1});panel.Refresh();return panel;
        }
        public static void Validate()
        {
            bool old=OriginalWithdrawalConfirmationFlow.IsHintGoldGet;OriginalWithdrawalConfirmationFlow.IsHintGoldGet=false;
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.UserLssInfo=JObject.Parse(@"{""Name"":""Fixture User"",""Email"":""fixture@example.test"",""GetType"":2,""OtherInfo"":""""}");
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));var services=new Services();
            var panel=Create(null,user,tables,services);
            try
            {
                Check(panel.GetComponentsInChildren<Transform>(true).Length==14,"Original fourteen-object confirmation hierarchy");
                foreach(var component in panel.GetComponentsInChildren<Component>(true))Check(component!=null,"No missing scripts");
                var buttons=panel.GetComponentsInChildren<Button>(true);Check(buttons.Length==3,"Original three Buttons");
                foreach(var button in buttons)Check(button.onClick.GetPersistentEventCount()==0&&button.GetComponent<OriginalButtonFeedback>()!=null,"Code binding and source feedback");
                Check(panel.TipLabel.text=="Fixture User\nfixture@example.test"&&panel.ChannelImage.sprite!=null&&panel.TipLabel.font!=null,"Actual PayPal account display and font");
                services.Allowed=false;panel.ConfirmButton.onClick.Invoke();Check(services.Gets==0&&!panel.Closing,"Actual Button gate");
                services.Allowed=true;panel.ConfirmButton.onClick.Invoke();Check(services.Gets==1&&panel.Closing&&services.Closes==0&&string.Join(",",services.Trace)=="get,sound","Application boundary follows close initiation, before removal");
                panel.Advance(.5f);Check(services.Closes==1,"Actual close animation completion");
                OriginalWithdrawalConfirmationFlow.IsHintGoldGet=true;panel.Init(new object[]{1});Check(!panel.ConfirmButton.gameObject.activeSelf,"Hint mode hides actual confirmation Button");
                panel.ReenterButton.onClick.Invoke();Check(services.Panel==20&&(int)services.Args[0]==1&&panel.Closing,"Actual reenter Button routes captured level");
                panel.CloseButton.onClick.Invoke();Check(!OriginalWithdrawalConfirmationFlow.IsHintGoldGet&&services.Trace.Contains("guide"),"Actual close Button invokes guide and clears shared flag");
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);OriginalWithdrawalConfirmationFlow.IsHintGoldGet=old;}
            Debug.Log("NUT_WITHDRAWAL_CONFIRMATION_PANEL_VALIDATION_PASS actual source prefab/Buttons/font/icon, account display, hint visibility, reenter/close and GoldGet boundary order; no SDK/payment implementation.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
