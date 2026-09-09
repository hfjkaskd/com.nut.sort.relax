using NutSort.World;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalReplayController : MonoBehaviour
    {
        [SerializeField] private Button replayButton;
        [SerializeField] private OriginalPanelSettings settings;
        private OriginalGameScene game;
        private OriginalAudioPlayer audioPlayer;
        private Transform canvas;
        private GameObject clickMask;
        private string language;
        private System.Func<bool> additionalModal;
        public void BindModalBlocker(System.Func<bool> blocked) { additionalModal=blocked; }
        private OriginalCountedMask maskState;
        public OriginalReplayPanel Panel { get; private set; }
        public Button ReplayButton=>replayButton;
        public void Bind(OriginalGameScene scene, OriginalAudioPlayer audio, Transform panelCanvas, string languageCode, GameObject mask)
        {
            clickMask=mask;game=scene;audioPlayer=audio;canvas=panelCanvas;language=languageCode;
            maskState=new OriginalCountedMask(SetClickMask,game.ScheduleDelay);
            replayButton.onClick.RemoveAllListeners();replayButton.onClick.AddListener(Open);
        }
        public bool BeginClick()
        {
            if(game==null || game.InputBlocked || game.IsRestarting || !game.Level.AreNutsInitialized)return false;
            maskState.SetMask(true,settings.ClickMaskDuration);return true;
        }
        public void EndClick() { audioPlayer.PlaySound(settings.ClickSound); }
        private void Open()
        {
            if(Panel!=null || !BeginClick())return;
            OpenPanel();EndClick();
        }
        // Source panel creation, also reachable from the restored Bottom whose
        // common Button wrapper already owns click masking and sound.
        public void OpenPanel()
        {
            if(Panel!=null)return;
            var prefab=Resources.Load<GameObject>(settings.ReplayPath);
            Panel=Instantiate(prefab,canvas,false).GetComponent<OriginalReplayPanel>();
            Panel.Initialize(this,game.Tables,language);
        }
        public void Restart() { game.RestartLevel(); }
        public void PanelClosed(OriginalReplayPanel panel)
        {
            if(Panel!=panel)return;Panel=null;Destroy(panel.gameObject);
            game.ModalInputBlocked=maskState.Count>0 || (additionalModal!=null&&additionalModal());
        }
        private void SetClickMask(bool active)
        {
            clickMask.SetActive(active);
            game.ModalInputBlocked=Panel!=null || active || (additionalModal!=null&&additionalModal());
        }
        private void OnDestroy() { if(Panel!=null)Destroy(Panel.gameObject); }
    }
}
