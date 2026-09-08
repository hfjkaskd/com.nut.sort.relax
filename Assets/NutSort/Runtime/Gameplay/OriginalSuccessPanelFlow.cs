using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // SuccessPanel-specific Init/Refresh/TweenEnd/GetCallback. Base panel actions
    // remain explicit so incomplete presentation or SDK behavior is not simulated.
    public sealed class OriginalSuccessPanelFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<long> localTimeSeconds;
        private readonly Action playSound;
        private readonly Action<int> showPanel;
        public bool IsGuide { get; private set; }
        public OriginalSuccessPanelFlow(OriginalUserLocalData user,Func<long> localTimeSeconds,Action playSound,Action<int> showPanel)
        {this.user=user;this.localTimeSeconds=localTimeSeconds;this.playSound=playSound;this.showPanel=showPanel;}
        public void Init(Action baseInit)
        {
            baseInit();
            IsGuide=user.Level==2||user.Level==3;
            if(user.Level==2)user.Level1TXTime=localTimeSeconds();
            else if(user.Level==3)user.Level2TXTime=localTimeSeconds();
            playSound();
        }
        public void Refresh(Action baseRefresh,Action centerMoreLabel,Action<int> setMoreLabel,Action hideAd,Action hideGet)
        {
            baseRefresh();
            if(unchecked(user.Level-1)>2)return;
            centerMoreLabel();setMoreLabel(1);hideAd();hideGet();
        }
        public void TweenEndRefresh(Action baseTweenEnd)
        {
            baseTweenEnd();
            if(!IsGuide)return;
            user.GuideIndex=0;
            showPanel(7);
        }
        public void Hide(Action baseHide,Func<bool> newLevelMode,Func<bool> completePassStage2,Action synchronizeCompletedStage,Action restart)
        {
            baseHide();
            if(newLevelMode()){restart();return;}
            if(string.IsNullOrEmpty(user.TXTargetGold)&&completePassStage2())
            {
                synchronizeCompletedStage();return;
            }
            if(!IsGuide)restart();
        }
        public void GetCallback(Action close,Action baseGetCallback)
        {
            if(unchecked(user.Level-1)<=2)close();
            else baseGetCallback();
        }
    }
}
