# Newbie guide panel host

The source NewbieGuidePanel prefab has no child named main. Base CloseLssPanel (0x9C4F78) therefore takes its immediate HideLssPanel branch instead of creating a scale tween. NewbieGuidePanel.HideLssPanel (0x9E2F24) only kills its optional stored tweener, which is unassigned in the recovered methods; it does not invoke base hide actions or the delayed panel queue.

OriginalNewbieGuideHost now uses panel ID 7 and the existing registry's register-before-initialize-before-refresh lifecycle. It instantiates the configured prefab, binds teaching and close, localizes original label 1, initializes code Button listeners and cached user parameters, binds mask scheduling and the concrete branch adapter, then runs ShowGuide. Close removes the registration and destroys the panel immediately through the registry. It leaves the global callback intact, matching the original callback lifetime.

The original prefab's geometry is unchanged. A serialized label reference was added for the existing label_1 object. No static UI is generated and no Editor-only runtime fallback was introduced.

The static validation covers actual prefab registration, initialized teaching, cached guide index across refresh and no unrequested scheduling. The Play fixture covers current first/final teaching displays and immediate close with no base queue or global callback clearing. Initial validation incorrectly invoked runtime Destroy from edit mode; that check was moved to Play while runtime behavior stayed unchanged.

Production startup and screw-operation gate binding remain incomplete. The fixture deliberately supplies only teaching-required dependencies; it does not pretend missing success/withdrawal/request implementations exist. Native evidence remains local.

Verification: 89 PASS markers in Library/unity-guide-host-final-validation.log; NUT_NEWBIE_GUIDE_HOST_PLAY_PASS in Library/unity-guide-host-final-play.log, without compiler-error or exception markers. All 26 RectTransform YAML blocks match the source exactly. Latest 480x1040 guide-host-teach-0.png and guide-host-teach-3.png were inspected. The screenshot fixture was corrected to separate asynchronous capture from state changes by a frame. Preferences restored.
