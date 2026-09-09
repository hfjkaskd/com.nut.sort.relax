# 彩色遮罩烟雾的 Unity 原生转换

从当前逆向工程的 Game/Screw.prefab 追踪 MaskSpine 到 yanwu/skeleton_SkeletonData：scale=0.01，默认动画 animation、不循环。动画 JSON 为 21 根骨骼、24 个当前皮肤片层，只有缩放与颜色的线性/阶跃曲线，长度 1.1333 秒。资源中未使用的皮肤附件不实例化。

OriginalMaskSmokeSource.json 保存本次转换的源文件 SHA-256、原像素、骨骼、当前片层附件参数和原关键帧。OriginalMaskSmokeBuilder 在 Editor 生成 Texture2D、Sprite、共享材质、原生 AnimationClip、SortingGroup 与 SpriteRenderer 骨骼预制体。运行时只按 Resources 路径延迟加载这个已配置预制体，再通过 Unity Animation 播放，池复用时保留实例；没有运行时 JSON、反射调用或 Spine/DOTween 程序集。

原纹理实际解码尺寸 64×64，atlas 逻辑尺寸是 94×81。区域 UV 必须由逻辑 atlas 求得再映射到实际纹理，不能直接把 xy/size 当作 64×64 像素。源 RGBA 为预乘透明度，保留全部 RGBA 字节及双线性采样；原生 ShaderLab 片元使用 Blend One OneMinusSrcAlpha，并在采样后仅乘一次片层 alpha。直接用 Sprites/Default 会再次乘纹理 alpha；先除 alpha 再做双线性采样也不等价，所以没有使用这两种近似。

MaskDone 的 0.5 秒缩放完成后，激活原 MaskSpine 目标并播放实际烟雾。OriginalScrewTypeView 要求该预制体绑定存在，缺失时报错。固定柱与隐藏柱仍保留明确的待转换动画请求。

验证记录：core-mask-smoke-validation-fixed.log 包含 NUT_MASK_SMOKE_VALIDATION_PASS 和 NUT_CONTENT_VALIDATION_PASS，完整回归共 160 个 PASS 标记。逐像素比对 RGBA、检查归一化 UV、21 骨骼/24 片层结构及 136 条动画曲线，并在关键帧之间和阶跃边界采样对照原线性/阶跃值。首次运行暴露转换器不在验证 asmdef 内的问题，已移动到 NutSort.Validation 并保留 GUID。

core-mask-smoke-play.log 的真实棋盘用例通过：非整盘完成的转移解锁遮罩，完成回调实际启动 Unity Animation，24 片层中存在可见烟雾，暂停后的延迟隐藏仍工作。当前 480×1040 画面 Library/core-mask-smoke-current.png 已检查，烟雾出现在右上彩色遮罩柱，未见缺失材质的紫色错误。测试棋盘是显式机制夹具，未改动生产关卡。

资源成本：共享一张 64×64 RGBA32 纹理、一份材质、24 个可批处理 SpriteRenderer 和 136 条曲线；不逐帧生成网格、不分配逐帧对象、不烘焙整段视频或帧纹理。多遮罩同时触发的实际设备帧耗仍需要后续性能验证。

边界：本轮证明的是原始静态布局与曲线的原生转换和首次真实触发，不是原设备像素级全生命周期对照。源 SkeletonData 的 defaultMix=0.2；同一烟雾在旧轨道尚存时重复 SetAnimation 的混合还未完成对照，当前播放器从头播放，不将此项宣称 1:1。固定柱、隐藏柱的动画（后者含 shear/noScale/clipping）尚待转换。默认启动完整接线与正式过关策略待继续处理；SDK 和收益没有增加模拟。已有退出对象池警告未在本轮处理。
