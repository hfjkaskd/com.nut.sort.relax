# 默认入口接入原版失败面板

失败时原先显示本地通用卡片。本轮改为加载基于原 FailPanel 的 LocalFailure 预制体变体，复用 OriginalFailurePanelView 的初始化、标准 Restart Button、源文案 67/68、原按钮布局和失败/点击音效。源预制体没有 main 子节点，因而不添加源中不存在的弹出缩放动画。

本地变体只关闭 CoinTip 和 ReviveBtn 两个 SDK 数据/操作子树，保持本轮之前的本地重试能力。没有构造 GoldRewardTargetS2CData 或 ServerConfigData，也不调用依赖它们的 Refresh。源 FailPanel 和原数据解析类保持不变。该变体是明确的 SDK 跳过适配，不宣称与包含收益/复活 UI 的原版完整画面相同。

OriginalFailureFlow 仍负责无路可走后的三秒延迟。LocalGameplayController.Failure 负责实例化已配置的预制体、绑定回调及模态状态。重开按钮调用原 RestartAfterFailure：LevelSeed 加一、清除失败、开始重建，然后关闭面板。关闭沿用源 Hide 的管理器延迟队列，并保存本地待重建棋盘检查点；棋盘就绪后保存实际棋盘。重复点击会因已关闭/正在重建而被拒绝。

接入测试发现原面板配置的 Audio/GameLose 缺失，实际播放函数只报资源缺失而未发出 SoundStarted。已从本包 reference-typed 的 Resources/audio/GameLose.ogg 恢复原音频及导入设置：17,335 字节，SHA-256 abc350184d7491ff6a9d297bbdf4b83516f8de42fd71cad168493b487477e863。资源按原路径按需加载，没有新增第三方音频程序集。

完整核心回归还发现玩法解锁弹窗请求的 Audio/Reward_appear 缺失，同样恢复原导出音频及导入设置，SHA-256 c0238db39f2c4dd39f3dba317ae36be5f6cccefab44bb51f3daacedcf7ba7e8e。OriginalAudioValidation 新增两份面板音频的负载哈希、可加载性与原导入方式检查。

LocalFailurePlayValidation 使用显式存档棋盘夹具，从默认启动通过真实世界射线完成造成死局的一步操作，再检查原失败延迟、单次失败音效、源提示和按钮布局、模态输入阻挡以及隐藏的 SDK 子树。通过 EventSystem/GraphicRaycaster 命中并派发原 Restart Button，检查重建/关闭顺序、重复点击、单次点击音、待重建/已重建存档和再次读档。没有替换运行时完成回调或就绪标志。

全游戏生命周期、原收益面板、SDK 复活和源设备逐像素视觉对照不由本轮证明。

验证结果（Unity 2022.3.62f3）：

- `Library/local-native-failure-input-play.log`：NUT_LOCAL_FAILURE_PLAY_PASS，包含实际 UI 相机射线、原失败音效与重开点击音。最新 `Library/local-native-failure-current.png` 已检查。
- `Library/local-native-failure-core-play.log`：NUT_LOCAL_GAMEPLAY_PLAY_PASS，四段教学、正式关卡 2/3、结算和部分棋盘读档、失败重开及普通重开均通过；该运行中发现 Reward_appear 缺失，随后已恢复。
- `Library/local-native-failure-content-validation.log`：164 个 PASS 标记及 NUT_CONTENT_VALIDATION_PASS，包含新增两份面板音频的哈希、加载和导入检查。
- `Library/local-native-failure-unlock-play.log`：NUT_LOCAL_UNLOCK_PLAY_PASS，四类解锁、确认持久化、读档死局优先、原失败面板重开后待解锁提示均通过；无音频缺失错误。

原用户存档已恢复。既有场景退出对象池诊断及编辑模式工具飞行动画诊断未在本轮处理；不宣称全项目无警告。
