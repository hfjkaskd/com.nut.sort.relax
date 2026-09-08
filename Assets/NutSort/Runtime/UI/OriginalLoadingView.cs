using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalLoadingView : MonoBehaviour
    {
        [SerializeField] private Image Bar;
        [SerializeField] private Image Logo;
        [SerializeField] private RectTransform lS;
        [SerializeField] private TextMeshProUGUI BarValue;
        [SerializeField] private OriginalLoadingSettings settings;
        private string[] percentages;
        private bool showing;
        private int lastPercentage = -1;
        private float value;
        private struct Completion
        {
            public float Start, Elapsed;
            public bool Holding;
        }
        private readonly List<Completion> completions = new List<Completion>();
        public float Value => value;
        public bool IsVisible => gameObject.activeSelf;
        public Image ProgressImage => Bar;
        public RectTransform Marker => lS;
        public TMP_Text PercentageLabel => BarValue;

        private void Awake()
        {
            Prepare();
            Logo.SetNativeSize();
            OriginalUIAnimationDriver.Register(this, AdvanceCompletion);
        }
        private void Prepare()
        {
            if (Bar == null || Logo == null || lS == null || BarValue == null || settings == null)
                throw new InvalidOperationException("Original loading view references are incomplete.");
            if (percentages != null) return;
            percentages = new string[101];
            for (int i = 0; i < percentages.Length; i++) percentages[i] = i.ToString(CultureInfo.CurrentCulture) + "%";
        }
        public void SetState(bool isShow)
        {
            Prepare();
            showing = isShow;
            if (isShow)
            {
                value = 0f; SetValue();
                gameObject.SetActive(true);
            }
            else
            {
                completions.Add(new Completion { Start = value });
            }
        }
        private void Update() { AdvanceAutomatic(Time.deltaTime); }
        private void OnDestroy() { OriginalUIAnimationDriver.Unregister(this); }
        public void Advance(float deltaTime)
        {
            AdvanceAutomatic(deltaTime);
            AdvanceCompletion(deltaTime);
        }
        private void AdvanceAutomatic(float deltaTime)
        {
            if (!isActiveAndEnabled) return;
            if (deltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            if (showing)
            {
                value = Mathf.Min(value + deltaTime * settings.AutomaticRate, settings.AutomaticLimit);
                SetValue();
            }
        }
        public void AdvanceCompletion(float deltaTime)
        {
            if (deltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            // DOVirtual.Float creates independent tracks; SetState never kills
            // an earlier animation or its delayed hide. Preserve creation order.
            int count = completions.Count, index = 0;
            for (int i = 0; i < count; i++)
            {
                var completion = completions[index];
                completion.Elapsed += deltaTime;
                if (!completion.Holding)
                {
                    float t = Mathf.Clamp01(completion.Elapsed / settings.CompletionDuration);
                    value = Mathf.LerpUnclamped(completion.Start, 1f, 1f - (1f-t)*(1f-t));
                    SetValue();
                    if (t >= 1f)
                    {
                        value = 1f; SetValue();
                        completion.Holding = true; completion.Elapsed = 0f;
                    }
                }
                else if (completion.Elapsed >= settings.CompletedHoldDuration)
                {
                    completions.RemoveAt(index);
                    gameObject.SetActive(false);
                    continue;
                }
                completions[index++] = completion;
            }
        }
        private void SetValue()
        {
            Bar.fillAmount = value;
            lS.anchoredPosition = new Vector2(Bar.fillAmount * settings.MarkerTravel, 0f);
            int percent = Mathf.Clamp((int)Math.Round((double)(value * 100f), MidpointRounding.AwayFromZero), 0, 100);
            if (percent == lastPercentage) return;
            lastPercentage = percent;
            BarValue.text = percentages[percent];
        }
    }
}
