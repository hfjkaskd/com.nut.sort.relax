using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // LuoSiSortMgr.Success 0x9FD578; reward requests remain a required external consumer.
    public sealed class OriginalSuccessFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<bool> succeeded,newLevelMode;
        private readonly Action markSucceeded,restartTeaching,refreshTeaching,hideGoldHint,hideProgress,playSound,playEffect;
        private readonly Func<OriginalLevelInfo> getLevel;
        private readonly Action<float,Action> delay;
        private readonly Action<OriginalLevelInfo> requestReward;
        private readonly float requestDelay;
        public OriginalSuccessFlow(OriginalUserLocalData user,Func<bool> succeeded,Action markSucceeded,Func<bool> newLevelMode,
            Action restartTeaching,Action refreshTeaching,Func<OriginalLevelInfo> getLevel,Action hideGoldHint,Action hideProgress,
            Action playSound,Action playEffect,Action<float,Action> delay,float requestDelay,Action<OriginalLevelInfo> requestReward)
        {
            this.user=user;this.succeeded=succeeded;this.markSucceeded=markSucceeded;this.newLevelMode=newLevelMode;
            this.restartTeaching=restartTeaching;this.refreshTeaching=refreshTeaching;this.getLevel=getLevel;
            this.hideGoldHint=hideGoldHint;this.hideProgress=hideProgress;this.playSound=playSound;this.playEffect=playEffect;
            this.delay=delay;this.requestDelay=requestDelay;this.requestReward=requestReward;
        }
        public void Run()
        {
            if(succeeded())return;
            markSucceeded();
            if(!newLevelMode()&&user.Level==1&&user.LevelSeed<=2)
            {
                user.LevelSeed=unchecked(user.LevelSeed+1);
                restartTeaching();refreshTeaching();return;
            }
            OriginalLevelInfo captured=getLevel();
            if(captured!=null&&captured.SubTotalRound==captured.SubRound)
            {
                hideGoldHint();hideProgress();playSound();playEffect();
            }
            delay(requestDelay,()=>requestReward(captured));
            // No level increment, save, reward grant or SDK analytics here in the source.
        }
    }
}
