# 实际用户会话启动连接

OriginalUserSession 新增显式 StartUser 入口。先保留当前 Store，再绑定待执行的 OriginalUserStartup，然后执行初始化。Store 从该启动对象已经发布的 Store 读取；新 Store 尚未发布或加载失败时保留先前 Store。该机制不判断 Editor，所有平台同一路径。

因此同步 Config 回调也能通过实际音频/场景读取新 Store；延迟回调共享同一个用户。原音频 Initialize 不重建已经发布的 Store。用户配置完成标志以 IsUserInitDone 暴露，与关卡/UI 初始化 IsInitDone 分开，不能直接把配置完成当作可操作游戏。

离线和实际 Play 验证同步/延迟、失败读取保留、音频状态共享、游戏场景保存当前棋盘及重复音频初始化。Play 使用明确的已存配置和 US/USA 输入，保留原有请求端口，不自动造 SDK 回应。主场景自动启动调用方、完整 UI 完成门控仍待接通；本轮提供实际会话宿主入口，不声明生产生命周期完整。

验证：完整 Unity 回归 147 个 VALIDATION_PASS（Library/user-session-startup-validation.log）；实际 Play 通过（Library/user-session-startup-play.log），配置和国家输入为显式夹具，玩家偏好备份已恢复。没有视觉变更。
