# Failure panel data refresh

OriginalFailurePanelData restores RefreshLssPanel (0x9D6AD8) data/presentation ordering. CoinValue receives GoldRewardTargetS2CData.bear_zs_show (field offset 0x28) directly, without cash formatting, trimming, localization or a fallback amount. Next it reads CurrentLevelAddScrewCount, then ServerConfigData.LSSLSMAC, and requests gray when used >= maximum. The source constructor default maximum is 12; it is supplied by the eventual serialized view configuration rather than embedded into the data flow. Gray is a presentation request, not an interactable flag or a grant/revive decision.

The existing opaque JObject storage is read through explicit field names with case-insensitive last assignment semantics. Missing reward data fails before text/gray updates; missing configuration fails after the text update. Missing reward string stays null, and empty text stays empty. No reward response or server data is fabricated.

55 regression PASS markers in Library/unity-failure-panel-data-validation.log, including deterministic checks of raw string preservation, equality/default thresholds, live state read ordering, missing/null/empty values and exception boundaries. No compiler errors or exceptions found; preference fixture restored with backup absent. These tests cover data callbacks, not rendered TMP/material state or the failure panel prefab.

The actual FailPanel prefab, initialization GameLose audio, base panel animation, standard Button bindings and normal startup routing remain pending. This helper is ready for the view but is not yet invoked by normal gameplay. SDK treatment and revive handling remain unchanged; no simulated SDK success was added. Whole-game parity remains incomplete.
