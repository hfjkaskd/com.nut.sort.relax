using System;
using NutSort.Content;

namespace NutSort.UI
{
    // Coordinates already configured/bound prefab components. It creates no UI.
    public sealed class OriginalMainTopFlow
    {
        private readonly OriginalGoldItem gold;
        private readonly OriginalCoinItem coin;
        private readonly OriginalHiddenLevelView hidden;
        private readonly OriginalRewardProgressView progress;
        private readonly OriginalMarqueeLauncher marquee;
        private readonly OriginalMainLevelView level;
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly string language;
        private readonly Action initializeGM;

        public OriginalMainTopFlow(OriginalGoldItem gold,OriginalCoinItem coin,OriginalHiddenLevelView hidden,
            OriginalRewardProgressView progress,OriginalMarqueeLauncher marquee,OriginalMainLevelView level,
            OriginalUserLocalData user,OriginalTables tables,string language,Action initializeGM)
        {
            this.gold=gold ?? throw new ArgumentNullException(nameof(gold));
            this.coin=coin ?? throw new ArgumentNullException(nameof(coin));
            this.hidden=hidden ?? throw new ArgumentNullException(nameof(hidden));
            this.progress=progress ?? throw new ArgumentNullException(nameof(progress));
            this.marquee=marquee ?? throw new ArgumentNullException(nameof(marquee));
            this.level=level ?? throw new ArgumentNullException(nameof(level));
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.tables=tables ?? throw new ArgumentNullException(nameof(tables));
            this.language=language;
            this.initializeGM=initializeGM ?? throw new ArgumentNullException(nameof(initializeGM));
        }

        public void Init()
        {
            initializeGM();
            gold.Init();
            coin.Init();
            hidden.Init();
            progress.Init();
            marquee.Init();
        }

        public void Refresh()
        {
            gold.Refresh();
            coin.Refresh();
            hidden.Refresh();
            progress.Refresh();
            RefreshLevel();
        }

        // GoldItem.RefreshHint calls these two consumers in this order.
        public void RefreshRewardAndLevel()
        {
            progress.Refresh();
            RefreshLevel();
        }

        public void RefreshLevel()
        {
            level.Refresh(tables,user.Level,language,hidden.gameObject.activeSelf,progress.IsShow);
        }
    }
}
