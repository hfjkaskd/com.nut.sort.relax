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
    public sealed class LocalGameplayController : MonoBehaviour
    {
        [SerializeField] private TMP_Text modeLabel, message;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Button next, retry, dismiss;
        [SerializeField] private string modeText, successText, failureText, unavailableText;
        [SerializeField] private int maximumAddedTiles;
        private OriginalGameScene game;
        private OriginalAudioPlayer audio;
        private OriginalStartupFlow startup;
        private string language;
        private bool awaitingNext, bound;
        public bool AwaitingNext => awaitingNext;
        public bool ResultVisible => resultPanel.activeSelf;
        public Button NextButton => next;
        public Button RetryButton => retry;
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
            var items=new OriginalItemManager(game.User,game.SaveUserData,()=>{},(n,r,s)=>{},(n,r)=>{});
            game.BindAddScrew(()=>maximumAddedTiles,()=>false,items,()=>{},(id,type)=>ShowUnavailable(),id=>ShowUnavailable());
            retry.onClick.AddListener(Retry);
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
            RefreshLevel();
            game.SaveUserData();
        }
        private void RefreshLevel() => startup.MainLevel.Refresh(game.Tables,game.PlayerLevel,language,false,false);
        private void ClearGuide()
        {
            for (int i=0;i<game.Level.Board.Screws.Length;i++) game.Level.GetScrew(i).ClearGuide();
        }
        private void SaveTutorialTransition()
        {
            PersistPendingBoard();
            RefreshLevel();
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
            retry.gameObject.SetActive(false);
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
        private void ShowFailure()
        {
            if (awaitingNext || resultPanel.activeSelf) return;
            game.ModalInputBlocked=true;
            message.text=failureText;
            next.gameObject.SetActive(false);
            retry.gameObject.SetActive(true);
            dismiss.gameObject.SetActive(false);
            resultPanel.SetActive(true);
        }
        private void ShowUnavailable()
        {
            if (resultPanel.activeSelf) return;
            game.ModalInputBlocked=true;
            message.text=unavailableText;
            next.gameObject.SetActive(false);
            retry.gameObject.SetActive(false);
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
            if (resultPanel.activeSelf) yield break;
            if (game.IsFail) ShowFailure();
            else game.ModalInputBlocked=false;
        }
        private void Retry()
        {
            if (awaitingNext || !resultPanel.activeSelf) return;
            game.RestartAfterFailure(()=>
            {
                resultPanel.SetActive(false);
                game.ModalInputBlocked=false;
                PersistPendingBoard();
            });
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
            if (game!=null) game.BoardReady-=OnBoardReady;
            next.onClick.RemoveListener(Next);
            retry.onClick.RemoveListener(Retry);
            dismiss.onClick.RemoveListener(Dismiss);
        }
    }
}
