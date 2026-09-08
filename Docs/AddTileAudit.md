# World add-tile behavior

ScrewInfo.AddTile (0xA08888) reads LSSAB: true increments NutMaxCount with unchecked arithmetic; false assigns 4. The capacity write precedes Screw.Refresh (0xA0676C). This operation does not append NutInfo slots, alter occupancy, consume inventory, save, or reset selected/completed state.

Manager AddTile (0x9FE6B0) enumerates screws and stops after the first IsCanAddTile match. The getter (0xA07E48) is a constant true return, so the first entry wins even when already full. An empty board does not read the mode provider. No heuristic chooses a shorter or more useful rod.

OriginalLevelView.AddTile and OriginalScrewView.AddTile now perform the actual capacity mutation and existing prefab refresh. Existing segments are reconfigured; missing segments are rented from the configured pool. Decreasing to four retains extra segments with shaft/tip/hint hidden, matching the existing source refresh. Collider and ready/initial anchor geometry remain fixed source configuration. Capacity and logical slot count remain independent in the snapshot.

Validation instantiates the original level and screws, increases the first rod from four to five, verifies original segment reuse and the actual fifth segment, checks that logical slots and nut identity remain unchanged, assigns four and verifies surplus geometry, then reloads the saved capacity-five/slot-count-four board into actual prefabs.

This supplies the world add-tile dependency of OriginalAddScrewFlow. Unlock, new whole-screw construction and complete production startup/tool binding remain pending. SDK behavior is unchanged. Native evidence remains local, and this is not a whole-game visual parity claim.

Verification: `Library/unity-add-tile-final.log` contains 94 full-regression PASS markers including NUT_ADD_TILE_VALIDATION_PASS, with no compiler-error or exception markers. The first attempt had a missing test namespace import, corrected before the final run. No separate live Play-mode visual parity is claimed.
