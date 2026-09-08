using System;
using NutSort.Content;

namespace NutSort.Gameplay
{
    // InitLevel callbacks 0x9FFD10 / 0x9FFEC8. The caller supplies the existing
    // synchronization boundary; this class neither sends requests nor invents replies.
    public sealed class OriginalInitializationContinuation
    {
        private readonly OriginalUserLocalData user;
        private readonly Func<bool> completeStage2;
        private readonly Action<Action> synchronize;
        private readonly Action<bool, bool> initializeDone;

        public OriginalInitializationContinuation(OriginalUserLocalData user,
            Func<bool> completeStage2, Action<Action> synchronize, Action<bool, bool> initializeDone)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.completeStage2 = completeStage2 ?? throw new ArgumentNullException(nameof(completeStage2));
            this.synchronize = synchronize ?? throw new ArgumentNullException(nameof(synchronize));
            this.initializeDone = initializeDone ?? throw new ArgumentNullException(nameof(initializeDone));
        }

        public void Run(bool showBanner, bool firstInit)
        {
            if (string.IsNullOrEmpty(user.ComeOnGold) && completeStage2())
                synchronize(() => initializeDone(showBanner, firstInit));
            else
                initializeDone(showBanner, firstInit);
        }
    }
}
