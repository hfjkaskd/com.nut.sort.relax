# Deadlock and hidden-rod recovery

Implemented from the current ARM64 reference: CheckDie 0x9FDB20, IsOnlyDontMove 0x9FE304, IsCannotMove 0x9FC404, PlayHiddenTween 0xA0B614, and SetGameObjectLSSActive 0x9B7588/callback 0x9C0614. Raw native exports stay in the local reverse-engineering directory.

## Rules and ordering

IsCannotMove first runs CheckDie. For each hidden rod, CheckDie counts colors on unfinished, nonempty, unmasked rods. Locked rods, DontMove rods and hidden nuts still contribute; completed rods, color masks and hidden rods do not. If any color occurs at least four times, that hidden rod stays closed. Otherwise every mask entry on it has IsShow cleared, followed immediately by the hidden-break request and a whole-user SaveData for that entry. Obj.IsShow is not changed. The algorithm then restarts from the first rod and recounts; newly revealed colors can prevent subsequent reveals.

The native recursion is implemented as a restart of the loop, with reused dictionary and top-group buffers. This retains reveal/save order and avoids stack growth and repeated temporary collections. It is an explicit lifecycle operation, not per-frame board polling.

If every nonempty unfinished rod is DontMove, IsCannotMove returns true, even if an empty rod exists. Otherwise source rods exclude masks, locks, nonpositive capacity and DontMove; accessible emptiness returns false. Destinations exclude masks, locks and nonpositive capacity, but can be DontMove. A same-color match counts as available only when the entire source top group fits. This differs from actual input operations, which allow a partial transfer. Neither completed status nor the animation interaction gate is added to this predicate.

The level view exposes this stateful check and routes each reveal to its native screw-type view. The game scene subscribes to its save requests using the same whole-user save implementation as gameplay operations. No move attempt is added by a reveal, and no automatic lifecycle call is added ahead of the still-pending higher-priority guides.

## Visual timing and limits

The original requests non-looping `posui` on the hidden skeletal target, then deactivates its hidden root after one second. The delay uses DOVirtual.DelayedCall with ignoreTimeScale=true, so the replacement timer uses unscaledDeltaTime. Duration is stored in OriginalScrew.asset. Multiple requests retain the earliest pending deactivation; pooled release cancels pending work before reuse.

The existing animation-request contract now receives `posui`, and the native root deactivates at its deadline. **The skeletal clip itself is not restored or visually validated.** An inactive object or a recorded clip request is not evidence of the original fracture animation.

## Integration still required

The main initialization dispatcher remains unbound while its record/newbie/assessment/reward panels and unlock consumers are being restored. Its deadlock action can now use the level-view check. Fail's delayed panel branch and post-move lifecycle calls still need recovery; this change does not establish a complete win/fail/tutorial loop. SDK behavior is unchanged.
