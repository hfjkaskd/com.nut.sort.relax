using System;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    // Explicit UI/request boundary. The scene owns gameplay and initialization
    // state; UI owns panel registration and visual callbacks.
    public interface IOriginalInitializationUI
    {
        void SynchronizeCompletedStage(Action completed);
        void ShowPanel(int panelId);
        void ShowUnlockPanel(int panelId, int index, bool showBanner);
        void CloseAllPanels();
        void ShowTargetRewardBanner(Action completed);
        void HideLevelHint();
        void ShowEveryDayGift();
        void PushPlayerGoldHint();
        void CloseRecordGuide();
    }

    public sealed class OriginalSceneInitialization : IOriginalInitializationActions
    {
        private readonly OriginalGameScene game;
        private readonly OriginalRewardProgress progress;
        private readonly IOriginalInitializationUI ui;
        private readonly OriginalInitializationFlow flow;

        public OriginalSceneInitialization(OriginalGameScene game, OriginalRewardProgress progress,
            IOriginalInitializationUI ui)
        {
            this.game = game != null ? game : throw new ArgumentNullException(nameof(game));
            this.progress = progress ?? throw new ArgumentNullException(nameof(progress));
            this.ui = ui ?? throw new ArgumentNullException(nameof(ui));
            flow = new OriginalInitializationFlow(this);
        }

        public void Bind(Func<UnityEngine.Object> mainPanel)
        {
            game.BindInitialization(mainPanel, new OriginalInitializationContinuation(User,
                progress.IsCompletePassStage2Level, ui.SynchronizeCompletedStage, flow.Run));
        }

        public OriginalUserLocalData User => game.User;
        public int ShowLevel => game.ShowLevel;
        public bool IsInitDone { get => game.IsInitDone; set => game.IsInitDone = value; }
        public bool IsCannotMove() => game.Level.IsCannotMove();
        public bool IsGuidePassStage2Level() => progress.IsGuidePassStage2Level();
        public bool NewGameplayUnlock(bool showBanner) => game.NewGameplayUnlock(showBanner, ui.ShowUnlockPanel);
        public void Fail() => game.Fail(ui.ShowPanel);
        public void ShowPanel(OriginalInitializationPanel panel) => ui.ShowPanel((int)panel);
        public void CloseAllPanels() => ui.CloseAllPanels();
        public void ShowTargetRewardBanner(Action completed) => ui.ShowTargetRewardBanner(completed);
        public void HideLevelHint() => ui.HideLevelHint();
        public void ShowEveryDayGift() => ui.ShowEveryDayGift();
        public void PushPlayerGoldHint() => ui.PushPlayerGoldHint();
        public void CloseRecordGuide() => ui.CloseRecordGuide();
        public void CompleteRecordGuide() => flow.CompleteRecordGuide();
    }
}
