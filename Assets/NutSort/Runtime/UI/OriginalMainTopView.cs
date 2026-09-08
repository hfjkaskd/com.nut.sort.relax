using System;
using NutSort.Content;
using UnityEngine;

namespace NutSort.UI
{
    // The source Top's eight configured children share one set of live providers.
    public sealed class OriginalMainTopView : MonoBehaviour
    {
        [SerializeField] private OriginalGoldItem gold;
        [SerializeField] private OriginalCoinItem coin;
        [SerializeField] private OriginalGMButton gm;
        [SerializeField] private OriginalMainLevelView level;
        [SerializeField] private OriginalPlayerGoldHintView playerHint;
        [SerializeField] private OriginalHiddenLevelView hidden;
        [SerializeField] private OriginalRewardProgressView progress;
        [SerializeField] private OriginalMarqueeLauncher marquee;
        private OriginalMainTopFlow flow;
        public OriginalGoldItem Gold => gold;
        public OriginalCoinItem Coin => coin;
        public OriginalGMButton GM => gm;
        public OriginalMainLevelView Level => level;
        public OriginalPlayerGoldHintView PlayerHint => playerHint;
        public OriginalHiddenLevelView Hidden => hidden;
        public OriginalRewardProgressView Progress => progress;
        public OriginalMarqueeLauncher Marquee => marquee;

        public void Bind(OriginalUserLocalData user,OriginalTables tables,string language,
            OriginalGoldFormatter formatter,Func<string> country,Func<bool> tryClick,
            Action<Action> requestGold,Action<Action> requestCoin,Action<int> openPanel,
            Action playClick,Func<bool> isTest,Func<OriginalMarqueeItem> nextHint,
            Func<bool> hasPanel,Action<float,Action> delay)
        {
            if(formatter==null)throw new ArgumentNullException(nameof(formatter));
            // Bind every consumer before Gold.Init can reenter the Top refresh.
            flow=new OriginalMainTopFlow(gold,coin,hidden,progress,marquee,level,user,tables,language,gm.Init);
            Func<float,string> cash=n=>formatter.Format(n);
            progress.Bind(user,tables,language,cash);
            hidden.Bind(user,tables,language);
            gold.Bind(user,tables,language,cash,country,tryClick,requestGold,openPanel,playClick,flow.RefreshRewardAndLevel);
            coin.Bind(user,tables,language,formatter,tryClick,requestCoin,openPanel,playClick);
            gm.Bind(isTest,tryClick,openPanel,playClick);
            var marqueeText=new OriginalMarqueeText(user,tables,cash);
            marquee.Bind(()=>user.Level,nextHint,item=>item.Bind(marqueeText,tables.PayChannels,language,country,delay));
            var schedule=new OriginalPlayerGoldHintSchedule(()=>user.ServerConfigData,hasPanel,nextHint,playerHint.PresentPush);
            playerHint.Bind(schedule,new OriginalPlayerGoldHintText(user,tables,cash),
                new OriginalPlayerGoldHintIcons(tables.PayChannels,country),()=>language,
                ()=>tables.ChannelInfos.GetSelfChannel(tables.PayChannels,country(),user.UserLssInfo));
        }
        // MainPanel initializes PlayerHint separately, after Bottom and TargetReward.
        public void Init() => flow.Init();
        public void Refresh() => flow.Refresh();
        public void RefreshLevel() => flow.RefreshLevel();
    }
}
