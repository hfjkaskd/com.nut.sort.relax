# 首关棋盘手指与目标勾叉

本轮恢复当前资料中的 LevelInfo.RefreshGuide（0x9F94F4）、管理器 IsCanMove（0x9FB4E0）、Screw.SetGuide/ClearGuide（0xA0764C/0xA06CF0）。实际棋盘初始化和每次螺柱操作后自动刷新，不需要测试代码手动展示提示。

规则：新玩法模式、Level>1、LevelSeed>2 或管理器判断成功时直接返回，不清空旧标记。正常分支先逐柱清理，跳过完成/空柱，选择第一根可移动柱；第一次遇到已选中柱时，若该柱可移动则优先替换候选。尚无选中柱时显示手指；已有选中柱时，使用点击流程保存的 selected 柱顶颜色标注其他柱，空柱或未满且同色显示勾，其余显示叉。CanMove 要求整个顶部同色组放得下；它只过滤原有的目标颜色罩和非正容量，不额外增加锁柱、隐藏柱或操作门过滤，保留源引导判断与真正输入判断的区别。

默认场景在 Initialize 时用现有 SceneSession 的玩法分流配置、当前用户关卡/种子和管理器 IsSuccess 绑定棋盘引导。调用点对应 LevelInfo.Init 尾部（0x9F9FB0）和 Level.Update 的螺柱点击后（0x9F94D0），不在每帧轮询中重建提示。玩家初始化的其他服务器/面板依赖保持现状，未通过设置 IsInitDone=true 伪造入口完成。

原生资源：Game/GuideHand、GuideCorrect、GuideError 保留当前导出预制体的世界空间旋转、比例、层级和 Sprite 数据。勾叉各自的源纹理及 Sprite 已恢复；手指复用工程中已有的原始 hand Sprite。手指原 DOTween 组件替换为原生 AnimationClip：局部位置 (-0.4,0.4) 到 (-0.3,0.5)，线性 1 秒，往返循环。勾叉位置由配置的 ReadyPos 局部高度加 1 设置，沿用原字段别名；路径与偏移存于 OriginalScrew 配置。实例销毁保留源 ClearGuide 的延迟 Destroy，没有引入第三方程序集或动态搭建静态层级。

验证：Library/core-board-guide-validation-final.log 完整回归 164 个 PASS 标记，含 NUT_BOARD_GUIDE_VALIDATION_PASS 和最终 NUT_CONTENT_VALIDATION_PASS。覆盖清理/显示顺序、选中优先级、空柱/同色/异色目标、整个顶部组容量、锁状态独立性、模式/关卡/种子/成功短路和资源引用。离线射线测试在自身测试代码里显式清理旧实例，运行时不增加 Editor 分支；既有 ToolRewardFlight 离线 Destroy 提示没有在此轮修改。

Library/core-board-guide-play-final.log 包含 NUT_BOARD_GUIDE_PLAY_PASS：默认场景自动创建并播放手指；真实世界 Operate 选中后出现勾，位置符合源高度；下一帧旧手指销毁；教学种子超过边界时保留现有标记；新教学机制棋盘再次自动提示，选中后显示勾/叉/勾。测试后用户偏好已恢复。

最新画面已检查：Library/core-board-guide-hand-current.png、core-board-guide-selected-current.png、core-board-guide-targets-current.png。手指、螺母和世界提示显示正常；未使用历史截图。它们证明当前工程实际效果，不构成原版设备逐像素一致证明。

成本：仅在棋盘创建和低频点击时遍历少量螺柱并按原逻辑创建/销毁提示；顶部组缓冲复用。静态结构在 Prefab，手指动画由原生 Animation 播放。

仍未完成：默认入口的完整初始化/引导面板接线与输入解锁、实际账号/飞行消费者、正式过关推进。本轮 Play 由测试显式调用世界 Operate，不宣称正常入口已经完整可玩；SDK/收益处理不变。
