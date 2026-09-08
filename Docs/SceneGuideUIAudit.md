# Scene-backed guide UI context

OriginalSceneGuideUI connects the guide branch adapter to the real MainPanel and OriginalGameScene. Gold/Coin targets are their component RectTransforms, matching native GetComponent<RectTransform>; they are not substituted with icon or nested click geometry. RefreshGold/RefreshCoin call the actual items, and RefreshMain calls the main panel's ordinary Top/Bottom refresh. The main provider is resolved per operation so replacement panels can be observed where the native code performs a fresh lookup.

Scene scheduling, counted mask, three-flag InitLevel, NewGameplayUnlock and target banner operations reuse their existing implementations. SaveUserData exposes the existing user store and original board snapshot serialization; when a current board is absent it retains the existing LevelInfo instead of inventing a board. Existing board-save callbacks reuse this method.

Unimplemented panel management/daily gift and the three distinct request response handlers remain explicit IOriginalGuidePanels and IOriginalGuideRequests dependencies. They are not silent no-ops or fabricated SDK results. Production startup has not yet created this context and all required panels.

The main-panel regression now uses the actual scene UI context to verify correct RectTransform identity, visible gold/coin values and Top plus Bottom refresh. Its unused scene/panel/request dependencies are intentionally absent in that focused fixture. The scene Play fixture additionally verifies real user-store persistence and current board snapshot alongside initialization parameter behavior. No new visual parity claim is made.

Full regression produced 88 PASS markers in Library/unity-scene-guide-ui-validation.log. One Unity AndroidDeploymentTargetsExtension scanner process-exited exception occurred during module loading before validation; its stack is editor device discovery, not game runtime. No compiler error was reported. Native evidence stays local.

Play verification: NUT_INIT_LEVEL_ENTRY_PLAY_PASS in Library/unity-scene-guide-save-play.log, including actual user/board persistence. No exception/compiler-error markers in that Play log. Test preferences restored.
