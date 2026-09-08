using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalUnlockScrewPlayValidation
    {
        private const string Key="NutSort.UnlockScrewPlay";
        private static double timeout,deadline;
        private static int phase;
        private static OriginalGameScene game;
        private static OriginalGameplayEffects effects;
        static OriginalUnlockScrewPlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Unlock Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    game.User.Level=3;game.User.LevelSeed=0;game.User.AddScrewCount=2;
                    game.InitLevel(true,false,false);phase=1;return;
                }
                if(phase==1)
                {
                    var level=game.Level;var last=level.GetScrew(level.Board.Screws.Length-1);
                    Check(last.State.IsLocked&&last.State.Capacity==0,"Actual level-three reserved screw");
                    effects=game.GetComponent<OriginalGameplayEffects>();int refreshed=0;
                    var items=new OriginalItemManager(game.User,game.SaveUserData,()=>refreshed++,(a,b,c)=>throw new Exception("Unexpected gold"),(a,b)=>throw new Exception("Unexpected coin"));
                    // Explicit mode/limit fixture; real unlock, effects, inventory and board save.
                    var flow=new OriginalAddScrewFlow(game.User,()=>12,()=>true,()=>level.Unlock(()=>true),()=>level.AddTile(()=>true),
                        ()=>throw new Exception("Unlock should avoid new rod"),items.AddTool,(p,t)=>throw new Exception("Unexpected acquire"),id=>throw new Exception("Unexpected tip"));
                    flow.Run();
                    Check(!last.State.IsLocked&&last.State.Capacity==1&&last.TileCount==1&&!last.TypeView.IsShortLockVisible,"Actual lock visuals and first segment refreshed");
                    Check(game.User.AddScrewCount==1&&game.User.CurrentLevelAddScrewCount==1&&refreshed==1,"Real flow consumes once after world unlock");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.AddScrewCount==1&&saved.CurrentLevelAddScrewCount==1&&saved.LevelInfo==OriginalBoardSnapshotJson.Write(level.CaptureSnapshot()),"User and changed board persisted through scene save");
                    var systems=last.GetComponentsInChildren<ParticleSystem>();int playing=0;
                    foreach(var ps in systems)if(ps.isPlaying)playing++;
                    Check(effects.ActiveCount==1&&playing==3,"Actual original three-system effect plays under unlocked rod");
                    phase=2;deadline=now+3.4;return;
                }
                if(now<deadline)return;
                Check(effects.ActiveCount==0,"Unlock effect released after source lifetime");
                Debug.Log("NUT_UNLOCK_SCREW_PLAY_PASS actual level-three unlock, lock visuals, first tile, three playing particle systems, timed cleanup, add-tool consumption and real board/user save; explicit configuration fixture, default startup binding pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
