# Withdrawal account confirmation

Restored UIName 21 (TXUserInfoSurePanel) behavior, original prefab, native Unity view and panel registry host. No SDK or payment result is synthesized.

## Native behavior

Init 0x9d59cc: base initialization first; nonempty arguments directly unbox level from index zero, otherwise capture unchecked current user Level minus one. Capture the current UserLSSInfo object reference. Bind confirm, show confirm iff !IsHintGoldGet, bind reenter, bind close, in that order. Empty arguments are a non-null array; this matters to subsequent callbacks.

Refresh 0x9d5c54: use the captured account reference. Nonempty OtherInfo is displayed verbatim with Atlas/Pay/Other and bypasses normal channel/area lookup. Otherwise resolve the current country's pay channel using the captured record's selected index, then its IsEmail metadata. Email with a name is Name + newline + Email; nameless email is Email or empty. Phone with a name is Name + newline-plus + live area + hyphen + Number; nameless phone is live area + hyphen + Number, without plus. Empty fields use normal string concatenation semantics. Assign text before loading the full channel icon; selected index is read again after text assignment. Replacing user.UserLssInfo after initialization does not change the captured reference, but mutation of that captured object remains visible.

Confirm 0x9d6178: initiate base close, then call UserMgr.GoldGet(captured level, arguments != null). The boolean is array existence, not array length. It does not clear the hint flag or invoke the guide callback itself. This reconstruction retains an explicit Action<int,bool> application boundary; it does not fake application success.

Reenter 0x9d620c: show panel 20 with a fresh one-element array containing captured level when arguments exist (otherwise null), then initiate close. Close callback 0x9d6358: initiate close, invoke shared guide callback with captured level, then clear IsHintGoldGet. Exceptions preserve the original sequential failure behavior.

## Original view and composition

Fourteen source GameObjects remain. All 26 original RectTransform/CanvasRenderer blocks are unchanged. Root script and official Image/TMP/Button/layout scripts are remapped; three native Button feedback components are added. Source vertical layout script ID 1297475563 was verified against the Unity MonoScript MD4 file-ID mapping, with Button ID 1392445389 as a known check (HorizontalLayoutGroup maps to -405508275). Original layout, fonts, image references, labels 113/24/115 and independent main/title/close settings are retained. No additional artwork was required.

OriginalWithdrawalConfirmationHost registers before initialization/refresh, and removes the panel after animated close via the shared registry. The extended SuccessGuideHost Play fixture creates the real confirmation host from actual account submission, returns to the real saved form, resubmits, and calls the actual Confirm Button's GoldGet boundary. The withdrawal guide callback remains installed, awaiting a real downstream completion.

## Verification and remaining scope

Library/withdrawal-confirmation-validation.log: 128 PASS markers including final content PASS; no C# errors or exceptions. Tests cover native formatting branches, captured reference versus live mutations, fallback level/array semantics, error sequencing and real prefab Button/visibility/animation behavior.

The GoldGet application lifecycle, panel 22, downstream result branches, production initialization/transport composition and full country/AB lifecycle coverage are not complete. Fixture assertions and reconstructed screenshots do not prove complete original-device runtime equivalence. SDK boundaries remain unchanged.

The first extended Play run (success-guide-confirmation-play.log) hit the old 60-second editor wall-clock limit at phase 5, with Time.time=5.097781, Time.timeScale=1, InputBlocked=false and IsRestarting=false. The runtime remained in an unfinished scaled-time animation chain. The validation-only wall-clock watchdog was extended to 180 seconds; all scaled-time waits/assertions and production runtime code were left unchanged.

Extended Play result: Library/success-guide-confirmation-play-2.log contains NUT_SUCCESS_GUIDE_HOST_PLAY_PASS with no C# error or exception. It verifies real form submission, confirmation display, reentry with saved fields, resubmission and GoldGet boundary after close initiation, followed by registry removal. The expected duplicate TXPanel diagnostic remains part of the existing ownership test. Current Library/ValidationCaptures/withdrawal-confirmation-current.png (480 x 1040) was visually inspected: original PayPal artwork, name/email, vertical Withdraw now/Re-enter buttons and external close button. No old screenshot was used; this is reconstructed visual QA, not original-device pixel equivalence. Test processes exited and preference backup was removed after restoration.
