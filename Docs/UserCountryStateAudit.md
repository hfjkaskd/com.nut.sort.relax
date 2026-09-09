# UserMgr 地区初始化片段

依据当前 UserMgr.Init 中本地数据加载之后、Config/Register 之前的地区片段恢复。先通过 ConfigTable.GetCountryInfo(CountryCode) 查询并赋给 CountryInfo；比较原始 Area 和 CountryInfo.Area。不同则记录 "Area" + Area + " != CountryInfo.Area:" + CountryInfo.Area，再重读当前 CountryInfo 与 Area 执行赋值。

国家对象是表内共享引用，不能复制，也不能把来源 Area 替换成表内 Area。日志失败时国家赋值已发生但 Area 尚未修改；查询失败保留之前国家对象。日志回调期间若改变目标或 Area，写入使用改变后的值。没有新增保存、SDK 请求或配置成功回调。

回归覆盖匹配、不同、重复初始化、换国、共享配置变动、日志中途变更、空 Area、异常边界及日期文化跟随当前用户地区。首阶段 Play 现在先按该流程初始化地区，再使用用户国家对象格式化时间。US/USA 来源仍是显式测试输入，正式 SDK 来源、完整 UserMgr 初始化、AB/GM 路由未完成；用户要求跳过 SDK 的处理方式不变。

验证：完整 Unity 回归 143 个 VALIDATION_PASS（Library/user-country-state-validation.log），实际首阶段/提示页往返 Play 通过（Library/user-country-state-play.log），玩家偏好备份已恢复。
