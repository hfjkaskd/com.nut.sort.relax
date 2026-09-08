using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalUserInfoPanelValidation
    {
        public sealed class Services
        {
            public bool Allowed=true;public int Saves,Closes,Hidden,Panel,Tip;public object[] Arguments;
            public readonly List<string> Trace=new List<string>();
        }
        public static OriginalWithdrawalUserInfoPanel Create(Transform parent,OriginalUserLocalData user,OriginalTables tables,Services services)
        {
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXUserInfoPanel"),parent,false).GetComponent<OriginalWithdrawalUserInfoPanel>();
            panel.Bind(user,tables,"en",()=>"US",()=>"1",()=>services.Allowed,s=>services.Trace.Add("sound"),id=>services.Tip=id,
                (id,args)=>{services.Panel=id;services.Arguments=args;services.Trace.Add("show");},()=>{services.Saves++;services.Trace.Add("save");},
                value=>{Check((int)value==1,"Close guide level");services.Trace.Add("guide");},()=>services.Trace.Add("clear"),
                ()=>services.Closes++,()=>services.Hidden++,(delay,cb)=>Check(delay==2.5f,"Hide queue delay"),()=>{});
            panel.Init(new object[]{1});panel.Refresh();return panel;
        }
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.UserLssInfo=null;
            var services=new Services();var panel=Create(null,user,tables,services);
            try
            {
                foreach(var c in panel.GetComponentsInChildren<Component>(true))Check(c!=null,"No missing scripts");
                Check(panel.GetComponentsInChildren<Transform>(true).Length==41,"Original 41-object form hierarchy");
                Check(panel.GetComponentsInChildren<Button>(true).Length==6&&panel.GetComponentsInChildren<Toggle>(true).Length==0,"All six action/channel controls are official Buttons");
                var inputs=panel.GetComponentsInChildren<TMP_InputField>(true);Check(inputs.Length==4,"Four original input fields");
                foreach(var input in inputs)Check(input.textComponent!=null&&input.textComponent.font!=null&&input.textComponent.fontSharedMaterial!=null,"Official TMP input references and font materials");
                for(int i=0;i<3;i++)Check(panel.ChannelButton(i).image.sprite!=null,"Actual US full channel artwork loaded");
                foreach(var b in panel.GetComponentsInChildren<Button>(true))Check(b.onClick.GetPersistentEventCount()==0,"Code-only event binding");
                Check(panel.IsChannelOn(0)&&!panel.IsChannelOn(1),"Source first-channel selection");
                panel.ChannelButton(1).onClick.Invoke();Check(!panel.IsChannelOn(0)&&panel.IsChannelOn(1)&&panel.Flow.GetTypeIndex==1,"Real Button changes exclusive selection");
                panel.ChannelButton(1).onClick.Invoke();Check(!panel.IsChannelOn(1)&&panel.Flow.GetTypeIndex==-1,"Source allow-switch-off behavior");panel.ChannelButton(2).onClick.Invoke();
                panel.Name="Fixture User";panel.Email="fixture@example.test";panel.Number="12345";
                services.Allowed=false;panel.GetButton.onClick.Invoke();Check(services.Saves==0&&!panel.Closing,"Action gate prevents submission");
                services.Allowed=true;panel.GetButton.onClick.Invoke();Check(services.Panel==21&&(int)services.Arguments[0]==1&&services.Saves==1&&panel.Closing,"Real inputs submit through restored flow");
                Check(string.Join(",",services.Trace)=="show,save,sound","Button sound follows submission and saving");panel.Advance(.5f);Check(services.Closes==1,"Actual closing animation completes");
                panel.Hide();Check(services.Hidden==1,"Actual hide queues continuation");
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);}
            Debug.Log("NUT_WITHDRAWAL_USER_INFO_PANEL_VALIDATION_PASS original form hierarchy/artwork, official TMP inputs and Buttons, exclusive/deselect channel behavior, gated actual submission, closing and hide; production host pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
