# Add-screw main flow

OriginalAddScrewFlow restores LuoSiSortMgr.AddScrew (0x9FF33C), called by the existing OtherItem AddScrew Button. It has required world-operation and configuration providers, with no unlock or grant success defaults.

Source branch order:

1. Nonpositive AddScrewCount opens panel 4 with item type 4, without reading configuration or checking the board.
2. CurrentLevelAddScrewCount >= LSSLSMAC shows pop-tip 5 before reading the mode.
3. The current LSSAB value adds one usage unit when true, four when false. The addition is unchecked and can cross the maximum because only the old count is checked.
4. LevelInfo.Unlock is tried after the counter write. Success skips all further addition and the second mode read.
5. If unlock fails, the live LSSAB value is read again: true calls AddTile, false calls AddNullScrew. The value can differ from the one used for the increment.
6. ItemMgr.AddLssItem(type 4, count -1, refresh true) follows the world operation. World-operation failure leaves the usage increment but prevents consumption. SDK vibration and analytics remain excluded.

The maximum provider corresponds to ServerConfigData.LSSLSMAC at 0x60, and the mode provider to LSSAB at 0x64. This flow does not invent missing configuration values or change the existing local region handling.

Validation covers lazy reads, inclusive limit, crossing the limit by four, unlock success, both add branches, configuration changes during unlock, unchecked overflow and exception mutation. It also instantiates the complete main prefab and invokes its actual standard AddScrew Button, verifying the common gate and panel-then-audio entry path.

Remaining: LevelInfo.Unlock, AddTile and AddNullScrew world implementations must be restored and bound to this flow, along with actual configuration/panel context in default startup. Observed callback tests do not prove that a new world screw or tile exists. Full lifecycle and visual parity are still incomplete. Native evidence remains local.

Verification: `Library/unity-add-screw-flow-validation.log` contains 93 full-regression PASS markers including NUT_ADD_SCREW_FLOW_VALIDATION_PASS, with no compiler-error or exception markers. Validation preferences were restored and the backup is absent. No separate Play-mode world-addition success is claimed.
