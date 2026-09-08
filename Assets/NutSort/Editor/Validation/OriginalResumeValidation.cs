using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalResumeValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            string previous=PlayerPrefs.GetString(OriginalUserStore.Key,string.Empty);
            bool existed=PlayerPrefs.HasKey(OriginalUserStore.Key);
            try
            {
                var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
                var layout=Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
                var board=new OriginalBoardState(repository.LoadBoard(false,"4b56d_1_1-1"),layout);
                board.Screws[1].Slots[1].Nut=board.Screws[0].Slots[2].Nut;board.Screws[0].Slots[2].Nut=null;
                board.Screws[0].Coordinate=new Vector2Int(0,1);board.Screws[1].Coordinate=new Vector2Int(0,0);
                string snapshot=OriginalBoardSnapshotJson.Write(OriginalBoardSnapshot.Capture(board,new List<OriginalMoveRecord>
                    {new OriginalMoveRecord{FromScrewIndex=0,ToScrewIndex=1,NutCount=1}}));
                var user=new OriginalUserLocalData(defaults)
                {Level=3,LevelId=123,LevelSeed=0,LevelInfo=snapshot,IsAudio=false,PassLevelTime=12.5f,CurrentLevelAddScrewCount=2,LuckyScrewDoneCount=9,ScrewMoveCount=42};
                string stored=OriginalUserDataJson.Write(user);
                var game=Open(stored);
                Check(game.PlayerLevel==3 && game.ShowLevel==3,"Saved player level drives main scene stage");
                Check(game.IsRestarting && game.Level.Board==null,"Startup waits for original reconstruction callback");
                game.AdvanceInitialization(.29f);Check(game.Level.Board==null,"No board before 0.3-second delay");
                game.AdvanceInitialization(.011f);
                Check(game.Level.Board.LevelId==null && game.Level.Board.Screws.Length==2,"Resume uses saved board without reselecting level or appending lock");
                Check(game.Level.Board.Screws[0].Slots[2].Nut==null && game.Level.Board.Screws[1].Slots[1].Nut!=null,"Resume keeps saved occupancy");
                Check(game.Level.GetScrew(0).transform.position.x>game.Level.GetScrew(1).transform.position.x,"Saved reversed coordinates drive actual transforms");
                var state=CurrentUser();
                Check(state.LevelId==123 && state.PassLevelTime==12.5f && state.CurrentLevelAddScrewCount==2 && state.LuckyScrewDoneCount==0,
                    "Resume leaves level selection/time/tool counts intact and resets lucky count only");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==stored,"Startup does not overwrite the saved envelope");
                Ready(game);
                Check(game.Level.CaptureSnapshot().OperatorInfos.Count==1,"Saved operation history restored");
                game.Level.Operate(1);var moved=game.Level.Operate(0);
                Check(moved.Kind==ScrewOperationKind.Moved,"Resumed board accepts gameplay");
                var written=new OriginalUserStore(defaults,new UnityUserPreferences());
                Check(written.Data.ScrewMoveCount==43 && OriginalBoardSnapshotJson.Read(written.Data.LevelInfo).OperatorInfos.Count==2,
                    "Resumed gameplay persists cumulative attempts and appended history");

                game=Open(OriginalUserDataJson.Write(written.Data));
                game.AdvanceInitialization(.301f);
                Check(game.Level.Board==null && game.IsRestarting,"Completed saved board initializes then immediately schedules fresh reset");
                Check(CurrentUser().PassLevelTime==0 && CurrentUser().CurrentLevelAddScrewCount==0,"Completed-save reset clears original per-level fields");
                game.AdvanceInitialization(.29f);Check(game.Level.Board==null,"Completed save retains second reconstruction delay");
                game.AdvanceInitialization(.011f);
                Check(CurrentUser().Level==3 && CurrentUser().LevelId==6,"Fresh reset selects from saved player progress");
                Check(game.Level.Board.Screws[game.Level.Board.Screws.Length-1].IsLocked,"Fresh level three inserts one locked rod");
                Check(game.Level.CaptureSnapshot().OperatorInfos.Count==0,"Completed-save rebuild clears old move history");

                user.LevelInfo="{\"ScrewInfos\":[],\"OperatorInfos\":[]}";
                game=Open(OriginalUserDataJson.Write(user));game.AdvanceInitialization(.301f);
                Check(game.Level.Board==null,"Empty board takes reset branch");game.AdvanceInitialization(.301f);
                Check(game.Level.Board.LevelId!=null && CurrentUser().LevelId==6,"Empty board rebuilds original selected content");

                user.Level=2;user.LevelSeed=0;user.LevelInfo=string.Empty;
                game=Open(OriginalUserDataJson.Write(user));game.AdvanceInitialization(.301f);
                Check(game.PlayerLevel==2 && CurrentUser().LevelId==5,"New board uses persisted level and original offset");
                Check(!game.Level.Board.Screws[game.Level.Board.Screws.Length-1].IsLocked,"Level two does not append lock");

                user.Level=1;user.LevelSeed=int.MaxValue;user.IsRandomLevelSeed=false;
                game=Open(OriginalUserDataJson.Write(user));game.AdvanceInitialization(.301f);
                Check(CurrentUser().IsRandomLevelSeed && CurrentUser().LevelSeed==int.MaxValue && CurrentUser().LevelId==1,
                    "Seed overflow updates shared random flag without rewriting persisted seed");

                foreach(string invalid in new[]{"{broken","null"," "})
                {
                    user.LevelInfo=invalid;stored=OriginalUserDataJson.Write(user);game=Open(stored);
                    bool failed=false;
                    try{game.AdvanceInitialization(.301f);}catch(JsonException){failed=true;}catch(InvalidOperationException){failed=true;}
                    Check(failed && game.Level.Board==null && PlayerPrefs.GetString(OriginalUserStore.Key)==stored,
                        "Invalid nonempty saved board fails without erasing original preferences");
                }
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                if(existed)PlayerPrefs.SetString(OriginalUserStore.Key,previous);else PlayerPrefs.DeleteKey(OriginalUserStore.Key);
                PlayerPrefs.Save();
            }
            Debug.Log("NUT_RESUME_VALIDATION_PASS actual scene saved progress/occupancy/layout/history, delayed resume, completed/empty reset, original lock threshold, seed overflow propagation, operation autosave and invalid-save preservation; test preferences restored.");
        }

        private static OriginalGameScene Open(string json)
        {
            PlayerPrefs.SetString(OriginalUserStore.Key,json);PlayerPrefs.Save();
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            foreach(var root in scene.GetRootGameObjects())
                if(root.TryGetComponent(out OriginalGameScene game))
                {game.gameObject.SetActive(true);game.Initialize();return game;}
            throw new InvalidDataException("Original game scene missing");
        }
        private static OriginalUserLocalData CurrentUser()=>UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;
        private static void Ready(OriginalGameScene game)
        {
            game.Level.AdvanceInitialization(1.501f);game.AdvanceInitialization(0f);
            Check(!game.IsRestarting && game.Level.AreNutsInitialized,"Initialized nuts release scene input gate");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidDataException(message);}
    }
}
