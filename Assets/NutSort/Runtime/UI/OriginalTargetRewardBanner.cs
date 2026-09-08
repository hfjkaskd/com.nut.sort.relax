using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NutSort.Content;
using TMPro;
using UnityEngine;

namespace NutSort.UI
{
    public sealed class OriginalTargetRewardBanner : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private TMP_Text tip, subTip;
        [SerializeField] private OriginalTargetRewardSettings settings;
        [SerializeField] private OriginalPanelSettings panels;
        private OriginalUserLocalData user;
        private OriginalTables tables;
        private OriginalRewardProgress progress;
        private Func<float, string> formatGold;
        private Func<Vector3> goldPosition;
        private Action showGoldHint, showProgress, save;
        private string language;
        private enum MotionKind { Finished, Entry, Move, Scale }
        private struct Motion
        {
            public MotionKind Kind;
            public float Elapsed, Delay, Duration;
            public Vector3 Start, End;
            public bool Started;
            public Action Completed;
        }
        private readonly List<Motion> motions = new List<Motion>(4);
        public RectTransform Target => target;
        public TMP_Text Tip => tip;
        public TMP_Text SubTip => subTip;
        public bool IsAnimating => motions.Count != 0;

        public void Bind(OriginalUserLocalData data, OriginalTables originalTables, string languageCode,
            Func<float, string> currencyFormatter, Func<Vector3> currentGoldPosition,
            Action goldHint, Action rewardProgress, Action saveData)
        {
            user = data ?? throw new ArgumentNullException(nameof(data));
            tables = originalTables ?? throw new ArgumentNullException(nameof(originalTables));
            formatGold = currencyFormatter ?? throw new ArgumentNullException(nameof(currencyFormatter));
            goldPosition = currentGoldPosition ?? throw new ArgumentNullException(nameof(currentGoldPosition));
            showGoldHint = goldHint ?? throw new ArgumentNullException(nameof(goldHint));
            showProgress = rewardProgress ?? throw new ArgumentNullException(nameof(rewardProgress));
            save = saveData ?? throw new ArgumentNullException(nameof(saveData));
            language = languageCode;
            progress = new OriginalRewardProgress(user, tables, formatGold);
        }

        public void Init()
        {
            target.localPosition = settings.RestPosition;
            target.localScale = Vector3.one;
        }

        public void Show(Action completed = null)
        {
            RefreshTip();
            RefreshSubTip();
            Init();
            // Source Show does not kill an earlier invocation's tweens.
            motions.Add(new Motion { Kind = MotionKind.Entry, Duration = settings.EntryDuration, Completed = completed });
        }

        public void RefreshTip()
        {
            int stage = progress.GetStage(user.Level);
            tip.text = stage == 4
                ? tables.Text.GetText(20, language, formatGold(OriginalRewardProgress.ToFloat(user.TXTargetGold)))
                : Regex.Replace(progress.GetDescription(language), @"(<color\s*=\s*)(#?[0-9a-fA-F]{6,8})(\s*>)", "<color=#FFFF00>");
            if (stage == 7 && !user.IsCompleteSignIn)
            {
                user.IsCompleteSignIn = true;
                // Source SDK telemetry is excluded; retain the following save.
                save();
            }
        }

        public void RefreshSubTip()
        {
            OriginalLevelInfo level = tables.GetLevelInfo(user.Level, user.Level);
            if (level.TotalRound <= 0)
            {
                subTip.transform.parent.gameObject.SetActive(false);
                return;
            }
            subTip.transform.parent.gameObject.SetActive(level.Round > 1);
            subTip.text = tables.Text.GetText(150, language, tables.GetShowLevel(user.Level), string.Format("{0}/{1}", level.Round, level.TotalRound));
        }

        private void Update() { Advance(Time.deltaTime); }

        public void Advance(float delta)
        {
            if (delta < 0) throw new ArgumentOutOfRangeException(nameof(delta));
            if (delta == 0 || motions.Count == 0) return;
            // Original TweenManager snapshots its active range. Tracks created by
            // an entry completion start on the next update, with no carried delta.
            int count = motions.Count;
            for (int i = 0; i < count; i++)
            {
                Motion motion = motions[i];
                if (motion.Kind == MotionKind.Finished) continue;
                motion.Elapsed += delta;
                if (motion.Elapsed <= motion.Delay) { motions[i] = motion; continue; }
                if (!motion.Started)
                {
                    motion.Started = true;
                    motion.Start = motion.Kind == MotionKind.Move ? target.position
                        : motion.Kind == MotionKind.Scale ? target.localScale : target.localPosition;
                }
                float t = Mathf.Clamp01((motion.Elapsed - motion.Delay) / motion.Duration);
                if (motion.Kind == MotionKind.Entry)
                {
                    float backT = t - 1;
                    float eased = 1 + backT * backT * ((panels.BackOvershoot + 1) * backT + panels.BackOvershoot);
                    Vector3 local = target.localPosition;
                    local.x = Mathf.LerpUnclamped(motion.Start.x, 0, eased);
                    target.localPosition = local;
                }
                else if (motion.Kind == MotionKind.Move) target.position = Vector3.LerpUnclamped(motion.Start, motion.End, t);
                else target.localScale = Vector3.LerpUnclamped(motion.Start, motion.End, t);
                MotionKind completedKind = motion.Kind;
                if (t >= 1) motion.Kind = MotionKind.Finished;
                motions[i] = motion;
                if (t < 1) continue;
                if (completedKind == MotionKind.Entry)
                {
                    Vector3 destination = goldPosition();
                    motions.Add(new Motion { Kind = MotionKind.Move, Delay = settings.HoldDuration,
                        Duration = settings.FlightDuration, End = destination, Completed = motion.Completed });
                    motions.Add(new Motion { Kind = MotionKind.Scale, Delay = settings.HoldDuration,
                        Duration = settings.FlightDuration, End = Vector3.one * settings.FlightScale });
                }
                else if (completedKind == MotionKind.Move)
                {
                    motion.Completed?.Invoke();
                    showGoldHint();
                    showProgress();
                    Init();
                    // The separately registered scale track still updates after
                    // this reset, matching the original manager's creation order.
                }
            }
            int alive = 0;
            for (int i = 0; i < motions.Count; i++)
                if (motions[i].Kind != MotionKind.Finished) motions[alive++] = motions[i];
            if (alive < motions.Count) motions.RemoveRange(alive, motions.Count - alive);
        }
    }
}
