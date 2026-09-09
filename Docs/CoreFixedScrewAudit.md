# 固定螺柱待机与碎裂的原生动画

当前逆向 Game/Screw.prefab 的 DontMoveSpine 引用 dizuosuilie/skeleton_SkeletonData，scale=0.01、defaultMix=0.2。已转换 11 根骨骼、11 个片层、6 个图集区域及两段动画：animation 循环待机 8 秒，animation2 非循环碎裂 1.3333 秒。原始 PNG 实际尺寸 256×256，atlas 逻辑尺寸 244×259；按逻辑尺寸计算 UV 并保留源预乘透明度。

Editor 转换器生成原生 Sprite、共享材质、AnimationClip 与 Prefab。缩放使用 setupScale × timelineScale，位移和旋转使用 setup + timeline。首个关键帧之前显式保留 setup pose；未被当前动画驱动的属性也恢复 setup，以便从另一动画混合退出。颜色、位置、缩放与角度保留线性/阶跃时序。旋转打包的 E_glowstar_001 使用已配置的区域子节点旋转与尺寸映射，不复制 Spine 运行时。

图集旋转的角点约定同时核对了 [Spine 3.8 官方 RegionAttachment](https://raw.githubusercontent.com/EsotericSoftware/spine-runtimes/3.8/spine-csharp/src/Attachments/RegionAttachment.cs) 的 UV/顶点次序；工程只保存自有转换代码和 Unity 原生资产。

OriginalScrewTypeView.Configure 在未完成固定柱上播放实际 idle；RefreshScrewTypeObj 的同柱完成分支启动实际 animation2。OriginalNativeWorldEffect 在不同动画之间使用配置的 0.2 秒 CrossFade，首次播放和同名重播保持从头播放。加载在首次使用时发生，后续复用原实例，不在 Update 中创建层级或解析数据；静态结构全部在 Prefab 内。

验证：core-fixed-screw-validation.log 完整回归 161 个 PASS 标记，包含 NUT_FIXED_SCREW_VALIDATION_PASS 和 NUT_CONTENT_VALIDATION_PASS。逐动画采样首键之前、关键帧和帧间的位移、初始缩放、角度和 RGBA；对全部片层逐顶点检查旋转/非旋转区域 UV 以及几何宽高。

core-fixed-screw-play.log 包含 NUT_FIXED_SCREW_PLAY_PASS。真实世界 Prefab 的固定柱 idle 启动后，通过螺母转移完成该柱；验证混合期间两条 Unity AnimationState 同时参与，并以旧动画当前位移、新 setup pose 及权重核对实际混合位置；混合结束旧动画退出，独立完成回调只执行一次，外壳 1.34 秒后隐藏。当前 Library/core-fixed-screw-current.png 已检查，固定柱碎片显示正常。用户偏好备份已恢复。机制棋盘为显式测试夹具，不是修改生产关卡。

资源成本：一张 256×256 RGBA32 纹理、共享材质、11 个可批处理 SpriteRenderer、两个 AnimationClip，无第三方程序集或逐帧网格构建。大规模同时触发的设备性能尚未测量。

尚未完成：隐藏罩的 noScale/shear/clipping 转换；同名动画在旧轨道尚存时的 Spine 重入混合；任意快速多次打断轨道的完全对照；默认启动入口的完整玩法接线。此次只证明已列出的状态、曲线和真实不同动画切换，不宣称全生命周期 1:1。SDK/收益与待确认的本地关卡推进语义保持不变。既有退出对象池警告未在此轮处理。
