# 当前实现状态

## 已提交恢复点

初始恢复点 `6a99651f9cf74a1f900b2d93f8eff87945d31dad` 已推送到指定仓库 main，并校验远程一致。提交作者仅在此仓库配置为用户提供的身份。

## 已实现并验证

- 使用指定 Unity 2022.3.62f3 创建原生工程。
- 恢复两套原始加密索引与所有 1,474 份棋盘数据，资源路径和内容来自本包，不是生成关卡。
- 根据原 RVA 0x9B4718、0x9FBAE4、0x9FBC04、0x9FCE64 实现 Resources 路径加载、AES-CBC/PKCS7 解码及原关卡结构读取。
- 原主索引为 1–219 共 219 项；循环索引为 220–299 共 80 项。
- 数据路径和原解码参数放在 `Assets/Resources/Configuration/OriginalContent.asset`。运行时只保留已解码索引，棋盘按需求加载。
- Unity JsonUtility 会将缺省 BIM 读成非空对象，导致空槽语义变化。已改为 **Unity 官方** `com.unity.nuget.newtonsoft-json@3.2.1` 的显式 JToken 字段读取；不使用 JsonSerializer、ToObject、反射字段查找或枚举字符串 key。
- Unity 内验证通过：1,476 份资源的明文 SHA-256、1,474 份棋盘的螺杆/格子/占用数量和关卡标识、全部索引种子引用、首关 2 杆×4 槽与 4 枚颜色 11 螺母、无效资源/输入的明确失败行为。

## 仍未完成

- 完整可玩的启动 Scene、Prefabs、排序操作、胜负、引导与各事件分支尚未接入。关卡加载模块通过不等于游戏已经可以玩。
- 原函数体已在逆向资料目录生成，仍需逐方法验证、移植和回归。解码、加载之外的规则不因“导出了函数体”而视为已实现。
- 原场景和带字段 Prefab 在独立参考工程内；未将其中 DummyDll、SDK、Spine、DOTween 当作正式实现导入。
- 地区 AB/GM、US 收益广告默认路径、完整视觉/动画、Editor 与设备一致性尚未验证。
- SDK 未改动；当前新工程没有原有 SDK 实现可继承。

完整 1:1 状态仍为 **false**。开发继续优先主链与视觉。

## 验证入口

使用指定 Unity 执行 `-batchmode -nographics -projectPath <工程> -executeMethod NutSort.Validation.OriginalContentValidation.Run -logFile <日志>`。

日志必须出现 `NUT_CONTENT_VALIDATION_PASS`，且无编译错误或验证异常；不要仅依据启动 Unity 的 PowerShell 退出码判断，因为 Unity.exe 可提前返回进程启动结果。

期望清单由原加密数据在独立 PowerShell/.NET 解密后生成，Unity 验证实际 Resources 加载路径。测试代码仅在 Editor 验证程序集；游戏加载器没有 Editor 专用分支。
