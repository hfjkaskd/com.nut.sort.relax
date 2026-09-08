# First-level teaching flow

Native source: NewbieGuidePanel.SetTeach 0x9E2F70. UserLocalData offsets 0x2C/0x34 are Level/LevelSeed. Manager 0x32 is IsCanOperatorScrew; ServerConfigData 0x58 is LSS260820.

OriginalTeachingFlow restores the following order. At Level >= 2, disable screw operation, hide Teach and return false without touching other visuals or reading the skip configuration. At lower levels, enable operation, show Teach, hide Hand and the hollow mask, then update markers. A marker is filled when its index <= LevelSeed; marker count and seed are read during each iteration, including the terminating condition. Filled/unfilled markers correspond to the source dian1/dian sprite selection, delegated to the eventual prefab view.

The tip uses position zero. Seed > 2 selects text 151; otherwise the current seed plus 70 is used without clamping. Only after rendering does the source read LSS260820 and optionally close. This branch returns true even when closing. A missing required configuration must propagate its failure after the preceding display writes, not silently skip the teaching.

The runtime port requires a typed teaching view and a skip-configuration query. Validation covers native level and seed boundaries, inclusive markers, empty markers, negative values, live count/seed mutation, visual call order, special text and post-display configuration failure. No SDK behavior, save point, or scene hierarchy was added.

Full NewbieGuidePanel prefab, hollow-mask view, callbacks and production binding remain to be recovered. These tests establish SetTeach control semantics, not rendered or complete tutorial parity.

Unity 2022.3.62f3 validation: 73 full regression PASS markers in Library/unity-teaching-flow-validation.log, no compile or validation exceptions. Pure control-flow change; no visual capture is claimed.
