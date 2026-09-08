using System;
using NutSort.Content;
namespace NutSort.UI
{
    // NewbieGuidePanel display-class callback 0x9E41DC. The integer payload is
    // supplied by the existing application boundary, never invented here.
    public sealed class OriginalWithdrawalGuideCompletion
    {
        private readonly OriginalUserLocalData user;
        private readonly int guideIndex;
        private readonly Action refreshGold;
        private readonly Action<bool,bool,bool> initializeLevel;
        public OriginalWithdrawalGuideCompletion(OriginalUserLocalData user,int guideIndex,
            Action refreshGold,Action<bool,bool,bool> initializeLevel)
        {
            this.user=user;this.guideIndex=guideIndex;this.refreshGold=refreshGold;this.initializeLevel=initializeLevel;
        }
        public void Complete(object parameter)
        {
            int stage=(int)parameter;
            if(guideIndex==2)
            {
                bool reduced=false;
                if(stage==1 && !user.IsGoldReduceLevel1)
                {
                    user.IsGoldReduceLevel1=true;
                    user.Gold-=user.Level1Gold;
                    reduced=true;
                }
                else if(stage==2 && !user.IsGoldReduceLevel2)
                {
                    user.IsGoldReduceLevel2=true;
                    user.Gold-=user.Level2Gold;
                    reduced=true;
                }
                if(reduced){user.Gold=Math.Max(0f,user.Gold);refreshGold();}
            }
            initializeLevel(true,true,false);
        }
    }
}
