# Later withdrawal guide branches

ShowGuide shared cases 11, 13 and 15 (0x9E24AC) target TXPanel's original control. They enable the hand/Button, copy world position and RectTransform sizeDelta, hide the tip, install a click closure and replace the global callback. No ShowMask call appears in this branch.

Click closure 0x9E4750 invokes the bound TXPanel.GoldGetCallback, assigns GuideIndex exactly 19, closes the guide and saves. It does not increment the existing index. The standard listener clears the click action and plays audio after normal return.

Global callback 0x9E3D98 ignores its payload, calls ShowEveryDayGift, invokes the main panel's TweenEndRefreshPanel (virtual slot 8), then reads the panel's current ShowBanner flag. When true it calls TargetRewardBanner.Show with a non-null empty completion callback (0x9E4D98). This flag is read after refresh, including re-entry effects, rather than captured when the callback is bound.

OriginalNewbieGuideView.ShowWithdrawalStageGuide now implements these operations with typed boundaries, existing prefab controls and no SDK implementation or invented reward response. Production TXPanel GoldGet and main-panel bindings remain outstanding.

Validation instantiates the guide prefab for each index and both banner values. It checks gate rejection, get-before-index assignment, close/save/audio order, hidden tip and no mask scheduling, callback survival after guide destruction, daily/refresh/banner ordering, non-null empty banner completion, live flag changes during refresh, and failure before the guide-state mutation. No production Play or whole-screen visual parity is claimed for these unbound panels.

Remaining work is the production guide host and main/withdrawal panel integration, associated request response boundaries, and complete lifecycle verification. Native evidence stays local.

Unity 2022.3.62f3 regression: 86 PASS markers in Library/unity-withdrawal-stage-guide-validation.log; no compiler-error or exception markers; preferences restored.
