# 配置默认值与响应初始化链

ServerConfigData 构造函数恢复 18 个字段默认值，放入轻量 ScriptableObject 配置资产，由 OriginalUserDefaults 引用。标量依据 ARM64 常量；LSSLDR=[80,0,0,0,7,3,7,1,7,3] 与 LSSGPUL=[8,21,61,111] 依据原元数据初始化字节，并验证 SHA256 与原字段名哈希相等。LSSUPT=[90,120]，LSSNTY=true，LSS260820/LSSAB=false，ksCountry=ID-BR。每次构造分配独立数组文档，不污染配置资产或另一响应。

ServerConfigInfo.Request 使用共享缓存；存在缓存时直接同步返回，不重复 Init 或查国家。无缓存时 ConfigC2S.scrap_mark 使用当前 CountryInfo.Area，而非 CountryCode。收到空/null 字符串时回调 null，不改缓存；收到非空字符串后解析 ServerConfigData、发布共享缓存、调用已恢复的 ServerConfigData.Init、再返回当前缓存。这里没有 NO-0 成功信封检查。RequestMgr.Config 根据返回对象是否非 null 转为 bool，再进入已恢复的用户配置分流。

显式 Json.NET 字段投影避免反射赋值；按字段顺序和忽略大小写规则覆盖默认值，忽略未知字段。数组和字符串可显式为 null，非可空数值/布尔的 null 失败。网络和存档嵌套配置共用流式解析，保留重复字段处理顺序；不会先合并 JObject 而吞掉较早字段的转换错误。字面 null/空白解析为 null 并发布，随后保留原版 Init 前空引用错误；解析异常保留旧缓存；初始化异常保留新缓存及已经写入用户的数据；回调异常不回滚。SDK setKwaiCountry 保持跳过。

存档 ServerConfigData 同步改用同一字段投影，修复此前把未知字段原样保留、缺字段未补原版默认值的问题。旧 62 字段往返测试中的 opaque/flag 不是原版配置字段，预期改为固定 18 字段默认配置，其余存档字段往返检查保留。该变更不改存档键名，不主动重写用户存档。

Play 连接实际 UserSession、CountryState、UserStartup、ServerConfigRequests、UserConfigFlow 与 MessagePanel。注入的是原始配置字符串，检查发布、双阶段 Init（网络 Init 后启动尾链再次 Init）、初始化完成、收益请求及保存的顺序；也检查空响应弹窗和标准按钮确认后重新接收配置。原始传输及收益请求为明确夹具，生产启动与真实服务器仍未接通。

最终验证：server-config-stream-validation.log 输出 154 个 VALIDATION_PASS；server-config-stream-play.log 输出 NUT_SERVER_CONFIG_REQUESTS_PLAY_PASS。最终日志没有 C# 编译错误或未处理验证异常，Unity 均已退出，用户存档夹具已恢复且备份不存在。本轮仅修改配置与请求逻辑，没有改变 UI 视觉资产。初轮测试暴露旧存档测试预期、字符串 null token 表示及额外 JSON 内容的异常类型预期，均已按上述语义修正。
