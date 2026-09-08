# Target completion return into the guide

The case-one global callback 0x9E38E4 ignores its object payload, invokes GoldRewardInfo(false) with the original response handler, increments the current GuideIndex, saves and closes the guide. It reads the live index after the request call returns, including synchronous re-entry changes. The response handler remains an external application boundary; no payload, SDK success or reward grant is fabricated.

The typed ShowTargetCompletion overload now binds this return behavior to the existing register/close/show-ID-35 handoff. OriginalGuideTargetCompletion already closes its own panel, marks record-guide completion, invokes InitDone(true,false), then dispatches the global callback. The new validation composes both implementations and checks that the first handoff does not request or save; target completion then reaches request, live-index increment, save and guide close in order.

Additional validation covers callback survival after guide object destruction, request failure before increment/save/second-close, and save failure after increment but before second-close. A failed global invocation retains its callback under the existing original dispatch semantics. The fixture supplies request observers, not a production response. Full production host/startup integration is still incomplete and this does not prove an end-to-end withdrawal flow.

Native evidence stays local.

Unity 2022.3.62f3 regression: 87 PASS markers in Library/unity-target-guide-return-validation.log; no compiler-error or exception markers; preferences restored.
