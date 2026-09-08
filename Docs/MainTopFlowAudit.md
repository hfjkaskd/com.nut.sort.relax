# MainPanelTop component flow

OriginalMainTopFlow coordinates configured, already-bound native prefab consumers without creating UI or owning reward/network state. It restores MainPanelTop.Init (0x9DE464), Refresh (0x9DE93C) and the existing RefreshLevel behavior (0x9DC280). The caller supplies the GM initialization boundary first; the GM button/panel implementation is not fabricated by this class.

Init executes GM setup, GoldItem.Init, CoinItem.Init, HiddenLevel.Init (template hiding), TX_JD.Init (owner hiding), then PMDBullet.Init. All data providers must already be bound. GoldItem.Init invokes its hint refresh, which can call RefreshRewardAndLevel before the rest of Top initialization. That path refreshes TX_JD first and then the ordinary level label. Subsequent TX_JD.Init hides the owner without clearing IsShow.

Refresh runs gold amount, coin amount/visibility, hidden milestone display, withdrawal progress and ordinary level label in that order. The label uses HiddenLevel.activeSelf and TX_JD.IsShow, not TX_JD.activeSelf. Refresh updates progress eligibility/data but does not show it: the distinct lifecycle Show call remains necessary. This distinction was confirmed when the integration test's initial automatic-show expectation failed; runtime behavior was retained and the test corrected.

The integration validation composes actual current prefab components with validation-only reward data. It checks initialization's reentrant progress update followed by hiding, retained amount text during Init, full amount refresh, separate progress Show, and late/early/first-level visibility. No UI geometry or visual assets were changed in this round.

Remaining: production MainPanelTop prefab assembly/provider binding and GM setup, lifecycle Show routing, region AB/GM and original-device parity. This coordinator is not yet called by production startup. SDK behavior is unchanged.

Final validation: 39 Unity regression PASS markers in reverse-workspace reconstruction-nut/logs/unity-main-top-flow-validation-complete.log, no compiler/validation exceptions; preference backup absent after restoration.
