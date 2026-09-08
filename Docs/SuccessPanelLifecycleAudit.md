# SuccessPanel lifecycle branch audit

This round restores SuccessPanel-specific lifecycle and close routing. The complete RewardGetPanel/SuccessPanel visual host, button binding, reward response and production startup connection remain incomplete. No SDK implementation is added or changed.

## Evidence and behavior

Local native methods reviewed: InitLssPanel 9E78A4, RefreshLssPanel 9E7D94, TweenEndRefreshPanel 9E85E0, GetCallback 9E89C8 and HideLssPanel 9E7F90. Local raw assembly is not committed.

- Init invokes the base action first, then captures whether live user Level is 2 or 3. Level 2 writes LocalTimeSeconds to Level1TXTime; level 3 writes Level2TXTime. Other levels do not read the clock. Pass_level plays after those changes. No save or balance grant is performed here. The clock remains injected from the owner's existing time source.
- Refresh invokes base reward generation first. The native signed unchecked Level-1 <= 2 condition then centers the MoreGet label, applies text ID 1, hides its ad child and hides GetBtn, in that order. Later levels do not actively undo prior UI changes. Refresh actions remain explicit until the original prefab host is reconstructed.
- TweenEnd invokes the base lifecycle first, then uses the captured IsGuide flag, independent of subsequent Level changes. It writes GuideIndex=0 before requesting panel 7, with no invented one-shot guard.
- GetCallback evaluates live Level with the same signed comparison: early levels close directly, later levels delegate to the original base callback. It does not invent an ad result or reward grant.
- Hide calls base first. LSS260820 true immediately invokes InitLevel(true,true,false), bypassing the legacy progress checks. Otherwise only null/empty TXTargetGold queries IsCompletePassStage2Level. If complete, it invokes the existing synchronization boundary and returns. If not, a captured guide suppresses restart; non-guide panels restart. Literal zero and whitespace target strings count as nonempty. SDK/network synchronization remains an explicit supplied action, not a simulated completion.

OriginalGameScene.CreateSuccessPanelFlow binds the actual User and audio player. HideSuccessPanel binds existing OriginalRewardProgress and actual InitLevel. These interfaces do not automatically enable an incomplete production panel.

## Resources and verification

Restored the original 8,224-byte Pass_level.ogg and import metadata. SHA256 f3a6e1dbacd15b6855ca103d00806896767775d0f100422983180c2b1ec8acd3. Its path is configured in OriginalSceneSession, separately from the board's StageComplete sound; loading and playback use the current native prefab audio path and respect the live audio switch.

Full Unity 2022.3.62f3 regression: 104 PASS markers, including tests of base ordering, per-level timestamps, captured versus live state, exception ordering, signed comparison edges, all target/completion/guide Hide combinations and lazy progress reads. Final log: Library/unity-success-panel-flow-final.log.

Play validation uses the actual scene, scene flow factory, user state and AudioSource. It checks Pass_level starts after timestamp assignment, mute suppresses sound while preserving state mutations, guide capture survives Level changes, early callbacks close, and the new-mode Hide scene method enters the real InitLevel restart even with missing legacy reward data. Base lifecycle callbacks, clock value and panel consumer are explicit fixtures. This is not evidence that the original settlement screen or animation sequence is shown. Preferences are restored. Existing pool-parent warnings at scene destruction remain; no Editor-only runtime workaround is introduced.

Remaining main-path work includes actual SuccessPanel/RewardGetPanel prefab composition and lifecycle wiring, inherited reward/ad callbacks under the existing SDK boundary, clearance response-to-RewardPanel presentation, default startup and complete guide/event routing. Full 1:1 visual/lifecycle parity is not achieved.
