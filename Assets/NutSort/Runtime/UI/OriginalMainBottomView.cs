using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.World;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalMainBottomView : MonoBehaviour
    {
        [SerializeField] private Button setting,replay;
        [SerializeField] private List<OriginalToolItemController> items;
        [SerializeField] private int settingsPanelId,replayPanelId;
        private Func<bool> tryClick;
        private Action<int> openPanel;
        private Action playClick;
        public Button Setting => setting;
        public Button Replay => replay;
        public IReadOnlyList<OriginalToolItemController> Items => items;
        public void Bind(OriginalUserLocalData user,Func<bool> hasLevel,Func<int> historyCount,
            Action addScrew,Action revoke,Action<int,float,bool> applyItem,Action<bool,float> setExchangeState,
            Action<int,int> openToolPanel,Action<int> showTip,Func<bool> tryClick,Action playClick,Action<int> openPanel,Func<int> maximumAddedTiles=null)
        {
            this.tryClick=tryClick ?? throw new ArgumentNullException(nameof(tryClick));
            this.playClick=playClick ?? throw new ArgumentNullException(nameof(playClick));
            this.openPanel=openPanel ?? throw new ArgumentNullException(nameof(openPanel));
            foreach(var item in items)
            {
                item.Display.Bind(user,hasLevel,historyCount,maximumAddedTiles);
                item.Bind(user,historyCount,addScrew,revoke,applyItem,setExchangeState,openToolPanel,showTip,tryClick,playClick);
            }
        }
        // Compose the source manager operations once for both UI and world clicks.
        public void BindScene(OriginalGameScene game,OriginalItemManager items,Func<int> maximum,Func<bool> singleTile,
            Action<bool,float> setExchangeState,Action<int,int> openToolPanel,Action<int> showTip,
            Func<bool> tryClick,Action playClick,Action<int> openPanel)
        {
            game.BindAddScrew(maximum,singleTile,items,Refresh,openToolPanel,showTip);
            game.BindExchange(items,openPanel);
            Bind(game.User,()=>game.Level!=null,()=>game.Level.MoveHistoryCount,game.AddScrew,
                ()=>game.Revoke(RefreshOtherItemButtonState,openPanel),items.AddTool,setExchangeState,
                openToolPanel,showTip,tryClick,playClick,openPanel,maximum);
        }

        public void Init()
        {
            setting.transition=Selectable.Transition.ColorTint;
            setting.onClick.RemoveAllListeners();setting.onClick.AddListener(SettingClicked);
            replay.transition=Selectable.Transition.ColorTint;
            replay.onClick.RemoveAllListeners();replay.onClick.AddListener(ReplayClicked);
            foreach(var item in items)item.Init();
        }
        public void Refresh()
        {
            foreach(var item in items) { item.Display.Refresh();item.Display.RefreshButtonState(); }
        }
        public void RefreshOtherItem(int type)
        {
            foreach(var item in items)if(item.Display.ItemType==type) { item.Display.Refresh();return; }
        }
        public void RefreshOtherItemButtonState(int type)
        {
            foreach(var item in items)if(item.Display.ItemType==type) { item.Display.RefreshButtonState();return; }
        }
        public OriginalToolItemController GetOtherItem(int type)
        {
            foreach(var item in items)if(item.Display.ItemType==type)return item;
            // Source diagnostic enum names are explicit, never reflection or obfuscation-dependent.
            string name;
            switch(type) { case 0:name="Gold";break;case 1:name="Coin";break;case 2:name="Revoke";break;case 3:name="Exchange";break;case 4:name="AddScrew";break;default:name=type.ToString();break; }
            Debug.LogError("not find itemType: "+name);return null;
        }
        private void SettingClicked() { if(!tryClick())return;openPanel(settingsPanelId);playClick(); }
        private void ReplayClicked() { if(!tryClick())return;openPanel(replayPanelId);playClick(); }
    }
}
