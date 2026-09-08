# Success manager flow

Local LuoSiSortMgr.Success 0x9FD578 sets IsSucceed before reading ServerConfigData.LSS260820 (0x58), and returns immediately on repeated success. With that flag false, user Level==1 and LevelSeed<=2, it increments the seed unchecked, calls InitLevel(true,false,false), then refreshes teaching when its consumer exists. This branch does not query tables, celebrate or request rewards.

Normal settlement captures LevelTable.GetLevelInfoById(user.Level). A non-null entry with SubTotalRound==SubRound hides the gold hint, hides reward progress, plays StageComplete, and spawns the original SuccessEffect under the UI parent. Intermediate subrounds and null entries skip celebration but still schedule the reward-show request. The request delay is 0.6000000238418579, read directly from the ARM64 constant at VA 0x2017874. It retains the captured table reference. No level increment, direct save or reward grant is performed here. SDK analytics are excluded.

OriginalGameScene.BindSuccess composes the real success flag, existing InitLevel, tables, audio, effect and scheduler. Teaching, UI hide and reward-request consumers stay explicit. The request continuation does not fabricate ClearanceRewardShowS2C or implement the unresolved response/panel branch.

The original SuccessEffect prefab and StageComplete audio/import metadata are restored. The effect has one native autoplay ParticleSystem using the existing confetti material. Source ShowSuccessEffect relies on autoplay and a five-second lifetime; no forced replay is added. Its UI-parented instance survives board Clear and is released on its own game clock or full effect-manager teardown. Rows/nuts retain their prior cleanup behavior.

Remaining: DoneEvent reward/lucky branches, ClearanceRewardShow response handling and actual success/reward panels, first-level teaching host composition and default production startup are still incomplete. This is not a complete 1:1 lifecycle or rendered-visual parity claim. Raw native evidence stays local.

Verification: Library/unity-success-flow.log contains 100 PASS markers including NUT_SUCCESS_FLOW_VALIDATION_PASS and terminal NUT_CONTENT_VALIDATION_PASS. Library/unity-success-flow-play.log contains NUT_SUCCESS_FLOW_PLAY_PASS for actual scene flag, main progress hide, StageComplete audio, autoplay particle prefab, one delayed captured-table request, survival across board clear and five-second game-clock cleanup. Both logs have no compiler-error or exception markers. Preferences were restored and backup is absent. Existing application-exit pool reparenting warnings remain. The Play case directly invokes the composed manager with explicit configuration and request consumers; it does not establish the unresolved DoneEvent-to-production-reward chain.

Restored resource SHA-256: SuccessEffect.prefab c889d3a7a063d74163362283cbc09a3be886db8a2dab866c29a0141fa4e4f2b4; StageComplete.ogg e64a0f4d19ffcb86d4597791be242cb8119b7c6765f2107a8087390b58f08b1f.
