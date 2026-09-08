using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Content
{
    // A read-only projection of the fields consumed by TXMgr. The complete server
    // document remains in UserLocalData and is preserved by its explicit writer.
    public sealed class OriginalRewardProgressData
    {
        public int DailyLevels { get; private set; }
        public int StartLevel { get; private set; }
        public int Stage2StartRealLevel { get; private set; }
        public int RealLevel { get; private set; }
        public int Stage2RealLevel { get; private set; }
        public int Stage2StartShowLevel { get; private set; }
        public int LoginDays { get; private set; }
        public int UserLevel { get; private set; }

        public static OriginalRewardProgressData Read(JObject document)
        {
            if (document == null) throw new NullReferenceException("GoldRewardTargetS2CData");
            JToken list = Field(document, "bear_list");
            if (list == null || list.Type == JTokenType.Null) throw new NullReferenceException("bear_list");
            JToken third = ((JArray)list)[2];
            if (third == null || third.Type == JTokenType.Null) throw new NullReferenceException("bear_list[2]");
            var row = (JObject)third;
            return new OriginalRewardProgressData
            {
                DailyLevels = Integer(document, "cal_cfg"),
                StartLevel = Integer(row, "StartLevel"),
                Stage2StartRealLevel = Integer(row, "Stage2StartRealLevel"),
                RealLevel = Integer(row, "RealLevel"),
                Stage2RealLevel = Integer(row, "Stage2RealLevel"),
                Stage2StartShowLevel = Integer(row, "Stage2StartShowLevel"),
                LoginDays = Integer(row, "caliper_logs"),
                UserLevel = Integer(row, "caliper_rank")
            };
        }

        private static int Integer(JObject document, string name)
        {
            JToken value = Field(document, name);
            return value == null ? 0 : (int)value;
        }

        private static JToken Field(JObject document, string name)
        {
            JToken result = null;
            foreach (JProperty property in document.Properties())
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)) result = property.Value;
            return result;
        }
    }

    public sealed class OriginalRewardProgress
    {
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly Func<float, string> formatGold;

        public OriginalRewardProgress(OriginalUserLocalData user, OriginalTables tables, Func<float, string> formatGold)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.tables = tables ?? throw new ArgumentNullException(nameof(tables));
            this.formatGold = formatGold ?? throw new ArgumentNullException(nameof(formatGold));
        }

        // Low-frequency panel/initialization queries; never invoked by Update.
        public int GetStage(int level)
        {
            if (level < 3) return 1;
            return GetStage(level, OriginalRewardProgressData.Read(user.GoldRewardTargetS2CData));
        }

        private int GetStage(int level, OriginalRewardProgressData data)
        {
            if (level <= data.RealLevel) return 2;
            if (level <= data.Stage2RealLevel) return 3;
            if (string.IsNullOrEmpty(user.TXTargetGold))
            {
                Debug.LogError("data.target_show is null");
                return 0;
            }
            if (user.Gold < ToFloat(user.TXTargetGold) || user.IsGuideGold) return 4;
            if (user.LoginDay >= data.LoginDays) return 7;
            return user.TodayPassLevelCount >= data.DailyLevels ? 6 : 5;
        }

        // TXMgr.IsCompletePassStage2Level 0x9C1AD8: strict real-level boundary.
        public bool IsCompletePassStage2Level()
        {
            return user.Level > OriginalRewardProgressData.Read(user.GoldRewardTargetS2CData).Stage2RealLevel;
        }

        public bool IsGuidePassStage2Level()
        {
            if (user.IsCompleteGuidePassStage2Level) return false;
            return user.Level > OriginalRewardProgressData.Read(user.GoldRewardTargetS2CData).RealLevel;
        }

        public string GetDescription(string language)
        {
            OriginalRewardProgressData data = user.Level < 3 ? null : OriginalRewardProgressData.Read(user.GoldRewardTargetS2CData);
            int stage = user.Level < 3 ? 1 : GetStage(user.Level, data);
            switch (stage)
            {
                case 1:
                case 2: return tables.Text.GetText(6, language);
                case 3:
                    return tables.GetShowLevel(user.Level) == tables.LastLevel.ShowLevel
                        ? tables.Text.GetText(6, language)
                        : tables.Text.GetText(7, language, tables.LastLevel.ShowLevel);
                case 4:
                    float remaining = ToFloat(user.TXTargetGold) - user.Gold;
                    return remaining <= 0 ? tables.Text.GetText(24, language)
                        : tables.Text.GetText(9, language, formatGold(remaining), formatGold(ToFloat(user.TXTargetGold)));
                case 5: return tables.Text.GetText(10, language, unchecked(data.DailyLevels - user.TodayPassLevelCount));
                case 6: return tables.Text.GetText(11, language, unchecked(data.LoginDays - user.LoginDay));
                case 7: return tables.Text.GetText(12, language, unchecked(data.UserLevel - user.UserLevel));
                default: return string.Empty;
            }
        }

        public static float ToFloat(string value)
        {
            if (float.TryParse(value, out float result)) return result;
            Debug.LogError("ToFloat fail s:" + value);
            return 0;
        }
    }
}
