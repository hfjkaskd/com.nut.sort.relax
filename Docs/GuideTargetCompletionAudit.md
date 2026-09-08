# Guide target completion behavior

TXGuideTargetCompletePanel.RefreshLssPanel 0x9C9EF8 reads its captured integer stage argument. Stage 1 displays current UserLocalData.Level1Gold (0xE8), stage 2 displays current Level2Gold (0xEC), and every other value displays current Gold (0x4C), through the existing currency formatter. It does not grant or modify these amounts. OriginalGuideTargetCompletion.Refresh restores this lazy selection.

TXCallback 0x9CA004 closes the target panel, sets IsCompleteRecordGuide true, calls InitDoneEvent(true,false), then calls NewbieGuidePanel.CallbackActionInvoke(null). OriginalGuideTargetCompletion.Continue preserves that order. The callback is looked up only after initialization re-entry, so a new registration made by that re-entry replaces the old one. Close failure prevents all following work; initialization failure leaves the flag written and global callback pending. There is no extra SaveData or SDK operation in this method.

OriginalSceneInitialization.Run exposes the existing initialization flow for this consumer without using CompleteRecordGuide, whose close operation belongs to a different panel. Validation covers stage values, live amount refresh, single formatting, exact callback order, null payload, callback replacement during re-entry and failure boundaries. Previous static callbacks are restored after validation.

The target-complete prefab, standard Button binding and guide-step host remain outstanding. This runtime behavior is ready for that binding, but it does not make the whole target-complete branch playable or establish visual parity. SDK handling remains unchanged.

Unity 2022.3.62f3: 78 full regression PASS markers in Library/unity-guide-target-completion-validation.log; no compiler or validation exceptions; preferences restored.
