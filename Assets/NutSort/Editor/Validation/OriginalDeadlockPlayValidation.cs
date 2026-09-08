using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalDeadlockPlayValidation
    {
        private const string Active="NutSort.DeadlockPlayValidation";
        private static OriginalGameScene game;
        private static double timeout, started;
        private static int phase;
        private static string savedJson;
        static OriginalDeadlockPlayValidation()
        {
            if(SessionState.GetBool(Active,false))
            { timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick; }
        }
        public static void Run()
        {
            try
            {
                var snapshot=new OriginalBoardSnapshot { ScrewInfos=new List<SavedScrew>
                {OriginalDeadlockValidation.S(1,1,2,-1,-1),OriginalDeadlockValidation.S(7,3,4,-1,-1)} };
                var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
                var user=new OriginalUserLocalData(defaults) {Level=7,IsAudio=false,ScrewMoveCount=42,
                    LevelInfo=OriginalBoardSnapshotJson.Write(snapshot)};
                OriginalPreferenceFixture.Begin(OriginalUserDataJson.Write(user));
                EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                SessionState.SetBool(Active,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;
                Check(now<timeout,"Deadlock Play validation timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                var type=game.Level.GetScrew(1).TypeView;
                if(phase==0)
                {
                    Check(type.IsHiddenVisible,"Actual startup retains hidden mask before lifecycle check");
                    Time.timeScale=0;
                    Check(game.Level.IsCannotMove(),"Paused scene performs stateful deadlock check");
                    Check(type.IsHiddenVisible && !game.Level.Board.Screws[1].IsHidden,"State reveals before native visual is hidden");
                    savedJson=PlayerPrefs.GetString(OriginalUserStore.Key);
                    var saved=OriginalUserDataJson.Read(savedJson,Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.ScrewMoveCount==42 && !OriginalBoardSnapshotJson.Read(saved.LevelInfo).ScrewInfos[1].ScrewMaskDatas[0].IsShow,
                        "Real PlayerPrefs saved hidden state while paused without a move attempt");
                    phase=1;started=now;return;
                }
                if(now-started<.4)return;
                if(phase==1)
                {
                    Check(type.IsHiddenVisible,"Hidden root remains before one second while timeScale zero");
                    phase=2;return;
                }
                if(now-started<1.3)return;
                Check(Time.timeScale==0 && !type.IsHiddenVisible,"Actual Update closes hidden root using unscaled time");
                game.Level.IsCannotMove();Check(PlayerPrefs.GetString(OriginalUserStore.Key)==savedJson,"Repeated check keeps saved envelope unchanged");
                Debug.Log("NUT_DEADLOCK_PLAY_VALIDATION_PASS real startup, hidden state/save before visual hide, unscaled Update at timeScale zero, no repeated save; skeletal animation not validated.");
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Finish(int code)
        {
            Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Active,false);
            EditorApplication.update-=Tick;EditorApplication.Exit(code);
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
