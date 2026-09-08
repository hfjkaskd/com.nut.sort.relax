# Success-panel guide branch

Native ShowGuide case 0 (0x9E28E4) obtains SuccessPanel (ID 9), enables the guide hand and standard Button, places Pos at the source MoreGetBtn world position, copies the source button image sizeDelta to the guide button image, hides the tip with text ID -1 and assigns the click closure. It does not invoke ShowMask in this branch.

The click closure (0x9E3FB0) invokes virtual slot 12, RewardGetPanel.MoreGetCallback, then increments the current user's GuideIndex, closes the guide, calls SetMask(true,1.5,empty string) and schedules a callback after 0.5 seconds. That callback (0x9E487C) shows panel 7 with no arguments. There is no SaveData call in this closure. Failure before the increment prevents the later operations. The original AddLSSListener wrapper plays Click after successful continuation and clears ClickAction only after it returns.

OriginalNewbieGuideView.ShowSuccessGuide now implements the recovered view and continuation using typed supplied operations. Timing values are serialized on the original prefab. MoreGet remains an external operation; no advertising success, reward grant or SDK response is simulated. This is an implementation of the branch body, not a claim that full production ShowGuide dispatch or SuccessPanel registration already exists.

Unity 2022.3.62f3 regression: 81 PASS markers in Library/unity-newbie-success-guide-validation.log, with no compiler-error or exception markers. Test preferences were restored. The new validation instantiates the actual guide prefab and uses another existing configured Button as an explicit stand-in for SuccessPanel.MoreGetBtn. It checks world position, image size, hand/button/tip visibility, gate rejection, request/index/close/mask/schedule/audio order, request-failure retention, unchecked index overflow and delayed reopening after guide destruction. No new Play visual equivalence claim is made for the missing SuccessPanel integration.

Remaining main-flow work includes the SuccessPanel implementation, full cached-index ShowGuide routing, further withdrawal-guide branches and production initialization binding. Native assembly evidence stays local.
