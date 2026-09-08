using System;
namespace NutSort.UI
{
    // UnlockGameplayPanel.RefreshLssPanel (0x9E946C).
    public sealed class OriginalGameplayUnlockDisplay
    {
        private readonly Func<int> count;
        private readonly Action<int,bool> setIcon;
        private readonly Action<int> setText;
        public OriginalGameplayUnlockDisplay(Func<int> count,Action<int,bool> setIcon,Action<int> setText)
        {
            this.count=count ?? throw new ArgumentNullException(nameof(count));
            this.setIcon=setIcon ?? throw new ArgumentNullException(nameof(setIcon));
            this.setText=setText ?? throw new ArgumentNullException(nameof(setText));
        }
        public void Refresh(int index)
        {
            for(int i=0;i<count();i++)setIcon(i,i==index);
            setText(unchecked(index+63));
        }
    }
}
