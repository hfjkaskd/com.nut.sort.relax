using System;
using NutSort.World;
using UnityEngine;
namespace NutSort.UI
{
    public interface IOriginalGuidePanels
    {
        OriginalGuideSuccessBinding GetSuccessGuideTarget();
        OriginalGuideButtonBinding GetWithdrawal();
        void ShowPanel(int id);
        void ShowTargetPanel(int id,int level);
        void ShowUnlockPanel(int id,int index,bool showBanner);
        bool HasPanel { get; }
    }
    public interface IOriginalGuideRequests
    {
        void TargetGoldInfo(bool showMask);
        void CoinGoldInfo(bool showMask);
        void EntryGoldInfo(bool showMask);
    }
    // Real scene/main-panel operations; unresolved panels and request response
    // handlers stay explicit instead of being replaced by simulated successes.
    public sealed class OriginalSceneGuideUI:IOriginalGuideUI
    {
        private readonly OriginalGameScene game;
        private readonly Func<OriginalMainPanelView> main;
        private readonly OriginalCountedMask mask;
        private readonly IOriginalGuidePanels panels;
        private readonly IOriginalGuideRequests requests;
        public OriginalSceneGuideUI(OriginalGameScene game,Func<OriginalMainPanelView> main,
            OriginalCountedMask mask,IOriginalGuidePanels panels,IOriginalGuideRequests requests)
        {this.game=game;this.main=main;this.mask=mask;this.panels=panels;this.requests=requests;}
        public OriginalGuideSuccessBinding GetSuccessGuideTarget()=>panels.GetSuccessGuideTarget();
        public OriginalGuideButtonBinding GetWithdrawal()=>panels.GetWithdrawal();
        public RectTransform GoldTarget=>main().Top.Gold.GetComponent<RectTransform>();
        public RectTransform CoinTarget=>main().Top.Coin.GetComponent<RectTransform>();
        public void Save()=>game.SaveUserData();
        public void ShowPanel(int id)=>panels.ShowPanel(id);
        public void ShowTargetPanel(int id,int level)=>panels.ShowTargetPanel(id,level);
        public void SetMask(bool enabled,float duration,string text)=>mask.SetMask(enabled,duration,text);
        public void Schedule(float delay,Action action)=>game.ScheduleDelay(delay,action);
        public void RefreshGold()=>main().Top.Gold.Refresh();
        public void RefreshCoin()=>main().Top.Coin.Refresh();
        public void InitializeLevel(bool reset,bool showBanner,bool firstInit)=>game.InitLevel(reset,showBanner,firstInit);
        public void RequestTargetGoldInfo(bool showMask)=>requests.TargetGoldInfo(showMask);
        public void RequestCoinGoldInfo(bool showMask)=>requests.CoinGoldInfo(showMask);
        public void RequestEntryGoldInfo(bool showMask)=>requests.EntryGoldInfo(showMask);
        public void NewGameplayUnlock(bool showBanner)=>game.NewGameplayUnlock(showBanner,panels.ShowUnlockPanel);
        public void DailyGift()=>game.ShowEveryDayGift(()=>panels.HasPanel,panels.ShowPanel);
        public void RefreshMain()=>main().Refresh();
        public void ShowTargetBanner(Action completed)=>main().TargetReward.Show(completed);
    }
}
