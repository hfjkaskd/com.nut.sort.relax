using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    // UserMgr.Init after local-data assignment; transport and config initialization stay explicit ports.
    public sealed class OriginalUserInitializationTail
    {
        private readonly Func<OriginalUserLocalData> user;
        private readonly Action initializeCountry,register,save;
        private readonly Action<Action<bool>,bool> config;
        private readonly Action<JObject> initializeConfig;
        private readonly Action<bool> setInitialized;
        private readonly Action<Action<JObject>,bool> rewardInfo;
        private static readonly Action<JObject> IgnoreReward=IgnoredReward;
        public OriginalUserInitializationTail(Func<OriginalUserLocalData> user,Action initializeCountry,
            Action<Action<bool>,bool> config,Action register,Action<JObject> initializeConfig,
            Action<bool> setInitialized,Action<Action<JObject>,bool> rewardInfo,Action save)
        {this.user=user;this.initializeCountry=initializeCountry;this.config=config;this.register=register;this.initializeConfig=initializeConfig;this.setInitialized=setInitialized;this.rewardInfo=rewardInfo;this.save=save;}
        public void Run()
        {
            initializeCountry();
            if(user().ServerConfigData!=null)config(ConfigCompleted,false);else register();
            // Native isEditor return value is unused; no Editor-only branch belongs here.
            if(string.IsNullOrEmpty(user().ComeOnGold))return;
            user().GuideIndex=10;
            save();
        }
        private void ConfigCompleted(bool success)
        {
            JObject data=user().ServerConfigData;
            if(data==null)throw new NullReferenceException("ServerConfigData");
            initializeConfig(data);
            setInitialized(true);
            rewardInfo(IgnoreReward,false);
            save();
        }
        private static void IgnoredReward(JObject data){}
    }
}
