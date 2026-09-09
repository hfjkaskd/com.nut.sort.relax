# First withdrawal stage animation timeline

OriginalWithdrawalStageZeroAnimation restores the two source animation entry points on already-configured Transforms. It does not build runtime UI. Its serializable Timing data can be stored by the future source-prefab view. The normal scaled update pump, unscaled delayed-call provider and actual panel hosts must still be connected by that view; this change does not assert an end-to-end production page.

## Recovered first phase

PlayProgressTween1 0x9cf7d0 sets Sure to scale zero and collapses every direct Step child to (1,0,1). Only numeric rows 1 and 2 are then scheduled. Their Content scales become (1,0,1), existing Done objects are hidden and required Loading objects shown. Each row scales Y to 1 over 0.1 seconds, with delay 0.3 + (row-1)*2 seconds, using the recovered global default OutQuad.

Row-one completion callback 0x9d0274 creates its Content ScaleY over 0.2 seconds after a further 1.5-second delay. Its OnStart callback 0x9d042c shows an existing Done and hides Loading unconditionally, including when Done is missing. This differs from the earlier level-panel animation's conditional Loading behavior.

Row-two completion does not expand Content. It schedules DOVirtual.DelayedCall(2, callback, true): the true flag means ignore time scale. The callback 0x9d04f4 shows panel 33 with empty arguments and does not resume progress itself. Its action is independent of the panel targets; destruction of the original targets must not silently cancel the global prompt. The current animation exposes this scheduling contract rather than providing a fabricated timer or prompt page.

## Recovered second phase

PlayProgress 0x9cfda8 handles rows 2 through 4. It resets any existing Content to collapsed Y and resets their markers, without resetting Sure or all row scales again. Row 2 expands Content over 0.2 seconds after 0.5 seconds. OnStart 0x9d05d0 shows an existing Done and hides Loading.

Row 3 expands its row over 0.1 seconds after 0.6+0.3 seconds. OnComplete 0x9d0620 creates Content expansion over 0.2 seconds after another 1 second. Its OnStart callback 0x9d0724 only shows Done; it does not hide Loading. That source behavior is deliberately retained.

Row 4 expands over 0.1 seconds after 2*1.1+0.3 seconds. Completion 0x9d0200 creates a uniform Sure scale to one over 0.3 seconds. All these scales retain default OutQuad. ScaleY captures its starting Y at delayed startup and preserves the other live axes; uniform Sure captures all starting axes. Callback-created tracks begin on a later animation update, not using leftover delta from their creation frame. Repeated entry-point calls retain independent tracks.

The float constants were checked directly in the supplied arm64 ELF: 0x2017858=0.2, 0x2017870=0.1, 0x2017874=0.6, 0x2017880=0.3 and 0x20178a4=1.1. The third-row delay retains the source float addition rather than replacing it with a differently rounded decimal calculation.

## Validation and remaining integration

Library/withdrawal-stage-zero-animation-validation.log is the full batch verification for this change: 136 PASS markers, zero C# errors/exceptions and restored preference backup. The timeline fixture uses a synthetic UI hierarchy only inside Editor validation, with source timing values. It checks both phases at intermediate points, OnStart marker differences, a fifth child being collapsed but not scheduled, required unscaled delay arguments, an explicitly held prompt callback, no implicit resume, repeated calls, paused scaled updates, large-delta deferral, missing Done handling and prompt callback survival after target destruction.

This is timing/controller evidence, not original-device pixel equality or an actual stage-page screenshot. Source TXProgress0Panel prefab adaptation, the owning view and global unscaled scheduler, panel 33 and its resume handoff remain unfinished. The source's DOTween dependency is replaced by native Unity data/Transform operations as requested; SDK behavior is unchanged. Full startup/country/AB/lifecycle composition remains incomplete.
