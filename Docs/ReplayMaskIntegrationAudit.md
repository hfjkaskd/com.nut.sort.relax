# Replay click-mask integration

The normal startup replay controller now uses OriginalCountedMask, replacing its single Update countdown. Each admitted callback acquires the original configured 0.2-second mask and schedules an independent release through the scene-owned scaled-time DelayCallback. Disabling the controller no longer strands the mask. Closing the replay panel retains any outstanding mask; mask expiration retains the existing modal-panel input block.

Native evidence: AddLSSListener callback 0x9C04A8 does not reject an invocation merely because a mask acquisition already exists; UIMgr.SetMask 0x9F8694 increments on each acquisition and callback 0x9F89D8 decrements/clamps on release. Normal pointer suppression still comes from the serialized UI mask. Direct repeated callbacks are not pointer interactions.

Validation: OriginalReplayMaskPlayValidation runs the real normal startup, repeats acquisitions, disables the controller across release, invokes the actual main replay and Continue Buttons, and checks panel-close/mask lifetime ordering. Full existing content regression remains required.

Scope: this does not claim complete initialization parity. InitLevel clears IsInitDone at 0x9E8298. Its waiting callback 0x9FFC1C schedules another 1.5 seconds before 0x9FFD10; InitDoneEvent 0x9FBD2C then owns guide/banner-dependent readiness. OriginalInitializationFlow implements those branches but is not fully bound to production consumers. The pre-existing replay initialization guard remains until that lifecycle can be connected faithfully. SDK handling and visual assets are unchanged.

Verified in Unity 2022.3.62f3: 64 full-regression PASS markers in Library/unity-replay-counted-mask-validation.log and NUT_REPLAY_MASK_PLAY_VALIDATION_PASS in Library/unity-replay-counted-mask-play.log. Both runs restored the preference fixture; no pending GameplayUserLocalData.bin backup remains.
