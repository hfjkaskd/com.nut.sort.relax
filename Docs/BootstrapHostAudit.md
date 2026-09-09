# 启动协程实际宿主

依据 LuoSiSort.Start 恢复 60 FPS、当前线程 CurrentCulture 与 CurrentUICulture 两次独立 new CultureInfo("en-US") 赋值，然后 StartCoroutine。三个参数在 OriginalBootstrapHost Prefab 中序列化，主界面等待为 0.5 秒。

新增宿主根节点保持激活，原版场景提取的 LoadingPanel 放在子级；隐藏加载页不会停止宿主协程。Bind 必须在 Unity Start 前供应系统操作，不添加默认成功或 Editor 特例。SetLoading 直接驱动真实加载页，其他动作仍通过显式端口，SDK 端口保留现有处理。

Play 使用实际 Prefab、StartCoroutine、WaitUntil、WaitForSeconds 与加载页：分阶段保持国家/用户/LevelInfo 未就绪以验证门控，再释放；检查 0.5 秒主界面等待、请求/BGM 顺序及宿主在加载页隐藏后存活。系统操作端口是明确夹具，完整生产主场景自动绑定仍待完成。无原机像素对齐完成声明。

验证：Unity 2022.3.62f3 完整回归输出 150 个 VALIDATION_PASS；bootstrap-host-validation-fixed.log 未发现 C# 编译错误。bootstrap-host-play-fixed.log 输出 NUT_BOOTSTRAP_HOST_PLAY_PASS，Unity 已退出，用户存档夹具已恢复。首次编译暴露 World→UI 反向依赖，宿主已移入 UI 程序集并保留 GUID。首次 Play 固定 1.5 秒终点检查失败，夹具改为等待真实加载页隐藏后检查完整动作序列，仍保留 180 秒总超时；运行时动画和延迟未为测试修改。
