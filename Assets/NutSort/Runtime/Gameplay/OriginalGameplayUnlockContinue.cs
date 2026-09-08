using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // UnlockGameplayPanel.ContinueCallback (0x9E9588).
    public sealed class OriginalGameplayUnlockContinue
    {
        private readonly OriginalUserLocalData user;
        private readonly Action<Action> showBanner;
        private readonly Action close;
        public OriginalGameplayUnlockContinue(OriginalUserLocalData user,Action<Action> showBanner,Action close)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.showBanner=showBanner ?? throw new ArgumentNullException(nameof(showBanner));
            this.close=close ?? throw new ArgumentNullException(nameof(close));
        }
        public void Run(int index,bool isShowBanner)
        {
            if(isShowBanner)showBanner(null);
            user.NewGameplayUnlockIndex=unchecked(index+1);
            close();
        }
    }
}
