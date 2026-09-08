# Tool reward flight audit

Restored UILSSUtil.FlyItem (9BF100), move completion (9C06A4), empty scale completion (9C07F4), and inspected ItemMgr.FlyItem dispatch (9F649C). Local native assembly remains uncommitted. This implementation covers tool types through the source MainPanelBottom.GetOtherItem lookup; cash and coins use separate particle-flight branches and are not routed through this class.

The source resolves the current HUD tool before cloning. Missing tool targets return without instantiation or scheduling the optional callback. It instantiates the supplied Image under UIMgr.TopCanvas, retaining the configured component/children, sprite, material, rect and local scale, then assigns the source world position. The original icon remains in place.

Movement captures the HUD icon world destination and scaling captures that icon's localScale at dispatch. Both wait 0.5 seconds and run for 0.6000000238418579 seconds (native constant at 2017874). Source start values are sampled when the delayed animation begins. Exported DOTweenSettings.defaultEaseType=6 selects OutQuad. The native replacement updates world position/local scale under the existing scaled-time OriginalUIAnimationDriver. Timing parameters live in the ToolRewardFlight prefab; no runtime static UI construction or third-party tween assembly is used.

The caller callback is independently scheduled after 0.5 seconds, even when null. It is not the landing callback. At landing the clone is destroyed before obtaining the live bottom view and invoking RefreshOtherItem(type). This refreshes the count only; it does not modify stock, refresh all buttons, or emit another reward grant. The clone is parented outside RewardPanel so it survives the panel's earlier destruction.

Flight records are allocated only on low-frequency reward dispatch and removed on completion. Per-frame processing uses cached transforms, in-place list iteration and value-type interpolation, without LINQ, lookups or repeated UI hierarchy creation. The short-lived Image clone matches the original source behavior.

## Verification

Unity 2022.3.62f3 regression: 107 PASS markers in Library/unity-tool-flight.log. The added validation checks clone identity/configuration, source world position, captured destination/local scale, no movement through delay equality, independent callback timing, half-way OutQuad values, paused zero-delta behavior, live count refresh and missing-target short circuit.

The existing RewardPanel Play test now uses actual OriginalToolRewardFlight under the scene TopUICanvas and an actual MainPanelBottom prefab. Real inventory saves occur at collection; RewardPanel closes after its native delay while both top-level clones remain in motion; landing updates actual HUD count text before the queue continues. Latest screenshot Library/ValidationCaptures/tool-reward-flight-current.png was inspected: the reward strip is gone and the two tool icons are between the reward origin and their HUD destinations. The test uses explicit tool rewards and adds the restored bottom prefab for validation; it is not evidence of complete production startup. No older screenshots were used. Preference data is restored. Existing shutdown pool-parent warnings remain.

Remaining main-path work: recover cash/coin world-position particle flights and related target/audio/refresh handling, connect claim response to the real RewardPanel host, and finish SuccessPanel/RewardGetPanel prefab/button/animation composition and default lifecycle wiring. SDK handling is unchanged. Full 1:1 lifecycle/visual parity remains incomplete.
