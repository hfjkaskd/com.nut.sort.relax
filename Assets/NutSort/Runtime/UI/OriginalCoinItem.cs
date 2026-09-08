using System;
using System.Collections.Generic;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalCoinItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI value, hintText;
        [SerializeField] private Button click;
        [SerializeField] private Transform target;
        [SerializeField] private OriginalGoldItemSettings settings;
        [SerializeField] private OriginalReplayPanel.Label[] labels;
        private OriginalUserLocalData user;
        private OriginalCoinProgress progress;
        private Func<double,string> formatCoin;
        private Func<bool> beginClick;
        private Action<Action> requestReward;
        private Action<int> showPanel;
        private Action clickSound, rewardReady;
        private string language;
        private struct Pulse { public float Elapsed, Amount; public Vector3 Start; public bool Started; }
        private struct FloatPair
        {
            public Image Icon;
            public TextMeshProUGUI Text;
            public Vector3 IconStart, TextStart;
            public Color IconColor, TextColor;
            public float Elapsed;
        }
        private readonly List<Pulse> pulses = new List<Pulse>(4);
        private readonly List<FloatPair> floats = new List<FloatPair>(4);
        public Button Click => click;
        public TextMeshProUGUI Value => value;
        public TextMeshProUGUI HintText => hintText;
        public Image Icon => icon;
        public int PulseCount => pulses.Count;
        public int FloatingPairCount => floats.Count;

        public void Bind(OriginalUserLocalData data, OriginalTables tables, string languageCode,
            OriginalGoldFormatter formatter, Func<bool> tryClick, Action<Action> requestGoldReward,
            Action<int> openPanel, Action playClick)
        {
            user = data ?? throw new ArgumentNullException(nameof(data));
            if(formatter == null)throw new ArgumentNullException(nameof(formatter));
            formatCoin = formatter.FormatCoin;
            beginClick = tryClick ?? throw new ArgumentNullException(nameof(tryClick));
            requestReward = requestGoldReward ?? throw new ArgumentNullException(nameof(requestGoldReward));
            showPanel = openPanel ?? throw new ArgumentNullException(nameof(openPanel));
            clickSound = playClick ?? throw new ArgumentNullException(nameof(playClick));
            language = languageCode;
            foreach(var label in labels)label.Text.text=tables.Text.GetText(label.Id,language);
            progress = new OriginalCoinProgress(user,tables,n => formatter.Format(n));
            rewardReady = RewardReady;
        }

        public void Init()
        {
            // CoinItem explicitly opts out of AddLSSListener's press tween.
            click.transition = Selectable.Transition.ColorTint;
            click.onClick.RemoveAllListeners(); click.onClick.AddListener(Clicked);
            RefreshHint();
        }
        private void Clicked()
        {
            if(!beginClick())return;
            requestReward(rewardReady);
            clickSound();
        }
        private void RewardReady() { showPanel(19); }
        public void HideHintText() { hintText.transform.parent.gameObject.SetActive(false); }
        public void ShowHintText() { hintText.transform.parent.gameObject.SetActive(true); }
        public void RefreshHint()
        {
            if(progress.GetStage(0) >= 5) { HideHintText(); return; }
            ShowHintText();
            hintText.text = progress.GetDescription(language).Replace("\n"," ");
        }
        public void Refresh()
        {
            RefreshCoin();
            gameObject.SetActive(progress.IsShown);
        }
        public void RefreshCoin(bool showTip = false, float addCoin = 0)
        {
            value.text = formatCoin(user.Coin);
            RefreshHint();
            if(showTip) { pulses.Add(new Pulse { Amount = addCoin }); Schedule(); }
        }
        private Action<float> animationStep;
        private void Schedule()
        {
            if(animationStep==null)animationStep=Advance;
            OriginalUIAnimationDriver.Register(this,animationStep);
        }
        private void OnDestroy() { OriginalUIAnimationDriver.Unregister(this); }

        public void Advance(float delta)
        {
            if(delta<0)throw new ArgumentOutOfRangeException(nameof(delta));
            if(delta==0)return;
            for(int i=0;i<floats.Count;)
            {
                FloatPair pair=floats[i]; pair.Elapsed+=delta;
                float t=Mathf.Clamp01(pair.Elapsed/settings.FloatDuration);
                Vector3 p=pair.IconStart; p.y=Mathf.Lerp(p.y,settings.FloatTargetY,t); pair.Icon.transform.localPosition=p;
                p=pair.TextStart; p.y=Mathf.Lerp(p.y,settings.FloatTargetY,t); pair.Text.transform.localPosition=p;
                float cubic=t*t*t;
                pair.Icon.color=Color.LerpUnclamped(pair.IconColor,settings.FloatColor,cubic);
                pair.Text.color=Color.LerpUnclamped(pair.TextColor,settings.FloatColor,cubic);
                if(t>=1) { Destroy(pair.Icon.gameObject); Destroy(pair.Text.gameObject); floats.RemoveAt(i); }
                else { floats[i]=pair; i++; }
            }
            for(int i=0;i<pulses.Count;)
            {
                Pulse pulse=pulses[i]; pulse.Elapsed+=delta;
                float time=pulse.Elapsed-settings.PulseDelay;
                if(time<=0) { pulses[i]=pulse; i++; continue; }
                if(!pulse.Started)
                {
                    pulse.Started=true; pulse.Start=target.localScale;
                    SpawnFloat(pulse.Amount);
                }
                if(time>=settings.PulseDuration*settings.PulseLoops)
                {
                    target.localScale=Vector3.one; pulses.RemoveAt(i); continue;
                }
                int loop=(int)(time/settings.PulseDuration);
                float t=(time-loop*settings.PulseDuration)/settings.PulseDuration;
                if((loop&1)!=0)t=1-t;
                target.localScale=Vector3.LerpUnclamped(pulse.Start,Vector3.one*settings.PulseScale,Mathf.Sin(t*Mathf.PI*.5f));
                pulses[i]=pulse; i++;
            }
            if(pulses.Count==0 && floats.Count==0)OriginalUIAnimationDriver.Unregister(this);
        }
        private void SpawnFloat(float addCoin)
        {
            // Source copies the two already-configured visual objects once per
            // low-frequency reward presentation, not per pulse loop or frame.
            Image image=Instantiate(icon,icon.transform.parent,false);
            TextMeshProUGUI text=Instantiate(value,value.transform.parent,false);
            Vector3 ip=image.transform.localPosition; ip.x+=settings.IconOffsetX; ip.z=0;
            Vector3 tp=text.transform.localPosition; tp.x+=settings.TextOffsetX; tp.z=0;
            image.transform.localPosition=ip; text.transform.localPosition=tp;
            image.transform.localScale=text.transform.localScale=Vector3.one*settings.FloatScale;
            text.text="+"+formatCoin(addCoin);
            floats.Add(new FloatPair { Icon=image,Text=text,IconStart=ip,TextStart=tp,IconColor=image.color,TextColor=text.color });
        }
    }
}
