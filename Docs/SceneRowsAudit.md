# Scene row hierarchy

Local ARM64 AddScrewPos 0xA059F0 creates Row_{row} parents and Pos_{column} children. Row world Z is assigned before SetParent(parent, true); ResetScrewPos 0xA05D2C assigns only column-local X and finds positions by name. This structure replaces the previous flattened ScenePosGroup representation.

The reconstruction uses lightweight configured ScrewRow and ScrewPosition prefabs in accordance with the project prefab-first rule. It does not construct static visual geometry in runtime code. Both new and reused transforms reset to unparented identity before the source parenting sequence. Row/position names use explicit numeric formatting without reflection or enum names.

Appending now reads actual row sibling order and child counts, so equal-count ties follow the last sibling, even when reordered. Existing positions stay alive; only the chosen row recenters. Clear returns positions before rows, preventing old children from remaining in reused row prefabs.

Library/unity-scene-rows-final.log contains 97 PASS markers including NUT_SCENE_ROWS_VALIDATION_PASS and the full terminal PASS, without compiler-error or exception markers. The added fixture checks three-row hierarchy, names, position component metadata, row/column coordinate separation, translated/rotated/scaled group parenting, reordered row ties, named-column layout despite reordered position siblings, stable objects and clear/rebind. Existing added-screw assertions now check world positions to retain their original intent.

Scope limits: these checks establish the row structure and tested layout behavior, not full game visual/lifecycle parity. The position prefab now includes the native-equivalent component carrying row/column/index metadata, initialized on each rent. Default production startup/tool composition and the previously observed application-exit pool warnings remain pending. SDK behavior is unchanged. Raw native evidence remains local.

Final Play: Library/unity-scene-rows-play-final.log contains NUT_ADD_NULL_SCREW_PLAY_PASS after the position component was added. Actual world append, retained nuts, three particle systems, game-time cleanup, two refreshes and save/consumption passed. No compiler-error or exception markers were found. The existing exit reparenting warnings remain; preference backup is absent after restoration. This does not establish clean application teardown or full production startup.
