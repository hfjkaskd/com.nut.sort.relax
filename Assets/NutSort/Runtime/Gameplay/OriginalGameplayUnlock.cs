using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // LuoSiSortMgr.NewGameplayUnlock (0x9FC960).
    public sealed class OriginalGameplayUnlock
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<int[]> levels;
        private readonly Action<int,int,bool> showPanel;
        public OriginalGameplayUnlock(OriginalUserLocalData user,Func<int[]> levels,Action<int,int,bool> showPanel)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.levels=levels ?? throw new ArgumentNullException(nameof(levels));
            this.showPanel=showPanel ?? throw new ArgumentNullException(nameof(showPanel));
        }
        public bool Run(bool showBanner)
        {
            int[] configured=levels() ?? throw new NullReferenceException("LSSGPUL");
            for(int i=0;i<configured.Length;i++)
            {
                if(user.Level!=unchecked(configured[i]-4) || i<user.NewGameplayUnlockIndex)continue;
                showPanel(13,i,showBanner);
                return true;
            }
            return false;
        }
    }
}
