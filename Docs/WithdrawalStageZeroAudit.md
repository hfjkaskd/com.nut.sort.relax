# First withdrawal progress stage controller

OriginalWithdrawalStageZeroFlow restores TXProgress0Panel controller behavior on the previously recovered progress base. OriginalWithdrawalQueue restores UserLSSInfo.RefreshQueueCount. The concrete first-progress animation, stage-zero prefab adapter/host and production routing remain pending.

## Source order

Refresh 0x9ce9b4 first calls base Refresh, including virtual RefreshSteps. Stage-zero RefreshSteps 0x9cf3ac calls base time-label refresh, checks live TX0IsOpened, and on first entry sets that flag, saves, then invokes PlayProgressTween1. Therefore first animation creation occurs before the derived Refresh computes IsGuide, sets the completion-guide flag or assigns GoldGetTime. A save failure keeps the opened flag but prevents animation; later RefreshSteps does not silently retry it.

After base Refresh returns, IsGuide is arguments.Length != 0; the elements are not unboxed or interpreted. Null arguments fail at this point, retaining earlier base effects. Set UserLocalData.IsCompleteGuidePassStage2Level (offset 0xe4). The source then emits SDK event ns_wdc_1; this SDK-only event remains excluded under the user's explicit scope.

Capture UserLssInfo; if GoldGetTime (offset 0x50) is nonpositive, assign a fresh LocalTimeSeconds read. Refresh that captured account's queue. On the live account, assign GongHao only if nonpositive using original integer Random.Range(6000000,8000000), then display label 44 with the live service number.

Capture the third bear_list row, query TXMgr.IsCompletePassStage2Level, and display either label 6 with only LF characters replaced by spaces, or label 49 with unchecked ShowLevel-1 and the captured row's Stage2StartShowLevel. Next display tip 40 with that same captured row's live Stage2StartShowLevel and save. No lower clamp or CR removal is added. The row is retained as a reference rather than a prematurely copied scalar; label callbacks can affect the later tip value.

Sure 0x9cf21c initiates base Close before reading IsGuide. True opens withdrawal panel 18 with empty arguments. False invokes the shared guide callback with null. It does not replace null with a level or add a second save. The bound inherited Close button remains close-only.

## Native queue transitions

RefreshQueueCount 0x9c1e74 uses exclusive integer upper bounds. Nonpositive queue initializes QueueCount from [8000,9000), then PassRate from [90,96). For queue >=101, subtract [50,100); if the new value <=99, replace it with [90,100). For queue 11..100, subtract [10,20); if the new value <=9, replace it with [5,10). Values 1..10 remain unchanged. PassRate is only changed by the nonpositive initialization branch. Writes precede any subsequent random call, preserving partial state on failure.

The random source is injected for deterministic verification and future native Unity Random.Range composition. These values and thresholds are recovered source algorithm constants, not generated cosmetic parameters. No timer-driven queue simulation or extra background work was added.

## Verification and unfinished work

Library/withdrawal-stage-zero-validation.log is the full batch verification for this round: 135 PASS markers, zero C# errors/exceptions and restored preference backup. The new test covers initial queue/pass-rate ranges, exact threshold boundaries, unchanged pass rate, random-call failure ordering, base save/first-animation order, two saves on first Refresh versus one on repeat, retained positive timestamps/service number, derived guide flag, both description branches, LF-only replacement, live row tip reads, close-before-guide/show behavior and first-open flag surviving save failure.

This is controller composition evidence with explicit fixture clock/random/UI callbacks. No SDK event or real withdrawal is simulated. PlayProgressTween1 0x9cf7d0 and PlayProgress 0x9cfda8 timelines are subsequently restored in WithdrawalStageZeroAnimationAudit.md; actual view/scheduler and hint-page handoff remain pending. The current source TXProgress0Panel prefab is available, but no new visual screenshot or production page claim is made here. Full startup, country/AB routing and the complete 1:1 lifecycle remain unfinished.
