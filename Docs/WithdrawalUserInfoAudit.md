# Withdrawal user information behavior

UIName 20 is TXUserInfoPanel. Its current source prefab contains a direct main/Title, three channel selectors, four TMP input fields, Get/Close/ChannelInfo buttons. The view has not yet been restored into the reconstruction; this round restores its native behavioral controller and channel requirements for that integration.

## Evidence and restored order

- Init 0x9d48c0: base initialization, optional direct boxed-int argument assignment to captured level, Get/Close/ChannelInfo listener binding in that order. Empty arguments leave the captured level unchanged; initial field values are zero.
- Refresh 0x9d4a74: copy saved GetType if a user record exists; SetInfo; area text is plus followed by live country phone code; retrieve the live country's channels; set each configured selector state before visibility, icon and listener binding. Available icons use the full pay-channel artwork. A shared callback is added to each available selector per refresh, without removing older listeners.
- Selection callback 0x9d5914: ignores the event boolean, sets selected index to -1, scans the captured channel count, and calls SetInfo for every selected entry. The last selected entry wins; no selected entry leaves prior input requirements/visibility untouched.
- SetInfo 0x9d4ef0: retrieves country channels even when index is -1, then returns for -1. Otherwise resolves channel metadata, reads email/name requirements, copies saved Name/Email/Number in order, and activates email versus area/number fields. It does not toggle the name field or rewrite area text here. Selecting a channel can overwrite unsaved edits with saved values.
- ChannelInfo.IsEmail 0x9ea7f8: exact case-sensitive list membership of email. IsName 0x9ea860: name01 or name02, short-circuited. Null metadata lists retain their failure behavior.
- Get 0x9d51b0: required name length <= 1 gives tip 118; email mode length <= 5 gives tip 120, otherwise phone length <= 4 gives tip 119; only after input checks does a negative selected index give tip 122. No trimming, email regex, phone numeric check, or area-code concatenation is performed in this method.
- On valid Get: allocate UserLSSInfo if absent (native constructor 0x9c1f34 has no custom initialization); update Name, Number, Email, GetType, OtherInfo=empty, keeping unrelated state; show panel 21 with a new single captured-level argument array, initiate current panel close, then save. Show/close failures prevent later steps without rolling back prior field updates.
- ChannelInfo 0x9d5758: show panel 22 with captured level, keep the current panel open.
- Close callback 0x9d5888: initiate close, invoke the shared newbie guide completion with captured level, then clear TXUserInfoSurePanel.IsHintGoldGet. Its actual shared callback and flag are supplied by the owning view/services rather than invented here.

## Implementation and scope

OriginalWithdrawalUserInfoFlow composes existing OriginalUserLocalData, OriginalPayChannels and OriginalChannelInfos with an explicit UI contract. OriginalChannelInfo now exposes the two native requirement properties. Existing JSON-backed account data is updated in place, with existing case-insensitive field spelling retained and unrelated fields preserved. No network, payment, SDK, production payload or successful withdrawal is generated.

OriginalWithdrawalUserInfoValidation uses real source US channel ordering (Venmo, Zelle, PayPal) plus clearly scoped synthetic metadata for exact boundary cases. It covers the input priority, saved-field reload, repeated listener behavior, multi-selection scan, hidden channel availability, invalid argument type, in-place mutation, confirmation/help/close dispatch and show/close exception sequencing.

Library/withdrawal-user-info-validation.log: 125 PASS markers including final content PASS; no C# error or exception. Unity process exited, preference fixture restored and backup removed.

Still required: restore the actual current prefab and input components, bind the controller through real Buttons and the scene panel registry, preserve original visuals/motion, compose panel 20 into the actual loading/withdrawal chain, and restore panels 21/22 and remaining downstream branches. These controller tests do not prove source-device visual equality or production lifecycle completion. SDK boundaries remain unchanged.
