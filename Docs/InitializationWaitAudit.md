# Initialization wait stage

Native predicate 0x9FFF64 compares UIMgr.MainPanel (offset 0x58) against Unity null. It does not check board/nut readiness, active state or IsInitDone. TimeLSSUtil Until coroutine 0x9BD200 first yields WaitUntil, then invokes its optional action. That action, 0x9FFC1C, schedules the next continuation after 1.5 scaled seconds.

OriginalInitializationWait expresses those two waits with native Unity WaitUntil/WaitForSeconds. OriginalGameScene.ScheduleAfterMainPanel owns the coroutine and uses the serialized MainPanelReadyDelay. Each call retains its own iterator and callback; no Editor-specific bypass or invented readiness condition is introduced.

Validation checks lazy first yield, null/inactive/destroyed/replacement object semantics, the second scaled-time yield and one-shot optional continuation. This restores the waiting stage only. The later initialization-routing change invokes it from every BeginInitialization when the UI lifecycle has been bound, followed by the state-dependent sync/InitDone continuation. Normal local startup still lacks the complete lifecycle binding. SDK handling is unchanged.

Unity 2022.3.62f3: full content PASS in Library/unity-initialization-wait-validation.log. Library/unity-initialization-wait-play.log passed two scene-owned waits with delayed provider availability, verifying each callback occurs only after the configured scaled delay, alongside the existing unlock/banner/queue route. No compiler or validation exceptions; preferences restored.
