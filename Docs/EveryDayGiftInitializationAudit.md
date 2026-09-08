# Daily gift scheduling during initialization

This is a dependency of InitDoneEvent's banner completion and of the withdrawal-stage guide continuation, not an independent reward implementation.

## Native evidence

- ShowEveryDayGift (0x9FCBF8) reads LastGetEveryDayGift at user offset 0x100 and returns if IsToday is true.
- IsToday (0x9BCA68) converts Unix **seconds** with DateTimeOffset.FromUnixTimeSeconds and compares its Date to DateTime.UtcNow.Date. Invalid timestamps throw; no local timezone or rollover fallback is applied.
- The wait predicate (0xA0000C) is the inverse of UIMgr.IsExistPanel (registry count greater than one, including MainPanel).
- UntilCallbackAsync.MoveNext (0x9BD200) first yields a native WaitUntil. It invokes the optional action afterward, with no scaled delay.
- Completion (0xA0008C) calls ShowPanel(16), fetches the live user, reads current TimeSeconds, assigns LastGetEveryDayGift and invokes SaveData, in that order.
- Each call can schedule a separate wait. There is no pending flag, second date check, deduplication, or cancellation when the claim timestamp changes. Panel-show failure prevents timestamp/save; save failure retains the prior timestamp write.
- InitDoneEvent banner callback (0xA002F0) calls HideLevelHint (0x9E38C4) before daily gift in its final branch. HideLevelHint is a single return instruction, so no extra UI hiding should be invented.

## Implementation and limits

OriginalEveryDayGiftFlow implements the date, predicate and completion ordering. OriginalGameScene.ShowEveryDayGift supplies live scene user data and the existing SaveUserData path, so the board snapshot and user timestamp use the same save envelope. ScheduleUntil runs OriginalUntilCallback on the scene MonoBehaviour with Unity WaitUntil. No Editor-only runtime path, SDK grant, or reward payload is added.

OriginalSceneGuideUI.DailyGift now calls this scene implementation directly. IOriginalGuidePanels supplies HasPanel and ShowPanel rather than an unimplemented daily-gift operation. Callers must supply the real global panel-registry predicate and panel dispatcher. The current partial startup does not yet supply these dependencies. The gift panel's own visuals/claim flow and complete production initialization remain pending. This change does not unblock normal startup or prove whole-lifecycle parity. Native disassembly remains local.

## Verification

- `Library/unity-daily-gift-validation.log`: 90 full regression PASS markers, including UTC midnight/same-day future time, invalid timestamp, lazy polling, independent waits, timestamp change while waiting, callback reentry and failure order.
- `Library/unity-daily-gift-guide-play.log`: `NUT_EVERY_DAY_GIFT_PLAY_PASS` exercises OriginalSceneGuideUI through OriginalGameScene, real Unity WaitUntil while timeScale is zero, current timestamp and actual user/board persistence. Panel dispatch is explicitly observed; no gift prefab or successful claim is simulated.
- No compiler-error or exception markers were found in these logs. Test preferences and timeScale restored; no validation preference backup remains.
