# Guide branch composition

OriginalGuideBranchAdapter implements IOriginalGuideBranches and connects every active ShowGuide index to the recovered view method: success, target completion, withdrawal, coin, gold entry and later withdrawal stage. The view's teaching-first cached-index dispatcher remains authoritative.

IOriginalGuideUI explicitly separates the target, coin and entry GoldRewardInfo operations because the native closures bind different response handlers, even where their boolean arguments are equal. The adapter does not produce a server response or an SDK result. The production UI implementation must preserve those existing boundaries.

Success and withdrawal targets are returned together with their bound activation callback. This preserves the original lookup at display time and captured panel instance for subsequent clicks. It avoids resolving a newer panel when the player later clicks. Gold and coin targets are likewise obtained at branch display time; live refresh and initialization operations remain delegated to the UI context.

Validation goes through the actual prefab's ShowGuide entry, this adapter and the recovered branch methods for all active indices 0/1/2/3/10/11/12/13/14/15. It checks exact target/request/save/close/schedule traces and changes the context's activation callback after display to detect accidental late lookup. The fixture uses existing prefab Buttons and isolated UI observers. It does not substitute a production host or prove full game startup behavior.

Remaining work: implement the production UI context and panel host with actual success/withdrawal/main references and original lifecycle behavior, then verify full scene flows. Native evidence stays local.

Unity 2022.3.62f3 regression: 88 PASS markers in Library/unity-guide-adapter-validation.log; no compiler-error or exception markers; test preferences restored.

Corrected bindings: the success visual target is MoreGetBtn but its activation operation is GetCallback, represented explicitly by OriginalGuideSuccessBinding. RefreshMain selects MainPanel.RefreshLssPanel. Both identifications use the exported 0x138 vtable start.
