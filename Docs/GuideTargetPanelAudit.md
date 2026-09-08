# Target completion panel reconstruction

## Source and restored structure

TXGuideTargetCompletePanel is panel ID 35. Its original prefab contains 14 GameObjects: 12 RectTransforms and two particle Transform nodes. All 12 RectTransform YAML blocks compare exactly with the current reverse project export. Original sprites, TMP font/material references, StarEffect and FireworksEffect are preserved. Restored sprite/texture dependencies retain source GUIDs. Large gold artwork loads through the original Atlas/Golds/{country}{type} resource path with type 3.

The Unity view binds the original standard Button in code, localizes labels 1 and 162, refreshes the live stage-selected amount with the existing currency formatter, and uses the existing original base-panel animation settings. Hand and glow loop components preserve the recovered animation settings; visual press feedback is attached to the same Button object.

The typed host registers before Initialize/Refresh and removes the panel after closing. Hide schedules the existing action queue after 2.5 seconds. Continue delegates to the previously recovered close, record-guide flag, initialization(true,false), then global guide callback(null) sequence. Click audio follows that sequence. No reward assignment, server payload, SDK success or save operation was added to this behavior.

## Verification

- Unity 2022.3.62f3 full content regression: 79 PASS markers; final log Library/unity-guide-target-panel-final-validation.log.
- Actual Play fixture: Library/unity-guide-target-panel-play.log reports NUT_GUIDE_TARGET_PANEL_PLAY_PASS. It verifies live prefab text, rejected/accepted Button gate, close/flag/init/callback/audio order, animated host removal, and one delayed queued action.
- Latest capture Library/ValidationCaptures/guide-target-complete.png was inspected at 480x1040: original title, amount, currency artwork, hand, button and particles render. The $12 amount is explicitly isolated test data, not a production reward or server response. Test preferences are restored on exit.
- Initial test assertion incorrectly counted all 14 nodes as RectTransforms. Corrected to 12 RectTransforms plus two particle nodes, then reran the full regression successfully.

## Remaining scope

The host is ready for the corresponding guide request, but production startup and the complete NewbieGuide ShowGuide routing are not yet fully connected. The Play fixture supplies initialization observers, not a complete production lifecycle. Full country/AB switching, other guide branches and whole-game visual equivalence remain unverified. This round does not claim a completed 1:1 game.
