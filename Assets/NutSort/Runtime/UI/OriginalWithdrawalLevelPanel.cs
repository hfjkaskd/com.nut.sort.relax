using System;
using System.Collections.Generic;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalLevelPanel:MonoBehaviour,IOriginalWithdrawalLevelUI
    {
        [SerializeField] private Button SureBtn,CloseBtn;
        [SerializeField] private Image backdrop;
        [SerializeField] private TMP_Text Tip,GoldCount;
        [SerializeField] private OriginalWithdrawalSteps steps;
        [SerializeField] private RectTransform main,title;
        [SerializeField] private OriginalPanelSettings settings;
        [SerializeField] private OriginalReplayPanel.Label[] labels;
        [SerializeField] private float queueDelay;
        private OriginalWithdrawalLevelFlow flow;
        private OriginalTables tables;private string language;
        private Func<bool> canClick;private Action<string> sound;
        private IOriginalWithdrawalLevelUI services;private Action closed,hidden,next;
        private Action<float,Action> schedule;
        private float elapsed;private bool opening;
        private sealed class CloseTrack{public float Elapsed;public bool Started;public Vector3 Start;}
        private readonly List<CloseTrack> closes=new List<CloseTrack>();
        public Button ConfirmButton=>SureBtn;
        public Button CloseButton=>CloseBtn;
        public TMP_Text TipLabel=>Tip;
        public TMP_Text GoldLabel=>GoldCount;
        public OriginalWithdrawalSteps Steps=>steps;
        public bool Closing=>closes.Count>0;
        public OriginalWithdrawalLevelFlow Flow=>flow;
        public void Bind(OriginalUserLocalData user,OriginalTables tables,string language,Func<float,string> format,
            Func<bool> canClick,Action<string> sound,IOriginalWithdrawalLevelUI services,
            Action<int,Action<Newtonsoft.Json.Linq.JObject>> request,Action save,Func<object,Action> guide,
            Action closed,Action hidden,Action<float,Action> schedule,Action next)
        {
            this.tables=tables;this.language=language;this.canClick=canClick;this.sound=sound;this.services=services;
            this.closed=closed;this.hidden=hidden;this.schedule=schedule;this.next=next;
            flow=new OriginalWithdrawalLevelFlow(user,this,format,request,save,guide,schedule);
        }
        public void Init(object[] args)=>flow.Init(args,()=>
        {
            main.localScale=Vector3.zero;elapsed=0;opening=true;
            if(title!=null)title.localScale=Vector3.zero;
            backdrop.color=new Color(0,0,0,settings.BackdropAlpha);
            foreach(var label in labels)label.Text.text=tables.Text.GetText(label.Id,language);
        },()=>BindButton(CloseBtn,flow.Close),()=>BindButton(SureBtn,flow.Get));
        private void BindButton(Button button,Action action)
        {button.onClick.RemoveAllListeners();button.onClick.AddListener(()=>{if(!canClick())return;action();sound(settings.ClickSound);});}
        public void Refresh()=>flow.Refresh();
        public void SetTip(int id,int level)=>Tip.text=tables.Text.GetText(id,language,level);
        public void SetGold(string value)=>GoldCount.text=value;
        public void PlaySteps()=>steps.Play();
        public bool HasPanel(int id)=>services.HasPanel(id);
        public void HidePanel(int id)=>services.HidePanel(id);
        public void RefreshGoldItem()=>services.RefreshGoldItem();
        public void ShowSelf(float amount)=>services.ShowSelf(amount);
        public void RefreshGold(bool showTip,float addGold)=>services.RefreshGold(showTip,addGold);
        public void Close(){if(this==null)return;closes.Add(new CloseTrack());}
        public void Hide(){hidden();schedule(queueDelay,next);}
        private void Awake()=>OriginalUIAnimationDriver.Register(this,Advance);
        private void OnDestroy()=>OriginalUIAnimationDriver.Unregister(this);
        public void Advance(float delta)
        {
            if(delta==0)return;
            if(opening)
            {
                float before=elapsed;elapsed+=delta;
                if(before<settings.PanelDurationTime)main.localScale=Vector3.one*settings.PanelOpenCurve.Evaluate(Mathf.Clamp01(elapsed/settings.PanelDurationTime));
                if(title!=null&&before<settings.TitleDelayTime+settings.TitleDurationTime&&elapsed>=settings.TitleDelayTime)title.localScale=Vector3.one*settings.TitleCurve.Evaluate(Mathf.Clamp01((elapsed-settings.TitleDelayTime)/settings.TitleDurationTime));
                if(elapsed>=(title!=null?Mathf.Max(settings.PanelDurationTime,settings.TitleDelayTime+settings.TitleDurationTime):settings.PanelDurationTime))opening=false;
            }
            int count=closes.Count,index=0;
            for(int i=0;i<count;i++)
            {
                var c=closes[index];if(!c.Started){c.Started=true;c.Start=main.localScale;}
                c.Elapsed+=delta;float t=Mathf.Clamp01(c.Elapsed/settings.CloseDuration);
                main.localScale=Vector3.LerpUnclamped(c.Start,Vector3.zero,t*t*((settings.BackOvershoot+1)*t-settings.BackOvershoot));
                if(t>=1){closes.RemoveAt(index);closed();}else index++;
            }
        }
    }
}
