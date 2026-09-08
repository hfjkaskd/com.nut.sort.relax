using System;
using NutSort.Content;
namespace NutSort.UI
{
    // TXPanel Init, TweenEnd and close-button closure. Presentation and the
    // separate GoldGetCallback branch are supplied by their owning view.
    public sealed class OriginalWithdrawalPanelFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<bool> onlineHint,targetHint;
        private readonly Action<bool> setOnlineHint;
        private readonly Action<int> showPanel;
        private readonly Action close;
        public int Level { get; private set; }
        public OriginalWithdrawalPanelFlow(OriginalUserLocalData user,Func<bool> onlineHint,
            Action<bool> setOnlineHint,Func<bool> targetHint,Action<int> showPanel,Action close)
        {this.user=user;this.onlineHint=onlineHint;this.setOnlineHint=setOnlineHint;this.targetHint=targetHint;this.showPanel=showPanel;this.close=close;}
        public void Init(object[] arguments,Action baseInit,Action bindWithdrawal,Action bindClose)
        {
            baseInit();
            Level=arguments.Length>0?(int)arguments[0]:user.Level;
            bindWithdrawal();bindClose();
        }
        public void TweenEnd()
        {
            int index=user.GuideIndex;
            if(index==2||index==11||index==13||index==15)showPanel(7);
        }
        public void CloseCallback()
        {
            if(onlineHint()){setOnlineHint(false);showPanel(30);}
            else if(targetHint())showPanel(31);
            else OriginalNewbieGuideView.CallbackActionInvoke(null);
            close();
        }
    }
}
