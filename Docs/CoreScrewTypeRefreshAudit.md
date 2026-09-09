# 核心完成螺柱后的类型更新

来源：RefreshScrewTypeObj 0x9FECAC；PlayMaskTween 0xA0B52C、其完成回调 0xA0B6A0；PlayDontMoveTween 0xA0A7C4。只在完成某根柱且整盘未完成时执行，保留已有 OriginalMoveCompletionFlow 的立即状态更新与独立延迟 DoneEvent。

原实现缺失类型更新，BindMoveCompletion 把它全部交给外部回调。现在由场景调用 Level.RefreshScrewTypes，之后才通知可选观察者，观察者为空也能完成核心状态变更。

按螺柱顺序，先用第一条类型判定当前是否彩色遮罩、隐藏或固定柱；被接纳后遍历全部类型条目。同色判定读取已完成柱最上层非空螺母，匹配后写 Object.IsShow=false，再请求动画、保存。没有添加每条 IsShow 检查或只更新第一条的简化。同一完成柱的固定类型触发 animation2 不循环，不额外保存。隐藏条目按原布局行列坐标判定同一行距离不超过一、或相邻行同一列，先写 IsShow=false，再播放并保存。对角和较远柱不揭开。

世界视觉保留原预制体层级。MaskDone 使用 0.5 秒 OutBack 缩放到一，完成后激活 MaskSpine 并请求非循环 animation，再独立计时 1.2 秒隐藏 Mask。固定柱请求 animation2，独立计时 1.34 秒隐藏 DontMove。参数保存在 OriginalScrew.asset；1.34/1.2 来自 ELF 0x2017964/0x2017968，Ease=27 对应源枚举 OutBack。隐藏柱仍用原有 posui 和独立一秒隐藏。逐帧推进由 Level 承担，单根柱暂时 inactive 不会停止时间；不加入第三方程序集或运行时生成静态 UI。

验证：core-mask-refresh-validation.log 完整 Editor 回归通过，159 个 PASS 标记；core-mask-refresh-play.log 包含 NUT_SCREW_TYPE_REFRESH_PLAY_PASS。实际世界 Prefab 完成一次非整盘胜利转移，即时解开彩色遮罩与相邻隐藏柱，远处柱保留；真实 PlayerPrefs 存下相同状态。完成回调与动画独立；暂停游戏时间后遮罩延迟隐藏继续执行。测试棋盘为显式机制夹具，没有修改生产关卡资产。

边界：骨骼效果的 animation/animation2/posui 请求已有真实目标和时序，但三套骨骼动画尚未转为 Unity 原生动画资源，不能声称这些视觉已经 1:1。池归还仍按已有 Release 清理该对象动画状态，跨关卡遗留 tween 的完整全局语义尚未证明。默认 StartupFlow 的完整初始化及过关消费接线仍未完成；本轮不虚构任何服务端回复或收益，也不宣称完整核心循环已经可从默认入口游玩。退出时既有对象池父节点警告仍待处理。
