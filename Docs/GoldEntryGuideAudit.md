# Gold entry guide cases 10, 12 and 14

Native shared branch 0x9E2234 targets the main gold component. It enables the hand/Button, copies target world position and RectTransform sizeDelta, assigns tip 126 at TipPos[0], overwrites it with 187 for cached index 14, calls ShowMask, then captures the panel index for its click closure. It does not replace the global guide callback.

Closure 0x9E44CC sets IsGuideGoldComplete for captured index 12 or IsGuideGoldTargetComplete for 14, clears IsGuideGold, calls the existing GoldRewardInfo boundary with true, increments live GuideIndex and closes. No additional save occurs. Native index-14 SDK analytics remain excluded under the user's SDK instruction. No reward or response is fabricated.

OriginalNewbieGuideView.ShowGoldEntryGuide implements this branch using the existing prefab references and typed request operation. The request operation must preserve its existing response binding when the production host is connected.

Validation instantiates the actual guide prefab for all three indices. It checks current localized tips, delayed hollow Graphic reveal, untouched global callback, click gate, completion flags derived from the cached index after live index mutation, request-before-increment/close, and failure after flag mutation but before increment/close/listener clearing. Main gold target binding and production routing remain incomplete; no new full-screen visual parity claim is made.

Remaining work includes later withdrawal-stage branch bodies and production startup/host bindings. Native evidence remains local.

Unity 2022.3.62f3 complete regression: 85 PASS markers in Library/unity-gold-entry-guide-validation.log; no compiler-error or exception markers; preferences restored.
