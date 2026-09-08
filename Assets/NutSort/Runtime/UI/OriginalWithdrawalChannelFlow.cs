using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalChannelUI
    {
        string Info{get;set;}
        void ShowTip(int id);
        void ShowPanel(int id,object[] args);
        void Close();
        void HidePanel(int id);
    }
    public sealed class OriginalWithdrawalChannelFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly IOriginalWithdrawalChannelUI ui;
        public int Level{get;private set;}
        public OriginalWithdrawalChannelFlow(OriginalUserLocalData user,IOriginalWithdrawalChannelUI ui){this.user=user;this.ui=ui;}
        public void Init(object[] args,Action baseInit,Action bindSure,Action bindClose)
        {baseInit();Level=(int)args[0];bindSure();bindClose();}
        public void Refresh()
        {
            JToken value=null;
            if(user.UserLssInfo!=null)foreach(var p in user.UserLssInfo.Properties())if(string.Equals(p.Name,"OtherInfo",StringComparison.OrdinalIgnoreCase))value=p.Value;
            ui.Info=(string)value;
        }
        public void Sure()
        {
            if(ui.Info.Length<=4){ui.ShowTip(120);return;}
            JObject account=user.UserLssInfo;bool created=account==null;
            if(created)account=new JObject();
            JProperty found=null;
            foreach(var p in account.Properties())if(string.Equals(p.Name,"OtherInfo",StringComparison.OrdinalIgnoreCase))found=p;
            string value=ui.Info;
            if(found==null)account["OtherInfo"]=value;else found.Value=value==null?JValue.CreateNull():new JValue(value);
            if(created)user.UserLssInfo=account;
            ui.ShowPanel(21,new object[]{Level});ui.Close();ui.HidePanel(20);
        }
        public void Close()=>ui.Close();
    }
}
