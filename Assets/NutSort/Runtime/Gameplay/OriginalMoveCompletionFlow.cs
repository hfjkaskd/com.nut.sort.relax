using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // Operator b__0 0xA0A9A4 runs after the last immediate NutInfo data transfer.
    // Its delayed b__2 0xA0AF38 is independent of landing/cap animation callbacks.
    public sealed class OriginalMoveCompletionFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<bool> isSuccess,isCannotMove;
        private readonly Action<float,Action> delay;
        private readonly Action<ScrewState,bool> doneEvent;
        private readonly Action hideGuide,pushHint,fail;
        private readonly Action<ScrewState> refreshTypes;
        private readonly float normalDelay,successDelay;
        public OriginalMoveCompletionFlow(OriginalUserLocalData user,Func<bool> isSuccess,Func<bool> isCannotMove,
            Action<float,Action> delay,Action<ScrewState,bool> doneEvent,Action hideGuide,Action pushHint,
            Action<ScrewState> refreshTypes,Action fail,float normalDelay,float successDelay)
        {
            this.user=user;this.isSuccess=isSuccess;this.isCannotMove=isCannotMove;this.delay=delay;
            this.doneEvent=doneEvent;this.hideGuide=hideGuide;this.pushHint=pushHint;this.refreshTypes=refreshTypes;
            this.fail=fail;this.normalDelay=normalDelay;this.successDelay=successDelay;
        }
        public void Run(ScrewState target)
        {
            if(!target.IsDone){if(isCannotMove())fail();return;}
            user.ScrewDoneCount=unchecked(user.ScrewDoneCount+1);
            user.LuckyScrewDoneCount=unchecked(user.LuckyScrewDoneCount+1);
            user.LuckyDrawScrewDoneTimes=unchecked(user.LuckyDrawScrewDoneTimes+1);
            bool success=isSuccess();
            if(success&&user.Level==1&&user.LevelSeed>=3){hideGuide();pushHint();}
            delay(success?successDelay:normalDelay,()=>
            {
                doneEvent(target,success);
                if(!success&&isCannotMove())fail();
            });
            if(!success)refreshTypes(target);
            // SDK done analytics excluded.
        }
    }
}
