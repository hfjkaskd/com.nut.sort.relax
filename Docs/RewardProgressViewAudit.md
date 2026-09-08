# Reward progress view recovery

The current source MainPanel's TX_JD subtree is restored under Top, with its original inactive state, eight transforms, five sprite assets, horizontal filled Image, three TMP labels, font materials and autosizing. The source three-second clockwise glow uses an authored native local-axis rotation component, with its duration and rotation stored in the prefab. It uses scaled time and retains its phase across hidden intervals without updating an inactive hierarchy. No DOTween assembly or serialized UnityEvent calls were introduced.

Behavior evidence: TX_JD.Refresh 0x9DB804, Init 0x9DF70C, Show 0x9E17BC and Hide 0x9E17EC. The source jump table at 0x20178C8 was decoded to distinguish stage 0 from the default rank branch. RewardDetailInfo.Stage2StartShowLevel at offset 0x48 is now included in the explicit read-only reward projection; the original saved document remains unchanged.

Refresh always reads the third reward record, even at early levels. Level <= Stage2RealLevel hides the object, clears IsShow and returns without rewriting text. Later levels set IsShow, write original text 160 to GoldValue, and obtain the stage description. RemoveNewline replaces LF with a space; the subsequent case-sensitive replacement changes FF0000 to a3ff8a. No broad whitespace or color normalization is added.

| Stage | Progress and guide behavior |
| --- | --- |
| 0 | Hide while retaining IsShow and previously assigned heading/description |
| 4 | Gold / target currency; before IsGuideGoldComplete, hide and replace with Stage2StartShowLevel / itself and text 24 |
| 5 | TodayPassLevelCount / cal_cfg; before IsGuideGoldTargetComplete, replace with gold / target currency and text 24 |
| 6 | LoginDay / caliper_logs; same pending-target-guide substitution as stage 5 |
| Default, including 7 | UserLevel / caliper_rank |

Refresh does not force activation in the normal branches. Show only activates when IsShow is true. Hide and Init deactivate without clearing IsShow. Text numerators are not clamped; the standard Image applies its own fill clamp. The original zero-over-zero guide fraction is retained, including NaN fill for a zero Stage2StartShowLevel. This is not replaced with a guessed full progress value.

Validation: all 27 Unity regression groups pass. Added checks cover source hierarchy, fill mode, resource references, inclusive level boundary, independent eligibility/visibility, hidden refreshes, stage 4/5/6 guide substitutions, daily/login/rank numerators, no text clamp, and zero denominator behavior. Real Play passes the actual restored banner callback into this view's Show method, then checks rendered content, glow rotation, scaled pause and phase continuation after hiding. The latest local 480x854 capture is Library/ValidationCaptures/reward-progress.png. It was visually inspected for missing resources, text placement and fill rendering. PlayerPrefs were restored.

**Production reward flow remains incomplete.** The view exists in the production MainPanel prefab but awaits its original Top refresh and initialization host calls. The Play fixture uses isolated synthetic reward values and a temporary destination; it does not create a production reward configuration or grant funds. GoldItem, full main initialization, subsequent guides, region AB/GM and original-device comparison remain pending. SDK behavior is unchanged. Current target rendering and branch tests do not prove complete 1:1 lifecycle or cross-device visual fidelity.
