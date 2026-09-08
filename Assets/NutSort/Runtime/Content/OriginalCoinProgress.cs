using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Content
{
    // TXMgr coin rules consume bear_zs_list, independently of cash bear_list.
    // Queries retain the complete original server document without rewriting it.
    public sealed class OriginalCoinProgress
    {
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly Func<float, string> formatGold;

        public OriginalCoinProgress(OriginalUserLocalData user, OriginalTables tables, Func<float, string> formatGold)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.tables = tables ?? throw new ArgumentNullException(nameof(tables));
            this.formatGold = formatGold ?? throw new ArgumentNullException(nameof(formatGold));
        }

        public bool IsShown => user.IsShowCoin || user.Level > 4;

        public int GetStage(int index = 0)
        {
            JObject document = Document();
            JObject row = Row(document, index);
            float target = OriginalRewardProgress.ToFloat(Text(row, "caliper_psi"));
            if (user.Coin < (double)target) return 4;
            if (user.LoginDayCoin >= Integer(row, "caliper_logs")) return 7;
            return user.TodayPassLevelCount >= Integer(document, "cal_cfg") ? 6 : 5;
        }

        public string GetDescription(string language)
        {
            int stage = GetStage(0);
            JObject document = Document();
            JObject row = Row(document, 0);
            switch (stage)
            {
                case 4:
                    // Native converts the double difference to float BEFORE dividing.
                    float rate = OriginalRewardProgress.ToFloat(Text(document, "bear_rates"));
                    float remaining = (float)(ToDouble(Text(row, "caliper_psi")) - user.Coin) / rate;
                    return tables.Text.GetText(9, language, formatGold(remaining),
                        formatGold(OriginalRewardProgress.ToFloat(Text(row, "psi_value"))));
                case 5: return tables.Text.GetText(10, language, unchecked(Integer(document, "cal_cfg") - user.TodayPassLevelCount));
                case 6: return tables.Text.GetText(11, language, unchecked(Integer(row, "caliper_logs") - user.LoginDayCoin));
                case 7: return tables.Text.GetText(12, language, unchecked(Integer(row, "caliper_rank") - user.UserLevel));
                default: return string.Empty;
            }
        }

        private JObject Document() => user.GoldRewardTargetS2CData ?? throw new NullReferenceException("GoldRewardTargetS2CData");

        private static JObject Row(JObject document, int index)
        {
            JToken list = Field(document, "bear_zs_list");
            if (list == null || list.Type == JTokenType.Null) throw new NullReferenceException("bear_zs_list");
            JToken row = ((JArray)list)[index];
            if (row == null || row.Type == JTokenType.Null) throw new NullReferenceException("bear_zs_list item");
            return (JObject)row;
        }

        private static JToken Field(JObject document, string name)
        {
            JToken result = null;
            foreach (JProperty property in document.Properties())
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)) result = property.Value;
            return result;
        }

        private static int Integer(JObject document, string name)
        {
            JToken value = Field(document, name);
            return value == null ? 0 : (int)value;
        }

        private static string Text(JObject document, string name) => (string)Field(document, name);

        private static double ToDouble(string value)
        {
            if (double.TryParse(value, out double result)) return result;
            Debug.LogError("ToDouble fail s:" + value);
            return 0;
        }
    }
}
