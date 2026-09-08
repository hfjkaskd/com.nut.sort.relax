# Record guide lifecycle integration

OriginalRecordGuidePanelHost registers UIName 34 before initialization, performs the native Init/Refresh sequence, and deregisters/destroys the view through the shared panel registry after its close animation. OriginalRecordGuidePanel.Refresh exposes the existing tip and currency-material update from 0x9D2060. Standalone Initialize retains its existing default refresh; the host explicitly separates those calls.

The inherited BaseLSSPanel.Hide behavior now schedules queue advancement using the original 2.5-second value serialized on the existing prefab. No hierarchy, layout, font, animation curve or texture asset was changed. The scene owns the delayed callback, so destroying the view does not cancel queue dispatch.

StartCallback 0x9D2140 closes the panel, writes IsCompleteRecordGuide and re-enters InitDoneEvent(true,false), without waiting for the closing animation. The existing view starts close before invoking its supplied callback. The runtime host can bind OriginalSceneInitialization.CompleteRecordGuide, which preserves the completion write and real flow re-entry; repeated Close calls are idempotent in the view. SDK telemetry remains excluded.

The record Play fixture now enters via actual RestartLevel, main-panel wait, delayed continuation, OriginalSceneInitialization and the original record branch. The standard Button uses the scene IsInitDone gate and invokes CompleteRecordGuide; the test no longer writes IsCompleteRecordGuide. It explicitly selects player level 2 before clicking to observe the next guide branch (panel 7, GuideIndex 1) without simulating a server reward. The next guide UI remains an assertion boundary. It verifies close-before-write, immediate re-entry, delayed host removal, queue dispatch after destruction, hidden/disabled animations, current US currency texture and no added save. Test-only reward input supplies the pre-initialization stage boundary, and preferences are restored.

Production startup still has no complete UI lifecycle binding; the record host is available for that binding. The next guide prefab and the complete main-panel startup remain outstanding, so this is not full player-lifecycle parity.

Validation: Unity 2022.3.62f3 full regression 72 PASS markers in Library/unity-record-routing-validation.log; actual record guide Play PASS in Library/unity-record-routing-play.log, current record-guide.png regenerated, preferences restored.
