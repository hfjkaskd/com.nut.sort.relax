# 原版国家配置查询

从当前 ConfigTable.GetCountryInfo / IsExistCountryInfo 与 CountryLssInfo.CountryLanguageCode 恢复。OriginalTables 直接解析既有加密归档 config.json，不复制或另建国家映射。保存源列表顺序和 Code、Area、LanguageCode、PhoneAreaNumber、Name 字段，语言地区组合使用 LanguageCode + "-" + Code。

查询逐行将输入执行当前文化 ToUpper，再与源 Code 精确比较；返回第一个匹配对象本身，不修剪输入、不做 invariant 大写。Contains 找不到仅返回 false。Get 找不到先记录源错误前缀，再递归查 US；保持源行为，不增加自定义默认国家。原始配置包含 US；损坏配置同时缺失 US 时原版存在无限递归风险，本次未人为改成另一个结果。源配置是可信随包资源，不在高频路径查询。

回归核对全部国家行及五个字段，覆盖返回引用、重复首项、土耳其文化大小写、未知地区、空查询/空列表、日志异常中断、当前国家驱动日期显示。首阶段 Play 的时间格式现在取实际原版国家配置；选择 US 仍是明确测试输入。正式 UserMgr 地区初始化、AB 分流及 GM 切换尚未完成，SDK 不变。

验证结果：原版国家表共 46 行；完整 Unity 回归 142 个 VALIDATION_PASS，日志 Library/country-infos-validation.log。实际首阶段/提示页往返 Play 通过，日志 Library/country-infos-play.log，玩家偏好备份已恢复。
