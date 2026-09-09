using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalTooFastUI
    {
        void SetLevelTip(int id,int level);
        void SetVideoTip(int id,int count);
        void SetLevelFill(float value);
        void SetVideoFill(float value);
        void SetLevelValue(string value);
        void SetVideoValue(string value);
        void Close();
    }
    public sealed class OriginalWithdrawalTooFastFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly IOriginalWithdrawalTooFastUI ui;
        private readonly Func<int> showLevel;
        private readonly Func<Action> findStageProgress;
        public OriginalWithdrawalTooFastFlow(OriginalUserLocalData user,IOriginalWithdrawalTooFastUI ui,Func<int> showLevel,Func<Action> findStageProgress)
        {this.user=user;this.ui=ui;this.showLevel=showLevel;this.findStageProgress=findStageProgress;}
        public void Init(Action baseInit,Action<Action> bindSure)
        {baseInit();bindSure(Sure);}
        public void Refresh()
        {
            OriginalWithdrawalStageZeroFlow.IsShowOnlineTimeHint=true;
            var row=(JObject)((JArray)OriginalWithdrawalQueue.Field(user.GoldRewardTargetS2CData,"bear_list"))[2];
            ui.SetLevelTip(165,Target(row));
            ui.SetVideoTip(166,10);
            ui.SetLevelFill((float)unchecked(showLevel()-1)/Target(row));
            // Source presentation is literal; this is not an advertisement completion check.
            ui.SetVideoFill(1);
            ui.SetLevelValue(string.Format("{0}/{1}",unchecked(showLevel()-1),Target(row)));
            ui.SetVideoValue("10/10");
        }
        public void Sure()
        {
            ui.Close();
            // Resolve the live UIName 23 after initiating close. Missing target must fail.
            findStageProgress()();
        }
        private static int Target(JObject row)=>(int?)OriginalWithdrawalQueue.Field(row,"Stage2StartShowLevel")??0;
    }
}
