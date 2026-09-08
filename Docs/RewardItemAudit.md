# Reward item reconstruction audit

This round restores the reward entry required by SuccessPanel. It does not restore or activate the complete settlement panel or claim full lifecycle/visual parity.

## Local native evidence

Reviewed UI.Item.Init (9C453C), ItemMgr.SetImage (9F61F0), GoldHelp.SetGold/GetGoldSprite (9C41DC/9C421C), ItemInfo.FloatCount/DoubleCount (9F8FB0/9F8FCC), ItemGetInfo.GenerateItem (9F8A98), and ResourceMgr.InstanceGameObject/GetSprite (9F5A74/9F63A4). Raw native evidence stays outside this repository.

- Init first retains the item reference, assigns the icon and invokes SetNativeSize, then formats type 0 as cash, type 1 as coins, all others as x{integer}. DoubleCount widens the selected float; it does not read DoubleCurrentCount. The item's own IsMore selects Count/MoreCount.
- Both Max branches hide the object. The isShowMax argument is unused by this native build.
- Cash uses GoldCode country aliases and image variant 1/3. Only big coins use Coin2; tool images do not change with size. Explicit serialized names replace enum.ToString resource keys for Obfuz compatibility. Unknown integers retain the numeric resource path and missing-resource diagnostic.
- Generation appends an instance for each entry, including zero amounts, retaining order and item identity. It neither clears the parent, propagates aggregate IsMore, grants balances, nor invokes CallBack. Missing prefabs preserve the log-then-component-lookup failure.

## Prefab restoration

Restored Resources/prefabs/items/Item and Item_Big from the current typed resource export. All non-script YAML blocks compare equal to the source (11/18 blocks); all RectTransforms, GameObjects, CanvasRenderers, particle data and renderer settings are retained. The large prefab contains one ParticleSystem. Official Unity UI and TMP script references replace exported DLL identifiers. Item's script is replaced by OriginalRewardItemView with a lightweight settings reference; its original Icon/Count/Max references are retained.

The source DOTweenAnimation has a scaled-time linear rotation of -360 degrees over three seconds, infinite Restart loops. It is mapped to the existing native OriginalLoopRotation driver, retaining the target and values. No third-party assembly is imported. Count retains the source font, outline material, color, autosizing bounds (small 18–70; large 18–94.1), anchors and margins. The large icon retains FlowLightMaterial, and existing cash/particle dependencies are reused.

Additional image dependencies are copied byte-for-byte:

- Coin2.png SHA256 b06a92250b4de11f3005138e1c7660d592494e8d6c02d46963e69caf089f193b
- max.png SHA256 41fd13c80728d3b89eab5ebe55b145cda9fca2cbb4e6d682d0f5f2297229b5c5

## Verification and remaining scope

Unity 2022.3.62f3 full content validation passes 103 markers, including both prefab variants, five item types, country aliases, live per-item count selection, float widening, native sizing, hidden Max and append-only generation. The final run additionally checks missing icon/prefab behavior.

OriginalRewardItemPlayValidation runs the actual scene and instantiates the configured big item under its UICanvas using an explicit $12.50 fixture. It verifies real autoplay rotation, particles and scaled-clock pause. Latest local capture: Library/ValidationCaptures/reward-item-current.png. Visual inspection confirms cash icon, glow and outlined amount are rendered. The fixture overlays the current board: it is NOT evidence that SuccessPanel layout, masking, full presentation sequence, reward actions or production startup are restored. No old screenshots were used for comparison. Existing scene-destruction pool-parent warnings remain at Play shutdown; no runtime exception or C# compiler error occurred in the successful Play run. The preference fixture restores the user's saved data.

Next main-path work: restore RewardGetPanel/SuccessPanel's actual prefab, lifecycle, animation and button behavior and connect the existing settlement payload. Default startup, full guide/event branches and comprehensive current-build visual comparison remain incomplete. SDK handling is unchanged.
