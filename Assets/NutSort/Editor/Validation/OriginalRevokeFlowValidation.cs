using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalRevokeFlowValidation
    {
        public static void Validate()
        {
            var trace=new List<string>();var records=new List<OriginalMoveRecord>();
            bool accepted=true,dead=false,throws=false;var record=new OriginalMoveRecord();
            var flow=new OriginalRevokeFlow(()=>records,item=>{Check(item==record,"Last record selected");trace.Add("record");if(throws)throw new InvalidOperationException("fixture");return accepted;},type=>{Check(type==2,"Revoke gray update");trace.Add("gray");},()=>{trace.Add("deadlock");return dead;},()=>trace.Add("fail"));
            flow.Revoke();Check(trace.Count==0,"Empty history skips even deadlock check");
            records.Add(record);accepted=false;dead=true;flow.Revoke();Check(records.Count==1 && string.Join(",",trace)=="record,deadlock,fail","Rejected record retained but failure still checked");trace.Clear();
            accepted=true;dead=false;records.Insert(0,record);var middle=new OriginalMoveRecord();records.Insert(1,middle);
            flow.Revoke();Check(records.Count==2 && records[0]==middle && records[1]==record && string.Join(",",trace)=="record,gray,deadlock","Remove first equal object, not last index");trace.Clear();
            throws=true;bool failed=false;try{flow.Revoke();}catch(InvalidOperationException){failed=true;}Check(failed && records.Count==2 && string.Join(",",trace)=="record","Exception prevents remove, gray and deadlock");
            var replacement=new List<OriginalMoveRecord>{record};var live=new List<OriginalMoveRecord>{record};
            var switching=new OriginalRevokeFlow(()=>live,item=>{live=replacement;return true;},type=>{},()=>false,()=>{});switching.Revoke();Check(replacement.Count==0,"Removal reacquires live history after callback");
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/RevokeItem"));
            try
            {
                var controller=instance.GetComponent<OriginalToolItemController>();var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.RevokeCount=2;
                var manager=new OriginalItemManager(user,()=>trace.Add("save"),()=>trace.Add("bottom"),(v,a,b)=>{},(v,a)=>{});
                throws=false;accepted=false;dead=false;records.Clear();records.Add(record);trace.Clear();
                controller.Bind(user,()=>records.Count,()=>{},flow.Revoke,manager.AddTool,(active,delay)=>{},(panel,type)=>{},id=>{},()=>true,()=>trace.Add("audio"));controller.Init();controller.Display.Click.onClick.Invoke();
                Check(records.Count==1 && user.RevokeCount==1 && string.Join(",",trace)=="record,deadlock,save,bottom,audio","Source rejected record still consumes through outer Button manager flow");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("NUT_REVOKE_FLOW_VALIDATION_PASS empty/success/rejection/exception branches, live reference removal, gray-before-deadlock and outer Button consumption after rejected record.");
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
