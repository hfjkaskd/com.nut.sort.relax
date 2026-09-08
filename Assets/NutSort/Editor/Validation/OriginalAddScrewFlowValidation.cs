using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalAddScrewFlowValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var calls=new List<string>();bool mode=true,unlocked=false;int max=12;
            var flow=new OriginalAddScrewFlow(user,()=>{calls.Add("max");return max;},()=>{calls.Add("mode");return mode;},
                ()=>{calls.Add("unlock");return unlocked;},()=>calls.Add("tile"),()=>calls.Add("rod"),
                (t,n,f)=>{Check(t==4&&n==-1&&f,"Original item delta");calls.Add("consume");},
                (p,t)=>{Check(p==4&&t==4,"Original acquisition panel and type");calls.Add("panel");},id=>{Check(id==5,"Original cap tip");calls.Add("tip");});
            user.AddScrewCount=0;flow.Run();Check(string.Join(",",calls)=="panel","Empty inventory never reads cap or mode");
            calls.Clear();user.AddScrewCount=1;user.CurrentLevelAddScrewCount=12;flow.Run();Check(string.Join(",",calls)=="max,tip","Inclusive cap rejects before mode/effects");
            calls.Clear();user.CurrentLevelAddScrewCount=11;mode=false;flow.Run();
            Check(user.CurrentLevelAddScrewCount==15&&string.Join(",",calls)=="max,mode,unlock,mode,rod,consume","Four-unit mode can cross cap after precheck");
            calls.Clear();user.CurrentLevelAddScrewCount=0;mode=true;unlocked=true;flow.Run();
            Check(user.CurrentLevelAddScrewCount==1&&string.Join(",",calls)=="max,mode,unlock,consume","Unlock success skips second mode read and addition");
            calls.Clear();user.CurrentLevelAddScrewCount=0;unlocked=false;flow.Run();
            Check(user.CurrentLevelAddScrewCount==1&&string.Join(",",calls)=="max,mode,unlock,mode,tile,consume","Single tile branch");
            var dynamicFlow=new OriginalAddScrewFlow(user,()=>12,()=>mode,()=>{mode=false;return false;},()=>throw new Exception("Stale mode"),()=>calls.Add("new rod"),(t,n,f)=>{},(p,t)=>{},id=>{});
            mode=true;user.CurrentLevelAddScrewCount=0;dynamicFlow.Run();Check(user.CurrentLevelAddScrewCount==1&&calls[calls.Count-1]=="new rod","Mode is live after unlock but increment used earlier value");
            var failed=new OriginalAddScrewFlow(user,()=>int.MaxValue,()=>false,()=>throw new InvalidOperationException("unlock"),()=>{},()=>{},(t,n,f)=>throw new Exception("Unexpected consume"),(p,t)=>{},id=>{});
            user.CurrentLevelAddScrewCount=int.MaxValue-1;bool caught=false;try{failed.Run();}catch(InvalidOperationException){caught=true;}
            Check(caught&&user.CurrentLevelAddScrewCount==int.MinValue+2,"Unchecked increment persists after unlock exception");
            // Actual configured AddScrew Button invokes the flow; it has no additional stock gate.
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete"));
            try
            {
                var item=root.GetComponent<OriginalMainPanelView>().Bottom.GetOtherItem(4);
                user.AddScrewCount=0;calls.Clear();bool allow=false;
                item.Bind(user,()=>0,flow.Run,()=>{},(t,n,f)=>{},(b,d)=>{},(p,t)=>{},id=>{},()=>allow,()=>calls.Add("audio"));item.Init();
                item.Display.Click.onClick.Invoke();Check(calls.Count==0,"Common Button gate remains first");
                allow=true;item.Display.Click.onClick.Invoke();Check(string.Join(",",calls)=="panel,audio","Actual Button reaches acquisition flow then audio");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_ADD_SCREW_FLOW_VALIDATION_PASS native inventory/cap priority, live mode, usage increment, unlock/add routing, consumption and exception order plus real Button entry; world addition implementations and production binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
