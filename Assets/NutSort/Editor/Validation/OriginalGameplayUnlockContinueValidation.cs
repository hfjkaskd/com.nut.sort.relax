using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGameplayUnlockContinueValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var trace=new List<string>();bool bannerThrows=false,closeThrows=false;
            var flow=new OriginalGameplayUnlockContinue(user,callback=>
            {
                Check(callback==null,"Source supplies no completion callback");
                trace.Add("banner:"+user.NewGameplayUnlockIndex);
                if(bannerThrows)throw new InvalidOperationException("fixture banner");
            },()=>
            {
                trace.Add("close:"+user.NewGameplayUnlockIndex);
                if(closeThrows)throw new InvalidOperationException("fixture close");
            });
            user.NewGameplayUnlockIndex=9;flow.Run(1,true);
            Check(string.Join(",",trace)=="banner:9,close:2","Banner precedes index write and close follows it");
            trace.Clear();flow.Run(0,false);
            Check(string.Join(",",trace)=="close:1","No-banner skips banner and assigns panel index plus one, without max clamp");
            trace.Clear();bannerThrows=true;Expect(()=>flow.Run(3,true));
            Check(user.NewGameplayUnlockIndex==1 && string.Join(",",trace)=="banner:1","Banner error prevents index write and close");
            bannerThrows=false;closeThrows=true;trace.Clear();Expect(()=>flow.Run(3,false));
            Check(user.NewGameplayUnlockIndex==4 && string.Join(",",trace)=="close:4","Close error retains new index");
            closeThrows=false;flow.Run(int.MaxValue,false);Check(user.NewGameplayUnlockIndex==int.MinValue,"Unchecked increment");
            user.Level=4;user.NewGameplayUnlockIndex=0;
            int opens=0;
            var unlock=new OriginalGameplayUnlock(user,()=>new[]{8,21},(id,index,banner)=>{opens++;flow.Run(index,banner);});
            Check(unlock.Run(true) && user.NewGameplayUnlockIndex==1,"Original decision feeds confirmation index");
            Check(!unlock.Run(true) && opens==1,"Completed index prevents reopening previous unlock");
            user.Level=17;Check(unlock.Run(false) && opens==2 && user.NewGameplayUnlockIndex==2,"Next configured unlock still works");
            Debug.Log("NUT_GAMEPLAY_UNLOCK_CONTINUE_VALIDATION_PASS banner/null-callback/index/close order, assignment vs max, exception mutations, overflow and unlock-to-confirm-to-next-unlock chain.");
        }
        private static void Expect(Action action){bool caught=false;try{action();}catch(InvalidOperationException){caught=true;}Check(caught,"Expected callback exception");}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
