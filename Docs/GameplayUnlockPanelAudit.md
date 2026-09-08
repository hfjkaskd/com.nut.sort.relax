# Full unlock gameplay prefab and view

Restored all 12 objects from the original UnlockGameplayPanel prefab. All twelve RectTransform YAML blocks match the source exactly. Original fonts/materials, four gameplay sprites, title, glow and Button references are retained. Official Unity UI/TMP components replace exported script references. The glow uses the existing global rotation driver with original 3-second, -360-degree settings. Static structure remains authored in the prefab.

OriginalGameplayUnlockPanel binds localized labels, original Reward_appear/Click sounds, the standard Continue Button, restored icon/text display and confirmation action. Opening/title/closing tracks use existing serialized base-panel curves and global updates. Hiding during close does not prevent completion. The UI assembly now explicitly references the project's own Gameplay assembly; no third-party dependency was added.

Validation: full content regression in Library/unity-unlock-panel-validation-2.log passed after correcting the initially missing assembly reference. Play validation in Library/unity-unlock-panel-play.log rendered all four variants at 480x1040 and checked actual Button gate, null banner callback, index-before-close and hidden closure. Latest Library/ValidationCaptures/unlock-0.png through unlock-3.png were visually inspected. Test preferences were restored.

Scope: banner invocation is a fixture callback boundary, not a fabricated reward or actual target-banner integration. Normal startup automatic show, complete panel-registry binding and the base Hide queue dispatch remain pending for this panel. The authored view is not proof of entire-game 1:1 parity. SDK handling remains unchanged.
