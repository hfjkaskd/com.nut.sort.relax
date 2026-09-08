using System;
using System.Collections.Generic;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalPlayerGoldHintView : MonoBehaviour
    {
        [SerializeField] private Image head;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text info;
        [SerializeField] private ParticleSystem effect;
        [SerializeField] private Vector3 hiddenPosition;
        [SerializeField] private float moveDuration, pushHold, callbackHold, backOvershoot;
        private OriginalPlayerGoldHintSchedule schedule;
        private OriginalPlayerGoldHintText text;
        private OriginalPlayerGoldHintIcons icons;
        private Func<string> language;
        private Func<OriginalChannelInfo> selfChannel;
        private Action<float> step;
        private enum MotionKind { PushIn, SelfIn, CallbackIn, PushOut, CallbackOut }
        private struct Motion
        {
            public MotionKind Kind;
            public Vector3 Start;
            public float Elapsed, Delay;
            public bool Started, Complete;
            public Action Callback;
        }
        private readonly List<Motion> motions=new List<Motion>(4);
        public long LastShowTime => schedule==null?-1:schedule.LastShowTime;
        public void Bind(OriginalPlayerGoldHintSchedule schedule,OriginalPlayerGoldHintText text,
            OriginalPlayerGoldHintIcons icons,Func<string> language,Func<OriginalChannelInfo> selfChannel)
        {
            this.schedule=schedule ?? throw new ArgumentNullException(nameof(schedule));
            this.text=text ?? throw new ArgumentNullException(nameof(text));
            this.icons=icons ?? throw new ArgumentNullException(nameof(icons));
            this.language=language ?? throw new ArgumentNullException(nameof(language));
            this.selfChannel=selfChannel ?? throw new ArgumentNullException(nameof(selfChannel));
        }
        public void Init() { transform.localPosition=hiddenPosition; schedule.Init(); }
        public void Push() => schedule.Push();
        public void NextPush() => schedule.NextPush();
        public void Show() => schedule.Show();
        private void Update() { if(schedule!=null)schedule.Update(); }
        public void PresentPush(OriginalMarqueeItem item)
        {
            icons.ApplyPush(head);
            playerName.text=text.PushName(language());
            info.text=text.PushInfo(item,language());
            Add(MotionKind.PushIn);
        }
        public void RefreshTip() => icons.RefreshRandom(head);
        public void Show(Action callback)
        {
            RefreshTip();
            Add(MotionKind.CallbackIn,0,callback);
        }
        public void ShowSelf(float gold)
        {
            OriginalChannelInfo selected=selfChannel();
            playerName.text=text.SelfName(language());
            icons.ApplySelf(head,selected);
            info.text=text.SelfInfo(gold,language());
            Add(MotionKind.SelfIn);
        }
        private void Add(MotionKind kind,float delay=0,Action callback=null)
        {
            motions.Add(new Motion { Kind=kind,Delay=delay,Callback=callback });
            if(step==null)step=AdvanceMotion;
            OriginalUIAnimationDriver.Register(this,step);
        }
        public void AdvanceMotion(float delta)
        {
            // Snapshot before callbacks append exit tracks: those begin on the next update.
            int count=motions.Count;
            try
            {
                for(int i=0;i<count;i++)
                {
                    Motion m=motions[i];
                    m.Elapsed+=delta;
                    if(m.Elapsed<m.Delay) { motions[i]=m;continue; }
                    if(!m.Started) { m.Started=true;m.Start=transform.localPosition; }
                    float t=Mathf.Min(1,(m.Elapsed-m.Delay)/moveDuration);
                    bool outgoing=m.Kind==MotionKind.PushOut || m.Kind==MotionKind.CallbackOut;
                    float eased=t;
                    if(m.Kind==MotionKind.PushOut)eased=t*(2-t);
                    else if(m.Kind==MotionKind.CallbackIn)
                    {
                        float u=t-1;
                        eased=u*u*((backOvershoot+1)*u+backOvershoot)+1;
                    }
                    Vector3 end=outgoing?hiddenPosition:Vector3.zero;
                    Vector3 position=Vector3.LerpUnclamped(m.Start,end,eased);
                    if(m.Kind==MotionKind.CallbackIn || m.Kind==MotionKind.CallbackOut)
                    {
                        Vector3 current=transform.localPosition;
                        position.x=current.x;position.z=current.z;
                    }
                    transform.localPosition=position;
                    m.Complete=t>=1;motions[i]=m;
                    if(!m.Complete)continue;
                    if(m.Kind==MotionKind.PushIn)
                    {
                        if(effect!=null)effect.Play();
                        Add(MotionKind.PushOut,pushHold);
                    }
                    else if(m.Kind==MotionKind.SelfIn)
                    {
                        effect.Play();
                        Add(MotionKind.PushOut,pushHold);
                    }
                    else if(m.Kind==MotionKind.CallbackIn)Add(MotionKind.CallbackOut,callbackHold,m.Callback);
                    else if(m.Kind==MotionKind.CallbackOut)m.Callback?.Invoke();
                }
            }
            finally
            {
                int alive=0;
                for(int i=0;i<motions.Count;i++)if(!motions[i].Complete)motions[alive++]=motions[i];
                if(alive<motions.Count)motions.RemoveRange(alive,motions.Count-alive);
                if(motions.Count==0)OriginalUIAnimationDriver.Unregister(this);
            }
        }
        private void OnDestroy() => OriginalUIAnimationDriver.Unregister(this);
    }
}
