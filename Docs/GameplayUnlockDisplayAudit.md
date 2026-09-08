# Gameplay unlock display refresh

UnlockGameplayPanel.RefreshLssPanel (0x9E946C) iterates Icons in order, activating only the element equal to the panel index. It rereads Count after each call, then sets Tip to localized text ID unchecked(index + 63), with no formatting arguments. Out-of-range indices do not select a fallback icon.

OriginalGameplayUnlockDisplay restores this refresh order through required adapters. Validation uses the actual original text table for IDs 63–66: hidden nuts, curtain, rock-blocked bolts, and surrounding-bolt unlock. It also checks out-of-range/empty collection behavior, a changed count during refresh, and a callback exception preventing later updates.

This is presentation logic for the pending full UnlockGameplayPanel prefab/controller. It does not claim that its icons or layout have been restored or shown in normal startup. SDK handling is unchanged.

Unity 2022.3.62f3: full content and unlock display PASS in Library/unity-unlock-display-validation.log. No compiler or validation exceptions; preference fixture restored.
