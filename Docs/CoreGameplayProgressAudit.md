# 核心过关进度

优先任务已转为核心玩法。默认 OriginalStartupFlow 缺少初始化完成、移动完成及完整过关消费接线，不能将已有规则验证通过视作默认场景可玩。

本轮先补 ClearanceRewardShowS2C.InitLss（0x9EFC28）的关卡状态应用：成功才写 UserLevel=gap_rank、Level=gap_all_gates+1、LevelSeed=0、LoginDay=ext_gap_logs、LoginDayCoin=gap_logs、TodayPassLevelCount=gap_day_gates，随后刷新 MainPanel。之后重新读当前用户 Level；等于 3 时清零 LuckyScrewDoneCount，分别读取两次本地秒时钟赋给幸运奖励、转盘时间。保留原生整数溢出、不重置 IsRandomLevelSeed、不增加保存、不发放货币。

这是外部过关结果的状态消费者，不是本地成功回复生成器。原版正式关卡进度是服务端返回驱动；第一关前三个教学 seed 则在已有 OriginalSuccessFlow 中本地推进。尚未将未返回的正式过关伪装成成功。

验证覆盖失败不触碰用户、真实写入顺序、界面刷新重入后重新读取用户、两个独立时间读取、缺失数据异常和溢出。默认场景核心循环接线及实际多关 Play 验证尚未完成。

本轮验证结果：core-clearance-progress-validation.log 为 158 个 PASS 标记并包含 NUT_CONTENT_VALIDATION_PASS；core-progress-play.log 包含 NUT_CORE_PROGRESS_PLAY_PASS。真实场景棋盘执行首关选取和转移，经原版两秒完成回调进入 Success，再重建教学 seed=1；显式过关返回应用后，下一关按原版索引选择加载，关卡和棋盘共同写入实际 PlayerPrefs。测试未手动设置 IsInitDone，但输入使用 Level.Operate，因此不证明默认场景屏幕输入已接通；完成消费由测试绑定。验证退出时已有对象池重设父节点警告仍在，未宣称退出零警告。偏好备份已恢复。

待决策：若按用户优先核心可玩的目标接入无服务端的本地关卡推进，需要明确它是 SDK 跳过模式下的本地行为，不能把它标作原版服务端返回或网赚收益。是否允许该行为已向用户询问；未获答复前不修改正式关卡推进语义。
