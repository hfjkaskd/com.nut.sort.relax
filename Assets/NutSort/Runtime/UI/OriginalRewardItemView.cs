using System;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    // UI.Item.Init (9C453C), ItemMgr.SetImage (9F61F0).
    public sealed class OriginalRewardItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text count;
        [SerializeField] private GameObject max;
        [SerializeField] private OriginalRewardItemSettings settings;
        private OriginalGoldFormatter formatter;
        private Func<string> country;
        public Image Icon => icon;
        public TMP_Text Count => count;
        public GameObject Max => max;
        public OriginalItemInfo ItemInfo { get; private set; }
        public void Bind(OriginalGoldFormatter sharedFormatter,Func<string> currentCountry)
        {
            formatter=sharedFormatter ?? throw new ArgumentNullException(nameof(sharedFormatter));
            country=currentCountry ?? throw new ArgumentNullException(nameof(currentCountry));
        }
        public void Init(OriginalItemInfo item,bool isBig=false,bool isShowMax=true)
        {
            ItemInfo=item;
            // Country state is read only for cash, as in GoldHelp.GetGoldSprite.
            string path=settings.GetIconPath(item.ItemType,isBig,item.ItemType==0?country():null);
            Sprite sprite=Resources.Load<Sprite>(path);
            if(sprite==null)Debug.LogError("GetSprite not find path: "+path);
            icon.sprite=sprite;
            icon.SetNativeSize();
            switch(item.ItemType)
            {
                case 0:count.text=formatter.Format(item.FloatCount);break;
                case 1:count.text=formatter.FormatCoin(item.DoubleCount) ?? string.Empty;break;
                default:count.text=string.Format("x{0}",item.IntCount);break;
            }
            // Both native branches hide Max; isShowMax is unused by this build.
            if(max!=null)max.SetActive(false);
        }
    }
}
