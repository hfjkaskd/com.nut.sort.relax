using System;
using UnityEngine;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalProgressUI
    {
        float Fill { get; set; }
        void SetTip(int id,params object[] arguments);
        void SetProgress(string text);
        void SetProgressTip(int id,params object[] arguments);
        void SetMarkerX(float x);
    }
    // TXPanel.Refresh stages one/two and their shared completion block.
    public sealed class OriginalWithdrawalEarlyProgress
    {
        private readonly Func<int> showLevel;
        private readonly IOriginalWithdrawalProgressUI ui;
        private readonly OriginalPanelSettings settings;
        public bool IsDoneTask { get; private set; }
        public OriginalWithdrawalEarlyProgress(Func<int> showLevel,IOriginalWithdrawalProgressUI ui,OriginalPanelSettings settings)
        {this.showLevel=showLevel;this.ui=ui;this.settings=settings;}
        public void Refresh(int stage,int level,object[] arguments)
        {
            int count,total;
            if(stage==1)
            {
                ui.SetTip(22,level);total=level;
                count=unchecked(level-(arguments.Length==0?1:0));
            }
            else if(stage==2)
            {
                total=settings.WithdrawalEarlyTotal;ui.SetTip(22,total);
                int first=showLevel();bool hasArguments=arguments.Length!=0;
                count=Math.Min(unchecked(first-1),total);
                if(!hasArguments)count=Math.Min(unchecked(showLevel()-1),unchecked(total-1));
            }
            else throw new ArgumentOutOfRangeException(nameof(stage));
            ui.Fill=(float)count/total;
            ui.SetProgress(string.Format("{0}/{1}",count,total));
            ui.SetProgressTip(25,string.Format("{0}",unchecked(total-count)));
            ui.SetMarkerX(Mathf.Clamp01(ui.Fill)*settings.WithdrawalProgressTravel+settings.WithdrawalProgressOffset);
            if(ui.Fill<settings.WithdrawalCompleteThreshold)return;
            IsDoneTask=true;ui.SetProgressTip(23);ui.SetTip(159);
        }
    }
}
