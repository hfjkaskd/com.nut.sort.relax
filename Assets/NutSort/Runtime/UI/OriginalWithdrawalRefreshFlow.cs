using System;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalRefreshHost
    {
        string MainGoldHint { get; }
        bool PlayGoldTween { get; set; }
        void SetRawTip(string text);
        void SetTip(int id);
        void SetClaimText(int id);
        void InitializePlayerInfo(int level);
    }
    // TXPanel.Refresh ordered composition. Existing adapters own each recovered branch.
    public sealed class OriginalWithdrawalRefreshFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly OriginalWithdrawalPanelFlow lifecycle;
        private readonly OriginalWithdrawalHeader header;
        private readonly OriginalWithdrawalPendingGold pending;
        private readonly OriginalWithdrawalEarlyProgress progress;
        private readonly OriginalRewardProgress stages;
        private readonly IOriginalWithdrawalRefreshHost host;
        private readonly Func<float,string> format;
        private readonly Func<int> startLevelShow,lastLevelShow;
        public OriginalWithdrawalRefreshFlow(OriginalUserLocalData user,OriginalWithdrawalPanelFlow lifecycle,
            OriginalWithdrawalHeader header,OriginalWithdrawalPendingGold pending,OriginalWithdrawalEarlyProgress progress,
            OriginalRewardProgress stages,IOriginalWithdrawalRefreshHost host,Func<float,string> format,
            Func<int> startLevelShow,Func<int> lastLevelShow)
        {
            this.user=user;this.lifecycle=lifecycle;this.header=header;this.pending=pending;this.progress=progress;
            this.stages=stages;this.host=host;this.format=format;this.startLevelShow=startLevelShow;this.lastLevelShow=lastLevelShow;
        }
        public void Refresh(object[] arguments)
        {
            header.Refresh(lifecycle.Level,arguments);
            pending.Refresh();
            host.SetClaimText(1);
            int stage=stages.GetStage(lifecycle.Level);
            if(stage==1||stage==2)progress.Refresh(stage,lifecycle.Level,arguments);
            else if(stage==3)progress.RefreshThird(arguments,startLevelShow,lastLevelShow);
            else progress.RefreshLater(stage,arguments,user,()=>host.MainGoldHint,host.SetRawTip,format,
                ()=>host.PlayGoldTween,host.SetClaimText);
            lifecycle.FinishRefresh(host.InitializePlayerInfo,value=>host.PlayGoldTween=value,host.SetTip);
        }
    }
}
