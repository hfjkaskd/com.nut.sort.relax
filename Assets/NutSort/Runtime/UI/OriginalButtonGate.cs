using System;

namespace NutSort.UI
{
    // Common AddLSSListener callback (0x9C04A8), before action/audio.
    public sealed class OriginalButtonGate
    {
        private readonly Func<bool> isInitDone;
        private readonly OriginalCountedMask mask;
        private readonly OriginalPanelSettings settings;
        public OriginalButtonGate(Func<bool> isInitDone,OriginalCountedMask mask,OriginalPanelSettings settings)
        {
            this.isInitDone=isInitDone ?? throw new ArgumentNullException(nameof(isInitDone));
            this.mask=mask ?? throw new ArgumentNullException(nameof(mask));
            this.settings=settings != null ? settings : throw new ArgumentNullException(nameof(settings));
        }
        public bool TryBegin()
        {
            if(!isInitDone())return false;
            mask.SetMask(true,settings.ClickMaskDuration);
            return true;
        }
    }
}
