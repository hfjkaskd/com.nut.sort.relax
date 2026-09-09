# 首阶段实际视图及提示返回链路

依据当前 TXProgress0Panel Prefab 和已恢复的原版控制/动画流程新增 OriginalWithdrawalStageZeroPanel 与 UIName 23 宿主。39 个源物体保持层级，69 个 RectTransform/CanvasRenderer 块原值不变，视觉/Button 组件除官方脚本映射外保留全部字段。三处加载旋转替换为现有 Unity 原生旋转组件，原值为 3 秒、线性、Z -360 无限循环。两颗标准 Button 代码绑定，反馈参数沿用源面板配置。

Gold 引用在源 Prefab 为空；仅步骤 1 和 2 含 Content/Time。视图保留这些缺省，不生成不存在的静态 UI。描述、客服编号、目标提示通过源本地化配置刷新。

动画参数序列化在 Prefab，包括浮点相加所得 ThirdRowDelay 0.90000004。首次进入调用已有首段动画，第二行完成后在全局非缩放时钟延迟 2 秒显示 33；提示确认关闭过程中实时取得实际 23 面板并继续动画。第三步加载标记按原版保留，第四步之后显露确认按钮。重复刷新不重播首次动画。确认与关闭遵守现有控制器的不同回调语义。

Play 集成夹具使用真实 23/33 Prefab 和宿主；时间、奖励数据与最终 UI18 目的地仍是明确测试边界。正式初始化、时间格式工具和主流程路由尚未接通，本轮不构成全生命周期完成证据。SDK 处理保持原样。

逐字段核对：32 个视觉/Button 组件保持源值，仅映射官方脚本。

验证结果：Unity 2022.3.62f3 完整回归 140 个 VALIDATION_PASS，日志 Library/withdrawal-stage-zero-panel-final-validation.log。首次离线运行因测试未注册 Awake 调度失败，修正仅限测试夹具；真实 Play 自动注册已通过。Play 日志 Library/withdrawal-stage-zero-play.log，当前 480×1040 截图 withdrawal-stage-zero-current.png 与 withdrawal-stage-zero-hint-current.png 已逐张查看，客服、步骤文本、第三步加载、Continue 及遮罩覆盖显示完整。当前截图不替代原机像素对照。玩家偏好备份已恢复。
