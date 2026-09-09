# 螺柱完成事件到胜利的原生衔接

当前 ARM64 的 ScrewInfo.DoneEvent（0xA09BC4）不是简单的 Success 回调。新增 OriginalScrewDoneFlow 恢复其同步分派，并由 OriginalGameScene.BindScrewCompletion 将已有移动完成时序直接接到该分派器和真实 game.Success。此前首关验证通过测试回调主动调用 Success；现在改为使用场景消费者，验证本身不再代为完成这一步。

原生顺序：显示关卡达到 3 时先触发金币飞行，再读取完成金币数并调用 AddGold(amount,true,!completedBoard)。随后重新读取显示关卡；达到 4 且完成硬币数严格大于零时触发硬币飞行，再次读取数量后 AddCoin(amount,true)。捕获的整盘成功为真时直接进入 Success 并返回，不读取后续可选分支。

非胜利分支按顺序检查：实际 Level>=3 且当前无面板时，LuckyScrewDoneCount>=LSSLR2 或时间差>=LSSLR1 触发幸运奖励请求；未触发才检查 LuckyDrawScrewDoneTimes>=LSSLD2 或时间差>=LSSLD1 并展示面板 12。触发前仅清零对应计数，再重新读取时钟写入对应上次时间。都未触发时，未完成评分、实际 Level>=LSSR1 且无面板才先写 IsCompleteAppRatingPanel=true，再展示面板 2。保留短路、包含边界、长整数减法溢出和多次动态读取，不增加 SaveData。

金币/硬币数量与写入、飞行表现、配置读取、面板、时钟及幸运请求由 IOriginalScrewDoneEffects 保留显式消费边界。这一轮没有实现虚构收益，没有补造缺失服务端文档，也没有将未返回的请求视为成功。SDK 转盘遥测继续跳过。幸运请求返回后的面板分派、实际账号和飞行组件的统一组合仍需后续接线，不以接口存在宣称这些消费者已完成。

验证：Library/core-screw-done-validation.log 完整回归 163 个 PASS 标记，含 NUT_SCREW_DONE_VALIDATION_PASS 和最终 NUT_CONTENT_VALIDATION_PASS。覆盖早期胜利不触碰奖励/配置、金币同步参数、飞行后重新读取硬币和关卡、NaN 条件、可选分支优先级、面板阻塞、计数及计时相等边界、两次时钟、评分标记和溢出。

Library/core-screw-done-play.log 包含 NUT_CORE_PROGRESS_PLAY_PASS。实际原始首关通过螺母转移、既有两秒回调、场景 DoneEvent、Success 和 InitLevel 自动重建下一教学种子，期间任何晚期奖励/配置调用都会使测试失败。随后通过明确的服务端结果夹具验证原有正式关卡数据应用和存档；此结果不是生产模拟响应。测试结束恢复原用户偏好，备份文件已移除。

未完成：默认 StartupFlow 的完整初始化/引导/面板绑定、正式过关请求与后续推进、DoneEvent 的账号及视觉消费者统一接线。此轮验证的是核心完成链的真实分派和首段教学推进，不等同于默认启动后全生命周期可玩，也不宣称整体 1:1。
