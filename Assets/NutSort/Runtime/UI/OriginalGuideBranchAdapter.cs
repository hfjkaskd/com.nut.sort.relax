using System;
using NutSort.Content;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public readonly struct OriginalGuideButtonBinding
    {
        public readonly Button Button;
        public readonly Action Activate;
        public OriginalGuideButtonBinding(Button button,Action activate){Button=button;Activate=activate;}
    }
    public interface IOriginalGuideUI
    {
        OriginalGuideButtonBinding GetSuccessMoreGet();
        OriginalGuideButtonBinding GetWithdrawal();
        RectTransform GoldTarget { get; }
        RectTransform CoinTarget { get; }
        void Save();
        void ShowPanel(int id);
        void ShowTargetPanel(int id,int level);
        void SetMask(bool enabled,float duration,string text);
        void Schedule(float delay,Action action);
        void RefreshGold();
        void RefreshCoin();
        void InitializeLevel(bool reset,bool showBanner,bool firstInit);
        void RequestTargetGoldInfo(bool showMask);
        void RequestCoinGoldInfo(bool showMask);
        void RequestEntryGoldInfo(bool showMask);
        void NewGameplayUnlock(bool showBanner);
        void DailyGift();
        void RefreshMainAfterTween();
        void ShowTargetBanner(Action completed);
    }
    // Connects every native ShowGuide branch to its recovered view implementation.
    // Target bindings are resolved when shown, preserving captured panel callbacks.
    public sealed class OriginalGuideBranchAdapter:IOriginalGuideBranches
    {
        private readonly OriginalNewbieGuideView view;
        private readonly OriginalUserLocalData user;
        private readonly IOriginalGuideUI ui;
        public OriginalGuideBranchAdapter(OriginalNewbieGuideView view,OriginalUserLocalData user,IOriginalGuideUI ui)
        {
            this.view=view;this.user=user;this.ui=ui;
        }
        public void ShowSuccess()
        {
            var target=ui.GetSuccessMoreGet();
            view.ShowSuccessGuide(user,target.Button,target.Activate,ui.SetMask,ui.Schedule,ui.ShowPanel);
        }
        public void ShowTargetCompletion()=>view.ShowTargetCompletion(user,ui.RequestTargetGoldInfo,ui.Save,ui.ShowTargetPanel);
        public void ShowWithdrawal()
        {
            var target=ui.GetWithdrawal();
            view.ShowWithdrawalGuide(user,target.Button.GetComponent<RectTransform>(),ui.Save,ui.ShowPanel,ui.RefreshGold,ui.InitializeLevel);
        }
        public void ShowCoin()=>view.ShowCoinGuide(user,ui.CoinTarget,ui.RefreshCoin,ui.Save,ui.RequestCoinGoldInfo,ui.NewGameplayUnlock);
        public void ShowGoldEntry(int guideIndex)=>view.ShowGoldEntryGuide(user,ui.GoldTarget,ui.RequestEntryGoldInfo);
        public void ShowWithdrawalStage(int guideIndex)
        {
            var target=ui.GetWithdrawal();
            view.ShowWithdrawalStageGuide(user,target.Button.GetComponent<RectTransform>(),target.Activate,ui.Save,ui.DailyGift,ui.RefreshMainAfterTween,ui.ShowTargetBanner);
        }
    }
}
