# Initialization event parity audit

Status: the ordered dispatcher is implemented and covered by offline Unity validation. It is **not yet bound to the production scene**. The playable build still lacks these guide panels and downstream consumers. This is a main-flow prerequisite, not completed onboarding or full lifecycle parity.

Evidence is the current local ARM64 export: `LuoSiSortMgr.InitDoneEvent` at 0x9FBD2C, its banner callback at 0xA002F0, and `TXRecordGuidePanel.StartCallback` at 0x9D2140. Field identities and panel IDs were checked against the typed export; raw disassembly is not included in this repository.

## Ordered dispatch

Only the first matching branch executes. The display-level-4 condition is sampled after the record-guide gate and before the remaining checks.

| Priority | Condition | Action |
| --- | --- | --- |
| 1 | Record guide incomplete | Set IsInitDone, then show panel 34 |
| 2 | Internal level 2/3 and corresponding gold-reduction flag false | Write GuideIndex 1, show panel 7 |
| 3 | Internal level 3 and extra-gold guide incomplete | Show panel 37 |
| 4 | IsCannotMove returns true | Fail |
| 5 | IsGuidePassStage2Level returns true | Write GuideIndex 10, show panel 7 |
| 6 | ComeOnGold is non-null/non-empty (including whitespace) | Write GuideIndex 12, show panel 7 |
| 7 | IsGuideGold | Write GuideIndex 14, show panel 7 |
| 8 | showBanner and display level 4 | Close all panels, then show panel 42 |
| 9 | NewGameplayUnlock(showBanner) returns true | Finish initialization |
| 10 | No unlock and showBanner false | Finish initialization |
| 11 | No unlock and showBanner true | Show target reward banner; completion is deferred to its callback |

Branches 2–10 write IsInitDone **after** their action. Branch 1 writes it **before** showing the panel. Branch 11 does not reset an already-true flag and does not eagerly write it. Do not conflate this flag with whether a modal panel is present.

The dispatcher keeps checks lazy. IsCannotMove calls CheckDie, which can reveal hidden masks and save state; precomputing every predicate would change behavior even while a higher-priority guide is active. The required action interface has no fallback implementations for unavailable systems.

## Banner completion

The callback first writes IsInitDone and reads current user state, while retaining this invocation's firstInit argument. Internal level <= 1 shows panel 7 without changing GuideIndex. Otherwise, incomplete coin-new-player reward and display level >= 5 shows panel 43 (`TXGuideCoinRewardPanel`). Otherwise it hides the level hint and checks the daily gift. All three paths then push the player-gold hint if firstInit is true.

Completing the record guide closes it first, sets IsCompleteRecordGuide, and re-enters InitDoneEvent(true, false). There is no direct SaveData in that callback. Its SDK telemetry is excluded as requested.

## Remaining integration

- Record-guide prefab, native panel animation and currency-particle material selection are now restored and exercised in a real-canvas validation host (RecordGuideAudit.md). Production dispatcher binding remains pending.
- Restore the original newbie, assessment and reward panels, banner and unlock/daily-gift consumers, then bind the required action interface to real scene components.
- IsCannotMove, CheckDie/IsOnlyDontMove, native hidden-root delay and per-mask save points are now available through the level view (see DeadlockAudit.md). Bind this action when the higher-priority panels are restored; the skeletal break animation is still pending.
- Connect the post-initialization main-panel wait and 1.5-second callback timing. The existing board initialization gate does not establish this lifecycle parity.
- Validate actual UI interaction and current-render comparisons, regional AB/GM behavior and device parity. No visual parity claim follows from dispatch tests.
