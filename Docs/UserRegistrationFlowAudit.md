# 首次用户注册初始化分支

恢复 UserMgr.Register（0x9C2194）及三个回调（0x9C3AE4、0x9C3C28、0x9C3D18）。注册失败显示原版 MessagePanel 文本 75，isCanClose=false，确认后重新调用 Register；不自动重试、不重新生成身份、不保存、不设置完成。注册成功调用 Config(callback,true)，使配置失败走原版确认重试分支。

配置回调为 false 时直接返回；true 时先 GoldRewardInfo(callback,true)，随后 SaveData。IsInitDone 在收益信息回调到达时才设为 true，回调内容完全未读取，因此 null 回调参数也设置完成。若回调同步发生，完成标志先于保存；若请求异步，则先保存且保持未完成。请求抛错时不保存，保存抛错时已发起的收益回调仍可完成初始化。重复 Register 不复位 IsInitDone。

这不同于已有玩家的启动尾链：已有配置分支先设置完成再请求收益，首次注册分支必须等待收益回调。两条路径保持独立的原版顺序。

Play 显式构造空存档的新用户，将真实 UserSession/UserStartup/CountryState、原始配置响应解析、UserConfigFlow 及 MessagePanel 串联。先注册失败并通过标准 Button 重试，再配置失败并重试，最后收到配置、请求收益、保存，并保持等待直到收益回调。原始真实存档通过既有夹具备份恢复。设备身份和收益响应仍为显式夹具。后续已在同一 Play 链路接入原始注册响应解析与用户 ID 替换（见 RegistrationRequestsAudit.md），传输及默认生产入口仍未接通。SDK 保持当前处理，没有伪造生产成功。

验证：user-registration-flow-validation.log 输出 155 个 VALIDATION_PASS；user-registration-flow-play.log 输出 NUT_USER_REGISTRATION_FLOW_PLAY_PASS，最终日志没有 C# 编译错误或未处理验证异常。Unity 均已退出，存档备份已恢复且文件不存在。本轮没有修改视觉资源或使用旧截图判断效果。
