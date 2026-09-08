using System;
using NutSort.Content;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalToolItemController : MonoBehaviour
    {
        [SerializeField] private OriginalToolItemDisplay display;
        [SerializeField] private int toolPanelId,noHistoryTipId;
        private OriginalUserLocalData user;
        private Func<int> historyCount;
        private Func<bool> tryClick;
        private Action addScrew,revoke,playClick,action;
        private Action<int,float,bool> applyItem;
        private Action<bool,float> setExchangeState;
        private Action<int,int> openToolPanel;
        private Action<int> showTip;
        public OriginalToolItemDisplay Display => display;
        public void Bind(OriginalUserLocalData user,Func<int> historyCount,Action addScrew,Action revoke,
            Action<int,float,bool> applyItem,Action<bool,float> setExchangeState,
            Action<int,int> openToolPanel,Action<int> showTip,Func<bool> tryClick,Action playClick)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.historyCount=historyCount ?? throw new ArgumentNullException(nameof(historyCount));
            this.addScrew=addScrew ?? throw new ArgumentNullException(nameof(addScrew));
            this.revoke=revoke ?? throw new ArgumentNullException(nameof(revoke));
            this.applyItem=applyItem ?? throw new ArgumentNullException(nameof(applyItem));
            this.setExchangeState=setExchangeState ?? throw new ArgumentNullException(nameof(setExchangeState));
            this.openToolPanel=openToolPanel ?? throw new ArgumentNullException(nameof(openToolPanel));
            this.showTip=showTip ?? throw new ArgumentNullException(nameof(showTip));
            this.tryClick=tryClick ?? throw new ArgumentNullException(nameof(tryClick));
            this.playClick=playClick ?? throw new ArgumentNullException(nameof(playClick));
        }
        public void Init()
        {
            switch(display.ItemType)
            {
                case 2:action=Revoke;break;
                case 3:action=Exchange;break;
                case 4:action=AddScrew;break;
                default:return;
            }
            display.Click.transition=Selectable.Transition.ColorTint;
            display.Click.onClick.RemoveAllListeners();display.Click.onClick.AddListener(Clicked);
        }
        private void Clicked()
        {
            if(!tryClick())return;
            action();
            playClick();
        }
        private void AddScrew() => addScrew();
        private void Revoke()
        {
            if(user.RevokeCount<=0) { openToolPanel(toolPanelId,display.ItemType);return; }
            if(historyCount()<=0) { showTip(noHistoryTipId);return; }
            revoke();
            // Source calls the item manager after Revoke; it does not decrement the field here.
            applyItem(display.ItemType,-1f,true);
            // Source SDK vibration/analytics remain excluded.
        }
        private void Exchange()
        {
            if(user.ExchangeCount>0)setExchangeState(true,0);
            else openToolPanel(toolPanelId,display.ItemType);
        }
    }
}
