using UnityEngine;
namespace NutSort.UI
{
    public sealed partial class LocalGameplayController
    {
        [SerializeField] private string successPrefabPath,localNextCaption;
        private OriginalSuccessPanel successPanel;
        public OriginalSuccessPanel SuccessPanel=>successPanel;
        private void ShowLocalSuccess()
        {
            successPanel=Instantiate(Resources.Load<GameObject>(successPrefabPath),transform.parent,false).GetComponent<OriginalSuccessPanel>();
            successPanel.InitLocal(game.Tables,language,TrySuccessClick,s=>audio.PlaySound(s),Next,localNextCaption);
            audio.PlaySound(gameplaySession.SuccessPanelSound);
        }
        private bool TrySuccessClick()
        {
            if(!awaitingNext||successPanel==null||successPanel.Closing||game.InputBlocked||game.IsRestarting||startup.Replay.IsClickMasked)return false;
            return startup.Replay.BeginClick();
        }
    }
}
