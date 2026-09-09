# 原版启动协程顺序

根据当前 LuoSiSort.Init MoveNext 的 12 项原生跳转表还原状态顺序，而非按反汇编地址顺序猜测：显示加载页→下一帧→初始化 UI→下一帧→初始化表→下一帧→现有 SDK 初始化边界→下一帧→初始化池→等待 CountryCode 非空→初始化用户→下一帧→初始化音频→等待用户 IsInitDone→下一帧→初始化场景→等待 LuoSiSortMgr.LevelInfo 非空→显示并赋值 MainPanel→缩放时间等待 0.5 秒→隐藏加载页→下一帧→初始化 RequestMgr→播放 BGM。

CountryCode 使用 IsNullOrEmpty，空白不被当作空。三个等待条件都是实时读取；LevelInfo 是关卡配置引用，不是关卡动画结束或游戏可操作状态。不得在用户完成配置时直接设置关卡/UI 初始化完成。

使用 Unity 原生 IEnumerator、WaitUntil、WaitForSeconds 实现，延迟由宿主传入原值 0.5，后续应序列化在场景/Prefab。所有操作都是显式必需端口；SDK 端口仅保留调用方当前处理，不新增实现。没有反射、Editor 兜底或超时放行。

回归逐次检查 MoveNext 产物、每帧顺序、延迟前后操作、实时等待谓词、空白国家、异常中断与重复完成。当前尚未将这组端口全部绑定到主场景自动入口，加载页/UI 首屏及原版场景初始化时序还需组合验证；本轮不宣称自动启动已经完成。

验证：Unity 2022.3.62f3 完整回归 148 个 VALIDATION_PASS（Library/bootstrap-validation.log），无编译错误或异常；偏好备份已恢复。当前验证为协程推进和谓词检查，不替代实际自动启动端到端 Play。
