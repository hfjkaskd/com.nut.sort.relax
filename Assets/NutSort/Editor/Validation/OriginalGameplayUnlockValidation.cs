using System;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGameplayUnlockValidation
    {
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var user=new OriginalUserLocalData(defaults) {Level=6,NewGameplayUnlockIndex=0};
            int[] levels={10,10,20};int calls=0,last=-1;bool banner=false,throws=false;
            var flow=new OriginalGameplayUnlock(user,()=>levels,(id,index,show)=>
            {Check(id==13,"Original panel identity");calls++;last=index;banner=show;if(throws)throw new InvalidOperationException("fixture");});
            Check(flow.Run(true) && calls==1 && last==0 && banner,"First matching index and banner forwarded");
            Check(user.NewGameplayUnlockIndex==0,"Show does not advance unlock index");
            Check(flow.Run(false) && calls==2 && last==0 && !banner,"No-banner invocation still opens matching panel");
            user.NewGameplayUnlockIndex=1;Check(flow.Run(true) && last==1,"Index equality remains eligible; earlier duplicate skipped");
            user.NewGameplayUnlockIndex=2;Check(!flow.Run(true) && calls==3,"Earlier indices excluded");
            user.Level=17;Check(!flow.Run(true),"Level beyond exact threshold does not match");
            user.Level=16;Check(flow.Run(false) && last==2,"Real player level uses configured value minus four");
            levels=Array.Empty<int>();Check(!flow.Run(true),"Empty configuration returns false");
            levels=new[]{int.MinValue};user.Level=unchecked(int.MinValue-4);user.NewGameplayUnlockIndex=-1;
            Check(flow.Run(true) && last==0,"Unchecked subtraction and negative saved index");
            throws=true;bool caught=false;try{flow.Run(true);}catch(InvalidOperationException){caught=true;}
            Check(caught && user.NewGameplayUnlockIndex==-1,"Show exception propagates without index mutation");
            levels=null;caught=false;try{flow.Run(true);}catch(NullReferenceException){caught=true;}
            Check(caught,"Missing configuration does not become an empty success fallback");
            Debug.Log("NUT_GAMEPLAY_UNLOCK_VALIDATION_PASS exact player-stage offset, inclusive saved index, first eligible match, forwarded panel/banner, live configuration, no index mutation and failure propagation.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
