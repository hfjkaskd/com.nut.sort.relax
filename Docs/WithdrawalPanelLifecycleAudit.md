# TXPanel lifecycle

Native entries: InitLssPanel 0x9cb9e0, TweenEndRefreshPanel 0x9cd5bc, close-button closure 0x9cd794.

Initialization first calls BaseLSSPanel.Init. A nonempty input array supplies its first boxed int as the captured level; an empty array captures the then-current UserLocalData.Level. Null array/null first element/wrong boxed type are not defaulted or coerced. TXBtn is bound before Close, both through the original animated Click listener wrapper. OriginalWithdrawalPanelFlow exposes these view binding operations explicitly without static UI generation.

TweenEndRefreshPanel reads live GuideIndex and opens NewbieGuidePanel (7) only for indices 2, 11, 13 and 15. It does not call a base method, mutate index, save or suppress repeated invocation.

Close-button callback first reads shared TXProgress0Panel.IsShowOnlineTimeHint. If true, it clears the flag before opening TXCheckHintPanel (30). Otherwise it tests the panel's _isShowTargetHint and opens TXTargetHintPanel (31) when true. Only when neither hint applies does it invoke NewbieGuidePanel.CallbackActionInvoke(null). All successful branches then call BaseLSSPanel.CloseLssPanel. Dispatch or callback exceptions prevent close; flag reset is not rolled back. The shared flag and panel target-hint state remain supplied by the owning UI.

Validation covers argument capture/order/type failures, all adjacent guide indices, every hint combination, global callback retention/clearing and failure ordering. No SDK implementation changed. GoldGetCallback (0x9c94e0), full Refresh (0x9cbb8c), actual TXPanel prefab/host and production binding remain pending; this is not complete withdrawal behavior or visual parity.

Verification: Unity 2022.3.62f3 full regression completed with 112 PASS markers including NUT_WITHDRAWAL_PANEL_FLOW_VALIDATION_PASS and NUT_CONTENT_VALIDATION_PASS in Library/withdrawal-panel-lifecycle-validation.log. No compiler errors or exception entries in the successful log. No visual assets changed, so no visual parity claim is made.
