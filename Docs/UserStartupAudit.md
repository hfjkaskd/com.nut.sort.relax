# 用户启动流程组合

将现有存档读取、用户地区初始化、配置本体和初始化尾部组合为 OriginalUserStartup，SDK 标识与请求仍由宿主显式注入，没有自动响应。

空存档先构造默认用户，依次赋值 SDK 标识输入、现有原版名字生成器结果、第一次 UTC 秒数为 RegisterTime、第二次 UTC 秒数为 LastGetEveryDayGift，然后才发布存档对象并初始化地区。LoginTime 在此处不赋值。非空存档只解析，不重复身份初始化；非空 null/空白仍遵循原解析行为，不当作新用户。

OriginalUserStore 新增可选的新用户初始化回调，原来的纯存档/音频调用保持现有处理；OriginalUserStartup 显式使用该回调。新用户回调或解析失败之前不会替换已发布 Store。随后使用真实地区和配置初始化流程，等待显式请求回调才设置用户初始化完成。

集成测试覆盖新用户顺序、两次独立时钟、发布时机、姓名/标识失败、已有存档不调用新用户步骤、实际配置完成回调与保存、null 和损坏存档。当前主场景仍由 OriginalUserSession 持有旧存档加载入口；此次完成可绑定的用户启动组合，尚未替换主场景/SDK 宿主，也不声明玩家完整启动链路完成。

验证：完整 Unity 回归 146 个 VALIDATION_PASS，日志 Library/user-startup-final-validation.log。初次检查的损坏存档异常类型预期已修正为现有解析器的 JsonSerializationException，未更改运行时解析行为。最终无编译错误或异常，偏好备份恢复。
