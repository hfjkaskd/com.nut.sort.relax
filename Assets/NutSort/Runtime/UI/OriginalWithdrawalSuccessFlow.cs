using System;
using NutSort.Content;
namespace NutSort.UI
{
    // TXSuccessPanel controller. Source visual prefab is not present in the current export.
    public sealed class OriginalWithdrawalSuccessFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<float,string> format;
        private readonly Action<string> count;
        private readonly Action<float,bool,bool> setGold;
        private readonly Action close;
        public OriginalWithdrawalSuccessFlow(OriginalUserLocalData user,Func<float,string> format,Action<string> count,
            Action<float,bool,bool> setGold,Action close)
        {this.user=user;this.format=format;this.count=count;this.setGold=setGold;this.close=close;}
        public void Init(Action baseInit,Action bindSure){baseInit();bindSure();}
        public void Refresh(){count(format(user.Gold));setGold(0,true,true);}
        public void Sure()=>close();
    }
}
