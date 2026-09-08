# Replay animation lifetime

OriginalReplayPanel now registers with the existing scene-owned OriginalUIAnimationDriver in Awake and unregisters on destruction. Its own Update is removed, preventing double stepping. Opening and closing no longer stop when the component is disabled or the panel GameObject becomes inactive.

Native BaseLSSPanel.CloseLssPanel (0x9C4F78) creates a DOScale track, uses InBack, and registers OnComplete plus AutoKill; it does not bind track lifetime to component enabled/active state. The original global tween pump semantics are already restored by OriginalUIAnimationDriver. Existing serialized opening/title curves, delays and close duration are unchanged, as is the independent title advancement during close. No third-party assembly is introduced.

OriginalReplayMaskPlayValidation now disables the actual opened replay component before waiting for its main scale to reach one, then invokes Continue and hides the GameObject during close. It verifies the global close finishes, destroys the hidden panel, releases input blocking, and still supports reopening and actual replay reconstruction. These cases fail with the former component-owned Update.

This fixes update ownership only. Complete base-panel lifecycle, registry binding and initialization/guide integration remain unfinished; no full visual or lifecycle parity claim is made.

Verified with Unity 2022.3.62f3: 65 regression PASS markers in Library/unity-replay-global-animation-validation.log and expanded NUT_REPLAY_MASK_PLAY_VALIDATION_PASS in Library/unity-replay-global-animation-play.log. No compiler errors or validation exceptions; test preferences restored.
