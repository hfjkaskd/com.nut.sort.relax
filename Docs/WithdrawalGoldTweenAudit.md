# Withdrawal number animation consumer

Adds a configured WithdrawalGoldTween prefab with original 1.5-second duration. OriginalWithdrawalGoldTween supplies the native float-animation consumer for OriginalWithdrawalPendingGold. It uses the existing scene OriginalUIAnimationDriver (scaled time), OutQuad, independent per-call tracks and creation-order callbacks, consistent with the already restored DOVirtual.Float consumer in LoadingView. No previous animation is replaced on refresh and inactive/disabled presentation does not stop the registered scene pump. Creation defers the first text update until an advance; zero scaled time does not update. Tracks created by a callback start on the next advance.

The component only interpolates floats and invokes the existing pending-gold formatter/display callback. It does not create UI, grant currency, save or call an SDK. Duration is serialized in a prefab, not fixed in runtime code. It allocates one small track per infrequent refresh and reuses the list during updates; no LINQ or per-frame hierarchy lookup is added.

Validation instantiates the actual configured prefab and checks increasing/decreasing endpoints, OutQuad midpoint, pause, hidden update, concurrent track order, callback-created tracks and composition with the actual pending-gold flow/shared formatter. Balance application and save happen once before animation and remain unchanged throughout display updates.

The component is ready to bind to the recovered TXPanel GoldCount, but that full view/host and production scene binding are not restored yet. This is a numerical/behavioral validation, not a screenshot or full visual parity claim. Existing scene-owned driver teardown releases the consumer; complete original cross-scene tween lifetime is outside this check.

Verification: Unity 2022.3.62f3 full regression passed 116 markers including NUT_WITHDRAWAL_GOLD_TWEEN_VALIDATION_PASS and NUT_CONTENT_VALIDATION_PASS in Library/withdrawal-gold-tween-validation.log. No compiler-error or exception entries in the successful log. Preference fixture restored and its backup removed.
