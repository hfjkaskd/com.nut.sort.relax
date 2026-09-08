using System;
using NutSort.Content;
namespace NutSort.UI
{
    // TXGuideTargetCompletePanel.RefreshLssPanel / TXCallback, 0x9C9EF8 / 0x9CA004.
    public sealed class OriginalGuideTargetCompletion
    {
        private readonly OriginalUserLocalData user;
        private readonly int level;
        private readonly Func<float,string> formatGold;
        private readonly Action close;
        private readonly Action<bool,bool> initializeDone;
        public OriginalGuideTargetCompletion(OriginalUserLocalData user,int level,
            Func<float,string> formatGold,Action close,Action<bool,bool> initializeDone)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));this.level=level;
            this.formatGold=formatGold ?? throw new ArgumentNullException(nameof(formatGold));
            this.close=close ?? throw new ArgumentNullException(nameof(close));
            this.initializeDone=initializeDone ?? throw new ArgumentNullException(nameof(initializeDone));
        }
        public string Refresh()=>formatGold(level==1?user.Level1Gold:level==2?user.Level2Gold:user.Gold);
        public void Continue()
        {
            close();
            user.IsCompleteRecordGuide=true;
            initializeDone(true,false);
            OriginalNewbieGuideView.CallbackActionInvoke(null);
        }
    }
}
