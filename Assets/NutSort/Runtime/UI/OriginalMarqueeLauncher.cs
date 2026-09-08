using System;
using System.Collections.Generic;
using NutSort.Content;
using UnityEngine;

namespace NutSort.UI
{
    public sealed class OriginalMarqueeLauncher : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private OriginalMarqueeItemView template;
        [SerializeField] private float minimumAspect, stopTime;
        [SerializeField] private int minimumLevel;
        [SerializeField] private Vector2 firstInterval, secondInterval, travelDuration;
        private Func<int> currentLevel;
        private Func<OriginalMarqueeItem> nextData;
        private Action<OriginalMarqueeItemView> bindItem;
        private Func<float,float,float> range;
        private List<OriginalMarqueeItemView> items;
        private bool started;
        private int capturedLevel, launchTimes;
        private float elapsed;

        public OriginalMarqueeItemView Template => template;
        public RectTransform Rect => rect;
        public int PoolCount => items==null?0:items.Count;

        public void Bind(Func<int> currentLevel,Func<OriginalMarqueeItem> nextData,
            Action<OriginalMarqueeItemView> bindItem,Func<float,float,float> range=null)
        {
            this.currentLevel=currentLevel ?? throw new ArgumentNullException(nameof(currentLevel));
            this.nextData=nextData ?? throw new ArgumentNullException(nameof(nextData));
            this.bindItem=bindItem ?? throw new ArgumentNullException(nameof(bindItem));
            this.range=range ?? UnityEngine.Random.Range;
            bindItem(template);
        }

        public void Init() => Init((float)Screen.height/Screen.width);
        public void Init(float screenAspect)
        {
            if(screenAspect<minimumAspect)return;
            capturedLevel=currentLevel();
            items=new List<OriginalMarqueeItemView> { template };
            template.IsReady=true;
            started=true;
        }

        private void Update() => AdvanceLaunch(Time.deltaTime);
        public void AdvanceLaunch(float delta)
        {
            if(!started || currentLevel()<minimumLevel)return;
            elapsed+=delta;
            if(elapsed<stopTime)return;
            launchTimes++;
            if(launchTimes<=1) stopTime=range(firstInterval.x,firstInterval.y);
            else { stopTime=range(secondInterval.x,secondInterval.y);launchTimes=0; }
            elapsed=0;
            StartLaunch();
        }

        public void StartLaunch()
        {
            if(nextData()==null)return;
            OriginalMarqueeItemView item=null;
            foreach(var candidate in items)
                if(candidate.IsReady) { item=candidate;break; }
            if(item==null)
            {
                item=Instantiate(template,transform);
                items.Add(item);
                bindItem(item);
            }
            // PMDSimple.SetText performs its own second selection.
            OriginalMarqueeItem data=nextData();
            if(data!=null)item.Init(data,capturedLevel);
            float width=rect.sizeDelta.x, height=rect.sizeDelta.y;
            range(-height*.5f,height*.5f); // The original consumes this unused sample.
            float halfItem=template.Rect.sizeDelta.y*.5f;
            float y=(launchTimes&1)==0?range(halfItem,height*.5f):range(-height*.5f,-halfItem);
            item.IsReady=false;
            item.transform.localPosition=new Vector3(width*.5f,y,0);
            item.StartTravel(-width*.5f,range(travelDuration.x,travelDuration.y));
        }
    }
}
