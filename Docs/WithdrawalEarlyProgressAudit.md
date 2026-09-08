# Withdrawal early-stage progress

Native TXPanel.Refresh blocks 0x9cc1e8-0x9cc570 and shared completion 0x9cc754-0x9cc80c. Stage one displays text 22 with captured level, computes count = level minus one for empty arguments or level for nonempty arguments, assigns Image fill count/level, numeric count/level, then text 25 with formatted remaining-count string. Stage two uses target five. It reads live ShowLevel and caps ShowLevel-1 at five for argument-bearing settlement entry. Empty arguments cause a second live ShowLevel read and a cap at four. No lower clamp is added to the count or numeric label; actual Image setter owns fill clamping.

ProgressTip x uses the current clamped Image fill * 800 - 400 and leaves y to the view. The shared block compares fill to the ELF float at 0x2017884, verified as 0.9990000128746033. If not less, it sets _isDoneTask true, replaces ProgressTipValue with text 23 and Tip with text 159. A below-threshold refresh does not reset the existing task-complete flag. Constants are stored in OriginalPanels configuration; runtime contains no fixed layout numbers.

OriginalWithdrawalEarlyProgress preserves ordered view operations and repeated live reads. Regression covers ordinary/settlement entry, four/five cap, negative displayed counts versus clamped fill, marker endpoints, completion replacement and retained flag. Later stages, full refresh composition and actual TXPanel prefab remain pending; no screenshot/whole-view parity claim is made.

Verification: Unity 2022.3.62f3 full regression passed 117 markers including NUT_WITHDRAWAL_EARLY_PROGRESS_VALIDATION_PASS and NUT_CONTENT_VALIDATION_PASS in Library/withdrawal-early-progress-validation.log. No compiler errors or exception entries in the successful log. Preference fixture restored and backup removed.

## Third stage follow-up

Native 0x9cc574-0x9cc750 and 0x9ccf9c-0x9cd124 distinguish input-array emptiness before any view change. Nonempty arguments resolve bear_list[2].StartLevel through the level table, read its ShowLevel, assign total/total fill and label, move marker and set Tip 159 before the shared completion block (which sets Tip 159 again after ProgressTip 23). Empty arguments read the last level's ShowLevel, set Tip 22 with that target, call current ShowLevel twice, and use the second result minus one capped at target minus one. It then uses the same remaining-label/marker/completion block as stages one/two. No invented lower cap or completion reset is added.

RefreshThird accepts branch-specific table providers, so an unused branch does not eagerly read unrelated data. Its tests verify the two live reads, preserved one-level gap, settlement table selection, exact repeated tip order and early null-array failure. Actual table-provider and complete view composition remain pending.

Third-stage verification: full Unity 2022.3.62f3 regression passed 117 markers in Library/withdrawal-third-progress-validation.log, with expanded NUT_WITHDRAWAL_EARLY_PROGRESS_VALIDATION_PASS and final NUT_CONTENT_VALIDATION_PASS. No compiler errors or exception entries; preference fixture restored.
