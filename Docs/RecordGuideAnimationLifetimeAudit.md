# Record guide animation ownership

OriginalRecordGuidePanel now advances through OriginalUIAnimationDriver rather than its own Update. It registers in Awake and unregisters on destruction, while retaining its existing owned-material cleanup. The original globally driven BaseLSSPanel scale tracks and TXRecordGuidePanel delayed Start track do not stop when the panel component is disabled or its GameObject is hidden. The existing serialized curves, durations, start delay, resource paths and Button binding remain unchanged.

The actual record-guide Play Mode fixture now disables the component during entry, hides the GameObject while the delayed Start track completes, restores visibility for a current screenshot and native Button raycast, and hides it again during close. It checks completion callback ordering and no added save point. This exercises global advancement rather than manually stepping the panel.

This fixes a concrete animation lifetime mismatch. The record guide still awaits complete production initialization-flow binding; this is not a claim of complete game parity. SDK handling is unchanged.

Native delayed Start evidence is TXRecordGuidePanel.InitLssPanel 0x9D1EA0 (DOScale and SetDelay at 0x9D2034/0x9D2040). Verified in Unity 2022.3.62f3: full content PASS in Library/unity-record-global-validation.log and extended record-guide Play PASS in Library/unity-record-global-play.log. Latest Library/ValidationCaptures/record-guide.png was visually inspected; Start is visible and the native Button raycast passed. Preferences restored; no compiler or validation exceptions.
