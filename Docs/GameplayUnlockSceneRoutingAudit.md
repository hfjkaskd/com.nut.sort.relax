# Scene decision to unlock host

OriginalGameplayUnlockPanelHost.TryShow now invokes the real OriginalGameScene.NewGameplayUnlock method and dispatches the decision into the host's registered Show path. It returns the source decision result; an unmatched configuration does not instantiate a panel. The current registered instance is exposed read-only for coordination.

Play validation now starts from the actual scene/configuration reader and decision for all four variants, rather than calling Show directly. It first excludes the configured index and verifies no panel is created, then permits it and verifies the real panel, Continue Button, target banner, index mutation, destruction and delayed queue. The per-variant LSSGPUL array is explicitly fixture configuration; it keeps first-level banner data independent of unimplemented server rewards.

This is a runtime connection usable by the future InitDone action adapter. Automatic invocation at the original initialization event, common IsInitDone gate and complete MainPanel consumers remain outstanding. SDK handling is unchanged.

Unity 2022.3.62f3: full content PASS in Library/unity-unlock-routing-validation.log and scene-configured route Play PASS in Library/unity-unlock-routing-play.log. No compiler or validation exceptions; preferences restored.
