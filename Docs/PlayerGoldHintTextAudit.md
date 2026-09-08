# PlayerGoldGetHint text branches

OriginalPlayerGoldHintText restores the distinct name and info consumers in PlayerGoldGetHint.Show (0x9DFFBC) and the text portion of ShowSelf (0x9E06D4). Separate methods allow the view to preserve icon/name/info assignment order and partial updates on failure; they do not combine the two labels into PMDItem's single marquee string.

PushName formats text 95 with the original generated display name. PushInfo reads the current user level when called, after name assignment in the original view. Above level two, cash uses text 96 and noncash uses text 152, with one value sample followed by the shared cash formatter for either type. Early levels ignore the marquee item's fields, read bear_list index zero only for level one (otherwise one), parse psi_value and use text 97. These are live-level semantics, unlike PMDBullet's captured level.

SelfName uses text 94 with no generated name or random draws. SelfInfo uses text 96 with the supplied float amount and cash formatter, independently of the current level. This does not implement ShowSelf's channel-info lookup or image assignment, which occur between the two text assignments in the native body.

41 complete Unity regression PASS markers in reverse-workspace reconstruction-nut/logs/unity-player-gold-hint-text-validation.log. New validation covers all text IDs, localization, both amount types, live early-level row selection, no early sampling, self amount/no generated name, missing early reward failure and unchanged JSON. No compiler/validation exceptions; preference backup absent after restoration.

Additional motion evidence for the next view implementation: ordinary Show and ShowSelf enter at local zero with a 0.5-second linear full-vector move. Completion starts the particle (ordinary Show permits a null effect, ShowSelf requires one), then schedules a 0.5-second move to (0,500,0) with a three-second delay and default tween easing. The separate Show(Action) overload uses a 0.5-second Y-only OutBack entry and a distinct completion callback. Those tracks, actual prefab and production lifecycle binding remain incomplete. SDK behavior and visual assets are unchanged in this round.
