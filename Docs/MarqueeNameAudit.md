# Marquee display-name recovery

OriginalMarqueeName recovers UserMgr.GetPlayerName (0x9B9674) and the static alphabet initialization (0x9C2F94) from the current ARM64 source. The alphabet is A through Z in order; initialization metadata order is not array order. Both letters sample the same array.

Each call consumes five Unity integer Range draws, in order: [0,26), [0,26), [0,10), [0,10), [0,4). Given letters A/I and digits 2/9, variants 0 through 3 produce Player_A2I9, Player_AI29, Player_Ai29 and Player_A2i9. Only the second letter is lowercased for variants 2/3, using current-culture String.ToLower as in the binary. Turkish I therefore becomes dotless lowercase i. Names are generated display strings, not real account identities.

OriginalMarqueeText now has a production constructor using this generator; the injectable constructor remains available for deterministic validation. No Unity random state is reset by runtime code. No reward data or SDK behavior is changed.

Validation: 35 complete Unity PASS markers in reconstruction-nut/logs/unity-marquee-name-validation.log in the local reverse-source workspace. New checks cover all four permutations, exact bounds and five-call order, alphabet endpoints, Turkish casing, and 32 seeded comparisons against independent Unity draws including the next random sample. Validation restores culture and random state; the gameplay preference backup is absent after restoration.

Additional presentation evidence: PMDItem's delayed width callback (0x9B9B24) sets width to the current Tip sizeDelta.x + 140 and preserves item height. DelayCallback (0x9B9A80) starts a coroutine on LuoSiSort; MoveNext (0x9BD0E8) yields Unity WaitForSeconds, then invokes the callback. This is a scaled coroutine on the main game owner, not a local item update or unscaled delay. The exact delay constant and presentation component still need final recovery/binding.

PMD prefab/icon/launch motion and production Top binding remain incomplete. This round changes no visual assets and does not establish device visual parity.
