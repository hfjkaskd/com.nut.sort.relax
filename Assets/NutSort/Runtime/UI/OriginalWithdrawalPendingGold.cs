using System;
using NutSort.Content;
namespace NutSort.UI
{
    // TXPanel.Refresh pending ComeOnGold block, 0x9cbfcc-0x9cc130.
    public sealed class OriginalWithdrawalPendingGold
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<bool> isPlayGoldTween;
        private readonly Action<float,bool,bool> setGold;
        private readonly Action save;
        private readonly Action<float,float,float,Action<float>> animate;
        private readonly Func<float,string> format;
        private readonly Action<string> display;
        private readonly float duration;
        public bool IsShowTargetHint { get; private set; }
        public OriginalWithdrawalPendingGold(OriginalUserLocalData user,Func<bool> isPlayGoldTween,
            Action<float,bool,bool> setGold,Action save,Action<float,float,float,Action<float>> animate,
            Func<float,string> format,Action<string> display,float duration)
        {this.user=user;this.isPlayGoldTween=isPlayGoldTween;this.setGold=setGold;this.save=save;this.animate=animate;this.format=format;this.display=display;this.duration=duration;}
        public void Refresh()
        {
            if(string.IsNullOrEmpty(user.ComeOnGold)||!isPlayGoldTween())return;
            IsShowTargetHint=true;
            float from=user.Gold;
            setGold(OriginalRewardProgress.ToFloat(user.ComeOnGold),true,true);
            user.ComeOnGold=string.Empty;
            save();
            animate(from,user.Gold,duration,UpdateDisplay);
        }
        private void UpdateDisplay(float value){display(format(value));}
    }
}
