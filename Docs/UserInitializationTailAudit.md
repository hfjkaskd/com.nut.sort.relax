# UserMgr 初始化后半段

依据当前 UserMgr.Init、配置回调及空奖励回调恢复本地数据赋值之后的流程。先初始化用户地区，当前 ServerConfigData 非空则调用 Config(callback,false)，为空调用 Register。调用返回后重读当前 ComeOnGold，非空（包括空白字符）则把当前用户 GuideIndex 设为 10 并保存。原版 Application.isEditor 的返回值未使用，不添加 Editor 分支。

Config 回调不检查 bool 参数。重读当前用户配置并调用其 Init，之后设置 IsInitDone=true，再请求 GoldRewardInfo(cachedEmptyCallback,false)，最后保存。回调对象静态复用，奖励返回内容在此处不处理。每一步异常都阻止后续步骤，但已发生的状态变化保留。同步返回时此处保存先于外层 ComeOnGold 的保存。

回归覆盖注册/配置分支、保留请求回调、同步返回、两种布尔值、配置与用户替换、空白触发、空配置、初始化/请求异常及空奖励回调。请求与 ServerConfigData.Init 均为必需注入端口，没有新增 SDK 调用或自动响应。正式初始化尚未组合到主场景，配置 Init 本体仍待恢复；本轮不能证明启动生命周期完成。

验证：本机 Unity 2022.3.62f3 完整回归 144 个 VALIDATION_PASS，日志 Library/user-initialization-tail-validation.log；没有编译错误或异常，进程已退出，玩家偏好备份已恢复。本轮仅控制流程变化，未声明新的视觉或生产启动验证。
