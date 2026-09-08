using System;
using System.Globalization;
using UnityEngine;

namespace NutSort.Content
{
    public sealed class OriginalGoldFormatter
    {
        private readonly Func<string> currentCountryLanguage;
        private CultureInfo culture;

        public OriginalGoldFormatter(Func<string> currentCountryLanguage)
        {
            this.currentCountryLanguage = currentCountryLanguage ?? throw new ArgumentNullException(nameof(currentCountryLanguage));
        }

        // CoinFormat and GoldLSSFormat share the original UILSSUtil culture cache.
        public string FormatCoin(double coin)
        {
            try
            {
                if (culture == null) culture = new CultureInfo(currentCountryLanguage());
                string country = currentCountryLanguage();
                if (country == "ja-JP" || country == "id-ID" || country == "ur-PK")
                    coin = unchecked((int)coin);
                string result = coin.ToString("F2", culture);
                return result.EndsWith(".00") || result.EndsWith(",00") || result.EndsWith(" 00")
                    ? result.Substring(0, result.Length - 3) : result;
            }
            catch (Exception error)
            {
                Debug.LogError(error);
                return coin.ToString("F2");
            }
        }
        // UILSSUtil.GoldLSSFormat: explicit country arguments reset the shared
        // formatter's culture; changing the default country alone does not.
        public string Format(float gold, string countryLanguageCode = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(countryLanguageCode)) culture = null;
                if (string.IsNullOrEmpty(countryLanguageCode)) countryLanguageCode = currentCountryLanguage();
                if (culture == null) culture = new CultureInfo(countryLanguageCode);
                if (countryLanguageCode == "ja-JP" || countryLanguageCode == "id-ID" || countryLanguageCode == "ur-PK")
                    gold = unchecked((int)gold);
                string result = string.Format(culture, "{0:C}", gold).Replace(" ", "");
                if (countryLanguageCode == "th-TH") result = result.Replace("THB", "฿");
                if (countryLanguageCode == "ja-JP") return result;
                if (countryLanguageCode == "de-DE" || countryLanguageCode == "fr-FR")
                    return result.EndsWith(",00€") ? result.Substring(0, result.Length - 4) + "€" : result;
                if (countryLanguageCode == "ru-RU")
                    return result.EndsWith(",00₽") ? result.Substring(0, result.Length - 4) + "₽" : result;
                return result.EndsWith(".00") || result.EndsWith(",00") || result.EndsWith(" 00")
                    ? result.Substring(0, result.Length - 3) : result;
            }
            catch (Exception error)
            {
                Debug.LogError(error);
                return gold.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
