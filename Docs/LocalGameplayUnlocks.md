# Local core initialization and gameplay unlocks

Default local play now composes the existing native UnlockGameplayPanel through OriginalGameplayUnlockPanelHost. Core initialization runs at real board readiness. It preserves the stateful IsCannotMove check before the unlock decision; a restored deadlock therefore dispatches the existing delayed failure without requiring another player move.

The original scene-session unlock configuration is [8,21,61,111]. OriginalGameplayUnlock tests internal Level == configured value - 4 and the existing NewGameplayUnlockIndex boundary. Thus internal levels 4/17/57/107 select the four original icons and localized descriptions (text IDs 63..66). Displayed level numbering is independently computed by the original tables and is not the same as every internal level number.

When a real ServerConfigData document exists, OriginalGameplayUnlockConfig retains its original case-insensitive override behavior and errors. When it is absent in local mode, the scene-session defaults are supplied directly as the typed configuration provider. No synthetic server/reward document is created.

The source popup, glow, fonts, layout, sounds, opening/closing motion, Button listener, registry lifecycle and delayed queue callback remain unchanged. Local mode calls the unlock decision with showBanner=false because its SDK/reward-stage banner is not supplied. OriginalGameplayUnlockContinue still assigns exactly index+1 and initiates close. The local host adds a durable save on Hide; the original reusable continuation itself is unchanged. Confirmation suppresses repeat display after reloading the scene.

Modal input covers both the local result/notice and the native unlock host. The original counted click-mask predicate includes this host so expiry cannot enable world input beneath an open popup. Closing waits for registry removal and accounts for other registered modals/replay before restoring world input. The UI click shield remains separate; see ExchangeTapInput.md. The source IsInitDone clock state remains true while the unlock modal is shown.

LocalUnlockPlayValidation opens the actual default scene using explicit starting-level fixtures for all four milestones, with no server/reward documents. It checks the matching icon/description, actual camera-ray input blocking, real Continue Button/close animation, duplicate-click gate, persisted unlock index, and no repeat after actual scene reload. A separate persisted deadlock fixture at an eligible milestone proves failure takes priority and retry returns through the pending unlock. These fixtures select starting states; they do not simulate server responses or grant rewards.

The four current local-unlock-0/1/2/3-current.png captures in Library were visually inspected. They show the native descriptions for hidden nuts, colored covers, immovable rock-bound rods, and surrounding-rod unlocking. This is current-engine visual evidence, not source-device pixel-equivalence proof or proof that every later board has been played through.

Runtime integration remains local SDK-skip mode. Full reward-guidance/peripheral event routing and country/AB GM routing are still incomplete.

Verified with Unity 2022.3.62f3: `local-unlock-play.log` contains NUT_LOCAL_UNLOCK_PLAY_PASS; `local-unlock-core-play.log` contains NUT_LOCAL_GAMEPLAY_PLAY_PASS for default tutorial/progression/reload/failure/replay including the new milestone modal; `local-unlock-tools-play.log` contains NUT_LOCAL_TOOLS_PLAY_PASS, including the still-active click-mask check after notice dismissal. `local-unlock-content-validation.log` ends with NUT_CONTENT_VALIDATION_PASS and 164 PASS markers. Logs are in Library. Preferences restored; existing teardown pool and edit-mode tool-flight diagnostics remain unchanged.
