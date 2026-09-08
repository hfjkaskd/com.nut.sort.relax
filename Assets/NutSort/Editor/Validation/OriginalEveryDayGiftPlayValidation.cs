using System;
using NutSort.Content;
using NutSort.World;
using NutSort.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalEveryDayGiftPlayValidation
    {
        private const string Key="NutSort.EveryDayGiftPlayValidation";
        private static OriginalGameScene game;
        private static int phase,shown;
        private static bool modal=true;
        private static double timeout,deadline;
        private static float previousTimeScale;
        static OriginalEveryDayGiftPlayValidation()
        {
            if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}
        }
        private sealed class PanelObserver:IOriginalGuidePanels
        {
            public bool HasPanel=>modal;
            public void ShowPanel(int id)
            {
                Check(id==16&&game.User.LastGetEveryDayGift==0,"Show precedes timestamp assignment");
                shown++;
            }
            public OriginalGuideSuccessBinding GetSuccessGuideTarget()=>throw new InvalidOperationException("Unexpected success panel");
            public OriginalGuideButtonBinding GetWithdrawal()=>throw new InvalidOperationException("Unexpected withdrawal panel");
            public void ShowTargetPanel(int id,int level)=>throw new InvalidOperationException("Unexpected target panel");
            public void ShowUnlockPanel(int id,int index,bool banner)=>throw new InvalidOperationException("Unexpected unlock panel");
        }
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;
                if(now>timeout)throw new InvalidOperationException("Daily gift Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||!game.Level.AreNutsInitialized||game.InputBlocked)return;
                if(phase==0)
                {
                    previousTimeScale=Time.timeScale;Time.timeScale=0;
                    game.User.LastGetEveryDayGift=0;
                    // Explicit dispatcher observation: no gift prefab or claim success is simulated.
                    var guideUI=new OriginalSceneGuideUI(game,null,null,new PanelObserver(),null);
                    guideUI.DailyGift();
                    Check(shown==0,"WaitUntil does not dispatch synchronously");
                    phase=1;deadline=now+.3;return;
                }
                if(phase==1)
                {
                    Check(shown==0&&game.User.LastGetEveryDayGift==0,"Modal retains the pending callback while paused");
                    if(now<deadline)return;
                    modal=false;phase=2;return;
                }
                if(shown==0)return;
                Check(shown==1&&Time.timeScale==0,"Native WaitUntil completes without scaled time");
                long stamp=game.User.LastGetEveryDayGift;
                Check(Math.Abs(OriginalPlayerGoldHintSchedule.UtcSeconds()-stamp)<5,"Completion uses current UTC seconds");
                var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                Check(saved.LastGetEveryDayGift==stamp&&!string.IsNullOrEmpty(saved.LevelInfo)&&saved.LevelInfo==game.User.LevelInfo,"Scene callback saves timestamp and actual board envelope");
                game.ShowEveryDayGift(()=>throw new Exception("Same-day predicate must not run"),id=>throw new Exception("Same-day gift must not show"));
                Debug.Log("NUT_EVERY_DAY_GIFT_PLAY_PASS guide UI routes to actual scene WaitUntil, which blocks on modal and runs while timeScale is zero, dispatch-before-stamp and real user/board persistence; panel dispatcher observed without simulated gift UI or grant.");
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code)
        {
            if(phase>0)Time.timeScale=previousTimeScale;
            OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);
        }
    }
}
