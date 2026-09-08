using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalStageUI
    {
        bool HasGold{get;}
        void SetGold(string value);
        int StepCount{get;}
        bool HasStepTime(int number);
        void SetStepTime(int number,string value);
        void Close();
    }
    // Shared TXProgressBasePanel lifecycle; stage controllers override Sure/RefreshSteps.
    public class OriginalWithdrawalProgressFlow
    {
        protected readonly OriginalUserLocalData User;
        protected readonly IOriginalWithdrawalStageUI UI;
        private readonly Func<long> localTimeSeconds;
        private readonly Func<float,string> goldFormat;
        private readonly Func<long,string,string> timeFormat;
        public OriginalWithdrawalProgressFlow(OriginalUserLocalData user,IOriginalWithdrawalStageUI ui,
            Func<long> localTimeSeconds,Func<float,string> goldFormat,Func<long,string,string> timeFormat)
        {User=user;UI=ui;this.localTimeSeconds=localTimeSeconds;this.goldFormat=goldFormat;this.timeFormat=timeFormat;}
        public void Init(Action baseInit,Action<Action> bindSure,Action<Action> bindClose)
        {baseInit();bindSure(Sure);bindClose(UI.Close);}
        public virtual void Refresh()
        {
            if(User.UserLssInfo==null)User.UserLssInfo=new JObject();
            if(Read(User.UserLssInfo,"ApplicationTime")<=0)
            {
                JObject account=User.UserLssInfo;long time=localTimeSeconds();Set(account,"ApplicationTime",time);
                account=User.UserLssInfo;time=localTimeSeconds();Set(account,"AccountSureTime",unchecked(time+3));
            }
            if(UI.HasGold)UI.SetGold(goldFormat(User.Gold));
            RefreshSteps();
        }
        public virtual void Sure()=>UI.Close();
        public virtual void RefreshSteps()
        {
            for(int index=0;index<UI.StepCount;index++)
            {
                int number=index+1;if(!UI.HasStepTime(number))continue;
                string field=number==1?"ApplicationTime":number==2?"AccountSureTime":number==3?"GoldGetTime":null;
                if(field!=null)UI.SetStepTime(number,timeFormat(Read(User.UserLssInfo,field),"g"));
            }
        }
        private static long Read(JObject row,string key)
        {JToken result=null;foreach(var p in row.Properties())if(string.Equals(p.Name,key,StringComparison.OrdinalIgnoreCase))result=p.Value;return (long?)result??0;}
        private static void Set(JObject row,string key,long value)
        {JProperty found=null;foreach(var p in row.Properties())if(string.Equals(p.Name,key,StringComparison.OrdinalIgnoreCase))found=p;if(found==null)row[key]=value;else found.Value=new JValue(value);}
    }
}
