using System;
using NutSort.Content;

namespace NutSort.Gameplay
{
    // Stable original UIName values; never use enum names as resource/save keys.
    public enum OriginalInitializationPanel
    {
        NewbieGuide = 7,
        RecordGuide = 34,
        AssessGuide = 37,
        CoinRewardHint = 42,
        CoinNewPeopleReward = 43
    }

    // Required scene/UI operations, with no success defaults for missing systems.
    // This port is not yet bound to the production scene: those panels and the
    // reward/unlock consumers must be restored before binding. The level view
    // now provides the original stateful deadlock check.
    public interface IOriginalInitializationActions
    {
        OriginalUserLocalData User { get; }
        int ShowLevel { get; }
        bool IsInitDone { get; set; }
        bool IsCannotMove();
        bool IsGuidePassStage2Level();
        bool NewGameplayUnlock(bool showBanner);
        void Fail();
        void ShowPanel(OriginalInitializationPanel panel);
        void CloseAllPanels();
        void ShowTargetRewardBanner(Action completed);
        void HideLevelHint();
        void ShowEveryDayGift();
        void PushPlayerGoldHint();
        void CloseRecordGuide();
    }

    // Authored from InitDoneEvent 0x9FBD2C, its callback 0xA002F0 and
    // TXRecordGuidePanel.StartCallback 0x9D2140. Keep checks lazy: IsCannotMove
    // invokes CheckDie and can reveal masks/save, so eager evaluation changes play.
    public sealed class OriginalInitializationFlow
    {
        private readonly IOriginalInitializationActions actions;

        public OriginalInitializationFlow(IOriginalInitializationActions actions)
        {
            this.actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public void Run(bool showBanner = true, bool firstInit = false)
        {
            if (!actions.User.IsCompleteRecordGuide)
            {
                actions.IsInitDone = true;
                actions.ShowPanel(OriginalInitializationPanel.RecordGuide);
                return;
            }

            bool showCoinHint = showBanner && actions.ShowLevel == 4;
            var user = actions.User;
            if ((user.Level == 2 && !user.IsGoldReduceLevel1) ||
                (user.Level == 3 && !user.IsGoldReduceLevel2))
                ShowGuide(1);
            else if (!user.IsCompleteExtraGoldGuide && user.Level == 3)
                actions.ShowPanel(OriginalInitializationPanel.AssessGuide);
            else if (actions.IsCannotMove())
                actions.Fail();
            else if (actions.IsGuidePassStage2Level())
                ShowGuide(10);
            else if (!string.IsNullOrEmpty(actions.User.ComeOnGold))
                ShowGuide(12);
            else if (actions.User.IsGuideGold)
                ShowGuide(14);
            else if (showCoinHint)
            {
                actions.CloseAllPanels();
                actions.ShowPanel(OriginalInitializationPanel.CoinRewardHint);
            }
            else if (!actions.NewGameplayUnlock(showBanner) && showBanner)
            {
                // Capture this invocation's firstInit; the callback reads current
                // user state, not a stale snapshot from when the banner opened.
                actions.ShowTargetRewardBanner(() => BannerCompleted(firstInit));
                return;
            }
            actions.IsInitDone = true;
        }

        private void ShowGuide(int index)
        {
            actions.User.GuideIndex = index;
            actions.ShowPanel(OriginalInitializationPanel.NewbieGuide);
        }

        private void BannerCompleted(bool firstInit)
        {
            actions.IsInitDone = true;
            if (actions.User.Level <= 1)
                actions.ShowPanel(OriginalInitializationPanel.NewbieGuide);
            else if (!actions.User.IsCompleteCoinNewPeopleReward && actions.ShowLevel >= 5)
                actions.ShowPanel(OriginalInitializationPanel.CoinNewPeopleReward);
            else
            {
                actions.HideLevelHint();
                actions.ShowEveryDayGift();
            }
            if (firstInit) actions.PushPlayerGoldHint();
        }

        public void CompleteRecordGuide()
        {
            actions.CloseRecordGuide();
            actions.User.IsCompleteRecordGuide = true;
            Run(true, false);
            // Original SDK telemetry is intentionally excluded. This callback
            // itself does not SaveData; do not introduce an extra save point.
        }
    }
}
