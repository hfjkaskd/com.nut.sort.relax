# 最后一颗螺母落位的完成分支

当前 ARM64 Operator 的落位回调 0xA0ADF4，只对最后一颗螺母生效，并在该时刻读取目标柱 IsDone。满柱调用 Screw.Done，动画完成回调 0xA0A98C 恢复 IsCanOperator；SDK 震动继续按要求跳过。未满柱则先恢复 IsCanOperator，再读取管理器当前 IsSkipLevel，为真时直接 DoneEvent(true)。它没有两秒延时，也不执行满柱计数递增。

OriginalLevelView.OnLanded 现在补齐该未满柱分支。OriginalGameScene.BindMoveCompletion 同时将延时回调和落位回调接到同一个完成消费者，因此前轮 BindScrewCompletion 的真实分派器也覆盖此路径。NutMoveBatch 增加 TryCompleteMovement 返回值，让世界层遵守已有的最后移动一次性保护，避免重复回调再次启动柱帽动画或分派完成。普通 CompleteMovement 调用方式保持兼容。

管理器 IsSuccess（0x9FB348）先检查 IsSkipLevel，再遍历棋盘。新增对应方法，并用于移动时捕获胜利结果及恢复存档后的判断。后者依据 InitLevel 恢复分支 0x9FFA28 中 0x9FFBB4 的调用：跳关标志为真时，即使存档棋盘尚未整理完成，也会按原流程再次请求新棋盘初始化。标志没有新增存档字段或自动清零；默认仍为 false，未增加默认跳关或虚构正式过关响应。

验证入口 OriginalLandingCompletionPlayValidation 使用当前原生世界预制体和显式机制棋盘，完成四类实际移动：普通未满柱、飞行途中关闭跳关、飞行途中开启跳关，以及满柱时捕获跳关成功后关闭标志。验证操作门先于完成事件恢复、未满柱不增加完成计数、非末颗/重复落位不重复分派，满柱保持原两秒回调且只触发一次。最后保存未完成棋盘、开启标志后恢复，确认会重建原始第一关棋盘。

Library/core-landing-play-final.log 已包含 NUT_LANDING_COMPLETION_PLAY_PASS。测试恢复用户偏好。完整回归记录在 Library/core-landing-validation-final.log，共 163 个 PASS 标记，含最终 NUT_CONTENT_VALIDATION_PASS。

范围：补齐落位路径和两个已接入的管理器成功判断调用点。原始 LevelInfo.RefreshGuide 也调用管理器 IsSuccess，该引导棋盘消费者仍需后续完整接线；本轮不宣称全部跳关/GM 界面已完成。默认启动入口、实际账号/飞行消费者及正式过关推进仍未完成，SDK 行为保持不变。
