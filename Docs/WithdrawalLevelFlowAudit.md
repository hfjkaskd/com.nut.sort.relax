# TXLevelPanel lifecycle and response consumer

Restored UIName 28 controller behavior as OriginalWithdrawalLevelFlow. The source view/steps are now restored in the companion WithdrawalLevelViewAudit.md; GoldGet1 transport remains an injected boundary. No runtime response is generated automatically.

## Native evidence

Init 0x9ca63c: base initialization first, then required boxed-int arguments[0]. Capture static IsRecordIn into the instance and clear the static flag, then bind Close before Sure. Invalid/null/empty arguments fail before consuming the entry flag.

Refresh 0x9ca7b8: tip 89 with Math.Min(captured level, 3), without a lower clamp. Display formatted Level1Gold only for captured level 1, otherwise Level2Gold. Invoke the owning view's PlayStepTween after both text assignments. Static visual construction is not performed by the controller.

Close callback 0x9cae48: invoke NewbieGuidePanel.CallbackActionInvoke(null) before initiating base close. An incompatible or failing callback prevents close; no defensive substitution of level for null has been introduced.

Get 0x9cacac: initiate close, call RequestMgr.GoldGet1 with a fresh instance response callback and the captured level, then inspect whether panel 5 is open and hide it if so. A synchronous response therefore executes before that panel-open check, and a thrown request prevents the check.

Response callback 0x9cae68: ignores its response parameter entirely; it does not share the higher-level GoldGet success-status gate. Level 1 sets IsTXLevel1; level 2 sets IsTXLevel2. For a non-record entry, if that level's IsGoldReduce flag was false, set it first, subtract that level's saved reward from Gold, clamp to zero, and invoke GoldItem.Refresh (verified source virtual slot 5, 0x9db4bc). Record entries and already-reduced entries skip this deduction and item refresh. Other captured levels skip these status changes.

All callbacks then SaveData, show PlayerGoldGetHint.ShowSelf using Level1Gold only for level 1 (otherwise Level2Gold), and call GoldItem.RefreshLssGold(false, 0). Non-record entries next invoke the guide immediately with the captured level and pass the returned Action to DelayCallback(1). Existing CallbackActionInvoke returns null; the implementation preserves the call order rather than turning it into a delayed guide invocation. Record entries skip this guide/delay tail. Each mutation, refresh, save and callback retains sequential exception behavior.

## Validation

Library/withdrawal-level-flow-validation.log: 130 PASS markers including final content PASS, with no C# errors or exceptions. Tests cover held requests (no mutation before callback), native null/failed-payload-independent behavior using explicitly synthetic responses, one-shot record entry, fresh callbacks, capture and argument failures, first versus repeated deductions, second-level zero clamp, display ordering, close callback errors, save/refresh errors, and upper-only tip clamp.

This test suite does not represent a successful real withdrawal. Production code only receives callbacks through the supplied request implementation; transport and response creation were not implemented or bypassed. SDK behavior remains unchanged.

## Next integration

The current TXLevelPanel prefab, PlayStepTween, real fields/Buttons and panel 28 host now extend the actual guide/confirmation Play fixture. GoldGet1 request handling, subsequent visual results, panel 22 and production startup/country/AB lifecycle composition remain incomplete.
