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

剩余：当前教学顶部的本地诊断横条与原教学徽章重叠，是实际设备截图暴露的视觉差异。物理 Android/iOS 设备、后期全部玩法/地区版本、原版逐帧视觉对照以及完整生命周期仍未验证。此轮没有改变 SDK 处理，独立测试包保留在模拟器第二关供继续检查；标准包 com.nut.sort.relax 的 APK 保持上一轮版本与哈希。
