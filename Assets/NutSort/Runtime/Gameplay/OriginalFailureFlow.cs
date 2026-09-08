using System;

namespace NutSort.Gameplay
{
    // LuoSiSortMgr.Fail (0x9FC800). Reset is explicitly owned by the
    // restart/revive actions, not level initialization or the delayed callback.
    public sealed class OriginalFailureFlow
    {
        public bool IsFail;

        public void Fail(float delay, Action<float, Action> schedule, Action showPanel)
        {
            if (IsFail) return;
            IsFail = true;
            schedule(delay, showPanel);
        }
    }
}
