# Android 本地核心构建

本轮首次从当前默认可玩场景生成 Android ARM64 IL2CPP Development APK，使用指定 Unity 2022.3.62f3 和自带 Android SDK/NDK/OpenJDK/Gradle。没有加入第三方 SDK、原广告启动器或账户插件。

依据原包 exports/com.nut.sort.relax-20260907-185616/apk/base.apk 的 aapt badging，以及 arm64 split 内的 libil2cpp.so/libunity.so，修正此前的新工程默认 Android 配置：包名 com.nut.sort.relax、版本 1.0.4 / code 4、最低 API 23、目标及编译 API 36、ARM64、IL2CPP。APK 显示名为 Nut Sort Relax。启动使用 Unity 官方 UnityPlayerActivity；原包自定义 SDK launcher 按本项目 SDK 跳过约束不接入。

LocalAndroidBuild 在构建期间设置 APK 显示名，完成后恢复 Editor 原 productName，避免改变已有 Editor PlayerPrefs 的命名空间；没有改变任何运行时存档路径或添加 Editor 专用逻辑。入口固定为 Assets/Scenes/LuoSiSortGame.unity。Builds/ 下产物遵循现有 gitignore，不将 APK/签名密钥提交仓库。

首轮构建暴露了未被 Editor 游玩覆盖的资源缺口：原动态 LiberationSans SDF - Fallback 的图集 GUID 934a6abaceb7f8846bd32d0d808d3ab3 未解析到任何资源，TMP_PreBuildProcessor 访问 atlasTexture.width 时报 MissingReferenceException。Unity 虽生成了 APK 并返回 Succeeded，却在报告中保留 errors=1。因此构建工具现在同时要求 Succeeded 和 totalErrors=0，避免误判通过。

修复使用 TMP 3.0.6 原生 CreateFontAsset 的同种 0×0 Alpha8 空动态图集初始化，保存为实际 Texture2D 资源并重绑字体与材质引用。保留源 TTF、动态模式、512×512 容量、padding 9、渲染模式和构建时清理开关。没有关闭 TMP 检查、冻结为静态图集或绕过缺失资源。新增检查遍历工程字体图集引用，并在临时副本中通过原生 FontEngine 补入拉丁、希腊和西里尔字符，确认动态图集实际扩展和绘字；不污染生产字形表。

修复后 Library/local-android-build-final.log 记录 NUT_ANDROID_BUILD_PASS，Unity 报告 warnings=0、errors=0。该首次构建 APK 为 Builds/Android/NutSortRelax-local.apk，53,767,621 字节，SHA-256 6fc89963d06c20314eeffffba8af32dc4d34842837e8722d09b94261a98da3b9。aapt 验证包名、版本、API、显示名和 arm64-v8a；apksigner verify 成功，v1/v2 签名有效。签名工具另有两条 META-INF 元数据条目的提示，不将其描述为全工具零警告。

此包为本地开发验证产物，不是商店发布包。当前 ADB 没有已连接设备，未进行安装、Android 触摸实玩、系统强杀/恢复或逐像素真机对照；构建成功不等于真机运行和全生命周期 1:1 已验证。Unity 自动生成的 URP shader prefilter 缓存差异不作为美术/渲染配置改动提交。

修复后 `Library/local-android-content-validation.log` 完成 165 个 PASS 标记及 NUT_CONTENT_VALIDATION_PASS，其中新增动态图集引用/补字检查。可通过 Unity batchmode 调用 `NutSort.Validation.LocalAndroidBuild.Run` 重建；产物使用 Development 选项。

上一轮更新包包含原版非模态道具提示（见 LocalPopTips.md）。Library/local-pop-tip-android-build.log 记录 NUT_ANDROID_BUILD_PASS，warnings=0、errors=0；APK 56,233,582 字节，SHA-256 50b6b066d0b8defe73d803cf0ccf0802f434635ba678b02b9b9ba7f3b2660344。重新核验 v1/v2 签名、包名、版本与 ARM64 均通过，签名工具的两条 META-INF 提示仍存在。该轮文件已替换上述首次构建产物，尚未完成真机验证。

上一轮背景修复包：Library/local-background-android-build.log 记录 NUT_ANDROID_BUILD_PASS，warnings=0、errors=0。APK 56,233,492 字节，SHA-256 ee20c6b8f5c9a77bba7be8a690df5be73165afeebb4b10cb615dace571917aab。签名、包名、版本及 ARM64 验证通过；产物替换上述上一轮 APK。场景背景恢复源 ASTC 10×10 格式，详情与误差测量见 BackgroundTextureFidelity.md。

上一轮子回合推进修复包：Library/local-subround-android-build.log 记录 NUT_ANDROID_BUILD_PASS，warnings=0、errors=0。APK 68,789,388 字节，SHA-256 0e47b80ebfe6573b8f916f5b7d250f299cb4b10cb3e6a4792fc5b54af95d34f0。v1/v2 签名、包名、版本和 ARM64 验证通过；替换前述开发 APK。

该次增量 APK 的 ZIP 中央目录有 2,112 项，压缩载荷合计 53,337,845 字节；额外空间主要是未被当前中央目录引用的旧局部 ZIP 记录（大间隙仍以 PK 本地文件头开头）。因此本次开发包增大不能直接当作运行时资源增长；尚未做发布打包体积清理，保留官方构建及签名产物。

最新子回合点阵包：Library/local-subround-dots-android-build.log 记录 NUT_ANDROID_BUILD_PASS，warnings=0、errors=0。APK 68,786,702 字节，SHA-256 a6e13eace9c95797a13c7635d9ae7f2617507f11ed622f153fb8acea1e2bc849。签名、包名、版本及 ARM64 复核通过，替换上一轮开发 APK。点阵接入和验证见 LocalSubroundView.md；增量开发打包与真机验证限制不变。
