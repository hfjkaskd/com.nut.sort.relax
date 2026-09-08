# Initialization state and elapsed time

LuoSiSortMgr.InitLevel 0x9E8174 clears IsInitDone (manager field 0x31) and IsSucceed (0x34) before clearing/rebuilding the board. It does not clear IsFail (0x33). OriginalGameScene now owns those distinct flags and clears both at each BeginInitialization, including repeated pending restarts. Board/nut readiness does not set either flag.

LuoSiSortMgr.Update 0x9FB8F4 tests only IsInitDone and adds Time.deltaTime to UserLocalData.PassLevelTime (field 0x38). OriginalGameScene.Update now invokes that same addition before its existing reconstruction/input work. No success, failure, modal, input, or animation-readiness guard is added. It allocates nothing and does not write a save each frame. InitLevel(reset=true) retains its existing timer reset; initialization completion remains the responsibility of the initialization event consumer.

The actual-scene replay regression verifies paused and running clocks, the absence of extra guards, clearing of both flags on repeated restarts, preservation of failure, and no new save. The unlock Play fixture explicitly completes the initialization event after opening the selected panel; it verifies zero elapsed time throughout the post-rebuild wait and actual Update accumulation while the modal is shown. That assignment is an explicit fixture consumer, not a production readiness fallback.

Normal startup still lacks the full initialization event binding, so its board becoming ready does not automatically start this newly restored clock. Full production initialization and success consumers remain outstanding. This change restores their shared state and timer semantics without claiming whole-lifecycle completion.

Unity 2022.3.62f3: 72 full regression PASS markers in Library/unity-initialization-state-validation.log; actual Play PASS in Library/unity-initialization-state-play.log. No validation exceptions or compile errors; preferences restored.

Subsequent integration: SceneInitializationAudit.md supersedes the fixture initialization consumer described above. The fixture now runs OriginalInitializationFlow through OriginalSceneInitialization; it uses explicit reward/guide inputs and no longer directly assigns IsInitDone or uses a nonempty ComeOnGold shortcut. Production startup remains unbound.
