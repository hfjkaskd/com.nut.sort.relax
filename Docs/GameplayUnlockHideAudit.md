# Unlock panel hide queue

UnlockGameplayPanel.HideLssPanel (0x9E9580) delegates to BaseLSSPanel.HideLssPanel (0x9C5218). The base method invokes its optional hidden callback and then schedules the manager queue Dequeue after 2.5 seconds. This now exists on OriginalGameplayUnlockPanel with its delay serialized in the prefab.

The live panel fixture now uses Hide before Destroy. Each of the four actual Continue Button paths schedules a scene-owned callback; the final check waits for all four queue actions after their panels are destroyed. The scheduled delegate targets the action queue rather than the panel, so destruction does not cancel it.

This supersedes the missing Hide dispatch note in GameplayUnlockPanelAudit.md. Automatic production initialization/registry binding and actual target reward banner connection remain pending. SDK treatment and visual layout are unchanged.

Unity 2022.3.62f3: full content PASS in Library/unity-unlock-hide-validation.log and post-destruction queue Play PASS in Library/unity-unlock-hide-play.log. No compiler or validation exceptions; preferences restored.
