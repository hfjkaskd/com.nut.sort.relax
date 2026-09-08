# Withdrawal success controller and native target-gold setter

Restored TXSuccessPanel controller behavior as OriginalWithdrawalSuccessFlow, plus its actual UserMgr.SetGold semantics as OriginalSetGoldFlow. These are composable runtime controllers, not an invented success-page visual or production transport implementation.

## Native evidence

TXSuccessPanel.InitLssPanel (0x9d3bdc) calls base initialization then binds the sole Sure listener. It does not require a captured level argument. RefreshLssPanel (0x9d3c94) reads the live UserLocalData.Gold (offset 0x4c), calls GoldLSSFormat and assigns Count.text before calling UserMgr.SetGold(0, true, true). SureCallback (0x9d3d70) only invokes base CloseLssPanel. No extra guide, save or success callback is added to Sure.

UserMgr.SetGold (0x9c25b4) takes a target balance, isRefreshUI and isSyncInfo, as verified by the typed declaration. It is not an additive reward or debit method. If target < current Gold, it logs the exact prefix 'targetGold < UserLocalData.Gold:' plus the target's default Single.ToString, then returns without mutation, refresh, sync or save.

Otherwise it computes target minus current, assigns target to Gold, refreshes GoldItem with (true, added) only when added > 0 and isRefreshUI, invokes InfoSyncCompleteTargetGold when isSyncInfo, then saves. Equal targets skip the HUD refresh but still sync/save. Refresh and sync failures propagate in order and preserve the already-assigned value. Source comparisons do not introduce an additional finite-number validation.

Consequently success Refresh with a positive current balance displays that balance and its SetGold(0) call is rejected. Replacing this with a direct zero assignment would change the source behavior. With zero balance it displays zero and reaches sync/save; with a negative balance it displays the negative value first and then raises the balance to zero. The sync operation stays injected at the existing SDK boundary; no actual server request is implemented here.

## Validation

Library/withdrawal-success-flow-validation.log: 133 PASS markers, zero C# errors/exceptions and restored preference backup; complete batch validation includes the new controller/setter coverage. Tests exercise lower/equal/increasing targets, independent flags, assignment/refresh/sync/save order, failure propagation, NaN comparison behavior, base/binding order, display before setter, positive/zero/negative success branches, formatter/text setter errors and close-only Sure behavior.

A composed explicit fixture holds OriginalGoldGetFlow's request, rejects a synthetic failure status without entering success, then sends a labeled synthetic NO-0 callback. It verifies pre-panel HUD refresh -> panel 29 dispatch -> displayed current balance -> native zero-target refusal. This is controller integration evidence, not a real payment callback or proof of visual success.

## Missing visual evidence and remaining work

Filename search across the supplied reverse workspace found TXSuccessPanel scripts but no matching prefab. Searching the current typed export's prefab, scene and asset files for its script GUID also found no references. This does not prove the source application never obtains it dynamically or uses another resource mapping. No substitute layout or old screenshot was used to hide the gap.

Panel 29's original visual resource and production host therefore remain unresolved. OriginalSetGoldFlow is available for runtime composition; existing unrelated fixture setters and production-wide gold routing were not silently replaced. Full startup, all country/AB branches and the complete lifecycle remain incomplete. SDK behavior is unchanged. No new screenshot/Play visual claim is made for this controller-only round.
