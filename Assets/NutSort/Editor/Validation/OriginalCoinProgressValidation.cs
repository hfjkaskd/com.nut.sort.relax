using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalCoinProgressValidation
    {
        public static void Validate()
        {
            var defaults = ScriptableObject.CreateInstance<OriginalUserDefaults>();
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                var user = new OriginalUserLocalData(defaults);
                var tables = new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var progress = new OriginalCoinProgress(user, tables, n => n.ToString("R", CultureInfo.InvariantCulture));
                user.Level = 4;
                Require(!progress.IsShown, "Level four hides coin without needing reward data.");
                user.Level = 5; Require(progress.IsShown, "Level five shows coin.");
                user.Level = 1; user.IsShowCoin = true;
                Require(progress.IsShown, "Saved coin visibility wins before level five.");
                Expect<NullReferenceException>(() => progress.GetStage());
                user.GoldRewardTargetS2CData = JObject.Parse("{\"cal_cfg\":5,\"bear_rates\":\"100\",\"extra\":true,\"bear_zs_list\":[{\"caliper_psi\":\"1000\",\"psi_value\":\"10\",\"caliper_logs\":3,\"caliper_rank\":20},{\"caliper_psi\":\"2000\",\"caliper_logs\":9}]}");
                string before = user.GoldRewardTargetS2CData.ToString();
                user.Coin = 900; user.LoginDay = 99; user.LoginDayCoin = 3; user.TodayPassLevelCount = 99;
                Require(progress.GetStage() == 4 && progress.GetDescription("en") == tables.Text.GetText(9, "en", "1", "10"), "Coin shortfall converts by bear_rates; target uses psi_value.");
                user.Coin = 1000; user.LoginDayCoin = 2; user.TodayPassLevelCount = 4;
                Require(progress.GetStage() == 5 && progress.GetDescription("en") == tables.Text.GetText(10, "en", 1), "Uses coin login count, not cash login count.");
                user.TodayPassLevelCount = 5;
                Require(progress.GetStage() == 6 && progress.GetDescription("en") == tables.Text.GetText(11, "en", 1), "Daily threshold equality advances to coin login requirement.");
                user.LoginDayCoin = 3; user.TodayPassLevelCount = 0; user.UserLevel = 22;
                Require(progress.GetStage() == 7 && progress.GetDescription("en") == tables.Text.GetText(12, "en", -2), "Login equality precedes daily count; rank difference is not clamped.");
                Require(progress.GetStage(1) == 4, "Explicit row index is honored.");
                user.Coin = double.NaN; Require(progress.GetStage() == 7, "NaN is not a shortfall.");
                Require(before == user.GoldRewardTargetS2CData.ToString(), "Rules preserve the complete server document.");
                Expect<ArgumentOutOfRangeException>(() => progress.GetStage(2));
                var row = (JObject)user.GoldRewardTargetS2CData["bear_zs_list"][0];
                row["caliper_psi"] = "16777217"; user.Coin = 16777216;
                Require(progress.GetStage() == 7, "Stage comparison rounds threshold to float before comparing double coin.");
                user.Coin = 16777215; user.GoldRewardTargetS2CData["bear_rates"] = "2";
                Require(progress.GetDescription("en") == tables.Text.GetText(9, "en", "1", "10"), "Description parses the threshold as double independently of stage rounding.");
                user.GoldRewardTargetS2CData["bear_rates"] = "0";
                Require(progress.GetDescription("en") == tables.Text.GetText(9, "en", float.PositiveInfinity.ToString("R", CultureInfo.InvariantCulture), "10"), "Zero rates retain original floating-point division behavior.");
                user.GoldRewardTargetS2CData["bear_zs_list"] = new JArray(JValue.CreateNull());
                Expect<NullReferenceException>(() => progress.GetStage());
                ValidateCoinFormat();
                Debug.Log("NUT_COIN_PROGRESS_VALIDATION_PASS native stages, independent coin login, indexed targets, float/double boundaries, descriptions, preserved JSON and shared currency culture.");
            }
            finally { CultureInfo.CurrentCulture = previous; UnityEngine.Object.DestroyImmediate(defaults); }
        }

        private static void ValidateCoinFormat()
        {
            string country = "en-US";
            var formatter = new OriginalGoldFormatter(() => country);
            Require(formatter.FormatCoin(1234) == "1234" && formatter.FormatCoin(1234.5) == "1234.50", "Coin uses F2 without grouping or currency symbol.");
            formatter.Format(1, "de-DE");
            Require(formatter.FormatCoin(12.5) == "12,50", "Explicit cash culture also changes coin formatting.");
            country = "ja-JP";
            Require(formatter.FormatCoin(-12.9) == "-12", "Live country controls truncation even while cached culture differs.");
            country = "en-US";
            Require(formatter.FormatCoin(12.5) == "12,50", "Default country changes do not reset cached culture.");
            var coinFirst = new OriginalGoldFormatter(() => "de-DE");
            coinFirst.FormatCoin(1);
            Require(coinFirst.Format(12.5f).Contains("12,50"), "Coin-first initialization supplies cash culture too.");
            foreach (string code in new[] { "ja-JP", "id-ID", "ur-PK" })
            {
                var truncated = new OriginalGoldFormatter(() => code);
                Require(truncated.FormatCoin(12.9) == "12" && truncated.FormatCoin(-12.9) == "-12", "Native integer conversion: " + code);
            }
        }

        private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
        private static void Expect<T>(Action action) where T : Exception
        {
            try { action(); } catch (T) { return; }
            throw new InvalidOperationException("Expected " + typeof(T).Name);
        }
    }
}
