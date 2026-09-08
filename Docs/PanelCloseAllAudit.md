# Bulk panel close and existence predicate

OriginalPanelRegistry.CloseAll restores UIMgr.CloseAllPanel (0x9F7F94): first copy the dictionary's non-main keys into a local list, excluding UIName.MainPanel (1), then call the existing Hide operation for each snapshot ID. Panels opened by hide callbacks survive this pass. Per-panel hide/destruction/deregistration and delayed queue behavior remain owned by the established adapters. An exception stops traversal; no rollback or continue-on-error behavior is added. A small per-call list matches this low-frequency source operation and preserves reentrant snapshot isolation.

IsExistPanel restores 0x9F7DF8 exactly as registration count > 1. It does not inspect active state, panel identity or whether the sole entry is actually MainPanel. In particular, one non-main registration alone returns false, as in the source.

62 complete regression PASS markers in Library/unity-panel-close-all-validation.log; no compiler errors or exceptions found, preference fixture restored and backup absent. New deterministic tests verify empty/single-panel cases, main retention, snapshot exclusion of callback-created panels, next-pass removal, one scheduled queue dispatch per hidden panel, and stopping at a lifecycle exception while retaining remaining registrations. No visual assets changed, so no new rendering test was added.

The registry remains a runtime primitive awaiting normal-startup unified manager binding and concrete event producers. These tests do not establish full initialization/panel lifecycle parity. SDK handling is unchanged; remaining main-flow, region AB and GM work is still incomplete.
