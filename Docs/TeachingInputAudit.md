# Teaching screw-operation override

Native LuoSiSortMgr.IsCanOperatorScrew is field 0x32. NewbieGuidePanel.SetTeach sets it true for levels below 2 and false otherwise. Level.Update (0x9F91B4) first checks UIMgr.IsExistPanel: an existing panel blocks screw input only when this flag is false. It is not a universal permission to bypass other readiness checks. Native guide close does not reset it.

OriginalGameScene now exposes this flag and applies it specifically to ModalInputBlocked. Startup InputBlocked, reconstruction and missing/unready board checks retain their existing behavior. The actual guide-host Play fixture writes the scene flag through the teaching binding and checks that close leaves it set. Production startup still needs to bind the host and supply the true panel-registry modal state.

The scene regression exercises real screen-point physics rays: startup block remains effective even with the teaching flag, modal blocks with the flag false, teaching permits selection through the modal, and the following move completes the source first board. This checks observable input behavior rather than only a boolean predicate.

Important remaining mismatch: native Level.Update also requires IsInitDone. The current local startup still lacks the complete initialization/panel chain, and this round does not establish that readiness gate's full native wiring. No synthetic initialization success was added. Full production lifecycle parity remains incomplete.

The full regression log Library/unity-teaching-input-validation.log includes an AndroidDeploymentTargetsExtension process-exited exception during editor platform loading. Its inspected stack is device discovery, not game code. Native evidence remains local.

Verification: 89 full-regression PASS markers; NUT_NEWBIE_GUIDE_HOST_PLAY_PASS in Library/unity-teaching-input-play.log without compiler-error or exception markers. Preferences restored.
