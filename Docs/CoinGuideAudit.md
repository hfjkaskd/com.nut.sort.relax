# Coin guide branch

Native ShowGuide case 3 (0x9E2C1C) targets the main top coin component: enable hand and standard Button, copy target world position and RectTransform sizeDelta, show text 127 at TipPos[0], and call ShowMask(target). It then sets IsShowCoin (offset 0x120), refreshes the coin component and saves. Only after those operations does it replace ClickAction and the global callback.

Click closure 0x9E3BF4 calls the existing GoldRewardInfo operation with false and its existing response binding, increments live GuideIndex, then closes the guide. It does not save again. Global callback 0x9E4BAC ignores its object parameter and invokes NewGameplayUnlock(false). It makes no direct reward assignment or initialization call.

OriginalNewbieGuideView.ShowCoinGuide now reproduces that branch with typed coin refresh, save, request and unlock boundaries. Existing ShowMask preserves delayed reveal and prefab geometry. No SDK implementation or fabricated request response was added.

The validation uses the actual guide prefab with its configured Button/Graphic. It checks tip 127, mask-before-flag ordering, flag-before-refresh/save, gated request-before-increment/close, no extra click save, delayed reveal, callback survival after guide destruction, unlock(false), and retention of old callbacks when save fails before callback replacement. Target geometry comes from an existing configured prefab control as an explicit fixture; actual main coin/production host binding remains incomplete.

Remaining scope: bind the branch to the production main coin view, restore later guide branches and complete startup/host wiring. This does not establish whole-game or visual 1:1 equivalence. Native evidence stays local.

Unity 2022.3.62f3 complete regression: 84 PASS markers in Library/unity-coin-guide-validation.log, no compiler-error or exception markers; preferences restored.
