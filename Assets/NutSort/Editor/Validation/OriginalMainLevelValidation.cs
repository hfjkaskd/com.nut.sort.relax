using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalMainLevelValidation
    {
        public static void Validate()
        {
            var tables = new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            Require(tables.Text.GetText(2, "en", 4) == "Level 4", "English original level template");
            Require(tables.Text.GetText(2, "ja", 4) == "4 レベル", "Japanese original parameter order");
            Require(tables.Text.GetText(2, "pt", 4) == "Nível 4", "Portuguese template");
            Require(tables.Text.GetText(2, "ru", 4) == "Уровень 4", "Russian template");
            Require(tables.Text.GetText(2, "ko", 4) == "Level 4", "Missing supported language falls back to English");
            Require(tables.Text.GetText(99999, "en") == "99999", "Unknown text id returns its numeric form");
            var synthetic = new OriginalTextCatalog(JObject.Parse("{\"list\":[{\"Index\":1,\"zh\":\"中文\",\"en\":\"English\",\"fr\":\" \"},{\"Index\":1,\"en\":\"Second\"}]}"));
            Require(synthetic.GetText(1, "EN") == "中文" && synthetic.GetText(1, null) == "中文", "Case-sensitive unknown code uses Chinese field first");
            Require(synthetic.GetText(1, "fr") == " " && synthetic.GetText(1, "en") == "English", "Whitespace and first duplicate semantics");
            bool failed = false;
            try { tables.Text.GetText(2, "en"); } catch (FormatException) { failed = true; }
            Require(failed, "Missing arguments keep original format failure");
            var settings = Resources.Load<OriginalMainLevelSettings>("Configuration/OriginalMainLevel");
            var instance = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(settings.PrefabPath));
            try
            {
                var view = instance.GetComponent<OriginalMainLevelView>();
                Require(view.Label.font.name == "zh_Custom SDF" && view.Label.enableAutoSizing && view.Label.fontSizeMin == 18 && view.Label.fontSizeMax == 40, "Original level font and autosizing");
                Require(view.Group.sizeDelta == new Vector2(202,72) && view.Label.rectTransform.sizeDelta == new Vector2(181.31f,54.1f), "Original badge/text sizes");
                view.Refresh(tables, 1, "en", false, false);
                Require(!view.Group.gameObject.activeSelf, "First internal level hides badge");
                view.Refresh(tables, 5, "en", false, false);
                Require(view.Group.gameObject.activeSelf && view.Label.text == "Level 4" && view.Group.anchoredPosition == new Vector2(0,-248), "Stage mapping and normal position");
                view.Refresh(tables, 5, "ja", false, true);
                Require(view.Label.text == "4 レベル" && view.Group.anchoredPosition == new Vector2(374,-248), "Withdrawal-progress horizontal offset");
                view.Refresh(tables, 6, "en", true, false);
                Require(!view.Group.gameObject.activeSelf && view.Label.text == "4 レベル", "Hidden-stage branch does not rewrite text");
                view.Refresh(tables, 52, "en", false, false);
                Require(view.Label.text == "Level 31" && view.Group.gameObject.activeSelf, "Overflow stage restores badge");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("NUT_MAIN_LEVEL_VALIDATION_PASS source locale fallback/formatting, stage visibility and positions, original prefab layout/font and displayed-level mapping.");
        }
        private static void Require(bool value, string message) { if (!value) throw new InvalidDataException(message); }
    }
}
