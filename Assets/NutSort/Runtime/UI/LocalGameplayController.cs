using System;
using NutSort.Gameplay;
using NutSort.Content;
using NutSort.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    // Explicit user-authorized offline progression. No fabricated server response
    // or reward document; the original selector, world rules and success timing remain.
    public sealed partial class LocalGameplayController : MonoBehaviour
    {
        [SerializeField] private TMP_Text modeLabel, message;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Button next, dismiss;
        [SerializeField] private string modeText, successText, unavailableText;
        [SerializeField] private int maximumAddedTiles;
        private OriginalGameScene game;
        private OriginalAudioPlayer audio;
        private OriginalStartupFlow startup;
        private string language;
        private bool awaitingNext, bound;
        public bool AwaitingNext => awaitingNext;
        public bool ResultVisible => resultPanel.activeSelf||failurePanel!=null;
        public Button NextButton => next;
        public Button RetryButton => failurePanel==null?null:failurePanel.Restart;
        public Button DismissButton => dismiss;
        public TMP_Text ModeLabel => modeLabel;

        public void Bind(OriginalGameScene scene, OriginalAudioPlayer player, OriginalStartupFlow flow, string languageCode)
        {
            if (bound) throw new InvalidOperationException("Local gameplay is already bound.");
            game=scene; audio=player; startup=flow; language=languageCode; bound=true;
            modeLabel.text=modeText;
            resultPanel.SetActive(false);
            next.onClick.AddListener(Next);
            dismiss.onClick.AddListener(Dismiss);
            BindTools();
            BindCoreInitialization();
            game.BoardReady+=OnBoardReady;
            // The native initialization continuation needs unavailable reward-stage
            // documents. Local mode finishes only once the real world is ready.
            BindBoardCallbacks();
            if (!game.IsRestarting && game.Level.AreNutsInitialized) OnBoardReady();
        }

        private void BindBoardCallbacks()
        {
            var board=game.Level.Board;
            game.BindMoveCompletion((target,complete)=>
            {
                if (game.Level.Board!=board || game.IsRestarting) return;
                if (complete) game.Success();
            }, ClearGuide, ()=>{}, null, id=>
            {
                if (game.Level.Board==board && !game.IsRestarting && !game.IsSucceed) ShowFailure();
            });
            game.BindSuccess(()=>game.NewLevelMode,SaveTutorialTransition,()=>{},()=>{},transform.parent,
                captured=>
                {
                    if (game.Level.Board==board && game.IsSucceed && !game.IsRestarting) CompleteLocalLevel();
                });
        }

        private void OnBoardReady()
        {
            awaitingNext=false;
            resultPanel.SetActive(false);
            game.IsFail=false;
            game.IsInitDone=true;
            BindBoardCallbacks();
            BindToolsScene();
            SetExchange(false,0);
            bottom.Refresh();
            RefreshLevel();
            RefreshTeaching();
            BeginCoreInitialization();
            game.SaveUserData();
        }
        private void RefreshLevel() => startup.MainLevel.Refresh(game.Tables,game.PlayerLevel,language,false,false);
        private void ClearGuide()
        {
            CloseTeaching();
            for (int i=0;i<game.Level.Board.Screws.Length;i++) game.Level.GetScrew(i).ClearGuide();
        }
        private void SaveTutorialTransition()
        {
            PersistPendingBoard();
            RefreshLevel();
            RefreshTeaching();
        }
        private void PersistPendingBoard()
        {
            // Persist the new progress with no previous board. A process exit at
            // this boundary reconstructs the next board, never settles twice.
            game.User.LevelInfo=string.Empty;
            audio.UserState.Store.SaveData(false,null);
        }
        private void CompleteLocalLevel()
        {
            if (awaitingNext) return;
            game.User.Level=checked(game.User.Level+1);
            game.User.LevelSeed=0;
            game.User.TodayPassLevelCount=checked(game.User.TodayPassLevelCount+1);
            PersistPendingBoard();
            awaitingNext=true;
            game.IsInitDone=false;
            game.ModalInputBlocked=true;
            message.text=successText;
            next.gameObject.SetActive(true);
            dismiss.gameObject.SetActive(false);
            resultPanel.SetActive(true);
        }
        private void Next()
        {
            if (!awaitingNext) return;
            awaitingNext=false;
            resultPanel.SetActive(false);
            game.ModalInputBlocked=false;
            game.InitLevel(true,true,false);
        }
        private void ShowUnavailable() => ShowNotice(unavailableText);
        private void ShowNotice(string text)
        {
            if (resultPanel.activeSelf) return;
            game.ModalInputBlocked=true;
            message.text=text;
            next.gameObject.SetActive(false);
            dismiss.gameObject.SetActive(true);
            resultPanel.SetActive(true);
        }
        private void Dismiss()
        {
            if (!resultPanel.activeSelf || !dismiss.gameObject.activeSelf) return;
            resultPanel.SetActive(false);
            StartCoroutine(ReleaseNoticeInput());
        }
        private System.Collections.IEnumerator ReleaseNoticeInput()
        {
            // Keep the release that clicked the UI from also selecting a world rod.
            yield return null;
            if (HasLocalModal) yield break;
            if (game.IsFail) ShowFailure();
            else game.ModalInputBlocked=startup.Replay.Panel!=null;
        }
        private void OnApplicationPause(bool paused)
        {
            if (paused) SaveSession();
        }
        private void SaveSession()
        {
            if (!bound || game==null || game.Level==null || awaitingNext || game.IsRestarting || !game.Level.AreNutsInitialized) return;
            game.SaveUserData();
        }
        private void OnDestroy()
        {
            if (game!=null) { game.BoardReady-=OnBoardReady;game.OperationApplied-=RefreshToolsAfterOperation; }
            if(startup!=null&&startup.Replay!=null)startup.Replay.BindModalBlocker(null);
            next.onClick.RemoveListener(Next);
            dismiss.onClick.RemoveListener(Dismiss);
        }
    }
}
