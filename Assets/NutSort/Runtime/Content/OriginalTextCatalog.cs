using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace NutSort.Content
{
    public sealed class OriginalTextCatalog
    {
        private readonly Dictionary<int, string[]> rows = new Dictionary<int, string[]>();
        public OriginalTextCatalog(JObject table)
        {
            foreach (JToken row in (JArray)table["list"])
            {
                int id = (int)row["Index"];
                // Source linear lookup returns the first matching record.
                if (rows.ContainsKey(id)) continue;
                rows.Add(id, new[] { (string)row["zh"], (string)row["en"], (string)row["ja"],
                    (string)row["pt"], (string)row["ko"], (string)row["de"], (string)row["fr"],
                    (string)row["id"], (string)row["ru"], (string)row["bn"], (string)row["ne"],
                    (string)row["ar"], (string)row["ur"], (string)row["es"], (string)row["hi"], (string)row["fil"] });
            }
        }
        public string GetText(int id, string languageCode, params object[] arguments)
        {
            if (!rows.TryGetValue(id, out string[] row)) return id.ToString(CultureInfo.CurrentCulture);
            string text = row[LanguageIndex(languageCode)];
            if (string.IsNullOrEmpty(text)) text = row[1];
            return string.Format(CultureInfo.CurrentCulture, text, arguments);
        }
        private static int LanguageIndex(string code)
        {
            switch (code)
            {
                case "en": return 1; case "ja": return 2; case "pt": return 3; case "ko": return 4;
                case "de": return 5; case "fr": return 6; case "id": return 7; case "ru": return 8;
                case "bn": return 9; case "ne": return 10; case "ar": return 11; case "ur": return 12;
                case "es": return 13; case "hi": return 14; case "fil": return 15;
                default: return 0;
            }
        }
    }
}
