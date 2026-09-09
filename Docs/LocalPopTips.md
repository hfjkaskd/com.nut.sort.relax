# 核心道具的原版浮动提示

差异：默认本地入口把撤销无历史和加螺杆达到上限转换成需要关闭的模态卡片，阻止继续操作。源 UIMgr.ShowLssPopTip（0x9F8450 / 0x9F85F4）在 TopCanvas 下独立实例化 Tip，写入原表文案，一秒后由管理器定时销毁（0x9F8A1C）；它不登记到 Panels，因此不会触发 Level.Update 的面板阻挡条件。Tip.Update（0x9B6F74）每帧按 Vector3.up × Time.deltaTime × 100 增加 localPosition，没有额外淡出或关闭按钮。

已恢复原 Tip 预制体和对应 717×127 的 tip_bg 精灵/纹理，重绑官方 Image/TMP 与原字体材质，保留根 800×100、子文本布局和图形射线设置。它是不可点击的提示，不添加伪按钮或自定义点击处理。OriginalStartupFlow 通过序列化引用使用已有 TopUICanvas，未在运行时创建静态层级。

LocalGameplayController.Tools 直接读取源文案 ID 3（英文 Not cleared）和 5（英文 Not enough space），加载原提示预制体；速度 100 和寿命 1 保存在预制体。各提示实例和定时销毁相互独立。缺少库存时的 SDK 跳过说明仍使用原有本地卡片，不改变广告/奖励处理。

LocalPopTipPlayValidation 从默认存档入口开始，通过真实 EventSystem/GraphicRaycaster 命中底栏标准 Button。覆盖源提示文字/TopCanvas/背景、无模态和无道具扣费、提示期间真实世界选择、每秒 100 单位的本地上移、重复提示独立存在与销毁、暂停 scaled time 时移动和寿命同时暂停。机制/库存均是显式测试夹具，结束恢复原用户存档。

Library/local-pop-tip-play.log 包含 NUT_LOCAL_POP_TIP_PLAY_PASS。最新 Library/local-pop-tip-current.png 已检查，显示两个独立提示重叠时的原层级效果。LocalToolsPlayValidation 已更新原无历史分支预期；Library/local-pop-tip-tools-play.log 包含 NUT_LOCAL_TOOLS_PLAY_PASS，涵盖实际搬移、撤销、交换、加螺杆、库存存档、SDK 不可用说明及普通重开。

本轮不证明全部画面或设备输入已达 1:1；该轮发现的 Android 平台 Editor 大背景色带已在后续 BackgroundTextureFidelity.md 中定位为自动 ETC_RGB4 导入并修复为源 ASTC 10×10。已知退出对象池诊断未在本轮处理。

Library/local-pop-tip-content-validation.log 完成 165 项内容检查和 NUT_CONTENT_VALIDATION_PASS。Android ARM64 IL2CPP 开发包重新构建成功，Unity 报告 warnings=0、errors=0；包名/版本/架构及 v1/v2 签名复核通过，最新产物校验值见 AndroidLocalBuild.md。
