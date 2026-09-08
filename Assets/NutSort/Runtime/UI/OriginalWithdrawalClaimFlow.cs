using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalClaimUI
    {
        bool OnlineTimeHint { get; set; }
        bool Progress2Guide { set; }
        string Tip { get; }
        string Text(int id);
        void ShowPanel(int id,object[] arguments);
        void ShowTip(int id);
        void ShowTip(string text);
        void Close();
    }
    // TXPanel.GoldGetCallback 0x9c94e0; this routes UI, never pays currency.
    public sealed class OriginalWithdrawalClaimFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly OriginalRewardProgress progress;
        private readonly IOriginalWithdrawalClaimUI ui;
        public OriginalWithdrawalClaimFlow(OriginalUserLocalData user,OriginalRewardProgress progress,IOriginalWithdrawalClaimUI ui)
        {this.user=user;this.progress=progress;this.ui=ui;}
        public void Run(int level,object[] arguments,bool isDoneTask,bool targetHint)
        {
            var list=(JArray)Field(user.GoldRewardTargetS2CData,"bear_list");
            var row=(JObject)list[2];
            int real=(int?)Field(row,"RealLevel")??0;
            if(user.Level<=real&&user.UserLssInfo==null)
            {
                bool mode=(bool?)Field(user.ServerConfigData,"LSS260820")??false;
                if(mode&&!isDoneTask){ui.ShowTip(13);return;}
                OpenClose(20,new object[]{level});return;
            }
            int stage=progress.GetStage(level);
            if(stage==0){ui.Close();return;}
            if(ui.OnlineTimeHint){ui.OnlineTimeHint=false;OpenClose(30,Array.Empty<object>());return;}
            if(stage==1||stage==2)
            {
                if(arguments.Length>0)OpenClose(28,new object[]{level});
                else ui.ShowTip(WhiteTip(ui.Tip));
                return;
            }
            if(stage==3)
            {
                if(progress.IsCompletePassStage2Level())ui.ShowTip(WhiteTip(ui.Text(6)));
                else OpenClose(23,arguments);
                return;
            }
            if(targetHint){OpenClose(31,Array.Empty<object>());return;}
            if(stage==5||stage==6)
            {
                ui.Progress2Guide=arguments.Length!=0;
                OpenClose(25,Array.Empty<object>());return;
            }
            OpenClose(stage==4?24:26,Array.Empty<object>());
        }
        private void OpenClose(int id,object[] arguments){ui.ShowPanel(id,arguments);ui.Close();}
        // Original StringUtil.ColorToWhite is case-sensitive and keeps closing tags.
        public static string WhiteTip(string text)=>Regex.Replace(text.Replace("\n"," "),@"(<color\s*=\s*)(#?[0-9a-fA-F]{6,8})(\s*>)","<color=#FFFFFF>");
        private static JToken Field(JObject obj,string name)
        {
            if(obj==null)throw new NullReferenceException(name);
            JToken result=null;foreach(var property in obj.Properties())if(string.Equals(property.Name,name,StringComparison.OrdinalIgnoreCase))result=property.Value;
            return result;
        }
    }
}
