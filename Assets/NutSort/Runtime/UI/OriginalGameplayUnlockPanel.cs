using System;
using NutSort.Content;
using NutSort.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalGameplayUnlockPanel : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private TMP_Text tip;
        [SerializeField] private GameObject[] icons;
        [SerializeField] private RectTransform main,title;
        [SerializeField] private Image backdrop;
        [SerializeField] private OriginalPanelSettings settings;
        [SerializeField] private OriginalReplayPanel.Label[] labels;
        [SerializeField] private string appearSound,clickSound;
        private Func<bool> gate;
        private Action<string> audio;
        private Action closed;
        private OriginalGameplayUnlockContinue continuation;
        private OriginalGameplayUnlockDisplay display;
        private int index;
        private bool showBanner,opening,closing;
        private float elapsed,closeElapsed;
        private Vector3 closeStart;
        public Button ContinueButton=>continueButton;
        public TMP_Text Tip=>tip;
        public GameObject[] Icons=>icons;
        public bool Closing=>closing;
        public void Initialize(OriginalUserLocalData user,int index,bool showBanner,OriginalTables tables,string language,
            Func<bool> gate,Action<string> audio,Action<Action> banner,Action closed)
        {
            this.index=index;this.showBanner=showBanner;this.gate=gate;this.audio=audio;this.closed=closed;
            continuation=new OriginalGameplayUnlockContinue(user,banner,Close);
            display=new OriginalGameplayUnlockDisplay(()=>icons.Length,(i,active)=>{if(icons[i]!=null)icons[i].SetActive(active);},id=>tip.text=tables.Text.GetText(id,language));
            foreach(var label in labels)label.Text.text=tables.Text.GetText(label.Id,language);
            continueButton.onClick.RemoveAllListeners();continueButton.onClick.AddListener(Continue);
            backdrop.color=new Color(0,0,0,settings.BackdropAlpha);
            main.localScale=Vector3.zero;title.localScale=Vector3.zero;
            elapsed=closeElapsed=0;opening=true;closing=false;
            audio(appearSound);
        }
        public void Refresh(){display.Refresh(index);}
        private void Continue(){if(!gate())return;continuation.Run(index,showBanner);audio(clickSound);}
        public void Close(){if(closing)return;closing=true;opening=false;closeElapsed=0;closeStart=main.localScale;}
        private void Awake(){OriginalUIAnimationDriver.Register(this,Advance);}
        private void OnDestroy(){OriginalUIAnimationDriver.Unregister(this);}
        public void Advance(float delta)
        {
            if(!opening&&!closing)return;
            elapsed+=delta;
            title.localScale=Vector3.one*settings.TitleCurve.Evaluate(Mathf.Clamp01((elapsed-settings.TitleDelayTime)/settings.TitleDurationTime));
            if(closing)
            {
                closeElapsed+=delta;float t=Mathf.Clamp01(closeElapsed/settings.CloseDuration);
                float eased=t*t*((settings.BackOvershoot+1)*t-settings.BackOvershoot);
                main.localScale=Vector3.LerpUnclamped(closeStart,Vector3.zero,eased);
                if(t>=1){closing=false;closed();}
            }
            else
            {
                main.localScale=Vector3.one*settings.PanelOpenCurve.Evaluate(Mathf.Clamp01(elapsed/settings.PanelDurationTime));
                if(elapsed>=Mathf.Max(settings.PanelDurationTime,settings.TitleDelayTime+settings.TitleDurationTime))opening=false;
            }
        }
    }
}
