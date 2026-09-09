# Global scaled and unscaled animation scheduling

The existing OriginalUIAnimationDriver now supports Schedule(duration, ignoreTimeScale, callback). This supplies the actual global timer required by the first withdrawal-stage animation's DOVirtual.DelayedCall(2, callback, true). Scene Update passes both Time.deltaTime and Time.unscaledDeltaTime; no Editor-only clock or runtime fallback was added.

## Lifetime and dispatch

Owner animation entries continue to use scaled delta, including while their component is disabled or hidden. Global delayed callbacks are separate ownerless entries in the same list. A scaled timer consumes only scaled delta; an unscaled timer consumes only unscaled delta. Compact retains live timers even if the originating panel has been destroyed. Unregister removes that owner's animation entry without clearing global callbacks, including when null is passed.

The active entry count is snapshotted before dispatch. Timers created from an animation callback cannot consume that frame's remaining delta. A timer created by another timer also waits for a later dispatch. Completed timer entries are removed before invoking their callbacks, ensuring they are not invoked twice. The existing recursive-update guard remains. The single-delta Advance overload forwards the same deterministic delta to both clocks, preserving existing validation callers; its zero delta remains a no-op.

The shared Update loop adds no per-frame hierarchy searches, object creation, string formatting or LINQ. Timer entries are allocated in the existing list only when scheduled. Native Unity Time and the already scene-owned MonoBehaviour pump drive the runtime.

## Verification

Library/animation-scheduling-validation.log: 137 PASS markers, zero C# errors/exceptions, restored preference backup. Existing complete content validation still passes. New tests distinguish scaled and unscaled deadlines, pause ordinary owners, verify one invocation, nested/large-delta creation deferral, owner unregister/destruction independence and legacy deterministic advancement.

The first withdrawal-stage timeline test now passes OriginalUIAnimationDriver.Schedule directly and registers its Advance with the actual global pump. Its row-two completion schedules the unscaled prompt; destroying the fixture's UI targets and advancing with zero scaled delta still executes the prompt after its two-second unscaled delay. The separate injected scheduling-contract test remains useful for inspecting source callback arguments.

Library/animation-scheduling-play.log: PASS. The current scene's real Update executes an unscaled timer while Time.timeScale is zero, leaves the scaled timer pending, and completes the latter after time resumes. Both callbacks execute once. The test restores the previous time scale and preference fixture. This is runtime clock evidence, not a new visual screenshot or payment simulation.

## Remaining work

TXProgress0Panel source-prefab adaptation and owning view/host still need to register the animation and bind the real prompt-page route. Panel 33 and its handoff to the second animation phase are not yet connected. All country/AB branches, production startup and complete 1:1 lifecycle remain incomplete. SDK behavior is unchanged.
