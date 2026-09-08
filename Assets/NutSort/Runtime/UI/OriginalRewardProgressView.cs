using System;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalRewardProgressView : MonoBehaviour
    {
        [SerializeField] private TMP_Text tip, progressValue, goldValue;
        [SerializeField] private Image bar;
        [SerializeField] private bool isShow;
        private OriginalUserLocalData user;
        private OriginalTables tables;
        private OriginalRewardProgress progress;
        private Func<float, string> formatGold;
        private string language;
        public bool IsShow => isShow;
        public TMP_Text Tip => tip;
        public TMP_Text ProgressValue => progressValue;
        public TMP_Text GoldValue => goldValue;
        public Image Bar => bar;

        public void Bind(OriginalUserLocalData data, OriginalTables originalTables, string languageCode, Func<float, string> currencyFormatter)
        {
            user = data ?? throw new ArgumentNullException(nameof(data));
            tables = originalTables ?? throw new ArgumentNullException(nameof(originalTables));
            formatGold = currencyFormatter ?? throw new ArgumentNullException(nameof(currencyFormatter));
            language = languageCode;
            progress = new OriginalRewardProgress(user, tables, formatGold);
        }

        public void Init() { gameObject.SetActive(false); }
        public void Hide() { gameObject.SetActive(false); }
        public void Show() { if (isShow) gameObject.SetActive(true); }

        public void Refresh()
        {
            // Unlike GetStage's early-level branch, TX_JD.Refresh always reads
            // the third reward record before deciding whether to hide the view.
            var data = OriginalRewardProgressData.Read(user.GoldRewardTargetS2CData);
            if (user.Level <= data.Stage2RealLevel)
            {
                Hide();
                isShow = false;
                return;
            }
            isShow = true;
            goldValue.text = tables.Text.GetText(160, language);
            int stage = progress.GetStage(user.Level);
            tip.text = progress.GetDescription(language).Replace("\n", " ").Replace("FF0000", "a3ff8a");
            switch (stage)
            {
                case 0:
                    Hide();
                    return;
                case 4:
                    RefreshGold();
                    if (!user.IsGuideGoldComplete)
                    {
                        Hide();
                        float showLevel = data.Stage2StartShowLevel;
                        bar.fillAmount = showLevel / showLevel;
                        progressValue.text = string.Format("{0}/{1}", data.Stage2StartShowLevel, data.Stage2StartShowLevel);
                        tip.text = tables.Text.GetText(24, language);
                    }
                    return;
                case 5:
                    bar.fillAmount = (float)user.TodayPassLevelCount / data.DailyLevels;
                    progressValue.text = string.Format("{0}/{1}", user.TodayPassLevelCount, data.DailyLevels);
                    if (!user.IsGuideGoldTargetComplete) RefreshPendingTargetGuide();
                    return;
                case 6:
                    bar.fillAmount = (float)user.LoginDay / data.LoginDays;
                    progressValue.text = string.Format("{0}/{1}", user.LoginDay, data.LoginDays);
                    if (!user.IsGuideGoldTargetComplete) RefreshPendingTargetGuide();
                    return;
                default:
                    bar.fillAmount = (float)user.UserLevel / data.UserLevel;
                    progressValue.text = string.Format("{0}/{1}", user.UserLevel, data.UserLevel);
                    return;
            }
        }

        private void RefreshGold()
        {
            bar.fillAmount = user.Gold / OriginalRewardProgress.ToFloat(user.TXTargetGold);
            progressValue.text = formatGold(user.Gold) + "/" + formatGold(OriginalRewardProgress.ToFloat(user.TXTargetGold));
        }
        private void RefreshPendingTargetGuide()
        {
            RefreshGold();
            tip.text = tables.Text.GetText(24, language);
        }
    }
}
