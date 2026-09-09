# 隐藏罩原生待机、碎裂与扫光裁剪

来源为当前逆向资料 suilie1/posui，原始资源哈希和像素、骨骼、片层、图集信息保存于 Editor/AnimationConversion/OriginalHiddenCoverSource.json。沿用 Screw.prefab 的 HiddenSpine 位置、比例和层级。转换 wanzheng（循环 8 秒）、posui（非循环 1 秒），不同动画间保留配置的 0.2 秒混合；隐藏计时保持独立于 timeScale 的 1 秒。

原生 AnimationClip 驱动 23 个骨骼参数和 18 个片层颜色。72 顶点的世界 Mesh 保留片层顺序、附件位置/缩放、旋转图集 UV 和独立的 shearX/shearY。noScale 的方向归一化保留父级对骨骼位置的影响，同时处理零缩放及反射父矩阵。参考 [Spine 3.8 Bone 的变换定义](https://raw.githubusercontent.com/EsotericSoftware/spine-runtimes/3.8/spine-csharp/src/Bone.cs)，没有复制第三方运行时或添加第三方程序集。

最后一个扫光片层由原生 SpriteRenderer 显示，SpriteMask 使用源 44 点凹多边形及 42 个三角形，仅覆盖扫光的排序范围。生成器使用 Unity 官方 SpriteDataAccessExtensions 写入可序列化原生 Sprite 几何；不依赖运行时 OverrideGeometry 或改变渲染管线。纹理保留预乘透明度，沿用工程的原生 PMA Shader。

Configure 自动播放待机；真实完成螺柱后的既有邻接刷新会启动破碎，独立延时隐藏罩体。所有静态结构在 Prefab 中，资源首次使用时加载并复用。运行时只更新已配置网格和动画参数，无 JSON 解析、反射或逐帧层级创建。

验证记录：
- Library/core-hidden-validation-final.log：完整回归 162 个 PASS 标记，包括最终 NUT_CONTENT_VALIDATION_PASS。隐藏罩逐动画检查所有关键帧与帧间采样，覆盖位移、缩放、旋转、双轴剪切、RGBA、图集旋转、附件原点、noScale/零缩放/反射，以及裁剪顶点与三角面积。
- Library/core-hidden-play-final.log：真实世界棋盘的选择、螺母转移和完成事件自动触发邻接罩体破碎；斜对角罩体保持待机。确认运行时剪切、有限顶点及暂停时间缩放后的独立隐藏。棋盘是明确的验证夹具，没有改写生产关卡或伪造服务端响应。
- 同一 Play 验证在工程现有 URP 管线进行 GPU 对照：保留 3058 个罩内采样像素，裁去 1337 个罩外采样像素，排除边界附近像素后检测到 0 个罩外泄漏。
- 当前画面：Library/core-hidden-idle-current.png、Library/core-hidden-break-current.png。用户存档在验证结束后恢复。

成本：一张 512×512 RGBA32 纹理，18 个片层合成一个 72 顶点动态 Mesh，加一个扫光 SpriteRenderer 和静态 SpriteMask。每个实例一次性分配矩阵、顶点、颜色缓冲并复用；未做大规模同时破碎的设备性能测试。

范围限制：本轮没有完成默认启动入口和正式过关推进接线。沿用前轮不同动画的 CrossFade；同名轨道重入、连续打断时的源混合语义仍需对齐。浮点颜色到 GPU 的量化、设备渲染边缘以及原版设备逐像素对照尚未证明完全一致，因此不宣称全生命周期或全部视觉已达到 1:1。SDK/广告/收益处理保持原样；既有退出对象池警告未在此轮处理。
