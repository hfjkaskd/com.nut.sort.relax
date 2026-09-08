using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    // OtherItem.Refresh and RefreshButtonState; click actions belong to its controller.
    public sealed class OriginalToolItemDisplay : MonoBehaviour
    {
        [SerializeField] private int itemType;
        [SerializeField] private Image icon,countIcon;
        [SerializeField] private TMP_Text value;
        [SerializeField] private Button click;
        [SerializeField] private string countPath,addPath,grayPath;
        [SerializeField] private int defaultMaximum;
        private OriginalUserLocalData user;
        private Func<bool> hasLevel;
        private Func<int> historyCount;
        private readonly List<Image> images=new List<Image>(4);
        private static Material gray;
        public int ItemType => itemType;
        public Image Icon => icon;
        public Image CountIcon => countIcon;
        public TMP_Text Value => value;
        public Button Click => click;
        public void Bind(OriginalUserLocalData user,Func<bool> hasLevel,Func<int> historyCount)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.hasLevel=hasLevel ?? throw new ArgumentNullException(nameof(hasLevel));
            this.historyCount=historyCount ?? throw new ArgumentNullException(nameof(historyCount));
        }
        public void Refresh()
        {
            int count;
            switch(itemType)
            {
                case 2:count=user.RevokeCount;break;
                case 3:count=user.ExchangeCount;break;
                case 4:count=user.AddScrewCount;break;
                default:return;
            }
            value.text=count>0?count.ToString():string.Empty;
            countIcon.sprite=Resources.Load<Sprite>(count>0?countPath:addPath);
        }
        public void RefreshButtonState()
        {
            bool disabled;
            switch(itemType)
            {
                case 2:disabled=hasLevel() && historyCount()<1;break;
                case 4:
                    if(!hasLevel())disabled=false;
                    else
                    {
                        int used=user.CurrentLevelAddScrewCount;
                        JObject config=user.ServerConfigData ?? throw new NullReferenceException("ServerConfigData");
                        JToken maximum=null;
                        foreach(JProperty field in config.Properties())
                            if(string.Equals(field.Name,"LSSLSMAC",StringComparison.OrdinalIgnoreCase))maximum=field.Value;
                        disabled=used>=(maximum==null?defaultMaximum:(int)maximum);
                    }
                    break;
                default:return;
            }
            // Source SetGray loads even when clearing, and only touches active Image descendants.
            if(gray==null)gray=Resources.Load<Material>(grayPath);
            GetComponentsInChildren(false,images);
            foreach(Image image in images)image.material=disabled?gray:null;
            // Gray is visual feedback, not a Selectable.interactable change.
        }
    }
}
