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
        private float maskRemaining;
        public OriginalReplayPanel Panel { get; private set; }
        public Button ReplayButton=>replayButton;
        public void Bind(OriginalGameScene scene, OriginalAudioPlayer audio, Transform panelCanvas, string languageCode, GameObject mask)
        {
            clickMask=mask;game=scene;audioPlayer=audio;canvas=panelCanvas;language=languageCode;
            replayButton.onClick.RemoveAllListeners();replayButton.onClick.AddListener(Open);
        }
        public bool BeginClick()
        {
            if(game==null || game.InputBlocked || game.IsRestarting || !game.Level.AreNutsInitialized || maskRemaining>0)return false;
            maskRemaining=settings.ClickMaskDuration;clickMask.SetActive(true);game.ModalInputBlocked=true;return true;
        }
        public void EndClick() { audioPlayer.PlaySound(settings.ClickSound); }
        private void Open()
        {
            if(Panel!=null || !BeginClick())return;
            var prefab=Resources.Load<GameObject>(settings.ReplayPath);
            Panel=Instantiate(prefab,canvas,false).GetComponent<OriginalReplayPanel>();
            Panel.Initialize(this,game.Tables,language);EndClick();
        }
        public void Restart() { game.RestartLevel(); }
        public void PanelClosed(OriginalReplayPanel panel)
        {
            if(Panel!=panel)return;Panel=null;Destroy(panel.gameObject);
            game.ModalInputBlocked=maskRemaining>0;
        }
        private void Update()
        {
            if(maskRemaining<=0)return;
            maskRemaining=Mathf.Max(0,maskRemaining-Time.deltaTime);
            if(maskRemaining==0)clickMask.SetActive(false);
            if(game!=null)game.ModalInputBlocked=Panel!=null||maskRemaining>0;
        }
        private void OnDestroy() { if(Panel!=null)Destroy(Panel.gameObject); }
    }
}
