# New gameplay unlock decision

OriginalGameplayUnlock implements LuoSiSortMgr.NewGameplayUnlock (0x9FC960). Source ServerConfigData.LSSGPUL is the int array at 0x50. It scans in order, requiring current UserLocalData.Level == unchecked(LSSGPUL[i] - 4) and i >= NewGameplayUnlockIndex. It opens UIName 13 with the index and isShowBanner, then returns true. No match returns false. It does not mutate the saved unlock index, save, or suppress the panel when isShowBanner is false.

The array provider is required and is evaluated on each invocation; missing arrays fail instead of becoming fake empty configuration. Typed callback arguments replace the original object[] boxing without reflection or enum-name keys. No thresholds or server rewards are invented.

Validation covers exact vs exceeded thresholds, inclusive indices, duplicate thresholds, empty/missing arrays, live replacement, unchecked subtraction and callback exceptions. InitializationFlowValidation now exercises the real unlock decision inside InitDone routing, proving it opens the unlock panel before setting initialization done and suppresses the target banner on a match, with either banner flag.

Production binding still requires the actual server-configuration consumer and the complete unlock panel. This is a restored main-flow decision, not a claim that the full lifecycle is connected. SDK handling remains unchanged.

Unity 2022.3.62f3 verification: full content PASS plus unlock and initialization-flow PASS in Library/unity-gameplay-unlock-validation.log. Preferences restored; no compiler or validation exceptions.
