using System;
using NutSort.Content;

namespace NutSort.Gameplay
{
    [Serializable]
    public sealed class OriginalClearanceProgressData
    {
        public int gap_rank, gap_all_gates, ext_gap_logs, gap_logs, gap_day_gates;
    }

    // ClearanceRewardShowS2C.InitLss 0x9EFC28. This applies a supplied result;
    // it never synthesizes a successful response, grants currency or saves.
    public sealed class OriginalClearanceProgress
    {
        private readonly Func<OriginalUserLocalData> user;
        private readonly Action refreshMain;
        private readonly Func<long> timeSeconds;
        private readonly Action<long> setLuckyRewardTime, setLuckyDrawTime;

        public OriginalClearanceProgress(Func<OriginalUserLocalData> user, Action refreshMain,
            Func<long> timeSeconds, Action<long> setLuckyRewardTime, Action<long> setLuckyDrawTime)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.refreshMain = refreshMain ?? throw new ArgumentNullException(nameof(refreshMain));
            this.timeSeconds = timeSeconds ?? throw new ArgumentNullException(nameof(timeSeconds));
            this.setLuckyRewardTime = setLuckyRewardTime ?? throw new ArgumentNullException(nameof(setLuckyRewardTime));
            this.setLuckyDrawTime = setLuckyDrawTime ?? throw new ArgumentNullException(nameof(setLuckyDrawTime));
        }

        public void Apply(bool success, OriginalClearanceProgressData data)
        {
            if (!success) return;
            // Keep live user reads and native write order: UI refresh may reenter
            // and replace the user before the subsequent level-three check.
            var current = user();
            if (data == null) throw new NullReferenceException("kinetic_data");
            current.UserLevel = data.gap_rank;
            user().Level = unchecked(data.gap_all_gates + 1);
            user().LevelSeed = 0;
            user().LoginDay = data.ext_gap_logs;
            user().LoginDayCoin = data.gap_logs;
            user().TodayPassLevelCount = data.gap_day_gates;
            refreshMain();
            if (user().Level != 3) return;
            user().LuckyScrewDoneCount = 0;
            setLuckyRewardTime(timeSeconds());
            setLuckyDrawTime(timeSeconds());
        }
    }
}
