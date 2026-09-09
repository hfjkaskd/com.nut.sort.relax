using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    public sealed class OriginalServerConfigInitialization
    {
        private readonly Func<OriginalUserLocalData> user;
        private readonly Func<DateTime> utcNow;
        private readonly Func<long> utcSeconds;
        private readonly Func<int,int,int> random;
        public OriginalServerConfigInitialization(Func<OriginalUserLocalData> user,Func<DateTime> utcNow=null,Func<long> utcSeconds=null,Func<int,int,int> random=null)
        {this.user=user;this.utcNow=utcNow??(()=>DateTime.UtcNow);this.utcSeconds=utcSeconds??OriginalPlayerGoldHintSchedule.UtcSeconds;this.random=random??UnityEngine.Random.Range;}
        public void Initialize(JObject data)
        {
            user().ServerConfigData=data;
            DateTime loginDate=DateTimeOffset.FromUnixTimeSeconds(user().LoginTime).Date;
            if(loginDate.Date!=utcNow().Date)
            {
                var target=user();target.LoginTime=utcSeconds();
                var account=user().UserLssInfo;
                if(account!=null)OriginalWithdrawalQueue.Refresh(account,random);
                target=user();target.TodayChallengeTimes=random(5,11);
            }
            if(user().TodayChallengeTimes<=0)
            {var target=user();target.TodayChallengeTimes=random(5,11);}
            // Original AndroidSdk.setKwaiCountry is excluded by the requested SDK boundary.
        }
    }
}
