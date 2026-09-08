using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // LuoSiSortMgr.AddScrew 0x9FF33C. World effects remain required operations:
    // do not replace an unavailable unlock/add implementation with a success default.
    public sealed class OriginalAddScrewFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<int> maximum;
        private readonly Func<bool> singleTile,unlock;
        private readonly Action addTile,addNullScrew;
        private readonly Action<int,float,bool> applyItem;
        private readonly Action<int,int> showToolPanel;
        private readonly Action<int> showTip;
        public OriginalAddScrewFlow(OriginalUserLocalData user,Func<int> maximum,Func<bool> singleTile,
            Func<bool> unlock,Action addTile,Action addNullScrew,Action<int,float,bool> applyItem,
            Action<int,int> showToolPanel,Action<int> showTip)
        {
            this.user=user;this.maximum=maximum;this.singleTile=singleTile;this.unlock=unlock;
            this.addTile=addTile;this.addNullScrew=addNullScrew;this.applyItem=applyItem;
            this.showToolPanel=showToolPanel;this.showTip=showTip;
        }
        public void Run()
        {
            if(user.AddScrewCount<=0){showToolPanel(4,4);return;}
            if(user.CurrentLevelAddScrewCount>=maximum()){showTip(5);return;}
            user.CurrentLevelAddScrewCount=unchecked(user.CurrentLevelAddScrewCount+(singleTile()?1:4));
            if(!unlock())
            {
                // Native code reads the live mode again after Unlock returns.
                if(singleTile())addTile();else addNullScrew();
            }
            applyItem(4,-1,true);
            // SDK vibration and analytics deliberately excluded.
        }
    }
}
