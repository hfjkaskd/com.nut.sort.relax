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
        [SerializeField] private float maskRevealDelay;
        [SerializeField] private float successGuideMaskDuration,successGuideReopenDelay;
        private Action<float,Action> scheduleMask;
        private OriginalTeachingFlow teachingFlow;
        private IOriginalGuideBranches guideBranches;
        private OriginalTables tables;
        private string language;
        private Action<bool> canOperate;
        private Action close;
        private Func<bool> beginClick;
        private Action clickSound;
        public Action ClickAction { get; set; }
        public static Action<object> CallbackAction { get; set; }

        // 0x9CF328 executes immediately and returns null, despite returning Action.
        // Do not clear before invocation or in finally: native exception and
        // replacement-during-callback behavior depends on this exact ordering.
        public static Action CallbackActionInvoke(object parameter)
        {
            CallbackAction?.Invoke(parameter);
            CallbackAction=null;
            return null;
        }

        // ShowGuide case 1, 0x9E2AE8. The supplied completion is the existing
        // request/application boundary, not a fabricated server response.
        public void ShowTargetCompletion(OriginalUserLocalData user,Action<object> completed,
            Action<int,int> showPanel)
        {
            if(completed==null)throw new ArgumentNullException(nameof(completed));
            if(showPanel==null)throw new ArgumentNullException(nameof(showPanel));
            CallbackAction=completed;
            Close();
            showPanel(35,unchecked(user.Level-1));
        }

        // ShowGuide case 0 (0x9E28E4), click closure 0x9E3FB0.
        // The supplied operation is SuccessPanel.MoreGetCallback (virtual slot 12).
        public void ShowSuccessGuide(OriginalUserLocalData user,Button moreGetButton,Action moreGet,
            Action<bool,float,string> setMask,Action<float,Action> schedule,Action<int> showPanel)
        {
            Hand.gameObject.SetActive(true);
            Button.gameObject.SetActive(true);
            Pos.position=moreGetButton.transform.position;
            Button.image.rectTransform.sizeDelta=moreGetButton.image.rectTransform.sizeDelta;
            SetTip(-1,0);
            ClickAction=()=>
            {
                moreGet();
                user.GuideIndex=unchecked(user.GuideIndex+1);
                Close();
                setMask(true,successGuideMaskDuration,string.Empty);
                schedule(successGuideReopenDelay,()=>showPanel(7));
            };
        }

        // ShowGuide case 2 (0x9E26E0); target is TXPanel's original control RectTransform.
        public void ShowWithdrawalGuide(OriginalUserLocalData user,RectTransform target,Action save,
            Action<int> showPanel,Action refreshGold,Action<bool,bool,bool> initializeLevel)
        {
            Hand.gameObject.SetActive(true);
            Button.gameObject.SetActive(true);
            Pos.position=target.position;
            Button.image.rectTransform.sizeDelta=target.sizeDelta;
            SetTip(-1,0);
            ClickAction=()=>
            {
                Close();
                user.GuideIndex=unchecked(user.GuideIndex+1);
                save();
                showPanel(36);
            };
            var completion=new OriginalWithdrawalGuideCompletion(user,InitialGuideIndex,refreshGold,initializeLevel);
            CallbackAction=completion.Complete;
        }

        // ShowGuide case 3, 0x9E2C1C. Request operation retains its existing
        // response binding; this view neither invents a payload nor grants coins.
        public void ShowCoinGuide(OriginalUserLocalData user,RectTransform target,Action refreshCoin,
            Action save,Action<bool> requestGoldInfo,Action<bool> newGameplayUnlock)
        {
            Hand.gameObject.SetActive(true);
            Button.gameObject.SetActive(true);
            Pos.position=target.position;
            Button.image.rectTransform.sizeDelta=target.sizeDelta;
            SetTip(127,0);
            ShowMask(target);
            user.IsShowCoin=true;
            refreshCoin();
            save();
            ClickAction=()=>
            {
                requestGoldInfo(false);
                user.GuideIndex=unchecked(user.GuideIndex+1);
                Close();
            };
            CallbackAction=parameter=>newGameplayUnlock(false);
        }

        public void BindGuide(OriginalUserLocalData user,Func<bool> skipTeaching,IOriginalGuideBranches branches)
        {
            teachingFlow=new OriginalTeachingFlow(user,this,skipTeaching);
            guideBranches=branches ?? throw new ArgumentNullException(nameof(branches));
        }
        // 0x9E201C: teaching precedes even the out-of-range/index no-op branches.
        public void ShowGuide()
        {
            if(teachingFlow.Run())return;
            switch(InitialGuideIndex)
            {
                case 0:guideBranches.ShowSuccess();break;
                case 1:guideBranches.ShowTargetCompletion();break;
                case 2:guideBranches.ShowWithdrawal();break;
                case 3:guideBranches.ShowCoin();break;
                case 10:case 12:case 14:guideBranches.ShowGoldEntry(InitialGuideIndex);break;
                case 11:case 13:case 15:guideBranches.ShowWithdrawalStage(InitialGuideIndex);break;
            }
        }

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
        public void BindMask(Action<float,Action> schedule)
        {
            scheduleMask=schedule ?? throw new ArgumentNullException(nameof(schedule));
        }
        // ShowMask 0x9E3564: source sizeDelta, world position, bounds, delayed reveal.
        // Null targets reveal immediately without changing or refreshing the hole.
        public void ShowMask(RectTransform target)
        {
            if(target==null){RevealMask();return;}
            MaskRect.sizeDelta=target.sizeDelta;
            MaskRect.position=target.position;
            hollowMask.RefreshBounds();
            scheduleMask(maskRevealDelay,RevealMask);
        }
        private void RevealMask()=>hollowMask.gameObject.SetActive(true);
        public void Close()=>close();
    }
}
