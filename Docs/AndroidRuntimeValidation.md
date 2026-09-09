# Android 模拟器核心运行验证

本轮实际使用当前运行的 MuMu Android 15 实例 1，通过 MuMuManager info 确认端口 127.0.0.1:16416，屏幕 1440×2560，ABI 列表 x86_64/arm64-v8a/x86。运行的是 Unity 2022.3.62f3 官方 ARM64 IL2CPP Development 构建，依靠该模拟器的 ARM64 兼容环境；这不是物理 ARM64 手机的性能或兼容性证明。

实例已经安装原版 com.nut.sort.relax。本轮没有卸载、更新或清除原版。新增 LocalAndroidBuild.RunDeviceTest，与 Run 共享 BuildPipeline、场景、资源、运行代码及构建选项，仅使用独立包名 com.nut.sort.relax.localtest、显示名 Nut Sort Relax Local Test 和单独输出文件。构建结束还原工程包名与 productName。运行时代码没有按测试包名分流，也没有 Editor 或设备专用玩法兜底。

Library/local-android-device-test-build.log 记录 NUT_ANDROID_BUILD_PASS，warnings=0、errors=0。Builds/Android/NutSortRelax-device-test.apk 为 53,708,319 字节，SHA-256 cddc61abb440aa54a098beeb2feaa23fdba771ba749adb8da92e804a7bedeb44。签名及 aapt 包元数据核验通过，实际安装/启动成功。原包 base.apk 在测试前后的 SHA-256 均为 c1f84a18c886afc8e4dcc9e3d350ff5f9a23fac29c328f26a3cf223061baed1a，路径也未变；没有读取或改写原版私有存档。

默认 ADB 5037 服务在测试间被其他进程重启，导致设备连接失效。验证后续使用 Unity 自带 adb 的独立服务器端口 5038，并连接已确认的模拟器端口。没有重启模拟器或终止其他工程。复测可执行 adb -P 5038 connect 127.0.0.1:16416，之后所有命令保持 -P 5038 -s 127.0.0.1:16416。

## 实际通过的链路

- 从全新独立应用存档启动，显示真实教学与世界棋盘；所有搬移均由 Android input tap 注入按下/释放事件，没有调用 Unity 验证回调或改写关卡进度。
- 完成四段原教学，累计 16 次搬移；达到 Level=2、LevelSeed=0、TodayPassLevelCount=1，原生 Level Complete 动画与 Next level 按钮实际出现。点击该按钮进入第二关，没有再次计数。
- 第二段教学的一次搬移后，Home 后台并返回：进程 PID 保持 11451，棋盘与历史 JSON 一致。
- 随后 am force-stop 只结束测试包，再次启动 PID 变为 11825；棋盘、操作历史、Level/LevelSeed、搬移次数、道具库存及过关数与停止前一致。
- 第二关再通过触摸搬移一枚，点击原生撤销 Button。棋盘和操作历史恢复到该次搬移前，RevokeCount 从 5 降到 4；再次强制结束并启动，棋盘和库存扣费保留，未重复消费。
- 最终 Level=2、LevelSeed=0、ScrewMoveCount=17、TodayPassLevelCount=1、RevokeCount=4。Gold/Coin 保持 0，GoldRewardTargetS2CData 保持 null，没有收益或 SDK 回调伪造。

只读取测试包自身 debuggable sandbox 的 shared_prefs/com.nut.sort.relax.localtest.v2.playerprefs.xml。Unity 的 string 值需要先 XML 解析再 URL 解码；未修改该存档。验证用的只读解析和离线搜路辅助保存在 Library，最终教学段的九步输入记录见 android-teaching3-input-path.json；辅助只选择触摸输入，不运行在游戏中。

证据：Library/android-device-restart-evidence.json、android-device-undo-evidence.json、android-device-final-evidence.json；对应三个已观测测试进程的 android-local-runtime-<pid>.log 中没有 Exception/FATAL/Shader error/DllNotFound 标记。日志筛查不能证明所有设备无问题。当前设备截图 android-local-initial-current.png、android-local-teaching2-settled-current.png、android-local-teaching3-current.png、android-local-success-current.png、android-local-level2-current.png 已检查。

上一轮发现教学顶部的本地诊断横条与原教学徽章重叠；本轮已在 LocalGameplay 预制体默认隐藏 LocalMode，并同步修改编辑器生成器，防止重建时恢复遮挡。教学 Local 徽章、本地推进说明及 SDK 跳过逻辑保持原状。物理 Android/iOS 设备、后期全部玩法/地区版本、原版逐帧视觉对照以及完整生命周期仍未验证。此轮没有改变 SDK 处理，独立测试包保留在模拟器第二关供继续检查。本轮两个 APK 均已更新，见下方记录。

## 教学遮挡修复复测

LocalTeachingViewPlayValidation 四段教学通过，包含源规则文本、世界操作及第二关教学退出；检查本轮 local-teaching-0-current.png 与 local-teaching-3-current.png，顶部徽章无遮挡。该轮使用明确教学种子夹具，测试结束恢复 Editor 存档；没有改写 Android 存档。

两个新构建日志 local-hud-android-build.log、local-hud-device-test-build.log 均 NUT_ANDROID_BUILD_PASS，warnings=0、errors=0；签名验证通过，原有两条 META-INF 提示仍存在。只通过 install -r 更新独立测试包，未安装标准包。更新前后 Level/LevelSeed/LevelInfo/搬移数/过关数/撤销库存逐项一致。Android 当前第二关稳定截图 android-hud-level2-settled-current.png 已检查，横条消失；首次启动截图处于入场阶段，未作为最终画面。PID 12801 日志未发现 Exception/FATAL EXCEPTION/Shader error/DllNotFound 标记。存档和构建摘要见 Library/android-hud-evidence.json。

- NutSortRelax-local.apk：53,708,267 字节，SHA-256 `2eaaa287f336957591ec03e1f6057456e17ccd84ad27bd8aec4a35696e691d91`。
- NutSortRelax-device-test.apk：53,708,319 字节，SHA-256 `2e625d1907c9b1a87eb4e509ee5f0c1fde5742ac02abbbe523d67bfd521ec919`。

本轮 Android 只复核更新与第二关画面，四段教学视觉回归在 Unity Play 中执行；未宣称重新完成全部 Android 教学或物理真机验证。

## 普通重开与取消的 Android 实测

使用教学遮挡修复后的同一独立 APK（SHA-256 2e625d1907c9b1a87eb4e509ee5f0c1fde5742ac02abbbe523d67bfd521ec919），未重新构建或修改运行时代码。通过 Android input tap 实际点击底栏重开、Continue 和 Replay；未调用 Unity 测试方法或改写任何 Android 存档。原版应用保持不动。

在 Level=2/LevelSeed=0 上先打开并取消重开，LevelInfo 原样保留。随后通过世界触摸完成一次真实搬移（左上柱到下中空柱），ScrewMoveCount 从 17 增到 18，棋盘与历史产生变化。再次打开重开并点击 Continue，部分棋盘和历史保持一致。随后重新打开并点击 Replay，ScrewInfos 与搬移前逐项一致，OperatorInfos 清空，Level=2、LevelSeed=0、TodayPassLevelCount=1、RevokeCount=4 不变，累计搬移数保留 18；Gold/Coin 和奖励文档没有变化。这符合原 ReplayCallback 保留种子、InitLevel(true,false,false) 重建的路径，与失败重试增加种子不同。

在重建完成后仅 force-stop 测试包，PID 从 12801 变为 13103；重新启动后的 Level/LevelSeed/LevelInfo/搬移数/过关数/撤销库存与退出前完全一致。PID 13103 日志未发现 Exception/FATAL EXCEPTION/Shader error/DllNotFound 标记。没有通过强杀证明重建尚未完成时的结果。

证据位于 Library/android-replay-evidence.json、android-replay-before.json、android-replay-partial.json、android-replay-reset.json、android-replay-runtime.log。已检查本轮 Android 截图 android-replay-open-current.png 和 android-replay-reset-current.png，原生 Restart? 面板、Continue/Replay 和重开的世界棋盘显示正常；另有重启截图 android-replay-resumed-current.png。模拟器保留第二关，累计搬移数 18，撤销库存 4。

此轮补充普通重开的真实设备输入与进程重启证据；失败重试仍以此前 Unity Play 证据为准，未将其计作本轮 Android 测试。没有发现需修改的普通重开逻辑差异；物理手机、所有关卡与完整生命周期尚未证明。
