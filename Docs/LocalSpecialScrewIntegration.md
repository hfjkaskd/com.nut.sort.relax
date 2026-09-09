# 特殊螺杆的默认入口验证

此前 OriginalScrewTypeRefreshPlayValidation、OriginalFixedScrewPlayValidation 和 OriginalHiddenCoverPlayValidation 直接绑定机制棋盘并替换完成回调，只能证明相应规则和效果本身。本轮新增 LocalSpecialScrewPlayValidation，将显式机制棋盘保存为原格式 LevelInfo，再从默认场景启动、读取存档、初始化和绑定本地核心流程。测试没有替换棋盘实例、完成回调或运行时就绪标志。

夹具为内部关卡 108、已确认四类玩法解锁、两次撤销库存；包含固定目标柱、同色遮罩柱、相邻隐藏柱和对角隐藏柱。它是独立的机制/库存测试数据，不是对真实第 108 关的替换，也没有修改关卡资源或提供服务端文档。

通过实际摄像机射线完成固定目标柱，检查立即写入的完成次数和操作历史、同色遮罩解除、相邻隐藏罩解除、对角隐藏罩保留，以及不同破碎外观各自的延迟隐藏。等待完整入场动画后再检查重载画面中的螺母显示，避免将入场缩放阶段误判成资源丢失。

原版 RefreshScrewTypeObj（0x9FECAC）只为完成的固定柱请求破碎，不清除固定类型。IsDontMove 仍由首个类型决定，固定外观初始化则由是否已完成决定。因此，完成后读档不显示固定外观；撤销后读档会恢复未完成柱的固定外观，而颜色遮罩/隐藏罩的解除状态不会回滚。本轮保持这些原规则，未将视觉隐藏改写成永久删除固定类型。

测试还通过默认底栏标准 Button 执行撤销，检查库存消耗、历史删除及再次读档，再完成同一目标柱并继续从已揭开的隐藏柱搬移。全过程保持正常本地操作权限，不打开整盘结算或失败界面，不生成金币、现金或 SDK 回复。

此验证覆盖上述默认入口与存档组合分支，不证明全部生产关卡可解、所有设备触摸输入、全部动画重入或原包逐像素视觉一致。未修改运行时代码。旧的独立效果测试仍保留更细的动画曲线/暂停计时验证。

Unity 2022.3.62f3 验证结果：`Library/local-special-screw-resume-play.log` 包含 `NUT_LOCAL_SPECIAL_SCREW_PLAY_PASS`，无编译错误或验证异常。当前 `Library/local-special-before-current.png`、`local-special-break-current.png`、`local-special-restored-current.png` 已逐张检查。原有用户存档已恢复，测试备份文件已移除。既有场景退出对象池诊断未由本轮处理。
