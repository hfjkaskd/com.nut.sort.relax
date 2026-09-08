using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    // GoldRewardInfo completion used by ShowGuide case one, 0x9e4958.
    // Transport/InitLss remain supplied; this callback does not consume payload.
    public sealed class OriginalGuideTargetRequest
    {
        private readonly OriginalUserLocalData user;
        private readonly Action<bool,Action<JObject>> request;
        private readonly Action<int,int> showPanel;
        public OriginalGuideTargetRequest(OriginalUserLocalData user,Action<bool,Action<JObject>> request,Action<int,int> showPanel)
        {this.user=user;this.request=request;this.showPanel=showPanel;}
        public void Run(bool showMask){request(showMask,Receive);}
        private void Receive(JObject ignored){showPanel(18,unchecked(user.Level-1));}
    }
}
