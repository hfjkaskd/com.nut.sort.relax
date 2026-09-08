using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using NutSort.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalResumePlayValidation
    {
        private const string Active="NutSort.ResumePlayValidation";
        private static OriginalGameScene game;
        private static int phase;
        private static double timeout,deadline;
        private static string persisted;
        static OriginalResumePlayValidation()
        {
            if(SessionState.GetBool(Active,false))
            {timeout=EditorApplication.timeSinceStartup+70;EditorApplication.update+=Tick;}
        }
        public static void Run()
        {
            try
            {
                var repository=new OriginalLevelRepository(Resources.Load<OriginalContentSettings>("Configuration/OriginalContent"));repository.Initialize();
                var board=new OriginalBoardState(repository.LoadBoard(false,"4b56d_1_1-1"),Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout"));
                var snapshot=OriginalBoardSnapshot.Capture(board,new List<OriginalMoveRecord>());
                var extra=OriginalBoardSnapshotJson.Read(OriginalBoardSnapshotJson.Write(snapshot)).ScrewInfos[1];
                extra.Index=2;extra.Coordinate=new SavedCoordinate{x=0,y=2};
                foreach(var nut in extra.NutInfos)nut.NutData=null;
                snapshot.ScrewInfos.Add(extra);
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"))
                {Level=3,LevelId=123,LevelInfo=OriginalBoardSnapshotJson.Write(snapshot),IsAudio=false,PassLevelTime=12.5f,CurrentLevelAddScrewCount=2,LuckyScrewDoneCount=9,ScrewMoveCount=40};
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
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Actual resume timeout phase="+phase);
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||!game.Level.AreNutsInitialized||game.IsRestarting||game.InputBlocked||now<deadline)return;
                var user=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;
                if(phase==0)
                {
                    Check(game.PlayerLevel==3 && game.Level.Board.Screws.Length==3 && user.LevelId==123,"Actual Start restores saved board instead of selecting a fresh one");
                    Check(user.PassLevelTime==12.5f&&user.CurrentLevelAddScrewCount==2&&user.LuckyScrewDoneCount==0,"Actual resume reset scope");
                    Check(UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel.Group.gameObject.activeSelf,"Saved player progress drives visible main-level badge");
                    Check(Click(1).Kind==ScrewOperationKind.Ready&&Click(2).Kind==ScrewOperationKind.Moved,"Actual camera-ray transfer on resumed board");
                    Check(!game.Level.Board.IsSuccess,"Saved continuation fixture remains unfinished");
                    persisted=PlayerPrefs.GetString(OriginalUserStore.Key);
                    Check(user.ScrewMoveCount==41,"Resumed attempt updates shared count");
                    phase=1;deadline=now+.9;return;
                }
                if(phase==1)
                {
                    Check(persisted==PlayerPrefs.GetString(OriginalUserStore.Key),"Operation saved before scene unload");
                    phase=2;game=null;deadline=now+2.5;
                    SceneManager.LoadScene("LuoSiSortGame",LoadSceneMode.Single);return;
                }
                if(phase==2)
                {
                    Check(user.LevelId==123&&user.ScrewMoveCount==41,"Fresh user session reloads persisted identity/count");
                    Check(game.Level.Board.Screws[1].IsNull&&game.Level.Board.Screws[2].Slots[0].Nut!=null,"Scene reload restores moved slots");
                    Check(game.Level.CaptureSnapshot().OperatorInfos.Count==1,"Scene reload restores prior history");
                    Check(Click(2).Kind==ScrewOperationKind.Ready&&Click(0).Kind==ScrewOperationKind.Moved,"Continue from reloaded position to completion");
                    Check(game.Level.Board.IsSuccess&&user.ScrewMoveCount==42,"Continued original board completes with cumulative attempts");
                    phase=3;deadline=now+.9;return;
                }
                if(phase==3)
                {
                    phase=4;game=null;deadline=now+2.8;
                    SceneManager.LoadScene("LuoSiSortGame",LoadSceneMode.Single);return;
                }
                Check(user.Level==3&&user.LevelId==6&&user.ScrewMoveCount==42,"Reloading completed save reselects saved level with original offset");
                Check(user.PassLevelTime==0&&user.CurrentLevelAddScrewCount==0,"Completed-save reset clears per-level progress");
                Check(game.Level.Board.LevelId!=null&&game.Level.CaptureSnapshot().OperatorInfos.Count==0,"Completed-save reload builds fresh content and history");
                Check(game.Level.Board.Screws[game.Level.Board.Screws.Length-1].IsLocked,"Fresh rebuilt level retains original extra lock");
                Debug.Log("NUT_RESUME_PLAY_VALIDATION_PASS actual Start, saved-level UI, camera-ray operation autosave, scene reload with persisted slots/history/counters, continued completion and completed-save reload to fresh level-three locked board; original preferences restored on exit.");
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static ScrewOperation Click(int index)
        {
            Physics.SyncTransforms();
            return game.TryOperateAtScreenPoint(game.WorldCamera.WorldToScreenPoint(game.Level.GetScrew(index).Bounds.bounds.center));
        }
        private static void Finish(int code)
        {
            OriginalPreferenceFixture.Restore();SessionState.SetBool(Active,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);
        }
        private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    }
}
