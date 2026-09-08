# Withdrawal later-stage progress

Recovered TXPanel.Refresh block 0x9cc810–0x9cd48c. Before selecting a branch, copy UIMgr.MainPanel.Top.GoldItem.GoldHintText.text into Tip, then retrieve bear_list[2]. This is the rendered HUD string, not a freshly calculated description. The runtime adapter exposes the exact read/write boundary for later prefab binding.

- Stage 4: parse TXTargetGold once; Image fill is live Gold/target; both numeric label operands use GoldLSSFormat. ProgressTip text 27 receives formatted target minus a fresh Gold read. Move marker. With nonempty arguments, reread bear_list[2].Stage2StartShowLevel and overwrite fill and label with total/total, move marker again and set Tip 159.
- Stages 5 and 6: both parse TXTargetGold and reread/cache bear_list[2].caliper_logs. Fill, numeric label and text 28 remainder each read live LoginDay, with the cached denominator. TodayPassLevelCount does not supply displayed progress. Nonempty arguments then overwrite fill with Gold/target and numeric label with a boxed/raw Gold numerator and formatted target denominator, move marker and set Tip 159. The prior remaining-day hint persists unless shared completion replaces it.
- Other nonzero stages (normally 7): fill, label and text 29 remainder read current UserLevel and caliper_rank from the initially retrieved row. Field reads repeat on that same row; replacing the server document does not switch it mid-refresh. Arguments are not inspected.
- Stage 0 keeps existing numeric progress, fill and marker, but still retrieves the row and enters shared completion.

Shared completion reuses the existing serialized .999 threshold, retained IsDoneTask flag, text 23 and text 159. Then a nonempty TXTargetGold changes the claim caption to text 36 only if ComeOnGold is empty or IsPlayGoldTween is true, with native short-circuit ordering. No new completion reset or negative-count clamp is added. Progress marker configuration remains in OriginalPanels; runtime does not create UI.

The adapter reads only branch-consumed JSON fields rather than eagerly projecting unrelated server fields. Validation uses an actual Unity Image plus explicit fixture user/server data. It covers ordinary/settlement display differences, repeated live reads versus cached fields, row identity, zero-stage retained state, label/flag short circuits and null failure timing. Fixture callbacks are not a production network or SDK response.

Full TXPanel prefab, production binding and the shared PlayerInfo instantiation/tween-flag-reset tail remain pending. This change does not establish whole-game or screenshot parity. SDK handling is unchanged.

Verification: Unity 2022.3.62f3 full regression passed 118 markers, including NUT_WITHDRAWAL_LATER_PROGRESS_VALIDATION_PASS and final NUT_CONTENT_VALIDATION_PASS in Library/withdrawal-later-progress-validation.log. No compiler errors or exception entries in that log. Preference fixture backup removed after restoration. Existing pooled-object reparent warnings at shutdown remain outside this change.
