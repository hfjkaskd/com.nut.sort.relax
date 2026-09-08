# Other withdrawal channel: panel 22

Restored TXChannelPanel as OriginalWithdrawalChannelFlow, Panel and Host, using its current source prefab. The actual form/channel/confirmation/reenter chain is exercised in an explicit Play fixture. SDK, payment, transport and production startup composition are unchanged.

## Source behavior

Init 0x9c54dc calls base initialization, requires the boxed integer arguments[0], then binds Sure before Close. The close listener targets BaseLSSPanel.CloseLssPanel directly: no guide callback, no account-form hide and no save.

Refresh 0x9c5630 reads the live UserLssInfo each time and assigns its OtherInfo field (verified offset 0x30), or null if no account exists, to TMP_InputField.text. TMP normalizes the null text to empty. The existing reconstruction's JSON projection convention uses the last case-insensitive matching property while preserving its spelling on updates.

Sure 0x9c56d0 rejects string.Length <= 4 with tip 120, without mutation or panel transitions. The controller does not trim or add email/phone syntax checks. The original TMP prefab configuration remains intact, including content type 6, line type 0 and character limit 50; controller behavior and interactive input configuration are distinct. Programmatic fixture input intentionally exercises raw string preservation, including newlines.

For accepted input, update only OtherInfo on the existing account, preserving reference identity and all other properties. If absent, construct an account, assign OtherInfo and then install it on the user. Open confirmation panel 21 with a fresh array containing the captured level, initiate own close, then hide account panel 20. The entire function ends there: no SaveData or guide invocation is present. Show failures retain the earlier mutation but prevent close/hide; close failures prevent the account hide. The implementation preserves these sequential effects.

## Prefab and lifecycle

All 13 source GameObjects and all 24 RectTransform/CanvasRenderer blocks are unchanged after newline normalization. The source controller-free Animator is retained. The 55 source blocks become 57 through added visual feedback components on the two existing standard Buttons; events are bound in code. Image, TMP, TMP_InputField, RectMask2D and LayoutElement scripts reference official Unity packages, preserving serialized layout, font, mask and input settings. Original bg_10 sprite/texture dependencies are restored with their source GUIDs.

The base panel animation uses main and its direct Title child with the existing configured curves/delays. Each track stops writing when its own duration ends. Closing removes the registered panel and retains the configured delayed queue advancement. No static UI hierarchy is built in runtime code, and no third-party assembly was imported.

## Verification

Library/withdrawal-channel-validation.log: 132 PASS markers including the complete content runner, zero C# errors/exceptions, preference fixture backup restored. The new validation covers argument and binding order, live account refresh, length boundaries, untrimmed input, mutation identity, preserved unrelated fields, fresh argument arrays, failure ordering, actual TMP input, original object count, standard Buttons, click gates and animated close.

Library/withdrawal-channel-play.log: PASS, zero C# errors/exceptions, preferences restored. Actual account form Button -> channel panel -> Close retains form -> reopen -> invalid short text -> valid fixture -> OtherInfo confirmation -> Re-enter -> account form -> reopen channel restores text -> Close retains form. Fixture save/payment/guide callbacks throw if unexpectedly called; none was called. The confirmation uses its existing Other icon and bypasses country/area lookup for OtherInfo.

Current 480x1040 captures were visually inspected: Library/ValidationCaptures/withdrawal-channel-current.png and withdrawal-channel-confirmation-current.png. They show the recovered overlapping form/channel state and subsequent standalone confirmation with the fixture text and source icon. These are current-engine screenshots, not old comparison references or original-device pixel-diff proof.

Production startup, all country/AB lifecycle branches, later withdrawal stages/results and the complete 1:1 game remain incomplete. Existing exit-time pooled-object reparent warnings remain outside this change. This work does not assert a real withdrawal or production completion.
