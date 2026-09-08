# Newbie guide target-completion transition

Native ShowGuide jump-table entry 1 (0x9E2AE8) registers the panel's Action<object>, calls CloseLssPanel, then shows UIName 35 / TXGuideTargetCompletePanel with current UserLocalData.Level minus one. The level is read after the close call. OriginalNewbieGuideView.ShowTargetCompletion now restores that order with the supplied completion operation and typed panel argument. It does not invoke or synthesize the completion.

CallbackActionInvoke 0x9CF328 dispatches the currently registered static Action<object> immediately, clears the static field after successful return, and returns null as Action. It does not return a deferred delegate. A replacement installed during the callback is cleared; an exception retains the callback. This static registration survives destruction of the guide view. Native HideLssPanel 0x9E2F24 only kills the stored mask tween, and does not clear the global callback or call the usual base Hide queue. No automatic OnDestroy reset or base queue behavior was added here.

Validation instantiates the real guide prefab, checks registration precedes close and the next panel reads the changed live level, destroys the guide and dispatches the retained global callback, verifies exact payload identity, immediate null return, single successful consumption and failure/replacement semantics. The previous global registration is restored after validation.

The ShowGuide switch itself, TXGuideTargetCompletePanel view and its completion consumer remain to be connected. This transition is a runtime operation for that binding, not proof the entire branch is playable. SDK requests and reward responses remain unchanged.

Unity 2022.3.62f3: 77 full regression PASS markers in Library/unity-newbie-completion-validation.log, no compiler or validation exceptions; preferences restored.
