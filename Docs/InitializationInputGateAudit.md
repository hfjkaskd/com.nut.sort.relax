# Initialization gate for screw input

Native CoreGame.Level.Update (0x9F91B4) checks the panel/teaching override, board data, and LuoSiSortMgr.IsInitDone (offset 0x31) before processing input and raycasts. IsCanOperatorScrew (0x32) overrides the panel block only; it cannot bypass initialization.

OriginalGameScene.TryOperateAtScreenPoint now rejects input while IsInitDone is false, including when the board has finished reconstructing and teaching is enabled. This uses the same runtime path in Editor and device builds.

The scene regression and actual Play fixture exercise the false gate using a real screw screen position, then explicitly supply readiness to test selection, transfer and completion. Resume fixtures explicitly supply readiness before operations in each loaded scene; the replay fixture supplies it before testing modal blocking. These are isolated validation states, not production initialization or simulated SDK success.

## Remaining production gap

Normal startup currently constructs only the partial main panel and does not bind the complete initialization/panel chain. It therefore does not reach IsInitDone and now correctly keeps screw input blocked. The next integration work must establish the real initialization continuation and panel dependencies. Board reconstruction must not silently set readiness. The game is not yet a complete playable 1:1 reproduction.

Native assembly remains local and is not included in the repository. SDK behavior is unchanged.

## Verification

- `Library/unity-init-input-gate-validation.log`: 89 full regression PASS markers.
- `Library/unity-init-input-gate-play.log`: scene and effects Play PASS, including blocked uninitialized input before fixture readiness.
- `Library/unity-init-input-gate-resume.log`: actual operation autosave, reload, resumed completion and completed-save reset PASS.
- `Library/unity-init-input-gate-replay.log`: actual standard Button/pointer dispatch, modal blocking and restart/cancel Play PASS.

No compiler-error or exception markers were found in these logs. Validation preference backup is absent after restoration. These checks do not establish production initialization or whole-game visual parity.
