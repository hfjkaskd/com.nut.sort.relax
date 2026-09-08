# Withdrawal refresh header

TXPanel.RefreshLssPanel 0x9cbb8c, initial executable block 0x9cbce8-0x9cbfc8, selects gold using the panel's captured _level. Level one formats UserLocalData.Level1Gold without changing either stage title. Level two first sets Level2Title to text 30 with parameter 1, then formats Level2Gold. Every other integer first sets Level1Title to text 30/1 and Level2Title to text 30/2, then formats current Gold. Existing labels are not reset outside these branches.

After assigning GoldCount, it checks original argument-array nonemptiness and reads live UserMgr.ShowLevel. Main Title receives text 30 with ShowLevel minus one if any argument exists, otherwise ShowLevel. It does not derive this title from the captured panel level or parse argument contents. Subtraction is unchecked. The class keeps operation ordering, with stage titles before gold reads and gold assignment before argument validation/live title lookup.

OriginalWithdrawalHeader exposes these text operations through typed view actions and accepts the existing shared gold formatter. No static UI construction, new reward grant, SDK behavior or invented formatting is added. Validation covers first/second/other captured levels, exact stage title variants, live mutation between operations, array nonemptiness independent of contents, null-array failure timing and integer overflow.

Only the header block is restored here. The remaining original refresh includes pending gold application/tween, stage-dependent task and progress display and player information creation. Those, the actual TXPanel prefab and production binding remain incomplete. No visual assets changed and no screenshot/whole-view parity claim is made.

Verification: Unity 2022.3.62f3 full regression passed 114 markers including NUT_WITHDRAWAL_HEADER_VALIDATION_PASS and NUT_CONTENT_VALIDATION_PASS in Library/withdrawal-header-validation.log. Successful log contains no compiler errors or exception entries. Preference fixture restored with backup removed.
