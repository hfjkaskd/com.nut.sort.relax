using System;
using NutSort.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalMainPanelView : MonoBehaviour
    {
        [SerializeField] private OriginalMainTopView top;
        [SerializeField] private OriginalMainBottomView bottom;
        [SerializeField] private Button exchangeMask;
        [SerializeField] private OriginalTargetRewardBanner targetReward;
        [SerializeField] private OriginalPlayerGoldHintView playerHint;
        [SerializeField] private float exchangeCancelDelay;
        private OriginalTables tables;
        private string language;
        private Func<bool> tryClick;
        private Action playClick;
        private Action<bool,float> setExchangeState;
        public OriginalMainTopView Top => top;
        public OriginalMainBottomView Bottom => bottom;
        public Button ExchangeMask => exchangeMask;
        public OriginalTargetRewardBanner TargetReward => targetReward;
        public OriginalPlayerGoldHintView PlayerHint => playerHint;
        public void Bind(OriginalTables tables,string language,Func<bool> tryClick,Action playClick,Action<bool,float> setExchangeState)
        {
            this.tables=tables ?? throw new ArgumentNullException(nameof(tables));this.language=language;
            this.tryClick=tryClick ?? throw new ArgumentNullException(nameof(tryClick));
            this.playClick=playClick ?? throw new ArgumentNullException(nameof(playClick));
            this.setExchangeState=setExchangeState ?? throw new ArgumentNullException(nameof(setExchangeState));
        }
        public void Init()
        {
            // The original MainPanel has no child named "main", so BaseLSSPanel's
            // optional panel tween is absent. Its label pass still precedes Top.Init.
            foreach(var label in GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if(!label.name.StartsWith("label_"))continue;
                string token=label.name.Trim().Split('_')[1];
                if(!int.TryParse(token,out int id)) { Debug.LogError("ToInt fail s:"+token);id=0; }
                label.text=tables.Text.GetText(id,language);
            }
            top.Init();bottom.Init();targetReward.Init();playerHint.Init();
            exchangeMask.transition=Selectable.Transition.None;
            exchangeMask.onClick.RemoveAllListeners();exchangeMask.onClick.AddListener(MaskClicked);
            exchangeMask.gameObject.SetActive(false);
        }
        public void SetExchangeState(NutSort.World.OriginalGameScene game,bool enabled,float delay=0)
        {
            game.SetExchangeState(exchangeMask.gameObject,enabled,delay);
        }
        public void Refresh() { top.Refresh();bottom.Refresh(); }
        private void MaskClicked()
        {
            if(!tryClick())return;
            setExchangeState(false,exchangeCancelDelay);playClick();
        }
    }
}
