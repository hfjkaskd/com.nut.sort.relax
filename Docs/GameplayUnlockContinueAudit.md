# Unlock confirmation continuation

UnlockGameplayPanel.ContinueCallback (0x9E9588) conditionally calls TargetRewardBanner.Show(null), assigns unchecked(panel index + 1) to UserLocalData.NewGameplayUnlockIndex, then calls CloseLssPanel. It neither saves nor re-enters InitDoneEvent. HideLssPanel (0x9E9580) delegates to the existing base hide behavior.

OriginalGameplayUnlockContinue restores this typed sequence. Banner failure prevents the write/close; close failure retains the index. Assignment can lower the existing index, and it is not a max or increment of the previous user value. The banner receives no fabricated callback.

Validation checks sequence, both banner flags, null completion callback, mutation boundaries, overflow, and chaining the existing unlock decision through confirmation so a completed entry no longer opens and the next configured entry remains eligible.

The complete UnlockGameplayPanel prefab/controller and automatic startup binding remain outstanding. This restores its action logic, not a live Button implementation or full lifecycle parity. SDK behavior is unchanged.

Unity 2022.3.62f3: full content and unlock continuation PASS in Library/unity-unlock-continue-validation.log. No compiler or validation exceptions; test preferences restored.
