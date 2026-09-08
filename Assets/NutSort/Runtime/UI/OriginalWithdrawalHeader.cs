using System;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalHeaderUI
    {
        void SetLevelTitle(int slot,int textId,int value);
        void SetGold(string value);
        void SetTitle(int textId,int value);
    }
    // Initial TXPanel.Refresh block 0x9cbce8-0x9cbfc8. Remaining refresh
    // (pending gold animation/tasks/progress) follows this block in the view.
    public sealed class OriginalWithdrawalHeader
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<int> showLevel;
        private readonly Func<float,string> format;
        private readonly IOriginalWithdrawalHeaderUI ui;
        public OriginalWithdrawalHeader(OriginalUserLocalData user,Func<int> showLevel,
            Func<float,string> format,IOriginalWithdrawalHeaderUI ui)
        {this.user=user;this.showLevel=showLevel;this.format=format;this.ui=ui;}
        public void Refresh(int level,object[] arguments)
        {
            if(level==1)ui.SetGold(format(user.Level1Gold));
            else if(level==2)
            {
                ui.SetLevelTitle(2,30,1);
                ui.SetGold(format(user.Level2Gold));
            }
            else
            {
                ui.SetLevelTitle(1,30,1);ui.SetLevelTitle(2,30,2);
                ui.SetGold(format(user.Gold));
            }
            int hasArgument=arguments.Length==0?0:1;
            ui.SetTitle(30,unchecked(showLevel()-hasArgument));
        }
    }
}
