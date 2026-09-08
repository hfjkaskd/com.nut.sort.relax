# Unlock to target reward banner integration

The unlock panel Play fixture now binds the actual OriginalTargetRewardBanner already instantiated by normal startup and calls its Show(null) from the real Continue Button path. The former empty banner callback has been removed. Validation observes real text refresh and animation registration before unlock-index mutation, waits for all four animations to finish, and requires both gold-hint and reward-progress callbacks for each.

The fixture deliberately uses the existing first-level user data, for which source banner text does not need fabricated server rewards. It verifies no added save. Destination coordinates and the two top-area consumers remain explicit fixture adapters, not a claim of complete production MainPanel wiring. Original source continuation sends a null completion callback; its normal banner completion still invokes both top consumers.

This supplies integration evidence for the restored unlock and banner components. Automatic InitDone routing and full normal startup consumer binding remain unfinished. No SDK response or reward grant is simulated.

Unity 2022.3.62f3 Play PASS in Library/unity-unlock-banner-play.log; all four real banner completions and both downstream callbacks per completion verified. Preferences restored; no compiler or validation exceptions. Runtime code was unchanged in this integration-validation round.
