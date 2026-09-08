# Initialization continuation audit

Native evidence: InitLevel delayed callback 0x9FFD10, synchronization callback 0x9FFEC8, and TXMgr.IsCompletePassStage2Level 0x9C1AD8. RewardDetailInfo.Stage2RealLevel is field 0x44.

The delayed continuation reads current ComeOnGold. Nonempty strings (including whitespace) enter InitDoneEvent directly without reading reward data. Otherwise the current player level is compared strictly greater than bear_list[2].Stage2RealLevel. Only that completed-stage branch supplies its captured showBanner/firstInit callback to the existing synchronization boundary; all other cases enter InitDoneEvent immediately. Missing data and synchronization exceptions do not create successful replies. Concurrent calls retain independent callbacks.

OriginalInitializationContinuation restores this decision with required typed operations. OriginalRewardProgress now exposes the strict stage-completion predicate. No SDK request, synthetic server document, grant, or save was added.

Validation covers below/equal/above boundary, lazy nonempty/whitespace behavior, missing and replaced documents, delayed callbacks with changed user state, independent captured flags, and synchronization failure. Included in OriginalContentValidation.

Integration remains incomplete: BeginInitialization now schedules the main-panel wait and this continuation when BindInitialization has supplied the typed UI lifecycle. Normal startup has not yet supplied that binding. Production still uses the partial main panel; complete initialization panel consumers and existing SDK boundary must be connected before claiming end-to-end parity. Component validation is not proof of whole-game 1:1 reconstruction.
