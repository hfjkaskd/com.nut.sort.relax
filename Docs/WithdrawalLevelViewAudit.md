# TXLevelPanel source view and step animation

Restored panel 28 as OriginalWithdrawalLevelPanel/Host with the existing OriginalWithdrawalLevelFlow. The guide/confirmation Play fixture now opens the actual prefab instead of merely observing a panel-number dispatch. GoldGet1 remains an injected transport boundary, with no automatic response or production payment simulation.

## Resource and implementation evidence

The current typed TXLevelPanel prefab has 37 GameObjects and 142 serialized blocks. The reconstructed prefab retains all objects, all 66 RectTransform/CanvasRenderer blocks unchanged after line-ending normalization, and the source controller-free Animator block. Its 145 blocks add a configured step driver and visual feedback components for the two original standard Buttons. All click listeners are code-bound, with source gate/action/sound order.

Images, fonts, outlines, amounts, label IDs 37/93/81/90/91/92, coordinates, anchors, scales and hierarchy come from the current source prefab. Dependencies include original loading/done/line/title textures, blue-outline font material and background. No old screenshots or third-party DLLs were imported. UI/TMP scripts use the project's official Unity packages.

The source contains nested Title1, not a direct Title child. Only main receives the base panel open tween; the completed main tween stops writing at its own duration. Closing and delayed queue advancement retain the existing base configuration. Three loading rotations preserve the prefab's linear -360-degree local rotation over 3 seconds, repeating indefinitely. Three uniform linear scale tracks preserve their 0.7/0.9/0.7 delays, 0.2 durations and target 1. They do not restart on visibility changes.

DOTween components are replaced with native Unity animation drivers to satisfy the official-assembly requirement. This is an explicit implementation dependency change; recovered timing, scale constraints, callbacks and update behavior are retained. Static layout/resources remain prefab-configured.

## Native step sequence

PlayStepTween 0x9ca954 first sets Sure scale to zero without changing Button activation or interactability. Each numeric child 1..N starts row and Content at (1,0,1), hides an existing Done and enables Loading. Row ScaleY runs to 1 over 0.1 seconds after 0.3 + index*1.5 seconds. Its completion callback 0x9cb2ac creates Content ScaleY to 1 over 0.1 seconds after 1 second.

Content OnStart (0x9cb3b0), not OnComplete, shows an existing Done and hides Loading. Missing/destroyed Done leaves Loading enabled. The captured index is compared with the live Step child count minus one; the last content start creates Sure's uniform scale to 1 over 0.3 seconds. All these programmatic scale tweens use recovered default OutQuad. Callback-created tracks start on the next animation update; large deltas do not carry into newly created tracks.

Fresh Refresh calls add independent tracks without cancelling previous ones. Delayed tracks capture the current scale at startup; ScaleY preserves the live other axes. The shared scene driver advances scaled time and continues animating hidden owners, removing destroyed owners. No layout construction or per-frame hierarchy searches are used; numeric row lookup occurs only on refresh.

## Validation and limitations

Final full batch log Library/withdrawal-level-panel-final.log: 131 PASS markers, zero C# errors/exceptions, preference backup restored. It initially caught an incorrect call to the existing localization formatter; the panel now passes the level directly to OriginalTextCatalog.GetText.

Final Play log Library/success-guide-level-panel-final-play.log: PASS, zero C# errors/exceptions. Actual success/guide/loading/account form -> confirmation -> reenter -> resubmit -> GoldGet -> panel 28 -> final Button -> held GoldGet1 request -> animated removal. Balance stays unchanged and the guide callback is not fabricated. The current 480x1040 capture Library/ValidationCaptures/withdrawal-level-current.png was visually inspected: original title, $5 fixture amount, completed status rows and confirmation Button. The source's literal '1th withdrawal' wording is retained.

The first batch attempt encountered a user-opened instance of this same project; that verified instance was closed under the user's explicit authorization before testing. No other project was closed.

These are explicit fixture-composition and structural/visual checks, not production initialization or original-device pixel-diff proof. Country/AB production routing, later withdrawal panels/results, panel 22 and full lifecycle composition are incomplete. Existing exit-time pooled-object reparent warnings remain outside this change. SDK behavior remains unchanged.
