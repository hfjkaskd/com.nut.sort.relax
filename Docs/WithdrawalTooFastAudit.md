# 操作过快提示控制链路（UIName 33）

依据当前逆向资料 TXTooFastHintPanel 的 Init、Refresh 与确认回调，恢复控制逻辑。初始化先执行基础面板初始化，再通过代码绑定确认按钮。

Refresh 首先设置首阶段共享 IsShowOnlineTimeHint 标志，再捕获奖励列表第三行。依次写入本地化 165（目标关卡）、166（固定 10）、关卡填充、视频填充 1、关卡分数字符串、固定 10/10。关卡两次读取 ShowLevel，目标字段在各次使用时从捕获行重读。控制器保留未钳制浮点除法；实际 Image setter 才负责 fillAmount 范围处理。视频数值是原版固定展示，不代表实际广告完成。

确认先启动关闭，再即时查找存活的 UIName 23 并调用 PlayProgress；不等待隐藏，也不自动创建丢失的首阶段面板。关闭失败阻止后续查找，目标缺失则失败。没有新增保存、引导或 SDK 调用。

新增回归覆盖初始化、赋值顺序、回调期间替换奖励容器/修改原行、两次关卡读取、零分母、整数溢出、关闭后动态解析目标、异常顺序及标志保留。校验恢复进入前的共享标志。

已恢复 33 的实际视图与面板宿主。源 Prefab 的 19 个 GameObject、37 个 RectTransform/CanvasRenderer 块保持原值；除官方脚本映射外，19 个 Image/TMP/Button 组件的全部序列化字段保持原值。资源包含原版进度条、背景、标题与绿色描边字体材质。新增唯一标准 Button 的反馈组件并通过代码绑定，不添加静态运行时布局。宿主注册后初始化和刷新，关闭动画完成才移除，隐藏后仍使用原版 2.5 秒队列调度。

首阶段 UIName 23 的实际视图和宿主已随后恢复，新增真实 23→33→23 Play 往返验证，详见 WithdrawalStageZeroViewAudit.md。时间工具已随后恢复并通过首阶段 Play 验证；正式初始化与最终 UI18 目的地仍未接通，不能作为完整生产链路或原机像素级 1:1 的完成证据。

验证结果：本机 Unity 2022.3.62f3 完整回归输出 138 个 VALIDATION_PASS 标记，包含新增提示控制器校验及总入口通过；进程正常结束，玩家偏好备份已恢复。日志：Library/withdrawal-too-fast-validation.log。

本轮验证：完整 Unity 回归 139 个 VALIDATION_PASS，包含实际 Prefab、字体/图片引用、Button 门控、进度条钳制及关闭顺序。日志 Library/withdrawal-too-fast-panel-validation.log。

Play 验证通过：Library/withdrawal-too-fast-play.log。已查看本轮生成的 Library/ValidationCaptures/withdrawal-too-fast-current.png（480×1040），标题、正文、6/12 与 10/10 进度条及确认按钮显示完整；截图仅证明当前工程渲染，不替代原机对照。实际 Button 立即调用测试返回目标，关闭动画完成后宿主移除面板。
