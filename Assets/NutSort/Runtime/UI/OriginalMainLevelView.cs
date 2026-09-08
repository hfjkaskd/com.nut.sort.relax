using NutSort.Content;
using TMPro;
using UnityEngine;

namespace NutSort.UI
{
    public sealed class OriginalMainLevelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Level;
        [SerializeField] private RectTransform levelGroup;
        [SerializeField] private OriginalMainLevelSettings settings;
        public TextMeshProUGUI Label => Level;
        public RectTransform Group => levelGroup;

        public void Refresh(OriginalTables tables, int playerLevel, string languageCode,
            bool hiddenLevelActive, bool withdrawalProgressVisible)
        {
            if (playerLevel < settings.MinimumLevel)
            {
                levelGroup.gameObject.SetActive(false);
                return;
            }
            levelGroup.gameObject.SetActive(!hiddenLevelActive);
            if (hiddenLevelActive) return;
            Level.text = tables.Text.GetText(settings.TextId, languageCode, tables.GetShowLevel(playerLevel));
            Vector2 position = levelGroup.anchoredPosition;
            position.x = withdrawalProgressVisible ? settings.WithdrawalProgressX : settings.NormalX;
            levelGroup.anchoredPosition = position;
        }
    }
}
