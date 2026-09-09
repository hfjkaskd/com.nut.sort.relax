using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalBootLoadingPanel:MonoBehaviour
    {
        [SerializeField] private Image Bar,Logo;
        [SerializeField] private RectTransform lS;
        [SerializeField] private TMP_Text BarValue;
        [SerializeField] private float markerWidth;
        [SerializeField] private OriginalBootLoadingFlow.Timing timing;
        private OriginalBootLoadingFlow flow;
        private float formattedValue=float.NaN;
        private CultureInfo formattedCulture;
        public OriginalBootLoadingFlow Flow=>flow;
        public Image Progress=>Bar;
        public TMP_Text Percent=>BarValue;
        public RectTransform Marker=>lS;
        public Image LogoImage=>Logo;
        private void Awake(){Initialize();OriginalUIAnimationDriver.Register(this,AdvanceTweens);}
        public void Initialize()
        {
            if(flow!=null)return;
            flow=new OriginalBootLoadingFlow(timing,()=>gameObject.activeSelf,v=>gameObject.SetActive(v),Display,
                (d,cb)=>OriginalUIAnimationDriver.Schedule(d,false,cb));
        }
        private void Start()=>StartDisplay();
        public void StartDisplay(){Initialize();flow.Start(Logo.SetNativeSize);}
        private void Update()=>flow.Update(Time.deltaTime);
        public void AdvanceProgress(float delta)=>flow.Update(delta);
        public void AdvanceTweens(float delta)=>flow.AdvanceTweens(delta);
        public void SetState(bool show){Initialize();flow.SetState(show);}
        private void Display(float value)
        {
            Bar.fillAmount=value;
            lS.anchoredPosition=new Vector2(Bar.fillAmount*markerWidth,0);
            // Avoid repeated formatting while stuck at the original 90% cap; preserve F0 culture/rounding.
            var culture=CultureInfo.CurrentCulture;
            if(value!=formattedValue||!ReferenceEquals(culture,formattedCulture))
            {BarValue.text=string.Format("{0:F0}%",value*100);formattedValue=value;formattedCulture=culture;}
        }
        private void OnDestroy()=>OriginalUIAnimationDriver.Unregister(this);
    }
}
