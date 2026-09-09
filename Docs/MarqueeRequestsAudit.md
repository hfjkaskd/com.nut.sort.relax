# 启动 PMD 响应与主界面数据

恢复 PMDInfo.Request、接收闭包、PMDS2C.InitLss、RequestMgr.PMD 和 Init 中 PMD 调用。传输保留显式端口，version_value 参数为原版 -1；不发送真实服务器请求、不制造运行时响应。SDK syncNotifyCode 按用户要求跳过。

请求读取全局共享的完整响应缓存。非空缓存同步返回，不检查 IsSuccess、不重新 Init、不发送请求；无缓存时每次调用都发送，不添加并发抑制。收到非空字符串时直接使用 Unity 官方 JsonUtility.FromJson，先发布缓存，再判断 kinetic_gap 是否精确为 NO-0，成功后 Init 数据、调用回调。message_status 不决定成功。空字符串和 null 不改变缓存，底层回调收到 null；失败响应仍写缓存并给本次底层回调 null，之后请求可直接返回这个失败对象。解析异常保留旧缓存；Init 异常保留新缓存和已发生的数据修改；回调异常不撤销缓存。多个请求按实际到达顺序覆盖，不加新旧请求检查。

OriginalMarqueeData/Item 增加 Serializable 并使用原版公开字段名，现有可读名称改为属性，保持现有调用方与列表/条目引用关系。网络路径使用 JsonUtility，避免把现有 JObject 投影的忽略大小写行为错误套入原版 Unity 解析。未引入反射调用、第三方序列化程序集或默认响应。

RequestMgr.PMD 的闭包在返回对象和调用方回调均非 null 时强制转换到 PMDS2CInfo（条目），而 PMDInfo 实际返回 PMDS2C（响应）；两者无继承关系。保留原版该路径的 InvalidCastException；启动及缺失数据请求的回调为 null，不触发此转换。没有擅自改为挑选一个条目作为回调结果。

选择器读取同一缓存，缺失时调用 PMD 并立即返回 null，即使传输同步完成也不会在该次选择返回新条目。缓存存在但 kinetic_data 为 null 时保留失败，不伪装成缺失缓存重试。实际主界面 Top 的跑马灯与玩家收益提示通过同一个 Next 提供器读取已初始化的数据。

Play 测试显式注入响应文本，检查等待响应期间无新跑马灯、释放后实际条目文字/支付图标/横向移动及收益提示；其他系统操作仍是夹具。默认生产启动与传输组合仍待完成，此项不证明生产请求、真实收益或所有地区分支已接通。

验证结果：marquee-requests-validation.log 共 152 个 VALIDATION_PASS；marquee-requests-play.log 输出 NUT_MARQUEE_REQUESTS_PLAY_PASS，最终日志未发现 C# 编译错误或未处理验证异常。已查看本轮 Library/ValidationCaptures/marquee-response-current.png：原版主界面跑马灯条目、支付图标及收益提示已显示。截图中的金额与名称来自明确测试数据，不表示真实提现、真实服务器响应或原机像素对齐完成。
