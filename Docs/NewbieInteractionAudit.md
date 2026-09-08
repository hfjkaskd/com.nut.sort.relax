# Newbie guide interaction lifecycle

Evidence: InitLssPanel 0x9E1D78, Click 0x9E2F3C, close-button callback 0x9E38D8, constructor 0x9E38C8. ShowBanner defaults true. Initialization marks IsOpen true, applies a supplied optional banner argument (absence preserves the field), snapshots GuideIndex, hides the hollow mask and Close button, then installs the close/local/full Button callbacks through the common click gate.

OriginalNewbieGuideView.InitializeInteractions restores this sequence after the future base/ShowGuide setup. Both continue Buttons share ClickAction. A permitted click invokes the current action before setting the property to null, then plays sound. A replacement installed during the callback is also cleared. An exception prevents both clearing and following sound. Null actions still allow click sound. Close marks IsOpen false before calling the supplied close operation and then sound. Rebinding removes prior UnityEvent listeners; callbacks are bound in code.

All three existing standard Buttons now have prefab-configured OriginalButtonFeedback using the already recovered common press parameters. It only animates visuals; Button remains responsible for activation. Original RectTransform layout is unchanged.

Validation instantiates the current prefab and invokes its real Button events, testing denied/permitted gate, null/replaced/failing actions, captured guide and banner flags, reinitialization without duplicate listeners, and close ordering. Full regression produced 76 PASS markers in Library/unity-newbie-interaction-validation.log, with no compiler or validation exceptions; preferences restored.

Full ShowGuide branch setup, callback payload dispatch, panel-host close behavior and normal startup are still outstanding. InitializeInteractions is available for those callers, but production does not yet initialize this guide. No SDK request or reward handling was changed.
