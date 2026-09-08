using System;
namespace NutSort.Content
{
    // UserMgr.SetGold: target assignment, not additive reward and not a debit API.
    public sealed class OriginalSetGoldFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Action<string> error;
        private readonly Action<bool,float> refresh;
        private readonly Action sync,save;
        public OriginalSetGoldFlow(OriginalUserLocalData user,Action<string> error,Action<bool,float> refresh,Action sync,Action save)
        {this.user=user;this.error=error;this.refresh=refresh;this.sync=sync;this.save=save;}
        public void Set(float gold,bool isRefreshUI=true,bool isSyncInfo=true)
        {
            if(gold<user.Gold){error("targetGold < UserLocalData.Gold:"+gold.ToString());return;}
            float added=gold-user.Gold;user.Gold=gold;
            if(added>0&&isRefreshUI)refresh(true,added);
            if(isSyncInfo)sync();
            save();
        }
    }
}
