# Actual scene InitLevel entry

The native InitLevel method (0x9E8174) accepts reset, showBanner and firstInit independently and stores each in its invocation closure. The reconstructed game scene previously exposed only restart entry points and forced true when dispatching the initialization continuation's banner parameter.

OriginalGameScene.InitLevel(reset=true,showBanner=true,firstInit=false) now exposes the required three-argument entry through the existing local initialization boundary. BeginInitialization captures the supplied banner value instead of replacing it with true. Existing restart and first scene initialization calls retain their existing default behavior. SDK/request handling is unchanged.

The dedicated Play fixture runs the actual scene and schedules all four banner/first combinations through the new public method. It checks immediate reset of IsInitDone/IsSucceed/time, deferred callbacks, independent captured flags and immediate nonreset preservation of elapsed time. The nonreset assertion is before deferred board reconstruction; existing resume regression remains the board-behavior evidence. The fixture explicitly selects the existing nonempty ComeOnGold branch without manufacturing a server response.

This supplies the world entry required by the guide UI context; the production context and complete startup/panel integration are still incomplete. Native evidence remains local.

Verified with Unity 2022.3.62f3: 88 full-regression PASS markers in Library/unity-init-level-entry-validation.log and NUT_INIT_LEVEL_ENTRY_PLAY_PASS in Library/unity-init-level-entry-play.log. No compiler-error or exception markers; test preferences restored.
