using System;
using System.Collections.Generic;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalCountedMaskValidation
    {
        public static void Validate()
        {
            var trace=new List<string>();var pending=new List<Action>();bool active=false;
            OriginalCountedMask mask=null;
            mask=new OriginalCountedMask(value=>{active=value;trace.Add(value?"on":"off");},(seconds,callback)=>
            {Check(mask.Count>0 && active,"Increment and activation precede scheduling");pending.Add(callback);trace.Add("schedule");});
            mask.SetMask(true,.5f,"unused text");mask.SetMask(true,1f);
            Check(mask.Count==2 && pending.Count==2 && string.Join(",",trace)=="on,schedule,on,schedule","Each acquisition activates and schedules independently");
            pending[1]();Check(mask.Count==1 && active,"One release retains other acquisition");
            pending[0]();Check(mask.Count==0 && !active,"Last release hides");
            trace.Clear();pending.Clear();mask.SetMask(false);mask.SetMask(false);
            Check(mask.Count==0 && string.Join(",",trace)=="off,off","Extra releases clamp and still call SetActive false");
            mask.SetMask(true,0f);mask.SetMask(true,-1f);Check(pending.Count==0 && mask.Count==2,"Nonpositive delays require explicit release");
            mask.SetMask(false,100f);Check(mask.Count==1 && pending.Count==0,"False ignores hideTime");mask.SetMask(false);
            mask.SetMask(true,.5f);mask.SetMask(false);mask.SetMask(true);
            pending[0]();Check(mask.Count==0 && !active,"Old timer can release newer acquisition without cancellation token");
            var failure=new OriginalCountedMask(value=>{throw new InvalidOperationException("fixture activation");},(seconds,callback)=>pending.Add(callback));
            int scheduled=pending.Count;bool caught=false;try{failure.SetMask(true,1f);}catch(InvalidOperationException){caught=true;}
            Check(caught && failure.Count==1 && pending.Count==scheduled,"Activation failure leaves increment and prevents scheduling");
            failure=new OriginalCountedMask(value=>active=value,(seconds,callback)=>{throw new InvalidOperationException("fixture schedule");});
            caught=false;try{failure.SetMask(true,1f);}catch(InvalidOperationException){caught=true;}
            Check(caught && failure.Count==1 && active,"Scheduling failure leaves active acquisition");
            failure.SetMask(false);Check(!active && failure.Count==0,"Explicit release recovers scheduled failure state");
            Debug.Log("NUT_COUNTED_MASK_VALIDATION_PASS overlapping acquisitions, release clamp/order, positive-only delays, ignored tip, uncancelled stale release and exception mutation boundaries.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
