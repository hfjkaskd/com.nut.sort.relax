using System;
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
        private int completionPhase, lastPercentage = -1;
        private float value, completionStart, elapsed;
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
                completionStart = value; elapsed = 0f; completionPhase = 1;
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
            if (completionPhase == 0) return;
            elapsed += deltaTime;
            if (completionPhase == 1)
            {
                float t = Mathf.Clamp01(elapsed / settings.CompletionDuration);
                value = Mathf.LerpUnclamped(completionStart, 1f, 1f - (1f-t)*(1f-t));
                SetValue();
                if (t < 1f) return;
                value = 1f; SetValue();
                completionPhase = 2; elapsed = 0f;
            }
            else if (elapsed >= settings.CompletedHoldDuration)
            {
                completionPhase = 0;
                gameObject.SetActive(false);
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
