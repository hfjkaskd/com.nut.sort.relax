# Runtime unlock panel host

OriginalGameplayUnlockPanelHost now owns actual prefab instantiation, registered initialization/refresh and Hide/Destroy/removal through OriginalPanelRegistry. Show accepts typed index/banner arguments per invocation; the registry overload applies that initializer only after registration and still refreshes afterward. No reflection or dragged Button event is used. The legacy registry initialization path remains intact for existing callers.

The Play fixture now uses this runtime host instead of directly instantiating/initializing/refreshing/destroying the unlock panel. It checks registration, deregistration, four real Button paths, real target-banner animation, scene-owned delayed queue dispatch and restored preferences.

Verified with Unity 2022.3.62f3: full content PASS in Library/unity-unlock-host-validation.log and Play PASS in Library/unity-unlock-host-play.log; no compiler or validation exceptions. The host is a typed adapter for this panel, not yet the fully shared production UIMgr. Automatic InitDone invocation, true common initialization gate and complete MainPanel top consumers remain pending. SDK treatment is unchanged.
