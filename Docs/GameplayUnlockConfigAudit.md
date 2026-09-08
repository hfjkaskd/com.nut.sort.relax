# Gameplay unlock configuration binding

ServerConfigData constructor 0x9F28AC initializes LSSGPUL with a four-int metadata array: field C54C7ECE... at metadata offset 0x49C1C0 decodes to [8,21,61,111]. These values are now serialized in OriginalSceneSession, not hardcoded in runtime logic.

OriginalGameplayUnlockConfig reads the existing user ServerConfigData document. Missing field retains constructor defaults; explicit null stays null, empty stays empty, and a missing whole config object throws. Explicit field matching avoids reflection and follows existing case-insensitive document handling. The scene now exposes NewGameplayUnlock, using actual user state, configuration and the restored decision with typed panel dispatch.

Validation exercises defaults, null/empty distinction, case/order and replacement configuration; the actual scene opens unlock index zero at real level four, skips after config replacement, and matches the new threshold at real level sixteen.

Full unlock panel and automatic initialization action binding remain incomplete. No SDK behavior or server response is fabricated.

Unity 2022.3.62f3: full content and actual-scene unlock configuration PASS in Library/unity-unlock-config-validation.log; no compiler or validation exceptions.
