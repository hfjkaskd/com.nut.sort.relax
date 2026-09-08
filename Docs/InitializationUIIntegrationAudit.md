# Initialization UI integration

OriginalInitializationUI now implements the concrete UI operations required by OriginalSceneInitialization. It opens the existing record guide host for panel 34, closes that host through its actual animation path, shows the main panel's target reward banner with the supplied continuation, pushes the actual Top.PlayerHint schedule, and invokes the scene's daily-gift scheduler. Other panel IDs, global close-all and unlock arguments stay at the explicit global panel dispatcher boundary. Completed-stage synchronization is a separate required callback boundary, without an invented response.

## Source ordering

InitDoneEvent 0x9FBD2C sets IsInitDone before opening the unfinished record guide. TXRecordGuidePanel.StartCallback 0x9D2140 closes the panel, marks IsCompleteRecordGuide, and reenters InitDoneEvent(true,false), with no extra save. The source banner callback 0xA002F0 sets readiness, chooses first-level teaching or the later branches, and only pushes the hint when its captured firstInit is true. Thus returning from the record guide does not push that first-init hint. HideLevelHint 0x9E38C4 is a single return, preserved as an empty operation rather than an invented visual change.

## Verification scope

The new Play fixture composes the actual game scene, complete main-panel prefab, record-guide host and prefab, target banner, initialization continuation, and teaching host/prefab. It supplies explicit test server-data documents and a bounded panel dispatcher; unrelated fixture UI actions are not production implementations. It calls InitLevel and never assigns IsInitDone. Checks cover the initial wait, record-modal input block, standard Button callback, record-close animation, banner callback into first teaching, and real world-camera ray selection through the teaching override. It also checks that the record-return firstInit=false path does not push the player hint.

This validates a composed first-entry chain, not the default startup. OriginalStartupFlow still instantiates the partial main panel and lacks the complete data/panel context. Its initialization input block remains unresolved. The full success, withdrawal, region/version split and later lifecycle have not been proven. SDK behavior is unchanged. Native evidence remains local.

## Results

`Library/unity-initialization-ui-play-final.log` contains NUT_INITIALIZATION_UI_PLAY_PASS. `Library/unity-initialization-ui-validation.log` contains 90 full-regression PASS markers. Both final logs have no compiler-error or exception markers; the validation preference backup is absent after restoration.

An initial fixture attempt incorrectly used nonreset InitLevel(false) without a saved board, triggering the existing invalid-save error. The fixture now uses InitLevel(true,true,true) for a fresh first board; no runtime fallback or exception suppression was introduced.
