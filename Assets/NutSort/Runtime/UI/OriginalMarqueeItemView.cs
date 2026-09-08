using System;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalMarqueeItemView : MonoBehaviour
    {
        [SerializeField] private Image headImage;
        [SerializeField] private TextMeshProUGUI tip;
        [SerializeField] private RectTransform rect;
        [SerializeField] private float widthDelay, widthPadding;
        [SerializeField] private string iconResourceFormat;
        private OriginalMarqueeText text;
        private OriginalPayChannels channels;
        private Func<string> currentCountry;
        private Action<float,Action> delayCallback;
        private Func<string,Sprite> loadSprite;
        private Func<int,int,int> range;
        private Action refreshWidth;
        private string language;

        public bool IsReady { get; set; }
        public RectTransform Rect => rect;
        public Image HeadImage => headImage;
        public TextMeshProUGUI Tip => tip;

        public void Bind(OriginalMarqueeText text,OriginalPayChannels channels,string language,
            Func<string> currentCountry,Action<float,Action> delayCallback,
            Func<string,Sprite> loadSprite=null,Func<int,int,int> range=null)
        {
            this.text=text ?? throw new ArgumentNullException(nameof(text));
            this.channels=channels ?? throw new ArgumentNullException(nameof(channels));
            this.language=language;
            this.currentCountry=currentCountry ?? throw new ArgumentNullException(nameof(currentCountry));
            this.delayCallback=delayCallback ?? throw new ArgumentNullException(nameof(delayCallback));
            this.loadSprite=loadSprite ?? Resources.Load<Sprite>;
            this.range=range ?? UnityEngine.Random.Range;
            refreshWidth=RefreshWidth;
        }

        public void Init(OriginalMarqueeItem item,int capturedLevel)
        {
            tip.text=text.Build(item,capturedLevel,language);
            string channel=channels.GetPayChannel(string.Empty,currentCountry).RandomChannel(range);
            Sprite sprite=loadSprite(string.Format(iconResourceFormat,channel));
            if(sprite!=null)headImage.sprite=sprite;
            delayCallback(widthDelay,refreshWidth);
        }

        private void RefreshWidth()
        {
            rect.sizeDelta=new Vector2(tip.rectTransform.sizeDelta.x+widthPadding,rect.sizeDelta.y);
        }
    }
}
