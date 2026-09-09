# 启动加载页控制

依据当前场景 LoadingPanel 的 Start、Update、SetState 及三个完成回调恢复。Start 仅把 value 清零并对 Logo 调用原生尺寸设置，不触发进度显示。SetState(true) 先更新 isShow，value=0 后显示，再激活对象。Update 在 isShow 且 activeSelf 时按 deltaTime*0.5 增加，最多 0.9。

SetState(false) 先清 isShow，然后从调用时 value 到 1 创建 0.5 秒默认 OutQuad 的独立浮点动画。每次更新显示；完成时再次设置 1 并显示，再调用全局缩放时间延迟 0.2 秒停用对象。重复 false 不取消旧动画，再次 true 也不取消旧隐藏回调。游戏启动协程在请求隐藏后继续下一帧的请求管理器/BGM 初始化，不能等同于加载页已消失。

参数由可序列化 Timing 提供。原版显示还需 Bar.fillAmount、移动 lS 的 x=fill*750/y=0，以及 value*100 的 F0 百分比文本；实际源场景视图已恢复为 BootLoadingPanel.prefab，8 个物体、33 个块；仅根父级/排序调整为可实例化 Prefab，其他布局和视觉字段保留原值。使用官方 Image/TMP，真实 Start 调整 Logo 原生尺寸，全局驱动补满动画和缩放时间隐藏。SDK 不变。

回归覆盖顺序、速率上限、中点 easing、完成时两次显示、延迟隐藏、重复和重开、非活动动画继续、Start 时序及显示失败的部分状态保留。

验证：本机 Unity 2022.3.62f3 完整回归 149 个 VALIDATION_PASS，日志 Library/boot-loading-validation.log；无编译错误或异常，玩家偏好备份恢复。当前为控制器验证，不替代真实加载页视图和启动 Play。

本轮逐字段核对：16 个布局/渲染块与 8 个视觉组件保留源字段（根父级/顺序及脚本映射除外）。停留在相同百分比时缓存 F0 结果，数值或当前 CultureInfo 引用改变后重新格式化，减少 90% 等待期间临时字符串。完整回归 150 个 VALIDATION_PASS，日志 Library/boot-loading-panel-validation.log。

实际 Play 通过：Library/boot-loading-panel-play.log。已查看本轮 480×1040 的 Library/ValidationCaptures/boot-loading-current.png，源 Logo、背景、进度条和 90% 标记显示完整；真实 Start/Update、完成动画及延迟停用通过。当前截图不替代原机像素对照，自动启动协程场景绑定仍待完成。
