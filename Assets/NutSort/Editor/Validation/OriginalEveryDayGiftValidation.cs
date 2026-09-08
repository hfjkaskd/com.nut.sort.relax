using System;
using System.Collections;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalEveryDayGiftValidation
    {
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var user=new OriginalUserLocalData(defaults);
            DateTime now=new DateTime(2026,9,8,0,0,0,DateTimeKind.Utc);
            long today=new DateTimeOffset(now).ToUnixTimeSeconds();
            var pending=new List<IEnumerator>();var trace=new List<string>();
            bool modal=true;int polls=0;
            var flow=new OriginalEveryDayGiftFlow(()=>user,()=>{polls++;return modal;},
                (predicate,done)=>pending.Add(OriginalUntilCallback.Run(predicate,done)),
                id=>{Check(id==16,"Original gift panel ID");trace.Add("show");},
                ()=>{Check(user.LastGetEveryDayGift==today+7,"Timestamp precedes save");trace.Add("save");},
                ()=>now,()=>{trace.Add("clock");return today+7;});
            user.LastGetEveryDayGift=today+86399;flow.Show();
            Check(pending.Count==0&&polls==0,"Same UTC date, including later today, never checks panels");
            user.LastGetEveryDayGift=today-1;flow.Show();flow.Show();
            Check(pending.Count==2&&polls==0&&trace.Count==0,"Prior day schedules independent lazy waits");
            foreach(var wait in pending)
            {
                Check(wait.MoveNext()&&wait.Current is WaitUntil,"Native yield instruction");
                Check(((WaitUntil)wait.Current).keepWaiting,"Existing modal keeps wait pending");
            }
            // A change in the claim timestamp while waiting does not cancel either request.
            user.LastGetEveryDayGift=today;modal=false;
            foreach(var wait in pending)
            {
                Check(!((WaitUntil)wait.Current).keepWaiting,"Live panel state releases wait");
                Check(!wait.MoveNext()&&!wait.MoveNext(),"One callback per wait");
            }
            Check(string.Join(",",trace)=="show,clock,save,show,clock,save","Independent callbacks retain source show/time/save order");
            Check(polls==4,"No eager or extra predicate polls");
            int clockReads=0;
            user.LastGetEveryDayGift=long.MaxValue;
            var invalid=new OriginalEveryDayGiftFlow(()=>user,()=>false,(f,a)=>{throw new Exception("Unexpected wait");},id=>{},()=>{},()=>{clockReads++;return now;});
            Expect<ArgumentOutOfRangeException>(invalid.Show);
            Check(clockReads==0,"Invalid Unix seconds throw before clock read");
            user.LastGetEveryDayGift=0;Action complete=null;
            var replacement=new OriginalUserLocalData(defaults);
            var reentrant=new OriginalEveryDayGiftFlow(()=>user,()=>false,(f,a)=>complete=a,
                id=>user=replacement,()=>trace.Add("reentrant save"),()=>now,()=>today);
            reentrant.Show();complete();Check(replacement.LastGetEveryDayGift==today,"User lookup occurs after panel initialization can replace user");
            user.LastGetEveryDayGift=0;
            var failed=new OriginalEveryDayGiftFlow(()=>user,()=>false,(f,a)=>complete=a,
                id=>throw new InvalidOperationException("panel"),()=>throw new Exception("Unexpected save"),()=>now,()=>throw new Exception("Unexpected clock"));
            failed.Show();Expect<InvalidOperationException>(complete);Check(user.LastGetEveryDayGift==0,"Panel failure leaves claim timestamp unchanged");
            var saveFailed=new OriginalEveryDayGiftFlow(()=>user,()=>false,(f,a)=>complete=a,
                id=>{},()=>throw new InvalidOperationException("save"),()=>now,()=>today);
            saveFailed.Show();Expect<InvalidOperationException>(complete);Check(user.LastGetEveryDayGift==today,"Save failure retains earlier timestamp mutation");
            var nullable=OriginalUntilCallback.Run(()=>true,null);
            Check(nullable.MoveNext()&&!((WaitUntil)nullable.Current).keepWaiting&&!nullable.MoveNext(),"Until callback may be null");
            Debug.Log("NUT_EVERY_DAY_GIFT_VALIDATION_PASS UTC date boundaries, lazy native wait, live panel state, duplicate waits, show/time/save order, reentry and exception mutations; actual gift panel and production startup routing pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action) where T:Exception
        {
            try{action();}catch(T){return;}throw new InvalidOperationException("Expected exception");
        }
    }
}
