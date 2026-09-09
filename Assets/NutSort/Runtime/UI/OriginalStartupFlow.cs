using System.Collections;
using NutSort.World;
using UnityEngine;

namespace NutSort.UI
{
    public sealed class OriginalStartupFlow : MonoBehaviour
    {
        [SerializeField] private OriginalLoadingView loading;
        [SerializeField] private OriginalGameScene game;
        [SerializeField] private Transform panelCanvas,topCanvas;
        public Transform TopCanvas=>topCanvas;
        [SerializeField] private OriginalMainLevelSettings mainLevelSettings;
        [SerializeField] private OriginalAudioPlayer audioPlayer;
        [SerializeField] private GameObject clickMask;
        public OriginalReplayController Replay { get; private set; }
        public OriginalMainLevelView MainLevel { get; private set; }
        public OriginalTargetRewardBanner TargetReward { get; private set; }
        public OriginalLoadingView Loading => loading;
        public LocalGameplayController LocalGameplay { get; private set; }

        private IEnumerator Start()
        {
            game.InputBlocked = true;
            loading.SetState(true);
            yield return null;
            audioPlayer.PlayBgm();
            // Current local boot has no SDK/server wait to retain. Their future
            // real completion belongs before this signal, never a fake grant.
            loading.SetState(false);
            yield return null;
            // Source boot activates GameScene on the frame after the loading
            // completion signal, while its progress tween is still finishing.
            game.gameObject.SetActive(true);
            game.Initialize();
            var mainPrefab = Resources.Load<GameObject>(mainLevelSettings.PrefabPath);
            MainLevel = Instantiate(mainPrefab, panelCanvas, false).GetComponent<OriginalMainLevelView>();
            TargetReward = MainLevel.GetComponentInChildren<OriginalTargetRewardBanner>(true);
            TargetReward.Init();
            Replay = MainLevel.GetComponent<OriginalReplayController>();
            Replay.Bind(game, audioPlayer, panelCanvas, mainLevelSettings.LanguageCode, clickMask);
            MainLevel.Refresh(game.Tables, game.PlayerLevel, mainLevelSettings.LanguageCode, false, false);
            if (!string.IsNullOrEmpty(mainLevelSettings.LocalGameplayPath))
            {
                var localPrefab = Resources.Load<GameObject>(mainLevelSettings.LocalGameplayPath);
                LocalGameplay = Instantiate(localPrefab, panelCanvas, false).GetComponent<LocalGameplayController>();
                LocalGameplay.Bind(game, audioPlayer, this, mainLevelSettings.LanguageCode);
            }
            while (loading.IsVisible) yield return null;
            game.InputBlocked = false;
        }
    }
}
