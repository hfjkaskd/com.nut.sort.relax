# Loading update ownership

LoadingPanel.Update (0x9D8894) runs automatic progress only when showing and active, at 0.5 per scaled second capped at 0.9. SetState(false), 0x9D89FC, creates a global DOVirtual.Float to one over 0.5 seconds; setter 0x9D8B60 updates the visuals. Completion 0x9D8B68 schedules a 0.2-second TimeLSSUtil hide independently of the loading component's enabled/active state.

OriginalLoadingView now keeps automatic progress in its own Update and sends completion/hold advancement through the existing global driver. Disable/hidden state stops automatic progress but cannot strand completion. Existing manual Advance remains a combined deterministic entry point for validation; runtime does not call it and cannot double-step the tween. Original settings/assets are retained.

The actual scene loading validation checks disabled automatic progress, disabled and hidden completion, and a previously pending hide still affecting a reopened panel. Full source parity for overlapping multiple SetState(false) calls remains to be implemented: the current single completion track still replaces earlier completion tracks. This patch corrects ownership, not that independent issue. SDK handling remains unchanged.

Unity 2022.3.62f3: 65 regression PASS markers in Library/unity-loading-global-validation.log; actual startup/loading exit and main/continue/replay Button flow passed in Library/unity-loading-global-play.log. No compiler or validation exceptions; preference fixture restored.
