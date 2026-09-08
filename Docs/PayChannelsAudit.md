# Pay-channel lookup for marquee presentation

OriginalPayChannels restores PayTable.GetPayChannel (ARM64 0x9EA318) from the current reverse source, and OriginalPayChannel.RandomChannel restores 0x9EA758. OriginalTables exposes the parsed payChannels section from the existing, hash-validated pay.json archive. No payment SDK, transaction, network request or synthetic country configuration is added.

Null/empty explicit country invokes the supplied live-country provider; nonempty country does not invoke it. Each source-order row comparison applies current-culture ToUpper to the requested country. The first exact match returns the shared row. Unknown country logs GetPayChannel not country: followed by the code and returns null; it does not default to US or apply currency-country aliases. Null data retains native failure behavior.

Channel order is preserved for all 46 countries. RandomChannel uses one integer Unity Range(0,count), then indexes the list. A null selected string becomes string.Empty, confirmed by the native conditional-select at 0x9EA7DC-0x9EA7E0. An empty list still draws Range(0,0) and then fails index access. The PMDItem body inlines the same selection before loading Atlas/PaySimple/ plus that result. Resource loading and keeping the prior sprite on a missing result remain presentation work.

Validation uses the real archive to check all 46 country rows and channel orders, US Venmo/Zelle/PayPal order, lazy live-country lookup, shared first-match identity, integer selection bounds, null/empty channel distinctions and current-culture uppercase. SDK behavior is unchanged. Presentation prefab, launch motion, production Top binding and device visual parity remain incomplete.

Final validation: 36 Unity regression PASS markers, including NUT_PAY_CHANNELS_VALIDATION_PASS and NUT_CONTENT_VALIDATION_PASS, in reverse-workspace reconstruction-nut/logs/unity-pay-channels-validation-final.log. No compiler errors or validation exceptions; gameplay preference backup absent after restoration.
