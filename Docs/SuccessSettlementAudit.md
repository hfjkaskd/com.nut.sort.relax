# Success settlement presentation data

Success b__2 at 0xA008B8 creates an ItemGetInfo with a fresh ItemInfos list, default false IsMore and null CallBack. It does not grant the listed amounts. The reconstruction uses OriginalItemGetInfo and existing OriginalItemInfo, preserving list order and defaults.

A nonempty kinetic_data.hg_amt creates gold (type 0): Count parses that string; MoreCount parses it again and multiplies by parsed bear_video. Parsing reuses the existing original-compatible current-culture float.TryParse helper, including its source error log and zero result. Empty/null hg_amt uses bear_list[0].psi_value for live user level two, or bear_list[1].psi_value for level three, writes Level1Gold/Level2Gold respectively, sets Count==MoreCount and calls the actual save before continuing. Other levels add no gold fallback.

After that save, the response data is read again for hg_zs_amt. Only null, empty or exact string "0" is skipped. Other strings (including "0.0") create coin (type 1) using float Count; MoreCount and other fields remain defaults. No numeric-zero filtering or automatic grant is added.

If panel 11 is open, source queues a callback retaining the already built ItemGetInfo. Otherwise it runs immediately. Both routes call CloseAllPanel before ShowPanel(9, info). The existing OriginalPanelActionQueue is reused by validation. OriginalGameScene.CreateSuccessSettlement supplies the real user and SaveUserData; panel lifecycle consumers remain explicit.

Remaining: rendered success panel, actual modal/panel host composition, transport/DoneEvent/default startup chain and later award callbacks are not completed by this builder. Opaque response JSON follows the project's existing representation without reflection. SDK handling is unchanged. Raw native evidence stays local.

Verification: Library/unity-success-settlement-verified.log contains 102 PASS markers including NUT_SUCCESS_SETTLEMENT_VALIDATION_PASS and terminal NUT_CONTENT_VALIDATION_PASS. Library/unity-success-settlement-play.log contains NUT_SUCCESS_SETTLEMENT_PLAY_PASS for real scene field/board save, existing queue release, retained built payload and unchanged balances/board. Final logs have no compiler-error or exception markers. Initial test-only JSON quoting and a missing namespace were corrected before verification. Preferences were restored and backup is absent. Existing application-exit reparent warnings remain. Panel consumers are explicit fixtures, not proof of a rendered settlement panel.
