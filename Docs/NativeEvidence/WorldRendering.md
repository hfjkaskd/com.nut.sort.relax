# 原生渲染恢复证据

源场景使用 URP-Balanced，主渲染资产 GUID 4f35ced5b749895488e8d1b3c3996639。渲染器 GUID 1cae831ab55b8c540af38dccb90ab780。OpaqueLayerMask=64，TransparentLayerMask=4294967295，DepthPrimingMode=1。SSAO 特性存在但 m_Active=0；不可为改善观感自行启用。

原版主灯阴影关闭、HDR 关闭、MSAA=1、RenderScale=1、SRP Batcher 开启。LightmapSettings 无烘焙 Lightmap 条目。管线资产已在 GraphicsSettings 中启用；可玩场景和光照效果尚未验证。

原螺杆 Prefab 的静态几何是 Mesh，世界 Layer=6，包含 base/tile/cap、ReadyPos、InitPos、锁定变体、SpriteRenderer 遮罩和粒子 DoneEffect。Mask/DontMove/Hidden 下含 spine-unity 组件，必须恢复为 Unity 原生资源/动画；不得导入反编译 DLL 替代运行时实现。

AssetRipper 导出的 shader 是占位实现。标准 URP 材质需映射官方同名 shader，保持原材质属性；自定义 shader 必须恢复原行为后验证，不得把能显示当作 1:1。

当前官方程序集已安装和锁定，完整回归通过。原版资源图位于逆向工作目录 reference-typed/ExportedProject；world-prefab-inventory.json 是此次直接读取原 Prefab 的依赖与层级清单。场景渲染、特效和屏幕对比未完成。

本轮已导入 5 个原始网格、19 个材质、3 张原贴图并通过 Unity 资源验证。Lit 使用官方同名 Shader。旧 Hidden/kMotion/CameraMotionVectors、ObjectMotionVectors 依据当前 UniversalRendererData 的 Reload 字段映射到官方同职责 Shader；迁移记录见 rendering-recovery-report.json。没有导入占位 Shader 或第三方 DLL。
