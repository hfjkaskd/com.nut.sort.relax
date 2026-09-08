using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGuideTargetCompletionValidation
    {
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"))
                    {Level1Gold=12,Level2Gold=34,Gold=56};
                float received=0;int formats=0;Func<float,string> format=value=>{received=value;formats++;return "formatted";};
                foreach(int stage in new[]{1,2,0,-1,3,int.MaxValue})
                {
                    var display=new OriginalGuideTargetCompletion(user,stage,format,()=>{},(a,b)=>{});
                    Check(display.Refresh()=="formatted" && received==(stage==1?12:stage==2?34:56),"Native stage amount mapping");
                }
                Check(formats==6,"Only one formatter call per refresh");
                var live=new OriginalGuideTargetCompletion(user,1,format,()=>{},(a,b)=>{});
                user.Level1Gold=17;live.Refresh();Check(received==17,"Refresh reads current amount, not construction snapshot");
                user.IsCompleteRecordGuide=false;var trace=new List<string>();
                OriginalNewbieGuideView.CallbackAction=value=>{Check(value==null,"Completion payload is null");trace.Add("original");};
                var flow=new OriginalGuideTargetCompletion(user,1,format,
                    ()=>{Check(!user.IsCompleteRecordGuide,"Close precedes completion flag");trace.Add("close");},
                    (banner,first)=>
                    {
                        Check(user.IsCompleteRecordGuide && banner && !first,"Initialization sees completion with true/false flags");trace.Add("init");
                        OriginalNewbieGuideView.CallbackAction=value=>{Check(value==null,"Replacement receives null");trace.Add("replacement");};
                    });
                flow.Continue();
                Check(string.Join(",",trace)=="close,init,replacement" && OriginalNewbieGuideView.CallbackAction==null,"Global callback is selected after initialization re-entry, not captured before");
                user.IsCompleteRecordGuide=false;Action<object> retained=value=>{};OriginalNewbieGuideView.CallbackAction=retained;
                flow=new OriginalGuideTargetCompletion(user,2,format,()=>throw new InvalidOperationException("close"),(a,b)=>throw new InvalidOperationException("unreachable"));
                Expect(flow.Continue);Check(!user.IsCompleteRecordGuide && OriginalNewbieGuideView.CallbackAction==retained,"Close failure prevents later mutation and dispatch");
                flow=new OriginalGuideTargetCompletion(user,2,format,()=>{},(a,b)=>throw new InvalidOperationException("init"));
                Expect(flow.Continue);Check(user.IsCompleteRecordGuide && OriginalNewbieGuideView.CallbackAction==retained,"Initialization failure retains completed flag and pending global callback");
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;}
            Debug.Log("NUT_GUIDE_TARGET_COMPLETION_VALIDATION_PASS live stage amounts, formatter, close/flag/init/global ordering, post-init callback replacement and exception boundaries; prefab binding pending.");
        }
        private static void Expect(Action action){bool failed=false;try{action();}catch(InvalidOperationException){failed=true;}Check(failed,"Expected native failure propagation");}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
