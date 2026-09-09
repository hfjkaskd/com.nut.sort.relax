# 核心场景背景压缩对齐

当前玩法截图曾出现明显的大块紫色色带。沿场景 BG 的 MeshRenderer → Resources/game/Scene.mat → GUID 0fdae0a7dfc80c944b518fdf6a06fab7 定位到 Resources/game/spirte/bg.png。

源包 data.unity3d 中 sharedassets0.assets 的 Scene 材质 path ID 29，_MainTex 指向同文件 Texture2D path ID 49。直接读取原对象得到 1024×2048、m_TextureFormat=52（ASTC RGB 10×10）、单 mip、sRGB、Bilinear、Repeat、不可读，压缩数据 337,840 字节。当前 PNG 与逆向导出 PNG 的 SHA-256 均为 9b02daa80d98a75a07b2b19362639387e850fd808443eb10eac64eef231c6073。

根因是导出 TextureImporter 的旧 textureFormat 字段虽为 52，但有效 platformSettings 使用自动格式且没有 Android override。Unity 2022.3.62f3 在当前 Android 平台实际导入为 ETC_RGB4。现在仅为这一张玩法背景配置 Android ASTC_10x10 / 2048 / quality 50；保留 PNG、GUID、原采样、无 mip、不可读与现有材质/世界场景。Unity 同时升级该 meta 的序列化版本。没有全局修改纹理质量，也没有改变 Editor 和设备的运行时加载逻辑。

OriginalBackgroundTextureValidation.Repair 使用相同 GPU Blit / Linear ARGB32 读回方式，将实际导入纹理与未经再次有损压缩的源 PNG 比较。Library/local-background-texture-repair.log：

- 修改前 ETC_RGB4：平均 RGB 绝对差 1.751259，最大单通道差 6。
- 修改后 ASTC_RGB_10x10：平均 RGB 绝对差 0.080328，最大单通道差 2。

这些是线性读回缓冲区的 8 位数值，不是原设备整屏误差。PNG 本身已经包含原包压缩结果；Unity 再编码 ASTC 后仍有微小差异，不能称为压缩数据逐字节一致。1024×2048 ASTC 10×10 的块数据为 337,840 字节，小于此前 ETC1 约 1 MiB；无需改成约 8 MiB RGBA32 常驻纹理。实际不支持 ASTC 的设备可能由 Unity 解压回退，仍须真机验证。

重新运行默认入口的 LocalPopTipPlayValidation，通过真实底栏 Button、世界选择、两个独立浮动提示与暂停计时回归；Library/local-background-play.log 记录 NUT_LOCAL_POP_TIP_PLAY_PASS。最新 Library/local-pop-tip-current.png 已检查，先前明显的大块色带得到消除；保留源图本身的渐变层次。新内容检查锁定 PNG 哈希、Android 格式、尺寸、mip 与采样设置，防止再次退回自动格式。

Library/local-background-content-validation.log 完成 166 个 PASS 标记及最终 NUT_CONTENT_VALIDATION_PASS。Library/local-background-android-build.log 构建成功，Unity 报告 warnings=0、errors=0。最新 APK 签名与包信息复核通过，SHA-256 见 AndroidLocalBuild.md。

直接读取最新 APK 的 sharedassets0.assets.split*（按数字顺序合并）确认：Scene 材质的 _BaseMap/_MainTex 均引用同文件 path ID 14，纹理为 1024×2048、format 52、单 mip、337,840 字节。Library/local-background-apk-verification.json 保存对应 APK 哈希与字段，证明格式变更确实进入安装包。
