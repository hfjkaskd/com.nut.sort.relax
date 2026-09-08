# Withdrawal account form view and host

Restored the current TXUserInfoPanel (UIName 20) source prefab and composed it with OriginalWithdrawalUserInfoFlow. No SDK or payment behavior was introduced.

## Structure and components

All 41 original GameObjects remain. All 73 RectTransform and CanvasRenderer serialized blocks are byte-equivalent after newline normalization. Existing input layout, anchors, positions, colors, labels 107–112, font/material references and artwork are retained. The direct main/Title opening animation uses existing OriginalPanels settings, independently of close tracks; close completion removes the host registration and Hide schedules the next panel after 2.5 seconds.

The user requires standard Buttons for clickable controls. Accordingly, the three original channel Toggle components were replaced with official Buttons on the same objects, preserving target graphics and selection checkmarks. Their original ToggleGroup component was removed. OriginalWithdrawalUserInfoPanel implements exclusive selection with the original allow-switch-off behavior, prior-selection callbacks before the newly selected callback, and the native unscaled 0.1-second checkmark fade. This is an explicit implementation difference required by the Button rule; it is not represented as unchanged source component identity. All three existing action Buttons use code-bound click gating, callback-then-sound order and existing native button feedback. No serialized persistent event callbacks are added.

All four inputs are official TMP_InputField. Their masking/layout scripts use official Unity components. The source LiberationSans font and fallback assets were remapped to official TMP_FontAsset, retaining source glyph/material/atlas data. Source plugin DLLs were not imported. Full pay-channel artwork (15 sprites) is available through the source resource path, loaded for displayed channels rather than serialized as all-country strong references.

## Behavior and chain

OriginalWithdrawalUserInfoHost registers panel 20 before binding/init/refresh and uses the shared registry for refresh/hide/destroy. The scene Play chain now receives the actual withdrawal callback's panel-20 dispatch and creates this real host/view. It then submits actual TMP input values via the PayPal Button and Confirm Button, dispatches panel 21 with the captured level, saves through OriginalGameScene.SaveUserData, and removes the form after its closing animation.

The withdrawal-guide callback stays installed during the form transition; no success payload is invented. Confirmation panel 21 remains a dispatch boundary. Initialization, transport and auxiliary services in this verification chain are explicit fixtures, not production integration.

## Verification

- Source comparison: 41 GameObjects and 73 identical RectTransform/CanvasRenderer blocks.
- Library/withdrawal-user-view-validation.log: 126 PASS markers, including actual prefab/font/input references, all six Buttons, no serialized events, full US icons, exclusive/cancel selection, gated submission, close and hide.
- Library/withdrawal-user-info-play.log: standalone Play PASS for empty-input rejection, actual PayPal selection, filled-input submission and animated close; no C# errors or exceptions.
- Current captures inspected: Library/ValidationCaptures/withdrawal-user-form-current.png and withdrawal-user-filled-current.png, both 480 x 1040. Default Venmo shows country code and number; PayPal shows email. Captures use the current reconstructed scene, never old screenshots. They are not proof of pixel equality to a currently running original device build.

Production startup composition, full panel-21/22 behaviors, remaining lifecycle branches and full country/AB lifecycle coverage are still pending. Existing exit-time pooled-object warnings remain outside this change.

Extended chain: Library/success-guide-user-info-play.log timed out; retained as a failed run (it also contains Android device-scanning warnings, but causation is not proven). Added phase/time/input-gate details to the validation timeout only. Library/success-guide-user-info-play-2.log then passed the real registered form, actual scene save and animated removal chain with no exception or C# error. No runtime gate or initialization bypass was added.

Final full regression: Library/withdrawal-user-view-final.log, 126 PASS markers including final content PASS, no C# errors or exceptions. Validation processes exited and the preference backup was removed after restoration.
