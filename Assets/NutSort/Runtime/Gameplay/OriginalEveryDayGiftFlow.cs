using System;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // ShowEveryDayGift 0x9FCBF8, predicate 0xA0000C, callback 0xA0008C.
    public sealed class OriginalEveryDayGiftFlow
    {
        private readonly Func<OriginalUserLocalData> user;
        private readonly Func<bool> hasPanel;
        private readonly Action<Func<bool>,Action> until;
        private readonly Action<int> showPanel;
        private readonly Action save;
        private readonly Func<DateTime> utcNow;
        private readonly Func<long> seconds;
        public OriginalEveryDayGiftFlow(Func<OriginalUserLocalData> user,Func<bool> hasPanel,
            Action<Func<bool>,Action> until,Action<int> showPanel,Action save,
            Func<DateTime> utcNow=null,Func<long> seconds=null)
        {
            this.user=user;this.hasPanel=hasPanel;this.until=until;this.showPanel=showPanel;this.save=save;
            this.utcNow=utcNow ?? (()=>DateTime.UtcNow);
            this.seconds=seconds ?? OriginalPlayerGoldHintSchedule.UtcSeconds;
        }
        public void Show()
        {
            // IsToday 0x9BCA68 uses UTC calendar dates and Unix seconds.
            // Keep timestamp conversion before the clock read, including failures.
            DateTime date=DateTimeOffset.FromUnixTimeSeconds(user().LastGetEveryDayGift).Date;
            if(date==utcNow().Date)return;
            // Each invocation owns a wait; the native code neither deduplicates
            // waits nor repeats the date check when the panel registry becomes free.
            until(()=>!hasPanel(),Complete);
        }
        private void Complete()
        {
            showPanel(16);
            var current=user();
            current.LastGetEveryDayGift=seconds();
            save();
        }
    }
}
