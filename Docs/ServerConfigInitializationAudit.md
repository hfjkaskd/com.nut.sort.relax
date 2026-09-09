# 配置初始化本体

依据当前 ServerConfigData.Init、TimeLSSUtil.IsToday、UserMgr.ChallengeCount 恢复非 SDK 部分。先将当前配置对象赋给当前用户。登录秒数先 FromUnixTimeSeconds 取日期，再与 DateTime.UtcNow.Date 比较；不使用上一轮提现 UI 的本地墙上时钟。

跨 UTC 日时捕获当前用户并写入 UTC 秒数，再对当前 UserLssInfo（非空时）调用已恢复的 RefreshQueueCount，之后捕获当前用户并设置 TodayChallengeTimes=Random.Range(5,11)。最后无论是否跨天，当前挑战次数 <=0 都再调用一次相同随机范围。保持中途回调时捕获写入目标与随后重读用户的顺序。

没有 LoginDay、自行添加的每日计数重置或保存。末尾 AndroidSdk.setKwaiCountry 按用户要求跳过，不更改现有 SDK 处理。初始化尾部流程负责完成标志、后续请求与保存。

回归覆盖同日/跨日、空提现账户、原版队列范围、第二次非正检查、其他计数保留、回调替换用户、非法时间与时钟异常顺序，并将实际本体接入上一轮保留的配置回调做组合验证。配置回调由测试显式调用，不自动请求或制造生产成功。正式主场景启动组合仍待完成。

验证：Unity 2022.3.62f3 完整回归 145 个 VALIDATION_PASS，日志 Library/server-config-initialization-validation.log；无编译错误或异常，进程已退出，玩家偏好备份已恢复。无视觉变更或新的生产启动完成声明。
