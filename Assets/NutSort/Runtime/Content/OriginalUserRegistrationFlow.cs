using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    // UserMgr.Register and its three closures; request and message operations
    // are supplied by the caller's actual managers.
    public sealed class OriginalUserRegistrationFlow
    {
        private readonly Action<Action<bool>> request;
        private readonly Action<Action<bool>,bool> config;
        private readonly Action<Action<JObject>,bool> rewardInfo;
        private readonly Action<int,Action,bool> showMessage;
        private readonly Action<bool> setInitialized;
        private readonly Action save;
        public OriginalUserRegistrationFlow(Action<Action<bool>> request,Action<Action<bool>,bool> config,
            Action<Action<JObject>,bool> rewardInfo,Action<int,Action,bool> showMessage,Action<bool> setInitialized,Action save)
        {this.request=request;this.config=config;this.rewardInfo=rewardInfo;this.showMessage=showMessage;this.setInitialized=setInitialized;this.save=save;}
        public void Register()=>request(Registered);
        private void Registered(bool success)
        {
            if(success)config(Configured,true);
            else showMessage(75,Register,false);
        }
        private void Configured(bool success)
        {
            if(!success)return;
            rewardInfo(RewardReceived,true);
            save();
        }
        private void RewardReceived(JObject ignored)=>setInitialized(true);
    }
}
