# RewardPanel reconstruction audit

Restored the actual RewardPanel prefab and a registry-backed host for panel 8. This advances the post-claim main flow; full SuccessPanel/RewardGetPanel composition, flight visuals and production startup are still incomplete.

## Original evidence

Reviewed RewardPanel Init 9E5A88, Refresh 9E5B58, GetReward 9E5C68, delayed continuation 9E5E5C, IsCustomAlpha 9E5A80, BaseLSSPanel Init/Close/Hide 9C4A78/9C4F78/9C5218. Raw native evidence remains local.

The source prefab has exactly three GameObjects: RewardPanel, Image and Items. It has no main child and no label_* text descendants, so base initialization has no main scale tween or label pass to apply. IsCustomAlpha is true: the root Image retains its serialized black alpha 1/255. Close has no main tween and immediately delegates to registry Hide. The backdrop is a fixed 1080 by 324 image. Items keeps the original RectTransform, centered HorizontalLayoutGroup and spacing 155. All 8 non-script YAML blocks compare equal to the current typed source export. UI DLL script references are mapped to official Unity UI components; the original RewardPanel script is replaced by its reconstructed component. No static hierarchy is generated in code.

Restored bg_4 sprite and texture dependency unchanged. Texture SHA256: 725b25adeafa1c651cb3d1dd7d5831b78fe5d96c6c8045ed4fa9afa2bc281eea. Existing Item prefabs and lazy-loaded icons/fonts are reused.

## Lifecycle

Init retains the supplied ItemGetInfo. Refresh forces aggregate IsMore=true, then generates small Item entries with isShowMax=true, retains the resulting view list, and schedules collection after one second. It does not clear old entries or propagate the aggregate flag to individual items.

At collection, each current view is processed in order: existing OriginalItemManager.Add(item,true), then FlyItem(type, actual icon Image). The source's optional fly callback is null; the adapter accepts no synthetic completion. After the whole loop succeeds it schedules Close after 0.5 seconds. A failure leaves earlier inventory mutations intact and does not schedule close. Repeated Refresh calls append entries and each delayed callback reads the latest stored view list, without invented deduplication or cancellation.

Hide invokes the optional base hidden callback, then schedules the existing panel action queue after 2.5 seconds. The registry calls Hide before destroying/unregistering the panel. The host requires a supplied flight implementation; no no-op flight is installed for production. Inventory updates use the established ItemManager semantics, including server current balances for cash/coins and saved increments for tools.

## Verification and limitations

Full Unity 2022.3.62f3 regression: 106 PASS markers in Library/unity-reward-panel.log. Tests cover prefab/script/resource integrity, custom alpha/layout, aggregate flag, small item generation, actual ItemManager balance calls, grant/fly order, delayed close/queue, repeated Refresh and exception ordering.

OriginalRewardPanelPlayValidation runs the actual scene and registry host with an explicit two-tool reward fixture. It verifies real one-second collection, live source icons at fly dispatch, actual scene inventory saves, 0.5-second close/destruction and subsequent 2.5-second queue continuation. Latest capture Library/ValidationCaptures/reward-panel-current.png was visually inspected: the source blue/yellow reward strip, two icons, glow and counts render with the restored layout. No older screenshots were used. The test uses an explicit flight consumer, so it does not verify reward travel to the HUD. The underlying startup remains partial; this screenshot does not prove full lifecycle parity. Preference fixtures restore saved data; existing scene shutdown pool-parent warnings remain.

Next work: implement native ItemMgr.FlyItem and its target/refresh branches, wire the actual claim response to the RewardPanel host, and finish SuccessPanel's prefab/button/animation lifecycle. SDK handling remains unchanged.
