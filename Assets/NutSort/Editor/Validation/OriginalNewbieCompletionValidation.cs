using System;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalNewbieCompletionValidation
    {
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            try
            {
                var v=instance.GetComponent<OriginalNewbieGuideView>();
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){Level=2};
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                int closed=0,shown=0,called=0;object payload=new object();
                Action<object> callback=null;
                callback=value=>{Check(ReferenceEquals(value,payload) && OriginalNewbieGuideView.CallbackAction==callback,"Global callback receives exact object while still registered");called++;};
                v.BindTeaching(tables,"en",value=>{},()=>
                {
                    Check(OriginalNewbieGuideView.CallbackAction==callback,"Register callback before starting close");
                    closed++;user.Level=3;
                });
                v.ShowTargetCompletion(user,callback,(id,arg)=>
                {
                    Check(id==35 && arg==2 && closed==1 && called==0,"Panel argument reads current level after close, without dispatching completion");shown++;
                });
                UnityEngine.Object.DestroyImmediate(instance);instance=null;
                Check(OriginalNewbieGuideView.CallbackActionInvoke(payload)==null && called==1 && shown==1,"Callback outlives guide and returns null after immediate execution");
                Check(OriginalNewbieGuideView.CallbackAction==null && OriginalNewbieGuideView.CallbackActionInvoke(payload)==null && called==1,"Subsequent empty invocation has no action");
                OriginalNewbieGuideView.CallbackAction=value=>OriginalNewbieGuideView.CallbackAction=other=>called+=100;
                OriginalNewbieGuideView.CallbackActionInvoke(null);
                Check(OriginalNewbieGuideView.CallbackAction==null,"Replacement during callback is cleared on successful return");
                Action<object> failing=value=>throw new InvalidOperationException("fixture failure");
                OriginalNewbieGuideView.CallbackAction=failing;
                bool failed=false;try{OriginalNewbieGuideView.CallbackActionInvoke(payload);}catch(InvalidOperationException){failed=true;}
                Check(failed && OriginalNewbieGuideView.CallbackAction==failing,"Throwing callback retains global registration");
            }
            finally
            {
                if(instance!=null)UnityEngine.Object.DestroyImmediate(instance);
                OriginalNewbieGuideView.CallbackAction=previous;
            }
            Debug.Log("NUT_NEWBIE_COMPLETION_VALIDATION_PASS case-one register-close-panel order, live level argument, global callback lifetime, payload identity, immediate null return and replacement/failure semantics; target panel and return flow validated separately; production binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
