# Success claim response audit

Restored SuccessPanel.GetReward (9E8A6C) and its response callback (9E8D2C) as OriginalSuccessRewardFlow. Raw assembly remains local. The full SuccessPanel/RewardPanel visual host and inherited ad/reward callbacks are still pending; this does not establish complete lifecycle or visual parity.

GetReward forwards the requested isMore value to the supplied request boundary and captures that value separately for each response. It does not display or close a panel before a response. The request implementation, SDK behavior and ClearanceRewardGetS2C.InitLss processing are unchanged and are not simulated here.

The response creates a new ItemGetInfo and ordered item list. Nonempty kinetic_data.hg_amt creates cash with Count and MoreCount independently parsed from hg_amt; CurrentCount comes from hg_psi. Nonempty hg_zs_amt creates coins with Count and MoreCount from hg_zs_amt; DoubleCurrentCount comes directly from hg_zs_psi. Other fields retain native defaults. Both amounts use the existing native float parser. Coin balance uses double.TryParse under the current culture, with the original ToDouble fail s: diagnostic and zero on failure (9BBFA8).

Unlike pre-claim settlement construction, the literal coin string zero is retained. Empty amounts suppress their entries and avoid parsing corresponding balances; an empty list still opens the reward panel. Whitespace/invalid nonempty values remain entries and follow native parse-failure handling. Aggregate ItemGetInfo.IsMore is the captured request flag; per-item IsMore remains false. CallBack is not synthesized.

ShowPanel(8, info) runs before closing the current SuccessPanel. A show failure leaves close uncalled. Repeated responses are not silently deduplicated. This UI callback does not grant balances, save user data, or advance the board. OriginalGameScene.CreateSuccessReward provides the scene composition entry point with explicit request, panel and close dependencies.

Unity 2022.3.62f3 full regression: 105 PASS markers in Library/unity-success-reward.log, including overlapping request capture, response order, zero/empty amounts, float-versus-double precision, current-culture parsing, malformed response and display failure boundaries.

OriginalSuccessRewardPlayValidation runs the actual scene and its adapter, uses an explicit response fixture, and passes the result to the restored reward prefab factory under the scene UICanvas. It verifies cash and zero-coin labels/icons, preserved double balance, delayed presentation, and show-before-close. Panel 8 is an explicit consumer; the complete original RewardPanel hierarchy, transition and lifecycle are not yet restored. User preferences are restored after validation. Existing scene destruction pool-parent warnings remain; no Editor-only runtime workaround is used.

Next main-path work: restore actual RewardGetPanel/SuccessPanel/RewardPanel host composition, button actions and animation-driven transitions and connect production startup. SDK functionality remains excluded per user instruction.
