using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // Native RewardGetPanel button dispatch. SDK delegates must supply their own
    // real completion; this adapter never fabricates an ad result or a reward.
    public sealed class OriginalRewardGetFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<int> normalThreshold;
        private readonly Func<int> interstitialThreshold;
        private readonly Action<Action<bool>> interstitial;
        private readonly Action<Action<bool>,bool> rewarded;
        private readonly Func<long> timeSeconds;
        private readonly Action<long> setLuckyLastGetTime;
        private readonly Action<bool> getReward;
        public OriginalRewardGetFlow(OriginalUserLocalData user,Func<int> normalThreshold,
            Func<int> interstitialThreshold,Action<Action<bool>> interstitial,
            Action<Action<bool>,bool> rewarded,Func<long> timeSeconds,
            Action<long> setLuckyLastGetTime,Action<bool> getReward)
        {
            this.user=user;this.normalThreshold=normalThreshold;
            this.interstitialThreshold=interstitialThreshold;this.interstitial=interstitial;
            this.rewarded=rewarded;this.timeSeconds=timeSeconds;
            this.setLuckyLastGetTime=setLuckyLastGetTime;this.getReward=getReward;
        }
        public void GetCallback()
        {
            user.NormalGetTimes=unchecked(user.NormalGetTimes+1);
            if(user.NormalGetTimes<normalThreshold()){getReward(false);return;}
            user.InterAdTimes=unchecked(user.InterAdTimes+1);
            user.NormalGetTimes=0;
            if(user.InterAdTimes<=interstitialThreshold())interstitial(ReceiveNormalAd);
            else rewarded(ReceiveNormalAd,false);
        }
        public void MoreGetCallback()
        {
            user.NormalGetTimes=0;
            rewarded(ReceiveMoreAd,false);
        }
        private void ReceiveNormalAd(bool ignored){getReward(true);}
        private void ReceiveMoreAd(bool ignored)
        {
            user.LuckyScrewDoneCount=0;
            setLuckyLastGetTime(timeSeconds());
            getReward(true);
        }
    }
}
