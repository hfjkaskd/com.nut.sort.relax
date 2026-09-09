# 启动收益信息请求管理层

恢复 RequestMgr.GoldRewardInfo（0x9EC7A0）及结果/确认闭包（0x9EDDB8、0x9EE08C）。将 isFaceRefresh 原样交给底层请求。结果为空或 IsSuccess=false 且强制刷新时，显示 MessagePanel 文本 75、isCanClose=false，保留当前请求闭包内缓存的重试 Action；确认后使用原调用方和原强制刷新标志重新请求。该失败分支不执行完成回调或金币提示刷新。

其他情况先调用完成回调（若非 null），然后查询当前 MainPanel 的 Unity 存活状态，有主界面时刷新 Top.GoldItem.RefreshHint。完成回调收到一个新建的 GoldRewardTargetS2C 对应对象，只赋 kinetic_data=当前 UserLocalData.GoldRewardTargetS2CData；不是传入网络响应，头部字符串仍 null、Time=0，IsSuccess 因此为 false。使用单独结果对象持有 JObject 引用，避免把同一 JObject 装入另一个 JObject 时产生隐式克隆。回调和用户数据共享同一文档。

非强制请求即使失败也会继续上述回调和提示刷新，不新增重试。回调为 null 时不创建包装对象、不读取用户，但仍检查主界面。回调可移除主界面；存活检查在回调之后进行。回调、弹窗或刷新异常均不吞掉，不额外保存或改变初始化标志。

首次注册 Play 已把该层接在 OriginalUserRegistrationFlow 之前：收益请求发起后保存，空收益响应显示重试框并保持初始化未完成；标准 Button 确认只重试收益请求，成功结果才释放原注册回调。UserMgr 注册回调本身仍不读取参数；“空回调参数可设置完成”不等于“空网络响应可直接完成”，上游强制请求会先拦截后者。

本轮底层收益响应解析和 GoldRewardTargetS2CData.Init 尚为明确端口，Play 注入预备的用户收益文档及类型化响应进行管理层验证。生产启动整合、真实传输仍待完成，SDK 保持现有处理；不把这一层通过当作完整收益数据链完成。

验证记录：完整 Editor 验证日志 gold-reward-info-flow-validation.log 包含本层及 NUT_CONTENT_VALIDATION_PASS，共 157 个 PASS 标记。新增 Play 重试用例尚未完成：本次 GUI Unity 停留在 project path 启动阶段，没有进入 executeMethod，也未创建偏好备份；不得计作 Play 通过。按用户最新优先级，保存本轮后转入核心玩法接线。

后续补验：改用 batchmode 保留图形后，gold-reward-info-flow-play-batch.log 已输出 NUT_USER_REGISTRATION_FLOW_PLAY_PASS，偏好备份恢复，进程退出。仅为此前记录的管理层用例收尾，没有继续开发外围收益功能。
