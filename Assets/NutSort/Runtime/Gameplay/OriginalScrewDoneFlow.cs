using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // Existing visual, account and request consumers. Never invent a reward
    // amount, config document or successful asynchronous response here.
    public interface IOriginalScrewDoneEffects
    {
        float RemoveGold { get; }
        float RemoveCoin { get; }
        void FlyGold(ScrewState target);
        void FlyCoin(ScrewState target);
        void AddGold(float amount,bool refreshUI,bool synchronize);
        void AddCoin(float amount,bool refreshUI);
        bool HasPanel { get; }
        int Config(string field);
        long TimeSeconds { get; }
        long LuckyRewardTime { get; set; }
        long LuckyDrawTime { get; set; }
        void RequestLuckyReward();
        void ShowPanel(int id);
    }
    // ScrewInfo.DoneEvent 0xA09BC4. Success is the captured board result from
    // the move callback; all level/config/panel checks below read current state.
    public sealed class OriginalScrewDoneFlow
    {
        private readonly Func<OriginalUserLocalData> user;
        private readonly Func<int> showLevel;
        private readonly Action success;
        private readonly IOriginalScrewDoneEffects effects;
        public OriginalScrewDoneFlow(Func<OriginalUserLocalData> user,Func<int> showLevel,
            Action success,IOriginalScrewDoneEffects effects)
        {this.user=user??throw new ArgumentNullException(nameof(user));this.showLevel=showLevel??throw new ArgumentNullException(nameof(showLevel));this.success=success??throw new ArgumentNullException(nameof(success));this.effects=effects??throw new ArgumentNullException(nameof(effects));}
        public void Run(ScrewState target,bool completedBoard)
        {
            if(showLevel()>=3)
            {
                effects.FlyGold(target);
                effects.AddGold(effects.RemoveGold,true,!completedBoard);
                if(showLevel()>=4&&effects.RemoveCoin>0)
                {
                    effects.FlyCoin(target);
                    // The original queries the amount again after the flight call.
                    effects.AddCoin(effects.RemoveCoin,true);
                }
            }
            if(completedBoard){success();return;}
            if(user().Level>=3&&!effects.HasPanel&&
                (user().LuckyScrewDoneCount>=effects.Config("LSSLR2")||
                 unchecked(effects.TimeSeconds-effects.LuckyRewardTime)>=effects.Config("LSSLR1")))
            {
                user().LuckyScrewDoneCount=0;
                effects.LuckyRewardTime=effects.TimeSeconds;
                effects.RequestLuckyReward();return;
            }
            if(user().Level>=3&&!effects.HasPanel&&
                (user().LuckyDrawScrewDoneTimes>=effects.Config("LSSLD2")||
                 unchecked(effects.TimeSeconds-effects.LuckyDrawTime)>=effects.Config("LSSLD1")))
            {
                user().LuckyDrawScrewDoneTimes=0;
                effects.LuckyDrawTime=effects.TimeSeconds;
                effects.ShowPanel(12);return;
            }
            if(!user().IsCompleteAppRatingPanel&&user().Level>=effects.Config("LSSR1")&&!effects.HasPanel)
            {
                user().IsCompleteAppRatingPanel=true;
                effects.ShowPanel(2);
            }
            // SDK spin telemetry is intentionally excluded. No new save point.
        }
    }
}
