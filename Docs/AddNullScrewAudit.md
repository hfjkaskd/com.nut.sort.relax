# AddNullScrew world branch

Implemented from local ARM64 evidence: LevelInfo.AddNullScrew 0x9FA0A4, ScenePosGroup.AddScrewPos 0xA06038, manager AddNullScrew 0x9FE818. Raw evidence stays local.

The new unlocked screw copies the last screw ID and increments its index unchecked. Live LSSAB selects capacity one or four; empty NutInfo slots instead use the previous screw capacity. These values remain independent, including after snapshot reload. An empty screw list throws before reading configuration.

The position group chooses the existing row with fewest screws, with later rows winning ties. Only that row recenters. Crossing the normal initial-layout row threshold does not create another row. Camera sizing uses the selected row's new count before the new screw prefab is initialized. Existing state, positions, screw and nut views remain alive.

The initialized rod requests the restored original three-system UnlockEffect for three seconds. Manager bottom refresh follows world creation; the enclosing AddScrew flow then consumes item type four and performs the item manager refresh/save. The two refreshes are retained.

Verification: Library/unity-add-null-screw-final.log has 96 PASS markers, including NUT_ADD_NULL_SCREW_VALIDATION_PASS. The isolated six-rod fixture covers tie breaking, next-smallest selection, independent capacity/slots, stable old state and ready flags, camera/effect ordering and snapshot restoration. These are implementation checks, not a complete lifecycle or visual parity claim.

Follow-up: Row/Pos parents and actual row sibling tie ordering are now restored; see SceneRowsAudit.md. Default startup still needs real configuration/panel context and tool-flow composition; this branch does not invent SDK or server responses. Full production startup, lifecycle and visual parity remain incomplete.

Play verification: Library/unity-add-null-screw-play-final.log contains NUT_ADD_NULL_SCREW_PLAY_PASS for actual new-rod flow, three playing particle systems, cleanup after scaled game time, two ordered bottom refreshes, item consumption and real save. The first run incorrectly measured lifetime against editor wall time; the corrected fixture uses Time.time, matching the existing runtime deltaTime clock. No runtime timer fallback was introduced. Validation preferences were restored and their backup is absent. Final Play has no compiler-error or exception markers, but editor exit still logs attempts to reparent pooled nuts under the destroying GameScene. That lifecycle cleanup difference remains unresolved; this is not a clean full-application-exit claim.
