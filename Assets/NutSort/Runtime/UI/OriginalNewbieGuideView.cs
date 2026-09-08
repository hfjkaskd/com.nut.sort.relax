using System;
using NutSort.Content;
using NutSort.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    // Original visual references; full ShowGuide routing is still being restored.
    public sealed class OriginalNewbieGuideView : MonoBehaviour,IOriginalTeachingView
    {
        [SerializeField] private RectTransform MaskRect,Pos,Hand,Arraw;
        [SerializeField] private TextMeshProUGUI TipValue;
        [SerializeField] private Button Closebtn,Button,ButtonFull;
        [SerializeField] private Transform[] TipPos;
        [SerializeField] private GameObject Teach;
        [SerializeField] private Image[] TeachLevels;
        [SerializeField] private OriginalHollowMaskGraphic hollowMask;
        [SerializeField] private string emptyMarkerPath,filledMarkerPath;
        private OriginalTables tables;
        private string language;
        private Action<bool> canOperate;
        private Action close;
        private Func<bool> beginClick;
        private Action clickSound;
        public Action ClickAction { get; set; }
        public bool IsOpen { get; private set; }
        public bool ShowBanner { get; private set; } = true;
        public int InitialGuideIndex { get; private set; }
        public Button CloseButton=>Closebtn;
        public Button ContinueButton=>Button;
        public Button FullButton=>ButtonFull;

        // InitLssPanel 0x9E1D78 after the base panel's label/visual setup.
        public void InitializeInteractions(OriginalUserLocalData user,bool? showBanner,
            Func<bool> beginClick,Action clickSound)
        {
            this.beginClick=beginClick ?? throw new ArgumentNullException(nameof(beginClick));
            this.clickSound=clickSound ?? throw new ArgumentNullException(nameof(clickSound));
            IsOpen=true;
            if(showBanner.HasValue)ShowBanner=showBanner.Value;
            InitialGuideIndex=user.GuideIndex;
            hollowMask.gameObject.SetActive(false);
            Closebtn.gameObject.SetActive(false);
            Closebtn.onClick.RemoveAllListeners();Closebtn.onClick.AddListener(CloseClicked);
            Button.onClick.RemoveAllListeners();Button.onClick.AddListener(ContinueClicked);
            ButtonFull.onClick.RemoveAllListeners();ButtonFull.onClick.AddListener(ContinueClicked);
        }
        private void ContinueClicked()
        {
            if(!beginClick())return;
            ClickAction?.Invoke();
            ClickAction=null;
            clickSound();
        }
        private void CloseClicked()
        {
            if(!beginClick())return;
            IsOpen=false;
            Close();
            clickSound();
        }

        public OriginalHollowMaskGraphic HollowMask=>hollowMask;
        public TMP_Text Tip=>TipValue;
        public int MarkerCount=>TeachLevels.Length;
        public void BindTeaching(OriginalTables tables,string language,Action<bool> canOperate,Action close)
        {
            this.tables=tables ?? throw new ArgumentNullException(nameof(tables));this.language=language;
            this.canOperate=canOperate ?? throw new ArgumentNullException(nameof(canOperate));
            this.close=close ?? throw new ArgumentNullException(nameof(close));
        }
        public void SetCanOperateScrew(bool value)=>canOperate(value);
        public void SetTeachingVisible(bool value)=>Teach.SetActive(value);
        public void SetHandVisible(bool value)=>Hand.gameObject.SetActive(value);
        public void SetHollowMaskVisible(bool value)=>hollowMask.gameObject.SetActive(value);
        public void SetMarker(int index,bool filled)=>TeachLevels[index].sprite=Resources.Load<Sprite>(filled?filledMarkerPath:emptyMarkerPath);
        public void SetTip(int textId,int positionIndex)
        {
            if(textId<0){TipValue.transform.parent.gameObject.SetActive(false);return;}
            Transform position=TipPos[positionIndex];
            if(position==null){position=TipPos[0];Debug.LogError("SetTipValue pos == null");}
            TipValue.transform.parent.position=position.position;
            TipValue.transform.parent.gameObject.SetActive(true);
            TipValue.text=tables.Text.GetText(textId,language);
        }
        public void Close()=>close();
    }
}
