using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed partial class LocalGameplayController
    {
        [SerializeField] private OriginalMainBottomView bottom;
        [SerializeField] private Button exchangeMask;
        [SerializeField] private float exchangeCancelDelay;
        [SerializeField] private string noHistoryText,limitText;
        private OriginalItemManager tools;
        public OriginalMainBottomView Bottom=>bottom;
        public Button ExchangeMask=>exchangeMask;

        private void BindTools()
        {
            tools=new OriginalItemManager(game.User,game.SaveUserData,bottom.Refresh,(n,r,s)=>{},(n,r)=>{});
            startup.Replay.ReplayButton.transform.parent.gameObject.SetActive(false);
            startup.Replay.BindModalBlocker(()=>HasLocalModal);
            BindToolsScene();bottom.Init();
            exchangeMask.onClick.RemoveAllListeners();exchangeMask.onClick.AddListener(CancelExchange);
            exchangeMask.gameObject.SetActive(false);
            game.OperationApplied+=RefreshToolsAfterOperation;
        }
        private void BindToolsScene()
        {
            var board=game.Level.Board;
            bottom.BindScene(game,tools,()=>maximumAddedTiles,()=>false,SetExchange,
                (id,type)=>ShowUnavailable(),ShowToolTip,TryToolClick,startup.Replay.EndClick,id=>
                {
                    if(game.Level.Board!=board||game.IsRestarting)return;
                    if(id==17)startup.Replay.OpenPanel();
                    else if(id==10&&!game.IsSucceed)ShowFailure();
                    else ShowUnavailable();
                });
        }
        private bool TryToolClick()
        {
            if(game.InputBlocked||game.ModalInputBlocked||game.IsRestarting||!game.IsInitDone||game.IsSucceed)return false;
            return startup.Replay.BeginClick();
        }
        private void RefreshToolsAfterOperation(ScrewOperation operation,ScrewState target) => bottom.Refresh();
        private void ShowToolTip(int id) => ShowNotice(id==3?noHistoryText:limitText);
        private void SetExchange(bool active,float delay) => game.SetExchangeState(exchangeMask.gameObject,active,delay);
        private void CancelExchange()
        {
            if(!TryToolClick())return;
            SetExchange(false,exchangeCancelDelay);
            startup.Replay.EndClick();
        }
    }
}
