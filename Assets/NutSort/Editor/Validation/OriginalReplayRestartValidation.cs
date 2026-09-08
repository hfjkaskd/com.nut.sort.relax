using System;
using NutSort.Content;
using NutSort.World;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalReplayRestartValidation
    {
        public static void Validate()
        {
            bool existed=PlayerPrefs.HasKey(OriginalUserStore.Key);
            string previous=PlayerPrefs.GetString(OriginalUserStore.Key,string.Empty);
            try
            {
                var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
                var initial=new OriginalUserLocalData(defaults) {Level=3,LevelSeed=0,IsAudio=false,ScrewMoveCount=41};
                string stored=OriginalUserDataJson.Write(initial);
                PlayerPrefs.SetString(OriginalUserStore.Key,stored);PlayerPrefs.Save();
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                OriginalGameScene game=null;
                foreach(var root in scene.GetRootGameObjects())
                    if(root.TryGetComponent(out OriginalGameScene found)){game=found;break;}
                Check(game!=null,"Actual scene game component");
                game.gameObject.SetActive(true);game.Initialize();game.AdvanceInitialization(.301f);
                game.Level.AdvanceInitialization(1.501f);game.AdvanceInitialization(0f);
                var user=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;
                Check(!game.IsRestarting,"Initial board ready");
                Check(!game.IsInitDone && !game.IsSucceed,"Board readiness does not manufacture initialization completion or success");
                user.PassLevelTime=2;
                game.AdvanceElapsedTime(.5f);
                Check(user.PassLevelTime==2,"Uninitialized clock remains unchanged");
                game.IsInitDone=true;game.IsSucceed=true;game.IsFail=true;
                game.ModalInputBlocked=true;game.InputBlocked=true;
                game.AdvanceElapsedTime(.5f);game.AdvanceElapsedTime(0f);
                Check(user.PassLevelTime==2.5f,"Initialized scaled clock ignores success, failure, modal and input flags");
                game.ModalInputBlocked=false;game.InputBlocked=false;
                var oldBoard=game.Level.Board;
                int seed=user.LevelSeed,levelId=user.LevelId;
                user.PassLevelTime=13;user.CurrentLevelAddScrewCount=2;user.LuckyScrewDoneCount=4;game.IsFail=true;
                game.RestartLevel();
                Check(game.IsRestarting && game.Level.Board==null,"Replay immediately clears board");
                Check(!game.IsInitDone && !game.IsSucceed,"Restart clears native initialization and success flags");
                game.AdvanceElapsedTime(.5f);
                Check(user.PassLevelTime==0,"Restarted clock stays reset before initialization event completes");
                Check(user.PassLevelTime==0 && user.CurrentLevelAddScrewCount==0 && user.LuckyScrewDoneCount==0,
                    "Replay applies reset initialization side effects");
                Check(game.IsFail && user.LevelSeed==seed && user.Level==3 && user.ScrewMoveCount==41,
                    "Replay preserves failure flag, seed, player level and cumulative moves");
                game.AdvanceInitialization(.2f);
                user.PassLevelTime=7;user.CurrentLevelAddScrewCount=1;user.LuckyScrewDoneCount=2;
                game.IsInitDone=true;game.IsSucceed=true;
                game.RestartLevel();
                Check(!game.IsInitDone && !game.IsSucceed,"Each pending restart clears both lifecycle flags again");
                Check(user.PassLevelTime==0 && user.CurrentLevelAddScrewCount==0 && user.LuckyScrewDoneCount==0,
                    "Repeated direct restart executes reset while reconstruction is pending");
                game.AdvanceInitialization(.11f);Check(game.Level.Board!=null,"Earlier callback keeps its own deadline after second call");
                var firstReplayBoard=game.Level.Board;
                game.AdvanceInitialization(.191f);
                Check(game.Level.Board!=null && !ReferenceEquals(game.Level.Board,oldBoard) && !ReferenceEquals(game.Level.Board,firstReplayBoard),"Both delayed callbacks rebuild independently");
                Check(user.LevelSeed==seed && user.LevelId==levelId,"Reconstruction retains original seed and selected level");
                game.Level.AdvanceInitialization(.49f);Check(!game.Level.AreNutsInitialized,"Replay uses short 0.5-second entry");
                game.Level.AdvanceInitialization(.011f);game.AdvanceInitialization(0f);
                Check(!game.IsRestarting && game.IsFail,"Board readiness does not clear source failure flag");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==stored,"No replay or reconstruction save point added");
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                if(existed)PlayerPrefs.SetString(OriginalUserStore.Key,previous);else PlayerPrefs.DeleteKey(OriginalUserStore.Key);
                PlayerPrefs.Save();
            }
            Debug.Log("NUT_REPLAY_RESTART_VALIDATION_PASS actual scene repeated reset/rebuild delay, unchanged seed/stage/failure/cumulative moves, short entry, native initialization/success reset, initialization-gated elapsed time and no early save.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
