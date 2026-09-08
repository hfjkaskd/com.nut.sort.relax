using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalConfirmationUI
    {
        void SetConfirmVisible(bool value);
        void SetTip(string text);
        void SetIcon(string path);
        void ShowPanel(int id,object[] args);
        void Close();
    }
    // TXUserInfoSurePanel native 0x9d59cc..0x9d63e0.
    public sealed class OriginalWithdrawalConfirmationFlow
    {
        public static bool IsHintGoldGet;
        private readonly OriginalUserLocalData user;
        private readonly OriginalPayChannels channels;
        private readonly OriginalChannelInfos infos;
        private readonly Func<string> country,area;
        private readonly IOriginalWithdrawalConfirmationUI ui;
        private readonly Action<int,bool> goldGet;
        private readonly Action<object> guide;
        private JObject account;
        private object[] arguments;
        public int Level{get;private set;}
        public OriginalWithdrawalConfirmationFlow(OriginalUserLocalData user,OriginalPayChannels channels,OriginalChannelInfos infos,
            Func<string> country,Func<string> area,IOriginalWithdrawalConfirmationUI ui,Action<int,bool> goldGet,Action<object> guide)
        {this.user=user;this.channels=channels;this.infos=infos;this.country=country;this.area=area;this.ui=ui;this.goldGet=goldGet;this.guide=guide;}
        public void Init(object[] args,Action baseInit,Action bindGet,Action bindReenter,Action bindClose)
        {
            arguments=args;baseInit();Level=args.Length!=0?(int)args[0]:unchecked(user.Level-1);account=user.UserLssInfo;
            bindGet();ui.SetConfirmVisible(!IsHintGoldGet);bindReenter();bindClose();
        }
        public void Refresh()
        {
            string other=(string)Field(account,"OtherInfo");
            if(!string.IsNullOrEmpty(other)){ui.SetTip(other??string.Empty);ui.SetIcon(OriginalChannelInfos.IconPath("Other",true));return;}
            var row=channels.GetPayChannel(string.Empty,country);
            var channel=infos.GetChannelInfo(row.Channels[(int?)Field(account,"GetType")??0]);
            bool email=channel.IsEmail;bool emptyName=string.IsNullOrEmpty((string)Field(account,"Name"));
            if(email)
                ui.SetTip(emptyName?((string)Field(account,"Email")??string.Empty):string.Concat((string)Field(account,"Name"),"\n",(string)Field(account,"Email")));
            else
                ui.SetTip(emptyName?string.Concat(area(),"-",(string)Field(account,"Number")):
                    string.Concat((string)Field(account,"Name"),"\n+",area(),"-",(string)Field(account,"Number")));
            ui.SetIcon(OriginalChannelInfos.IconPath(row.Channels[(int?)Field(account,"GetType")??0],true));
        }
        public void Get(){ui.Close();goldGet(Level,arguments!=null);}
        public void Reenter(){ui.ShowPanel(20,new object[]{arguments!=null?(object)Level:null});ui.Close();}
        public void Close(){ui.Close();guide(Level);IsHintGoldGet=false;}
        private static JToken Field(JObject row,string name)
        {
            if(row==null)throw new NullReferenceException("UserLssInfo");
            JToken value=null;foreach(var p in row.Properties())if(string.Equals(p.Name,name,StringComparison.OrdinalIgnoreCase))value=p.Value;return value;
        }
    }
}
