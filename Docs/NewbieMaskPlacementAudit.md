# Newbie guide mask placement

The current native ShowMask (0x9E3564) immediately reveals the hollow Graphic for a null or destroyed Unity target. That branch leaves the hole rectangle and cached bounds unchanged and schedules nothing.

For a live RectTransform it copies sizeDelta, then world position to MaskRect, refreshes the hollow bounds and schedules the reveal callback. The ELF constant at 0x2017858 is 0.2 seconds; the delay is configured on the actual prefab. Callback 0x9E3F84 only activates the hollow Graphic. No interpolation, copying of target rotation/scale, preliminary hiding or cancellation of earlier callbacks appears in this method.

OriginalNewbieGuideView now implements these operations through ShowMask and a typed scene scheduler binding. Geometry remains prefab-driven and the existing native-equivalent hollow mesh handles rendering. No third-party tween assembly, reflection, SDK behavior or generated static UI was introduced.

Validation covers world-space placement under a translated/scaled parent; sizeDelta rather than transformed bounds; unchanged rotation/scale; null and destroyed targets; reveal timing; independent pending callbacks; and geometry changes preceding a scheduling failure. The actual Play fixture binds OriginalGameScene.ScheduleDelay, targets the prefab's existing standard Button, then checks the rendered hollow mesh and subsequent teaching displays.

Full production ShowGuide routing and startup binding remain incomplete; restoring this operation does not establish whole-game 1:1 equivalence. Native evidence remains local and is not published.

Verification results: 80 PASS markers in Library/unity-newbie-mask-placement-validation.log; Play PASS in Library/unity-newbie-mask-placement-play.log; no exception/compiler-error markers in either log. Current 480x1040 captures newbie-mask-placed.png, newbie-teach-0.png and newbie-teach-3.png were inspected. The first capture intentionally precedes full guide text setup and contains the source placeholder; later teaching captures show localized text. Preference backup was cleared after restoration.
