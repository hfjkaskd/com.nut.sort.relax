# Normal replay reconstruction

ReplayPanel.ReplayCallback (0x9E5424) starts CloseLssPanel, calls HideLssPanel(5), then InitLevel(true, false, false). SDK telemetry follows and remains excluded. It neither increments LevelSeed nor explicitly clears IsFail. InitLevel (0x9E8174) has no reconstruction-in-progress early return.

OriginalGameScene.RestartLevel previously ignored a direct call while IsRestarting was true. Removed that extra guard. The former single pending-board timer also coalesced repeated requests. Each request now retains its own elapsed delay and reset/entry flags; a later call does not postpone or cancel the earlier reconstruction. This also corrects repeated failure-restart scheduling. Native callback 0x9FF7CC schedules a captured action through DelayCallback; 0x9FFA28 executes that invocation's reset/first-entry flags while reading current user data at callback time.

The request list is idle outside initialization, performs no per-frame allocation, and excludes requests added during callbacks from the current time-advance snapshot. Existing serialized delay and entry settings remain authoritative. Scene teardown clears pending requests.

OriginalReplayRestartValidation exercises the actual scene, verifies repeated reset side effects, both independent reconstruction deadlines and board instances, unchanged seed/player stage/selected level/failure flag/cumulative move count, the short 0.5-second nut entry delay, and unchanged stored preferences. The earlier failure-restart test has been corrected: expecting a newer call to cancel the previous deadline was inconsistent with the native delayed actions.

Remaining parity work includes the Settings panel hide in ReplayCallback, complete panel-manager binding, and the full initialization/guide/banner lifecycle. The current level-view replacement/pooling and coroutine ownership still need whole-lifecycle comparison; this patch does not claim full InitLevel parity. SDK handling is unchanged.

Verified in Unity 2022.3.62f3: 65 regression PASS markers in Library/unity-replay-restart-validation-2.log; actual main/continue/replay Button and reconstructed-board PASS in Library/unity-replay-restart-play.log. Preferences restored after both runs. No compiler errors or validation exceptions found.
