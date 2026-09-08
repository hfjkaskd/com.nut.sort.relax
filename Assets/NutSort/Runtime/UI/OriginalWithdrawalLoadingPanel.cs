using System;
using System.Collections.Generic;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    // TXGuideLoadingPanel 0x9c9120..0x9c94dc. Parameters belong to its prefab.
    public sealed class OriginalWithdrawalLoadingPanel:MonoBehaviour
    {
        [SerializeField] private Image Progress,backdrop;
        [SerializeField] private RectTransform main;
        [SerializeField] private TMP_Text label;
        [SerializeField] private int labelId;
        [SerializeField] private OriginalPanelSettings settings;
        [SerializeField] private float firstTarget,firstDelay,firstDuration,secondDelay,secondDuration,queueDelay;
        private OriginalTables tables;
        private string language;
        private Action closed,claim,hidden,next;
        private Action<float,Action> schedule;
        private sealed class Track { public float From,To,Delay,Duration,Elapsed;public bool Second; }
        private readonly List<Track> tracks=new List<Track>();
        private float openingElapsed;
        private bool opening;
        private sealed class CloseTrack { public float Elapsed;public bool Started;public Vector3 Start; }
        private readonly List<CloseTrack> closes=new List<CloseTrack>();
        public float Fill=>Progress.fillAmount;
        public bool Closing=>closes.Count>0;
        public TMP_Text Label=>label;
        public RectTransform Main=>main;
        public int ActiveCount=>tracks.Count;
        public void Bind(OriginalTables tables,string language,Action closed,Action claim,Action hidden,Action<float,Action> schedule,Action next)
        {
            this.closed=closed;this.claim=claim;this.hidden=hidden;this.schedule=schedule;this.next=next;
            this.tables=tables;this.language=language;
        }
        public void Init()
        {
            main.localScale=Vector3.zero;openingElapsed=0;opening=true;
            backdrop.color=new Color(0,0,0,settings.BackdropAlpha);
            label.text=tables.Text.GetText(labelId,language);
            Progress.fillAmount=0;
        }
        public void Refresh()=>tracks.Add(new Track{From=0,To=firstTarget,Delay=firstDelay,Duration=firstDuration});
        public void Hide(){hidden();schedule(queueDelay,next);}
        private void Awake()=>OriginalUIAnimationDriver.Register(this,Advance);
        private void OnDestroy()=>OriginalUIAnimationDriver.Unregister(this);
        public void Advance(float delta)
        {
            if(delta==0)return;
            // Animation created from a completion callback begins on the next update.
            if(opening)
            {
                openingElapsed+=delta;
                main.localScale=Vector3.one*settings.PanelOpenCurve.Evaluate(Mathf.Clamp01(openingElapsed/settings.PanelDurationTime));
                if(openingElapsed>=settings.PanelDurationTime)opening=false;
            }
            int closeCount=closes.Count,closeIndex=0;
            for(int i=0;i<closeCount;i++)
            {
                var close=closes[closeIndex];
                if(!close.Started){close.Started=true;close.Start=main.localScale;}
                close.Elapsed+=delta;float t=Mathf.Clamp01(close.Elapsed/settings.CloseDuration);
                main.localScale=Vector3.LerpUnclamped(close.Start,Vector3.zero,t*t*((settings.BackOvershoot+1)*t-settings.BackOvershoot));
                if(t>=1){closes.RemoveAt(closeIndex);closed();}else closeIndex++;
            }
            int count=tracks.Count,index=0;
            for(int i=0;i<count;i++)
            {
                var track=tracks[index];track.Elapsed+=delta;
                if(track.Elapsed<track.Delay){index++;continue;}
                float t=Mathf.Clamp01((track.Elapsed-track.Delay)/track.Duration);
                Progress.fillAmount=Mathf.LerpUnclamped(track.From,track.To,t);
                if(t<1){index++;continue;}
                tracks.RemoveAt(index);
                if(!track.Second)tracks.Add(new Track{From=Progress.fillAmount,To=1,Delay=secondDelay,Duration=secondDuration,Second=true});
                else
                {
                    closes.Add(new CloseTrack());
                    claim(); // Resolve the live TXPanel at completion, after initiating close.
                }
            }
        }
    }
}
