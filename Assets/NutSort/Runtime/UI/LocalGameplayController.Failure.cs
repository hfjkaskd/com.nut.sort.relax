using System;
using UnityEngine;
namespace NutSort.UI
{
    public sealed partial class LocalGameplayController
    {
        [SerializeField] private string failurePrefabPath;
        private OriginalFailurePanelView failurePanel;
        private readonly OriginalPanelActionQueue failureQueue=new OriginalPanelActionQueue();
        public OriginalFailurePanelView FailurePanel=>failurePanel;
        private void ShowFailure()
        {
            if(awaitingNext||ResultVisible)return;
            game.ModalInputBlocked=true;
            failurePanel=Instantiate(Resources.Load<GameObject>(failurePrefabPath),transform.parent,false).GetComponent<OriginalFailurePanelView>();
            failurePanel.Bind(game.User,game.Tables,language,TryFailureClick,s=>audio.PlaySound(s),game.RestartAfterFailure,
                ()=>game.IsFail=false,()=>throw new InvalidOperationException("Local failure does not expose SDK revive."),CloseFailure);
            failurePanel.BindHide(failureQueue,game.ScheduleDelay);
            failurePanel.Init();
            // Refresh consumes account reward/config documents. Its reward and
            // revive subtrees are absent from the active local prefab layout.
        }
        private bool TryFailureClick()
        {
            if(failurePanel==null||game.InputBlocked||game.IsRestarting||!game.IsInitDone||startup.Replay.IsClickMasked)return false;
            return startup.Replay.BeginClick();
        }
        private void CloseFailure()
        {
            if(failurePanel==null)return;
            failurePanel.Hide();
            Destroy(failurePanel.gameObject);failurePanel=null;
            PersistPendingBoard();
            game.ModalInputBlocked=HasLocalModal||startup.Replay.Panel!=null;
        }
    }
}
