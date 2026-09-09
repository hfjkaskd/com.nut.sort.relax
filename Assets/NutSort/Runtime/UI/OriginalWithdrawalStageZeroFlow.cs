using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalStageZeroUI:IOriginalWithdrawalStageUI
    {
        string Description{get;set;}
        void SetDescription(int id,params object[] args);
        void SetService(int id,int number);
        void SetTip(int id,int level);
        void PlayFirstProgress();
        void ShowPanel(int id,object[] args);
    }
    public sealed class OriginalWithdrawalStageZeroFlow:OriginalWithdrawalProgressFlow
    {
        private readonly IOriginalWithdrawalStageZeroUI view;
        private readonly object[] arguments;
        private readonly Func<long> clock;
        private readonly Func<int,int,int> random;
        private readonly Func<bool> complete;
        private readonly Func<int> showLevel;
        private readonly Action save;
        private readonly Action<object> guide;
        public static bool IsShowOnlineTimeHint;
        public bool IsGuide{get;set;}
        public OriginalWithdrawalStageZeroFlow(OriginalUserLocalData user,IOriginalWithdrawalStageZeroUI view,object[] arguments,
            Func<long> clock,Func<float,string> goldFormat,Func<long,string,string> timeFormat,Func<int,int,int> random,
            Func<bool> complete,Func<int> showLevel,Action save,Action<object> guide):base(user,view,clock,goldFormat,timeFormat)
        {this.view=view;this.arguments=arguments;this.clock=clock;this.random=random;this.complete=complete;this.showLevel=showLevel;this.save=save;this.guide=guide;}
        public override void Refresh()
        {
            base.Refresh();
            IsGuide=arguments.Length!=0;User.IsCompleteGuidePassStage2Level=true;
            // Source ns_wdc_1 SDK event intentionally remains excluded by task scope.
            JObject account=User.UserLssInfo;
            if(((long?)OriginalWithdrawalQueue.Field(account,"GoldGetTime")??0)<=0)OriginalWithdrawalQueue.Set(account,"GoldGetTime",clock());
            OriginalWithdrawalQueue.Refresh(account,random);
            if(((int?)OriginalWithdrawalQueue.Field(User.UserLssInfo,"GongHao")??0)<=0)
            {account=User.UserLssInfo;OriginalWithdrawalQueue.Set(account,"GongHao",random(6000000,8000000));}
            view.SetService(44,(int?)OriginalWithdrawalQueue.Field(User.UserLssInfo,"GongHao")??0);
            var row=(JObject)((JArray)OriginalWithdrawalQueue.Field(User.GoldRewardTargetS2CData,"bear_list"))[2];
            if(complete()){view.SetDescription(6,Array.Empty<object>());view.Description=view.Description.Replace("\n"," ");}
            else view.SetDescription(49,unchecked(showLevel()-1),(int?)OriginalWithdrawalQueue.Field(row,"Stage2StartShowLevel")??0);
            view.SetTip(40,(int?)OriginalWithdrawalQueue.Field(row,"Stage2StartShowLevel")??0);
            save();
        }
        public override void RefreshSteps()
        {
            base.RefreshSteps();
            if((bool?)OriginalWithdrawalQueue.Field(User.UserLssInfo,"TX0IsOpened")??false)return;
            OriginalWithdrawalQueue.Set(User.UserLssInfo,"TX0IsOpened",true);save();view.PlayFirstProgress();
        }
        public override void Sure()
        {
            UI.Close();
            if(IsGuide)view.ShowPanel(18,Array.Empty<object>());else guide(null);
        }
    }
}
