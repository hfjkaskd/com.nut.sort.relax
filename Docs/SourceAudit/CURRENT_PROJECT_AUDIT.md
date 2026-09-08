> 历史初查记录：下文描述初次检查时的空目录状态。初始提交现已成功推送；当前状态以 [ImplementationStatus](../ImplementationStatus.md) 为准。

# Nut Sort Relax 当前复刻核查

核查时间：2026-09-07（主机时区）。目标包：`com.nut.sort.relax`，导出版本 1.0.4。

## 结论与恢复点状态

尚未实现，更不能判定为完美 1:1。用户指定 `C:/Projects/com.nut.sort.relax` 在核查时为空，无 Assets、ProjectSettings、.git。指定 GitHub 仓库通过本机 Git 成功读取，但无引用。

已初始化 main、设置指定 origin 并暂存初始说明及开发规范；commit 因本机缺少 user.name / user.email 失败。因此当前没有成功提交或推送的恢复点。已请求用户提供提交身份。在用户要求的首次备份完成之前，不改动游戏实现。

## 来源核验

- 唯一游戏依据：`C:/Projects/Golden  Dragon Legend` 中的 Nut Sort 导出。
- 对比 `exports/com.nut.sort.relax-20260907-185616/apk` 与 `DeviceExports/com.nut.sort.relax_20260907_203224/apk`，两个 APK 的 SHA-256 分别完全相同；详见 `source-hashes.json`。因此较早目录的已解包 APK 可用于本次包内证据读取。没有使用旧截图做视觉判断。
- 设备存档依据为上述 20:32 快照；没有将其冒充实时服务端配置或当前设备画面。
- `C:/Projects/Nut Sort Relax/reconstruction/mumu-current/delivery` 的 README 明确指向 `com.sfflogdstudio.dragonlegend`。该框架与本次游戏不符，没有复制其代码、配置或美术。仅复用本机已有通用解析工具。
- 本包资源记录的 Unity 版本为 **2021.3.57f2**；指定目标编辑器 **2022.3.62f3** 存在。迁移后的着色器、字体、渲染结果需要实际验证。

## 逐项需求

|需求|当前证据及缺口|
|---|---|
|完整玩家生命周期|没有可运行复刻工程。元数据确认入口 LuoSiSort；玩法管理 LuoSiSortMgr；UI 管理 UIMgr；用户状态 UserMgr；提现阶段 TXMgr。具体调用顺序、条件、存档恢复、失败重试尚需核对机器代码和实际运行。|
|玩法与所有分支|原类型包含初始化、循环关卡、移动、胜负判定、死局、撤销、加槽、加空螺杆、每日礼物和新玩法解锁；并有成功、失败、重玩、道具、抽奖、评级及多阶段提现面板。发现类型不等于已证明其全部可达条件。|
|视觉、布局与字体|已导出原 Camera、Canvas、RectTransform、Material、RenderSettings、LightmapSettings、ParticleSystem 和可恢复组件字段。尚未接入 Prefab/Scene，也没有新工程画面可供像素或动画对照。|
|国家 AB 与 GM|CountryCode 有 ID、BR、GB、US、CA、AU、MX、AR、RU、JP、DE、FR 共 12 项，US 数值 3。枚举不代表完整支持国家列表或服务端实验矩阵。GM 和 US 收益广告默认版本均未实现。|
|Unity 官方依赖、Prefab 驱动|原始程序集含 Spine、DOTween、DOTweenPro。需用原生实现重建可观察行为，不能声称同时保留其第三方内部实现。尚未导入任何第三方运行程序集。|
|SDK 保持现状|本次没有改动 SDK。指定空工程中不存在可继承的 SDK 处理实现，也没有将其他游戏的 mock 当成本项目现状。|

## 已取得的具体证据

- IL2CPP 元数据主程序集类型范围 7619–7956，共 338 项（含编译器生成类型）。`game-schema.cs.txt` 是字段、签名和 RVA，不是可执行函数体；DummyDll 同样不能作为实现。
- 资源容器共 9,519 对象；包括 1,637 个 GameObject、1,408 个 RectTransform、229 个 Transform、1,715 个 MonoBehaviour、332 张 Texture2D、306 个 Sprite、5 个 Mesh、25 个 ParticleSystem、15 个 AudioClip、12 个 Animator。数量不代表复刻完成率。
- MonoBehaviour 通过本包 DummyDll 类型树补读后，1,713 个完成读取，仍有 2 个对象失败，见 `failures.json`。这是序列化证据读取，不是行为还原。
- 1,487 个 TextAsset 已导出，包括 `LevelConfig0`、`LevelConfig1`、`tables`。两份 LevelConfig 是编码内容，尚未还原解码路径，不可以用随机关卡替代。
- 核心 Nut 字段直接引用 MeshRenderer；Screw 字段包括 BoxCollider、ScrewTiles、ReadyPos、InitPos、ScrewCap、DoneEffect，支持按原世界对象结构还原。
- 3 个原 Camera 均为正交，size 分别为 8、5、8；对应对象见 `objects/Camera/level0_241.json` 至 `level0_243.json`。需连同 transform、layer、URP 相机组件及运行时调整一起复核，不可只照抄尺寸。
- Font 资源名为 Vagron、LiberationSans、PerfectDOSVGA437；字体存在不等于每个 UI 使用该字体，后续按文本组件实际引用恢复。
- 原 LightmapSettings 的 m_Lightmaps 为空，m_EnableBakedLightmaps=true、m_EnableRealtimeLightmaps=false；不能据此凭空添加烘焙贴图。

## 后续限定顺序

1. 使用用户确认的 Git 身份完成初始提交和推送，确认远程分支指向同一提交。
2. 从本包原函数体恢复配置解码、启动与关卡主链；把布局、坐标、材质、字体及参数落在正式 Prefab/Scene/ScriptableObject 中。
3. 按入口→引导→排序操作→胜负/撤销/加槽→解锁与收益分支，逐条建立有原 RVA 和资源对象出处的验证表，再接入原生实现。
4. 以最新运行画面验证视觉和动画，迁移至 2022.3.62f3 后运行 Editor 与设备一致性检查。
5. 在明确的地区与配置证据范围内建立 GM 切换，默认 US 收益广告路径；不编造未获得的服务端实验桶。SDK 工作继续排除。

推荐保留 Mesh 玩法、资源路径加载和对象池，普通动画使用原生 AnimationClip/Animator 或轻量运行时插值。不推荐默认把所有 Spine 效果转换为高分辨率逐帧图：其贴图内存、包体和加载峰值需要先测量；只有必要效果才考虑该成本。主链和视觉前不做无关外围功能。

## 验证界限

本次完成来源 hash 校验、元数据解析和序列化资源审计。没有 Unity 编译、PlayMode、设备操作或新工程视觉验证。`complete_1_to_1` 保持 false。
