using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    public sealed class OriginalGoldRewardInfoResponse
    {
        public string message_status,kinetic_gap;
        public long Time;
        public JObject kinetic_data;
        public bool IsSuccess=>kinetic_gap=="NO-0";
    }
    // RequestMgr.GoldRewardInfo response/retry layer; the lower request owns
    // response parsing and GoldRewardTargetS2C.InitLss before invoking this port.
    public sealed class OriginalGoldRewardInfoFlow
    {
        private readonly Func<OriginalUserLocalData> user;
        private readonly Action<bool,Action<OriginalGoldRewardInfoResponse>> request;
        private readonly Action<int,Action,bool> showMessage;
        private readonly Func<bool> hasMainPanel;
        private readonly Action refreshGoldHint;
        public OriginalGoldRewardInfoFlow(Func<OriginalUserLocalData> user,Action<bool,Action<OriginalGoldRewardInfoResponse>> request,
            Action<int,Action,bool> showMessage,Func<bool> hasMainPanel,Action refreshGoldHint)
        {this.user=user;this.request=request;this.showMessage=showMessage;this.hasMainPanel=hasMainPanel;this.refreshGoldHint=refreshGoldHint;}
        public void GoldRewardInfo(Action<OriginalGoldRewardInfoResponse> callback,bool isFaceRefresh=false)
        {
            Action retry=null;
            request(isFaceRefresh,response=>
            {
                if((response==null||!response.IsSuccess)&&isFaceRefresh)
                {
                    if(retry==null)retry=()=>GoldRewardInfo(callback,isFaceRefresh);
                    showMessage(75,retry,false);
                    return;
                }
                if(callback!=null)
                    callback(new OriginalGoldRewardInfoResponse{kinetic_data=user().GoldRewardTargetS2CData});
                if(hasMainPanel())refreshGoldHint();
            });
        }
    }
}
