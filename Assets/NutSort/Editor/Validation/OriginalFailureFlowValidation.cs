using System;
using System.Collections.Generic;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalFailureFlowValidation
    {
        public static void Validate()
        {
            var settings=Resources.Load<OriginalSceneSession>("Configuration/OriginalSceneSession");
            Check(settings.FailPanelDelay==3f && settings.FailPanelId==10,"Source delay and panel routing serialized");
            var failure=new OriginalFailureFlow();
            var callbacks=new List<Action>();int shown=0;
            Action<float,Action> schedule=(seconds,callback)=>
            {
                Check(failure.IsFail && seconds==3f,"Flag precedes scheduling");
                callbacks.Add(callback);
            };
            failure.Fail(settings.FailPanelDelay,schedule,()=>shown++);
            failure.Fail(settings.FailPanelDelay,schedule,()=>shown++);
            Check(callbacks.Count==1 && shown==0 && failure.IsFail,"Repeated fail is suppressed, panel is deferred");
            failure.IsFail=false;
            failure.Fail(settings.FailPanelDelay,schedule,()=>shown+=10);
            Check(callbacks.Count==2,"Explicit action reset permits another fail");
            failure.IsFail=false;
            callbacks[0]();callbacks[1]();
            Check(shown==11 && !failure.IsFail,"Queued callbacks survive resets and do not change flag");
            var throwing=new OriginalFailureFlow();bool caught=false;
            try { throwing.Fail(3f,(seconds,callback)=>{throw new InvalidOperationException("fixture");},()=>{}); }
            catch(InvalidOperationException) { caught=true; }
            Check(caught && throwing.IsFail,"Scheduling exception retains already-written failure state");
            throwing.Fail(3f,(seconds,callback)=>{throw new Exception("Must not schedule twice");},()=>{});
            var records=new List<OriginalMoveRecord>{new OriginalMoveRecord()};
            failure.IsFail=false;callbacks.Clear();
            var revoke=new OriginalRevokeFlow(()=>records,item=>false,type=>{throw new Exception("Rejected record gray");},()=>true,
                ()=>failure.Fail(settings.FailPanelDelay,schedule,()=>shown++));
            revoke.Revoke();revoke.Revoke();
            Check(records.Count==1 && callbacks.Count==1 && failure.IsFail,"Repeated rejected revokes share failure guard");
            Debug.Log("NUT_FAILURE_FLOW_VALIDATION_PASS serialized 3-second panel 10, flag-before-schedule, duplicate guard, explicit resets retain pending callbacks, exception ordering and revoke failure integration.");
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
