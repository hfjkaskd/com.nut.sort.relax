# Independent loading completion requests

SetState(false), native 0x9D89FC, creates a new DOVirtual.Float on every call using the current value as its start. No previous tween is killed. Completion 0x9D8B68 arranges its own delayed hide. The old single completionPhase/start/elapsed state incorrectly replaced earlier work.

OriginalLoadingView now retains independent completion records, including captured start value, elapsed time and each post-completion hold. Global updates visit records in creation order. Completed holds are removed before hiding; other records continue even when the GameObject is hidden or later reopened. No per-frame allocations are introduced; the list only grows on low-frequency completion requests. Durations still come from the existing configuration.

The actual loading-view regression now overlaps two requests a quarter-second apart. It checks 0.9375 after the older track finishes and the newer reaches its midpoint, the older hide firing before the newer completion, continued hidden progress, and the newer hide affecting a reopened panel after its own full hold. This supersedes the overlapping-request limitation in LoadingUpdateOwnershipAudit.md.

Full initialization and guide/panel integration remain incomplete. SDK handling is unchanged.

Unity 2022.3.62f3 validation: 65 regression PASS markers in Library/unity-loading-concurrent-validation.log and actual startup/replay Play PASS in Library/unity-loading-concurrent-play.log. No compiler or validation exceptions; preference fixture restored.
