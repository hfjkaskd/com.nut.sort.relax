# 注册响应与用户 ID 替换

恢复 RegisterInfo.Request/接收闭包（0x9F2374、0x9F2574）、RegisterS2C.InitLss（0x9F2650）及 RequestMgr.Register 结果闭包（0x9EDC50）。请求使用当前 CountryInfo.Area，注册请求不缓存、不合并。checkSystemProp/checkNetProp 按用户要求继续留在跳过的 SDK 边界，传输仍由显式端口提供。

响应使用 Json.NET 显式流式字段读取，避免反射赋值。保留 message_status、kinetic_gap、64 位 Time，以及 kinetic_data.scrap_mark/copp_uuid；字段名忽略大小写、未知字段跳过。重复嵌套对象按 Json.NET 默认对象复用规则更新已有字段，显式 null 清除对象。字符串字段通过原生 JsonTextReader.ReadAsString 读取。

非空响应解析后先 InitLss（只求值 IsSuccess，无其他副作用），再向底层调用方返回响应，包括失败响应；空/null 字符串给底层回调 null，byte[] 参数不读取。字面 null/空白解析为 null 后保持原版空引用错误，不擅自变为普通注册失败。解析和回调错误直接传播。

RequestMgr 只在 kinetic_gap 精确为 NO-0 时读取当前用户，并把 UserId 替换为返回的 copp_uuid，然后回调 true。message_status 不参与判断，响应 scrap_mark 不更新用户地区。空或缺失 ID 仍可覆盖原 ID 为对应值，成功响应缺失 kinetic_data 则失败且不触发成功回调；非成功响应回调 false 且不读取用户。回调抛错不回滚 ID。身份更新不自行保存，由已恢复的注册/配置后续链路按原时机保存。

现有首次用户 Play 夹具已升级为原始注册与配置字符串输入：先失败弹窗重试，注册成功替换服务器 ID 后进入配置，请求收益并保存含服务器 ID 的用户数据，收益回调到达才完成初始化。原始传输、初始设备身份和收益响应仍为明确夹具，默认生产入口与真实服务器未接通。没有新增伪造成功或 Editor 专用运行时分支。

验证结果：registration-requests-validation-final.log 输出 156 个 VALIDATION_PASS；registration-requests-play.log 输出 NUT_USER_REGISTRATION_FLOW_PLAY_PASS，覆盖实际首次用户会话中的原始注册响应及服务端 ID 后续保存。最终日志无 C# 编译错误或未处理验证异常，Unity 已退出，真实用户存档夹具已恢复且备份不存在。初轮不完整 JSON 测试预期已校正为解析器实际抛出的 JsonSerializationException，未吞掉异常。本轮未修改视觉资产。
