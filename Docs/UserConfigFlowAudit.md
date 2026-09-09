# 用户配置失败与确认重试

依据 UserMgr.Config（0x9C2264）、结果闭包（0x9C3F84）和确认闭包（0x9C40CC）恢复 OriginalUserConfigFlow。底层 RequestMgr.Config 将非 null 对象转换为 true、null 转换为 false；本轮将这个布尔结果作为显式请求端口，没有制造成功结果。

当结果 false 且 isPopMessageBox=true 时，显示原版 MessagePanel 文本 75，isCanClose=false，不调用外部完成回调，也不自动重试。当前请求闭包只创建一次重试 Action，重复失败响应复用此 Action。玩家确认后重新调用 Config，保留最初完成回调并强制开启弹窗。确认动作通过原版标准 Button 执行，先隐藏面板再发起重试；同步重试失败可在同一次点击中重新显示面板，保留显示状态及新回调。

成功或禁止弹窗时，把原始 bool 传递给可空外部回调，异常不吞掉。UserMgr.Init 的配置调用传 false；已有启动尾链不检查完成 bool，而是初始化当前已保存的配置、设置用户初始化完成、请求收益信息并保存。因此启动配置失败不是弹重试框分支，不能擅自增加弹窗或无限等待。

实际 Play 将 Config 方法绑定到 OriginalUserStartup 和真实 OriginalUserSession，并使用实际场景 MessagePanel Prefab 验证允许弹窗的失败、确认、再次失败和成功链路。服务器布尔结果、已有配置及地区是显式夹具；生产服务器配置解析、默认启动整合仍待完成。SDK 处理未变。

验证结果：user-config-flow-validation.log 输出 153 个 VALIDATION_PASS；user-config-flow-play.log 输出 NUT_USER_CONFIG_FLOW_PLAY_PASS。两次 Unity 均已退出，存档夹具已恢复且备份不存在。已查看本轮生成的 Library/ValidationCaptures/config-retry-current.png：原版 Notice、网络失败文本 75 及 Confirm 按钮完整显示。此画面仅证明当前工程实际渲染，未宣称原机像素对齐或真实网络请求已完成。
