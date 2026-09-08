using System;

namespace NutSort.Content
{
    public sealed class OriginalItemManager
    {
        private readonly OriginalUserLocalData user;
        private readonly Action save,refreshBottom;
        private readonly Action<float,bool,bool> setGold;
        private readonly Action<double,bool> setCoin;
        public OriginalItemManager(OriginalUserLocalData user,Action save,Action refreshBottom,
            Action<float,bool,bool> setGold,Action<double,bool> setCoin)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.save=save ?? throw new ArgumentNullException(nameof(save));
            this.refreshBottom=refreshBottom ?? throw new ArgumentNullException(nameof(refreshBottom));
            this.setGold=setGold ?? throw new ArgumentNullException(nameof(setGold));
            this.setCoin=setCoin ?? throw new ArgumentNullException(nameof(setCoin));
        }
        public void AddTool(int type,float count,bool refresh) => Add(new OriginalItemInfo { ItemType=type,Count=count },refresh);
        public void Add(OriginalItemInfo item,bool refresh=true)
        {
            if(item==null)throw new NullReferenceException("itemInfo");
            switch(item.ItemType)
            {
                case 0:setGold(item.CurrentCount,true,true);return;
                case 1:setCoin(item.DoubleCurrentCount,true);return;
                case 2:user.RevokeCount=unchecked(user.RevokeCount+item.IntCount);break;
                case 3:user.ExchangeCount=unchecked(user.ExchangeCount+item.IntCount);break;
                case 4:user.AddScrewCount=unchecked(user.AddScrewCount+item.IntCount);break;
                default:return;
            }
            save();
            if(refresh)refreshBottom();
        }
    }
}
