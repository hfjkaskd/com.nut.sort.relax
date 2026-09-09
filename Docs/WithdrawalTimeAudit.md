# 提现步骤时间显示

从当前 TimeLSSUtil.LocalTime / LocalTimeSeconds、构造函数以及 UILSSUtil.TimeFormat 和 CountryLssInfo.CountryLanguageCode 恢复。

时间基准是 new DateTime(1970,1,1)，Kind 未指定。获取 DateTime.Now.Ticks 减去基准 ticks，先整数除以 10000，再除以 1000；负数向零截断。不是 DateTimeOffset.Now.ToUnixTimeSeconds，不进行 UTC 转换。

显示先执行 DateTimeOffset.FromUnixTimeSeconds(seconds).DateTime，再即时读取地区语言组合，new CultureInfo(LanguageCode + "-" + Code)，最后 DateTime.ToString(format,culture)。源路径没有 ToLocalTime，也没有文化信息回退；非法时间在地区读取前失败，地区读取失败阻止格式化。每次格式化重读地区，支持地区测试切换后更新。操作属于低频面板刷新，遵循原版每次创建 CultureInfo，不放入 Update。

新增测试覆盖正负小数秒、DateTime Kind 不触发转换、US/GB/DE 日期顺序、动态地区、时间范围和回调异常顺序。首阶段实际 Play 链路使用本机实时时钟与已恢复的格式化工具，地区 en-US、奖励数据与最终 UI18 目的地仍是显式夹具。正式 UserMgr 地区与启动路由连接未完成，SDK 不变。

验证：完整回归 141 个 VALIDATION_PASS，日志 Library/withdrawal-time-validation.log。首次 Play 在返回动画阶段达到 150 秒测试观察时限；仅把测试观察上限改为 300 秒并补充时间诊断后，Library/withdrawal-time-final-play.log 已通过。已查看本轮 480×1040 最新截图，步骤时间为本机实时的 en-US 日期与 AM/PM 格式，布局正常；运行时动画参数未变。
