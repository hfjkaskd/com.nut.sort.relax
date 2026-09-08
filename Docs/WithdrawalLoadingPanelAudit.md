# Withdrawal guide loading panel

Restored UIName 36, TXGuideLoadingPanel, using the current reverse-export prefab and native method bodies. No SDK behavior was changed.

## Native behavior

- Init (0x9c9120): base panel initialization followed by Image.fillAmount = 0.
- Refresh (0x9c9148): each invocation creates an independent linear float tween, 0 to 0.8 over 2 seconds after a 0.5-second delay. The float was decoded from ELF virtual address 0x2017878. Refresh does not synchronously reset the image or cancel older tracks.
- First completion (0x9c92c0): reads the actual current image fill, creates another linear tween from that value to 1 over 1 second after a 1-second delay. A callback-created tween begins on a subsequent animation update.
- Value callbacks (0x9c92a4 / 0x9c9420): assign Image.fillAmount only.
- Final completion (0x9c943c): starts BaseLSSPanel close, resolves registered TXPanel 18, calls GoldGetCallback (0x9c94e0) immediately. It does not wait for close completion and does not directly show panel 37.
- Native base opening/closing settings and delayed hide queue are reused. There is no direct main/Title child in this prefab.

## Assets and implementation

OriginalWithdrawalLoadingPanel is a native Unity component driven by OriginalUIAnimationDriver. Timing parameters and lightweight component references are serialized in the recovered prefab; no UI hierarchy is constructed by runtime code. OriginalWithdrawalLoadingHost uses the existing panel registry and resolves the live withdrawal view at completion.

All original block IDs, six GameObjects, and all 17 non-script serialized blocks are unchanged. Images and TMP use official Unity component scripts. Existing font/material/sprite references are retained; missing bg2_2 sprite and texture were restored. No Button exists in the original loading panel.

## Validation

Full content validation includes OriginalWithdrawalLoadingValidation: original hierarchy, script references, font/localization, scaled-time pause, linear intermediate values, delays, hidden updates, repeated refresh ownership, immediate claim versus delayed close, and hide queue.

The existing SuccessGuideHost Play fixture now clicks the actual guide index-2 button, verifies index 3 is saved, opens the real loading prefab, runs real animations, calls the actual registered withdrawal panel claim flow, observes panel-20 dispatch, and verifies both panels close. Prior refresh/duplicate registration/close/reopen checks remain covered.

Current visual capture: Library/ValidationCaptures/withdrawal-loading-current.png (480 x 1040), inspected after the Play run. It shows the original purple loading strip, label 167, decorative cash/wrenches, green progress and original underlying TXPanel. No old screenshot was used. This is current reconstruction visual QA, not proof of source-device pixel equivalence.

## Remaining scope

Production startup/gameplay initialization and all downstream panel-20 branches remain incomplete. The Play composition uses explicit initialization, transport and withdrawal-service fixtures; dispatch to panel 20 is observed, not represented as a finished real payment or downstream panel. The captured withdrawal-guide completion callback is preserved through loading and is not synthesized as a successful withdrawal. Country/AB coverage and whole-lifecycle 1:1 equivalence remain unverified. Existing exit-time pooled-object warnings are outside this change.

## Recorded runs

- Library/withdrawal-loading-validation.log: 124 PASS markers, no compiled-code exception or C# error.
- Library/success-guide-loading-play.log: NUT_SUCCESS_GUIDE_HOST_PLAY_PASS; current capture inspected; no exception or C# error. Expected duplicate TXPanel diagnostic comes from the ownership assertion.
- Library/withdrawal-loading-final-validation.log: 124 PASS markers including final content PASS after the initialization-order adjustment; no C# errors. Unity AndroidDeploymentTargetsExtension reported an already-exited process during device scanning at startup. This is recorded separately from successful project checks, not claimed as an exception-free run.
- Both final validation processes exited; preference backup was removed after restoration.
