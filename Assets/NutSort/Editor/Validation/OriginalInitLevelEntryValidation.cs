using System;
using System.Collections.Generic;
using NutSort.Gameplay;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalInitLevelEntryValidation
    {
        private const string Key="NutSort.InitLevelEntryPlay";
        private static double timeout;
        private static int phase;
        private static readonly List<string> flags=new List<string>();
        private static OriginalGameScene game;
        static OriginalInitLevelEntryValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"InitLevel entry timeout");
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(startup==null || startup.MainLevel==null || game==null || game.IsRestarting || game.InputBlocked)return;
                    // Explicit fixture selects the existing nonempty-data continuation branch.
                    game.User.ComeOnGold="fixture";
                    game.BindInitialization(()=>startup.MainLevel,new OriginalInitializationContinuation(game.User,
                        ()=>throw new Exception("Unexpected reward query"),done=>throw new Exception("Unexpected synchronization"),
                        (banner,first)=>flags.Add((banner?"1":"0")+(first?"1":"0"))));
                    foreach(bool banner in new[]{false,true})foreach(bool first in new[]{false,true})
                    {
                        game.User.PassLevelTime=23;game.IsInitDone=true;game.IsSucceed=true;
                        game.InitLevel(true,banner,first);
                        Check(!game.IsInitDone && !game.IsSucceed && game.IsRestarting && game.User.PassLevelTime==0,"Reset entry clears state immediately");
                    }
                    Check(flags.Count==0,"All continuations wait for main-panel delay");phase=1;return;
                }
                if(flags.Count<4)return;
                Check(string.Join(",",flags)=="00,01,10,11","Concurrent scene coroutines retain each invocation's banner/first flags");
                game.User.GuideIndex=19;game.SaveUserData();
                var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                Check(saved.GuideIndex==19 && !string.IsNullOrEmpty(saved.LevelInfo) && saved.LevelInfo==game.User.LevelInfo,"Scene guide save persists live user and actual board snapshot");
                game.User.PassLevelTime=31;game.InitLevel(false,false,false);
                Check(game.User.PassLevelTime==31 && !game.IsInitDone && game.IsRestarting,"Nonreset entry preserves elapsed time before deferred reconstruction");
                Debug.Log("NUT_INIT_LEVEL_ENTRY_PLAY_PASS actual scene InitLevel entry, four independent captured flag pairs, delayed continuation and immediate reset/nonreset behavior plus actual user/board save; SDK boundary unchanged.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
