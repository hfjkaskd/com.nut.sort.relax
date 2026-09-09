using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    // Explicit UserMgr.Init composition. The host must supply the existing SDK/request boundaries.
    public sealed class OriginalUserStartup
    {
        private readonly OriginalUserDefaults defaults;
        private readonly IOriginalUserPreferences preferences;
        private readonly OriginalUserCountryState country;
        private readonly Func<string> identifier,name;
        private readonly Func<long> seconds;
        private readonly OriginalUserInitializationTail tail;
        public OriginalUserStore Store{get;private set;}
        public OriginalUserLocalData Data=>Store.Data;
        public bool IsInitDone{get;set;}
        public OriginalUserStartup(OriginalUserDefaults defaults,IOriginalUserPreferences preferences,OriginalUserCountryState country,
            Func<string> identifier,Action<Action<bool>,bool> config,Action register,Action<Action<JObject>,bool> rewardInfo,Action save,
            Func<string> name=null,Func<long> seconds=null,Func<DateTime> utcNow=null,Func<int,int,int> random=null)
        {
            this.defaults=defaults;this.preferences=preferences;this.country=country;this.identifier=identifier;
            this.name=name??OriginalMarqueeName.Generate;this.seconds=seconds??OriginalPlayerGoldHintSchedule.UtcSeconds;
            var initializeConfig=new OriginalServerConfigInitialization(()=>Data,utcNow,this.seconds,random);
            tail=new OriginalUserInitializationTail(()=>Data,country.Initialize,config,register,initializeConfig.Initialize,value=>IsInitDone=value,rewardInfo,save);
        }
        public void Initialize()
        {
            // Publish only after loading or all first-user initializers have succeeded.
            Store=new OriginalUserStore(defaults,preferences,InitializeFresh);
            tail.Run();
        }
        private void InitializeFresh(OriginalUserLocalData fresh)
        {
            fresh.UserId=identifier();
            fresh.UserName=name();
            fresh.RegisterTime=seconds();
            fresh.LastGetEveryDayGift=seconds();
        }
    }
}
