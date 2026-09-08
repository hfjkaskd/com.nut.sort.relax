using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // Success response callback 0xA00684. Payload construction belongs to its continuation.
    public sealed class OriginalSuccessResponseFlow
    {
        private readonly Func<bool> completedCoinNewPeopleReward;
        private readonly Action restart;
        private readonly Action<int,Action> showPanel;
        private readonly Action<JObject> settlement;
        public OriginalSuccessResponseFlow(Func<bool> completedCoinNewPeopleReward,Action restart,
            Action<int,Action> showPanel,Action<JObject> settlement)
        {
            this.completedCoinNewPeopleReward=completedCoinNewPeopleReward;this.restart=restart;
            this.showPanel=showPanel;this.settlement=settlement;
        }
        public void Run(OriginalLevelInfo capturedLevel,JObject response)
        {
            Action continuation=()=>settlement(response);
            if(capturedLevel.SubTotalRound>capturedLevel.SubRound){restart();return;}
            if(capturedLevel.Level==6&&!completedCoinNewPeopleReward())
            {
                showPanel(43,continuation);return;
            }
            continuation();
        }
    }
}
