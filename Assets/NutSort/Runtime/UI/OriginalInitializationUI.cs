using System;
using NutSort.World;
namespace NutSort.UI
{
    public interface IOriginalInitializationPanels
    {
        bool HasPanel { get; }
        void ShowPanel(int id);
        void ShowUnlockPanel(int id,int index,bool showBanner);
        void CloseAll();
    }
    // Concrete UI side of InitDoneEvent. Request completion and global panel
    // dispatch remain explicit; no request reply or initialization success is invented.
    public sealed class OriginalInitializationUI:IOriginalInitializationUI
    {
        private readonly OriginalGameScene game;
        private readonly Func<OriginalMainPanelView> main;
        private readonly Func<OriginalRecordGuidePanelHost> record;
        private readonly IOriginalInitializationPanels panels;
        private readonly Action<Action> synchronize;
        public OriginalInitializationUI(OriginalGameScene game,Func<OriginalMainPanelView> main,
            Func<OriginalRecordGuidePanelHost> record,IOriginalInitializationPanels panels,Action<Action> synchronize)
        {this.game=game;this.main=main;this.record=record;this.panels=panels;this.synchronize=synchronize;}
        public void SynchronizeCompletedStage(Action completed)=>synchronize(completed);
        public void ShowPanel(int id)
        {
            if(id==34)record().Show();else panels.ShowPanel(id);
        }
        public void ShowUnlockPanel(int id,int index,bool showBanner)=>panels.ShowUnlockPanel(id,index,showBanner);
        public void CloseAllPanels()=>panels.CloseAll();
        public void ShowTargetRewardBanner(Action completed)=>main().TargetReward.Show(completed);
        // NewbieGuidePanel.HideLevelHint 0x9E38C4 is a single native return.
        public void HideLevelHint() { }
        public void ShowEveryDayGift()=>game.ShowEveryDayGift(()=>panels.HasPanel,panels.ShowPanel);
        public void PushPlayerGoldHint()=>main().Top.PlayerHint.Push();
        public void CloseRecordGuide()=>record().Close();
    }
}
