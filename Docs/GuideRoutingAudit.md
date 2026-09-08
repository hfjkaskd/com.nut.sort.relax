# ShowGuide entry and dispatch

The current ELF jump table at 0x20178D6 contains 16 unsigned halfword offsets relative to 0x9E21A0. The recovered mapping is 0 -> 0x9E28E4, 1 -> 0x9E2AE8, 2 -> 0x9E26E0, 3 -> 0x9E2C1C, 4 through 9 -> return, 10/12/14 -> 0x9E2234, and 11/13/15 -> 0x9E24AC. The unsigned range check also returns for negative indices and indices greater than 15.

ShowGuide first calls SetTeach and returns if teaching handled the display. Only afterward does it read the guide panel's cached initialization index. Therefore even invalid/no-op indices must reset teaching visibility and screw operation at higher levels; changing the user's live GuideIndex must not redirect an existing panel.

OriginalNewbieGuideView now exposes this entry with a cached OriginalTeachingFlow and typed IOriginalGuideBranches UI operations. BindGuide establishes dependencies; ShowGuide applies the original teaching-first dispatch. Missing branch operations are not silently substituted with successful SDK responses. Existing target/success/withdrawal branch bodies remain available for the production host to bind.

Validation uses the actual guide prefab. It checks all 16 source mappings, negative/overflow indices, cached index after live mutation, real level-one tip and teaching short-circuit, and dispatch into the existing ShowTargetCompletion method with ID 35 and level-minus-one. This establishes entry behavior, not completion of the remaining branch bodies or production host.

The remaining coin/gold/withdrawal-stage implementations and full production startup binding are still incomplete. No whole-game or new screenshot parity claim is made. Native evidence stays local.

Unity 2022.3.62f3 full regression: 83 PASS markers in Library/unity-guide-routing-validation.log; no compiler-error or exception markers. Test preferences were restored.
