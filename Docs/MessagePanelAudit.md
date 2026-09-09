# 启动全局消息面板

UIMgr.Init 先调用场景内 MessagePanel.Init，再初始化 SystemInfo。恢复 MessagePanel 的原场景子树为独立 Prefab：7 个对象、30 个序列化块，布局、颜色、字体、图片及标准 Button 参数来自当前逆向场景。仅替换脚本为 Unity 官方组件及重建组件、将根父引用与根顺序改为独立 Prefab。新增 bg0/title 图片及 Sprite，不引入外部插件程序集。

Init（0x9E1814）只 AddListener，不清空旧监听，不接通用点击门控、音效或缩放。重复初始化会追加回调。OkCallback（0x9E18A8）先隐藏，再读取并调用当前可空回调，不清空回调；回调可重新显示面板，异常直接传播。

Show(int)（0x9E18EC）先对子级（含隐藏对象）的 label_ 文本执行原版本地化，再读取指定文本，调用字符串重载。Show(string)（0x9E1A04）按文本赋值、回调替换、激活顺序执行，不做本地化。两种重载均不使用 isCanClose；没有新增关闭按钮或关闭策略。

这是主启动 UI 初始化缺失的实际组件；当前生产 UIMgr 组合及自动入口尚未接通。本轮 Play 的调用方是显式夹具，不证明全部生产异常/消息触发分支已接通。SDK 处理保持原样，未实现调试 SystemInfo 等边缘功能。

验证：message-panel-validation.log 输出 151 个 VALIDATION_PASS；message-panel-play.log 输出 NUT_MESSAGE_PANEL_PLAY_PASS。两次 Unity 均已退出，存档夹具恢复且备份不存在。逐块对照确认 14 个 RectTransform/CanvasRenderer 块与原场景一致（根父/顺序除外），8 个视觉/Button 块除官方脚本映射外一致。已查看本轮生成的 Library/ValidationCaptures/message-panel-current.png：Notice 标题、原版图案、文本 Please check the email 与 Confirm 按钮完整显示。该截图来自当前工程，不等于原机逐像素一致证明。
