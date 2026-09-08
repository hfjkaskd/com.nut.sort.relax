using System;
using NutSort.Content;
using NutSort.World;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalFailureRestartValidation
    {
        public static void Validate()
        {
            bool existed=PlayerPrefs.HasKey(OriginalUserStore.Key);
            string previous=PlayerPrefs.GetString(OriginalUserStore.Key,string.Empty);
            try
            {
                var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
                var initial=new OriginalUserLocalData(defaults) {Level=3,LevelSeed=0,IsAudio=false};
                string stored=OriginalUserDataJson.Write(initial);
                PlayerPrefs.SetString(OriginalUserStore.Key,stored);PlayerPrefs.Save();
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                OriginalGameScene game=null;
                foreach(var root in scene.GetRootGameObjects())
                    if(root.TryGetComponent(out OriginalGameScene found)) {game=found;break;}
                Check(game!=null,"Actual scene exists");
                game.gameObject.SetActive(true);game.Initialize();game.AdvanceInitialization(.301f);
                game.Level.AdvanceInitialization(1.501f);game.AdvanceInitialization(0f);
                var user=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;
                Check(!game.IsRestarting,"Fixture board initialized");
                user.PassLevelTime=15;user.CurrentLevelAddScrewCount=2;user.LuckyScrewDoneCount=4;
                game.IsFail=true;int closes=0;
                game.RestartAfterFailure(()=>
                {
                    Check(user.LevelSeed==1 && !game.IsFail && game.IsRestarting && game.Level.Board==null,
                        "Close follows seed increment, failure reset and board clear");
                    Check(user.PassLevelTime==0 && user.CurrentLevelAddScrewCount==0 && user.LuckyScrewDoneCount==0,
                        "Original initialization side effects precede close");
                    closes++;
                });
                Check(closes==1 && PlayerPrefs.GetString(OriginalUserStore.Key)==stored,
                    "Failure restart has no immediate extra save");
                game.AdvanceInitialization(.29f);Check(game.Level.Board==null,"Original rebuild delay");
                game.AdvanceInitialization(.011f);
                Check(game.Level.Board!=null && user.Level==3 && user.LevelSeed==1,"Next seed reconstructs same player level");
                game.Level.AdvanceInitialization(.49f);Check(!game.Level.AreNutsInitialized,"Restart uses short nut entry delay");
                game.Level.AdvanceInitialization(.011f);game.AdvanceInitialization(0f);
                Check(!game.IsRestarting,"New board ready after original delays");
                game.RestartAfterFailure(()=>closes++);
                game.AdvanceInitialization(.2f);
                game.IsFail=true;game.RestartAfterFailure(()=>closes++);
                Check(user.LevelSeed==3 && !game.IsFail && closes==3,"Repeated action while rebuilding is not suppressed");
                game.AdvanceInitialization(.11f);Check(game.Level.Board==null,"Repeated initialization restarts its delay");
                user.LevelSeed=int.MaxValue;bool caught=false;
                try { game.RestartAfterFailure(()=>{throw new InvalidOperationException("fixture close");}); }
                catch(InvalidOperationException) {caught=true;}
                Check(caught && user.LevelSeed==int.MinValue && !game.IsFail && game.IsRestarting,
                    "Unchecked seed overflow and close exception retain earlier mutations");
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                if(existed)PlayerPrefs.SetString(OriginalUserStore.Key,previous);else PlayerPrefs.DeleteKey(OriginalUserStore.Key);
                PlayerPrefs.Save();
            }
            Debug.Log("NUT_FAILURE_RESTART_VALIDATION_PASS actual scene seed/reset/rebuild/close ordering, unchanged player stage, no early save, short entry delay, repeated restart and overflow/exception semantics.");
        }
        private static void Check(bool value,string message) {if(!value)throw new InvalidOperationException(message);}
    }
}
