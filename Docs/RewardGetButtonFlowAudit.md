# RewardGetPanel button dispatch

Original ARM64 entries: GetCallback 0x9e5554, MoreGetCallback 0x9e5768, completion handlers 0x9e5958 / 0x9e596c / 0x9e5980. ServerConfigData offsets 0x18/0x1c identify LSSLRFP/LSSLRFV; LuckyRewardGetPanel static offset 0 is LastGetTime.

Normal click increments NormalGetTimes unchecked before reading LSSLRFP. Below threshold it calls virtual GetReward(false). Otherwise it increments InterAdTimes and resets NormalGetTimes before reading LSSLRFV. An inclusive comparison selects interstitial; larger counts select rewarded with option false. Both completion handlers ignore the SDK boolean and invoke GetReward(true); there is no one-shot guard or save in this method.

More click resets NormalGetTimes immediately and requests rewarded with option false. Its completion clears LuckyScrewDoneCount, reads TimeSeconds (not LocalTimeSeconds), assigns shared LuckyRewardGetPanel.LastGetTime, then invokes GetReward(true). These effects do not happen while the SDK completion is pending.

OriginalRewardGetFlow exposes existing SDK, configuration, clock and shared timestamp boundaries as delegates. It supplies no fake ad completion, defaults, balance update or persistence. Existing SuccessPanel early-level dispatch can wrap its GetCallback; the existing success reward response adapter can consume its GetReward output.

Validation covers both thresholds, inclusive interstitial boundary, signed integer overflow, callback false/true and repetition, pending completion, mutation before configuration failure, lucky timestamp order, and early/later SuccessPanel composition. Fixtures do not call an SDK.

Remaining: restore and bind the actual SuccessPanel prefab, inherited button visuals, panel host and production configuration/time/SDK composition. This change alone does not enable production startup or establish visual parity.

Verification: local Unity 2022.3.62f3 full content regression passed, including the new NUT_REWARD_GET_FLOW_VALIDATION_PASS and final NUT_CONTENT_VALIDATION_PASS. No C# compiler errors or runtime exception entries in Library/reward-get-flow-validation.log.
