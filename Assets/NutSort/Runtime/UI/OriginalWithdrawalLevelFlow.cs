using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalLevelUI
    {
        void SetTip(int id,int level);
        void SetGold(string value);
        void PlaySteps();
        void Close();
        bool HasPanel(int id);
        void HidePanel(int id);
        void RefreshGoldItem();
        void ShowSelf(float amount);
        void RefreshGold(bool showTip,float addGold);
    }
    // TXLevelPanel native lifecycle and GoldGet1 consumer; transport remains external.
    public sealed class OriginalWithdrawalLevelFlow
    {
        public static bool IsRecordIn;
        private readonly OriginalUserLocalData user;
        private readonly IOriginalWithdrawalLevelUI ui;
        private readonly Func<float,string> format;
        private readonly Action<int,Action<JObject>> request;
        private readonly Action save;
        private readonly Func<object,Action> guide;
        private readonly Action<float,Action> delay;
        public int Level{get;private set;}
        public bool RecordEntry{get;private set;}
        public OriginalWithdrawalLevelFlow(OriginalUserLocalData user,IOriginalWithdrawalLevelUI ui,Func<float,string> format,
            Action<int,Action<JObject>> request,Action save,Func<object,Action> guide,Action<float,Action> delay)
        {this.user=user;this.ui=ui;this.format=format;this.request=request;this.save=save;this.guide=guide;this.delay=delay;}
        public void Init(object[] args,Action baseInit,Action bindClose,Action bindGet)
        {
            baseInit();Level=(int)args[0];RecordEntry=IsRecordIn;IsRecordIn=false;bindClose();bindGet();
        }
        public void Refresh(){ui.SetTip(89,Math.Min(Level,3));ui.SetGold(format(Level==1?user.Level1Gold:user.Level2Gold));ui.PlaySteps();}
        public void Close(){guide(null);ui.Close();}
        public void Get()
        {
            ui.Close();request(Level,new Action<JObject>(Receive));
            if(ui.HasPanel(5))ui.HidePanel(5);
        }
        private void Receive(JObject ignored)
        {
            bool reduced=false;
            if(Level==1)
            {
                user.IsTXLevel1=true;
                if(!RecordEntry&&!user.IsGoldReduceLevel1){user.IsGoldReduceLevel1=true;user.Gold-=user.Level1Gold;reduced=true;}
            }
            else if(Level==2)
            {
                user.IsTXLevel2=true;
                if(!RecordEntry&&!user.IsGoldReduceLevel2){user.IsGoldReduceLevel2=true;user.Gold-=user.Level2Gold;reduced=true;}
            }
            if(reduced){user.Gold=Math.Max(user.Gold,0f);ui.RefreshGoldItem();}
            save();ui.ShowSelf(Level==1?user.Level1Gold:user.Level2Gold);ui.RefreshGold(false,0);
            if(!RecordEntry){Action callback=guide(Level);delay(1,callback);}
        }
    }
}
