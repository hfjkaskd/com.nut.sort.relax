# Currency reward world-flight audit

Restored UILSSUtil.FlyGoldWordPos (9BDF08), FlyCoinWorldPos (9BEB10), their return callbacks (9C0650/9C0688), ItemMgr.FlyItem dispatch (9F649C) and ShowStarEffect (9F5328). Native evidence remains local. Screen-position wrappers and full default startup remain pending.

## Native behavior

ItemMgr routes tools 2–4 to the restored tool flight. Cash and coins use the source Image world position, six instances, audio enabled with zero audio delay, and scales 1 / 0.699999988079071 respectively. Unknown types return without dereferencing the source or invoking the callback.

Each currency icon is rented from Prefabs/Items/Gold or Coin and parented to TopCanvas with reset local rotation. Its local scale is assigned uniformly. Random.insideUnitCircle is normalized using Unity's original epsilon (approximately 1e-5); the ring offset is radius times requested scale, preserving world Z. Cash radius is 0.8, coins 0.6. This is a normalized ring, not uniform disk placement. One random sample is consumed per instance.

Each icon gets a pooled StarEffect at local zero and scale 100. Destination world position is captured from the cash/coin HUD icon for each instance. The movement starts after 0.5+i*0.1 seconds and lasts 0.5 seconds, using the exported default OutQuad ease. Start position is read when the tween begins. Completion returns the icon to the existing pool without a second inventory grant or invented HUD-refresh event. The star's independent pool lifetime is delay+0.6, preserving the original extra 0.1-second lease after the icon returns.

After instance creation, the supplied callback is scheduled for 0.5 seconds independently of all arrivals, including count<=0. Audio is optionally scheduled as Reward_fly with the passed audio delay. The source timings/radii/scales/count/resource paths are stored on CurrencyRewardFlight's prefab. The implementation uses the scene's existing animation driver and prefab pool, no reflection or third-party runtime assembly. All hot-loop transforms are cached; allocations occur on low-frequency dispatch and resources are reused.

## Restored resources

Gold and Coin keep all non-script serialized blocks equal to the current typed source export. Their Images use official Unity UI. GoldHelp's Start-only country sprite initialization maps to the existing OriginalGoldImage, bound before Start; pooled reuse does not artificially rerun Start. Existing source US1/Coin sprite resources are reused. StarEffect is copied unchanged and has one native ParticleSystem with the already-restored material; no runtime particle topology is constructed.

- Resources/prefabs/effects/StarEffect.prefab: a9f16f54bded8e2509dbadfc90cc6a9f42c63d86a383f4a78ae78db54b4baf49
- Resources/Audio/Reward_fly.ogg: 266d43c6bfb9a5e5a9dc971b8518f4e51e208a98c4b2422a5d429ad8c9c978af

## Verification and limits

Unity 2022.3.62f3 full regression: 108 PASS markers in Library/unity-currency-flight.log. Tests verify both normalized ring radii, scale and world Z, source resources, independent callbacks/audio, staggered interpolation, destination capture, item versus star return times, reuse and count/unknown-type short circuits.

OriginalCurrencyRewardFlightPlayValidation composes the actual claim-response adapter, RewardPanel host, MainPanelTop targets, scene pool and audio player. An explicit response/balance-consumer fixture generates six cash and six coin icons plus twelve star effects. The real RewardPanel closes while flights continue, and every item/star lease returns afterward. Preference data is restored. Successful runtime verification reports no C# compiler error or runtime exception; existing scene-shutdown pool-parent warnings remain.

Latest capture Library/ValidationCaptures/currency-reward-flight-current.png was inspected. Currency icons and star trails are visible travelling toward the actual cash/coin target icons. The HUD was bound but not fully initialized for this isolated fixture and retains serialized sample labels/counts; this is explicitly not evidence of complete HUD fidelity or production main flow. No old screenshots were used.

Remaining main-path work: fully compose SuccessPanel/RewardGetPanel prefab/button/animation lifecycle, production reward host/balance/HUD wiring, screen-position currency wrappers where used, and default startup/remaining guide/event branches. SDK implementation remains unchanged. Full 1:1 visual and lifecycle parity is not achieved.
