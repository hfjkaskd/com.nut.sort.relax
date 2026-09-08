using System;
using System.Globalization;
using NutSort.Content;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalRewardProgressValidation
    {
        public static void Validate()
        {
            var defaults = ScriptableObject.CreateInstance<OriginalUserDefaults>();
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                var tables = new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var user = new OriginalUserLocalData(defaults);
                int formatCalls = 0;
                var progress = new OriginalRewardProgress(user, tables, value => { formatCalls++; return "$" + value.ToString(CultureInfo.InvariantCulture); });
                Require(progress.GetStage(-1) == 1 && progress.GetStage(2) == 1, "Early levels must not require a server reward document.");
                Require(progress.GetDescription("en") == tables.Text.GetText(6, "en"), "First-level description must not require reward data.");
                user.IsCompleteGuidePassStage2Level = true;
                Require(!progress.IsGuidePassStage2Level(), "Completed guide short-circuits missing reward data.");
                Expect<NullReferenceException>(() => progress.GetStage(3));
                user.GoldRewardTargetS2CData = JObject.Parse("{\"cal_cfg\":5,\"unknown\":{\"preserve\":true},\"bear_list\":[null,null,{\"RealLevel\":5,\"Stage2RealLevel\":10,\"caliper_logs\":3,\"caliper_rank\":20}]}");
                string originalDocument = user.GoldRewardTargetS2CData.ToString();
                Require(progress.GetStage(3) == 2 && progress.GetStage(5) == 2 && progress.GetStage(6) == 3 && progress.GetStage(10) == 3, "Inclusive real-level boundaries precede target-gold validation.");
                user.Level = 6; user.IsCompleteGuidePassStage2Level = false;
                Require(progress.IsGuidePassStage2Level(), "Stage-two guide starts strictly after RealLevel.");
                user.Level = 5; Require(!progress.IsGuidePassStage2Level(), "Guide excludes RealLevel itself.");
                user.Level = 50; Require(progress.IsGuidePassStage2Level(), "Guide has no Stage2RealLevel upper bound.");
                user.Level = 6;
                Require(progress.GetDescription("en") == tables.Text.GetText(7, "en", 30), "Stage-three text uses last display level, not a remaining count.");
                var row = (JObject)user.GoldRewardTargetS2CData["bear_list"][2];
                row["Stage2RealLevel"] = 51; user.Level = 51;
                Require(progress.GetDescription("en") == tables.Text.GetText(6, "en"), "Last display level reuses the early-stage text.");
                row["Stage2RealLevel"] = 10; user.Level = 11; user.TXTargetGold = "100";
                user.Gold = 99; user.LoginDay = 9; user.TodayPassLevelCount = 9;
                Require(progress.GetStage(11) == 4, "Gold shortfall precedes login and daily thresholds.");
                Require(progress.GetDescription("en") == tables.Text.GetText(9, "en", "$1", "$100") && formatCalls == 2, "Shortfall description formats remaining and target gold.");
                user.Gold = 100; user.IsGuideGold = true;
                Require(progress.GetStage(11) == 4 && progress.GetDescription("en") == tables.Text.GetText(24, "en") && formatCalls == 2, "Gold guide retains stage four at the target, without currency formatting.");
                user.IsGuideGold = false; user.LoginDay = 2; user.TodayPassLevelCount = 4;
                Require(progress.GetStage(11) == 5 && progress.GetDescription("en") == tables.Text.GetText(10, "en", 1), "Daily requirement branch.");
                user.TodayPassLevelCount = 5;
                Require(progress.GetStage(11) == 6 && progress.GetDescription("en") == tables.Text.GetText(11, "en", 1), "Daily equality advances to login requirement.");
                user.LoginDay = 3; user.TodayPassLevelCount = 0; user.UserLevel = 22;
                Require(progress.GetStage(11) == 7 && progress.GetDescription("en") == tables.Text.GetText(12, "en", -2), "Login equality wins over daily count; rank subtraction is not clamped.");
                user.Gold = float.NaN;
                Require(progress.GetStage(11) == 7, "Unordered gold comparison does not count as a shortfall.");
                Require(user.GoldRewardTargetS2CData.ToString() == originalDocument, "Queries preserve all original document fields.");
                var copy = OriginalUserDataJson.Read(OriginalUserDataJson.Write(user), defaults);
                Require(JToken.DeepEquals(copy.GoldRewardTargetS2CData, user.GoldRewardTargetS2CData), "Reward projection must not change persisted server JSON.");
                var missing = OriginalRewardProgressData.Read(JObject.Parse("{\"bear_list\":[null,null,{}]}"));
                Require(missing.RealLevel == 0 && missing.DailyLevels == 0, "Missing integer fields retain source constructor defaults.");
                var duplicate = OriginalRewardProgressData.Read(JObject.Parse("{\"CAL_CFG\":2,\"cal_cfg\":7,\"bear_list\":[null,null,{}]}"));
                Require(duplicate.DailyLevels == 7, "Case-varied fields are consumed in source order.");
                Expect<ArgumentOutOfRangeException>(() => OriginalRewardProgressData.Read(JObject.Parse("{\"bear_list\":[]}")));
                Expect<NullReferenceException>(() => OriginalRewardProgressData.Read(JObject.Parse("{\"bear_list\":[null,null,null]}")));
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                Require(OriginalRewardProgress.ToFloat("12,5") == 12.5f, "Source target parsing follows current culture.");
                ValidateFormatter();
                Debug.Log("NUT_REWARD_PROGRESS_VALIDATION_PASS seven stages, inclusive boundaries, guide priority, native text IDs, lazy data access, preserved JSON and country currency formatting.");
            }
            finally { CultureInfo.CurrentCulture = previous; UnityEngine.Object.DestroyImmediate(defaults); }
        }

        private static void ValidateFormatter()
        {
            string country = "en-US";
            var formatter = new OriginalGoldFormatter(() => country);
            Require(formatter.Format(100) == "$100" && formatter.Format(12.5f) == "$12.50", "US currency precision and zero-decimal removal.");
            Require(formatter.Format(12.9f, "ja-JP") == formatter.Format(12f, "ja-JP"), "Japan truncates fractional currency.");
            Require(formatter.Format(-12.9f, "id-ID") == formatter.Format(-12f, "id-ID"), "Indonesia truncates negative values toward zero.");
            Require(formatter.Format(12.9f, "ur-PK") == formatter.Format(12f, "ur-PK"), "Pakistan uses the original integer branch.");
            foreach (string code in new[] { "de-DE", "fr-FR", "ru-RU", "th-TH", "en-GB", "pt-BR" })
            {
                string actual = formatter.Format(12.5f, code);
                string raw = string.Format(new CultureInfo(code), "{0:C}", 12.5f).Replace(" ", "");
                if (code == "th-TH") raw = raw.Replace("THB", "฿");
                Require(actual == raw, "Fractional formatting retains source currency and non-ASCII spacing: " + code);
            }
            Require(formatter.Format(1, "en-US") == "$1", "Explicit country resets cached culture.");
            country = "en-GB";
            Require(formatter.Format(1) == "$1", "Default-country changes alone preserve source cached culture.");
            Require(formatter.Format(1, country) == "£1", "Explicit new country replaces cached culture.");
        }

        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
        private static void Expect<T>(Action action) where T : Exception
        {
            try { action(); } catch (T) { return; }
            throw new InvalidOperationException("Expected " + typeof(T).Name);
        }
    }
}
