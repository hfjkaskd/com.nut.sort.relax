using System;
using Newtonsoft.Json.Linq;
namespace NutSort.UI
{
    // UserMgr.GoldGet 0x9c2bec, response callback 0x9c3da0.
    // Request owns transport and response initialization; this consumer only routes UI.
    public sealed class OriginalGoldGetFlow
    {
        private readonly Action<Action<JObject>> request;
        private readonly Func<string> mainGoldHint;
        private readonly Action<string> showRawTip;
        private readonly Action<int> showTip;
        private readonly Action<int,object[]> showPanel;
        private readonly Action<bool,float> refreshGold;
        private readonly Action<JObject> response;
        public OriginalGoldGetFlow(Action<Action<JObject>> request,Func<string> mainGoldHint,Action<string> showRawTip,
            Action<int> showTip,Action<int,object[]> showPanel,Action<bool,float> refreshGold)
        {
            this.request=request;this.mainGoldHint=mainGoldHint;this.showRawTip=showRawTip;
            this.showTip=showTip;this.showPanel=showPanel;this.refreshGold=refreshGold;response=Receive;
        }
        public void Run(int level,bool fromConfirmation)
        {
            if(level>2){request(response);return;}
            if(fromConfirmation)showPanel(28,new object[]{level});
            else showRawTip(OriginalWithdrawalClaimFlow.WhiteTip(mainGoldHint()));
        }
        private void Receive(JObject value)
        {
            if(value!=null&&Status(value)=="NO-0")
            {refreshGold(true,0);showPanel(29,Array.Empty<object>());}
            else showTip(2);
        }
        private static string Status(JObject value)
        {
            JToken status=null;foreach(var field in value.Properties())
                if(string.Equals(field.Name,"kinetic_gap",StringComparison.OrdinalIgnoreCase))status=field.Value;
            return (string)status;
        }
    }
}
