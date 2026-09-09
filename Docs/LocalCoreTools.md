# Default local core tool composition

The default local-mode prefab now contains the restored MainPanelBottom prefab and a copy of MainPanelComplete's native ExchangeMask subtree. Their source layout, images, fonts, standard Buttons and 0.5-second exchange-cancel delay are retained. The partial legacy ReplayBg is hidden as a whole. The unbound SettingsBg is hidden in this core-only local prefab; original source prefabs are unchanged.

LocalGameplayController.Tools binds OriginalMainBottomView.BindScene to the existing OriginalItemManager, OriginalAddScrewFlow, OriginalRevokeFlow/OriginalRecordRevoke, world Exchange and OriginalGameScene.SetExchangeState. This makes the source operations reachable from the actual default UI. Normal move callbacks refresh inventory/button state; item costs still save through the source item manager. Board readiness refreshes the bottom and restores exchange camera/mask state after a restart.

The local serialized add-tile maximum is injected into both the operation and display predicates. OriginalToolItemDisplay keeps its existing server-document behavior when no maximum provider is bound. Local mode does not manufacture ServerConfigData to make the UI work.

OriginalReplayController exposes panel creation separately from its common Button wrapper, so the original bottom can own click masking/audio without playing twice. A bound modal predicate keeps the existing counted click-mask expiry from releasing world input while a local notice is still displayed. Full bottom replay opens the existing native replay prefab.

Insufficient inventory opens the existing local SDK-skipped notice. No-history and maximum-add cases have distinct serialized local notices. No ad, reward, cash or inventory grant is fabricated. Tests supply explicitly identified inventory fixtures only.

Validation: LocalToolsPlayValidation runs the actual default scene, standard Button listeners and camera-ray gameplay. It covers a normal transfer, undo cost/history and reverse-animation board restoration, exchange mode/slot rotation/save/display/cancel delay, add-rod unlock and cost/save, no-history and insufficient-stock notices, modal-mask expiry, and the actual bottom replay popup. No runtime callbacks or initialization flags are substituted. Physical pointer event ordering on a target device remains to be checked; this test feeds the real ray-input method and Button listeners explicitly.

The source-specific operation/effect behavior remains in its existing original classes. Complete reward/acquisition systems, settings and country/AB GM integration remain outside this core-operation checkpoint. This is not evidence of full game or source-device visual equivalence.

Verified in Unity 2022.3.62f3: `Library/local-tools-play-final.log` contains NUT_LOCAL_TOOLS_PLAY_PASS. `Library/local-tools-content-validation.log` finishes with NUT_CONTENT_VALIDATION_PASS and 164 PASS markers (1476 resources / 1474 boards). `Library/local-tools-bottom-current.png` and `local-tools-exchange-current.png` were inspected from the current project. The preference backup was restored. Existing scene teardown pool diagnostics and edit-mode tool-flight diagnostics remain unchanged.
