# Marquee text recovery

OriginalMarqueeText restores the text branch of PMDItem.Init (ARM64 RVA 0x9B914C) against the current reverse-source binary. It uses the level captured by the caller, rather than reading the live user level again.

For captured levels greater than two, IsGold selects text 35 or 153. The display-name provider runs before the item's random amount sample, followed by the shared cash formatter and original localized text lookup. Both item types use cash formatting in this source path.

For levels at or below two, the amount comes from GoldRewardTargetS2CData.bear_list: index zero only for level one, otherwise index one. psi_value is parsed before requesting the name; text 35 is used without sampling or inspecting the marquee item. The original reward document remains unchanged. Missing data is not replaced with fabricated reward values.

Validation: 34 Unity regression PASS markers, including NUT_MARQUEE_TEXT_VALIDATION_PASS and NUT_CONTENT_VALIDATION_PASS, in the local reverse-workspace log reconstruction-nut/logs/unity-marquee-text-validation.log. Cases cover both text IDs, localization, captured versus live level, early reward row selection, null item handling in early levels, callback/random order, missing reward data and unchanged JSON. The preference backup is absent after restoration.

This is a text component prerequisite. The original display-name generator still requires recovery and is supplied explicitly; the fixed name exists only in validation. PMDItem prefab/icon/width timing, PMDBullet presentation and production Top binding remain incomplete. No SDK behavior or visual assets changed in this round, and no device visual parity is claimed.
