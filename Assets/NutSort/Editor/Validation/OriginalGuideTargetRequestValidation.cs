using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGuideTargetRequestValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            Action<JObject> response=null;int shown=0,argument=0;bool mask=true;
            var flow=new OriginalGuideTargetRequest(user,(flag,cb)=>{mask=flag;response=cb;},(id,level)=>{Check(id==18,"Native panel 18");shown++;argument=level;});
            user.Level=2;flow.Run(false);Check(!mask&&response!=null&&shown==0,"No panel while transport completion is pending");
            user.Level=6;response(null);Check(shown==1&&argument==5,"Null payload ignored and level read on completion");
            user.Level=int.MinValue;response(new JObject());Check(shown==2&&argument==int.MaxValue,"Repeated completion and unchecked subtract");
            flow.Run(true);Check(mask&&shown==2,"Forward actual request mask option without eager callback");
            var synchronous=new OriginalGuideTargetRequest(user,(flag,cb)=>{user.Level=9;cb(null);},(id,level)=>argument=level);
            synchronous.Run(false);Check(argument==8,"Synchronous transport mutation precedes live level lookup");
            bool failed=false;var failure=new OriginalGuideTargetRequest(user,(flag,cb)=>throw new InvalidOperationException("request"),(id,level)=>shown++);
            try{failure.Run(false);}catch(InvalidOperationException){failed=true;}
            Check(failed&&shown==2,"Request exception produces no panel");
            Debug.Log("NUT_GUIDE_TARGET_REQUEST_VALIDATION_PASS deferred/synchronous response, ignored null payload, live level, overflow, repeat completion and mask forwarding; transport and actual panel 18 remain explicit boundaries.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
