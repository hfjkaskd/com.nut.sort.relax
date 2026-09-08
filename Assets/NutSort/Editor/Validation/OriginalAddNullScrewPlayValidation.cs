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
    public static class OriginalAddNullScrewPlayValidation
    {
        private const string Key="NutSort.AddNullScrewPlay";
        private static double timeout,deadline;
        private static int phase;
        private static OriginalGameScene game;
        private static OriginalGameplayEffects effects;
        static OriginalAddNullScrewPlayValidation()
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
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Add null screw Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    game.User.Level=1;game.User.LevelSeed=0;game.User.AddScrewCount=2;
                    game.InitLevel(true,false,false);phase=1;return;
                }
                if(phase==1)
                {
                    var level=game.Level;int count=level.Board.Screws.Length;var first=level.GetScrew(0);
                    var nut=first.State.Slots[0].Nut;int nutCount=level.NutViewCount;
                    effects=game.GetComponent<OriginalGameplayEffects>();int refreshed=0;
                    var items=new OriginalItemManager(game.User,game.SaveUserData,()=>refreshed++,(a,b,c)=>throw new Exception("Unexpected gold"),(a,b)=>throw new Exception("Unexpected coin"));
                    var flow=new OriginalAddScrewFlow(game.User,()=>12,()=>false,()=>level.Unlock(()=>false),()=>throw new Exception("Unexpected tile branch"),
                        ()=>game.AddNullScrew(()=>false,()=>{Check(game.User.AddScrewCount==2,"World refresh precedes inventory consumption");refreshed++;}),items.AddTool,(p,t)=>throw new Exception("Unexpected acquire"),id=>throw new Exception("Unexpected tip"));
                    flow.Run();
                    var added=level.GetScrew(count);
                    Check(level.Board.Screws.Length==count+1&&added.State.Capacity==4&&added.TileCount==4,"Actual new four-segment rod");
                    Check(level.GetScrew(0)==first&&first.State.Slots[0].Nut==nut&&level.NutViewCount==nutCount,"Existing scene nuts and rods retained");
                    Check(game.User.AddScrewCount==1&&game.User.CurrentLevelAddScrewCount==4&&refreshed==2,"World refresh then real item consumption refresh");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.AddScrewCount==1&&saved.CurrentLevelAddScrewCount==4&&saved.LevelInfo==OriginalBoardSnapshotJson.Write(level.CaptureSnapshot()),"Real user and appended board save");
                    var systems=added.GetComponentsInChildren<ParticleSystem>();int playing=0;
                    foreach(var ps in systems)if(ps.isPlaying)playing++;
                    Check(effects.ActiveCount==1&&playing==3,"Original effect plays under new rod");
                    phase=2;deadline=Time.time+3.4;return;
                }
                if(Time.time<deadline)return;
                Check(effects.ActiveCount==0,"Unlock effect released after source lifetime");
                Debug.Log("NUT_ADD_NULL_SCREW_PLAY_PASS actual scene add-tool branch, four-segment appended rod, retained old nuts, three playing particles and timed cleanup, ordered bottom refreshes, inventory consumption and real board/user save; explicit configuration fixture, default startup binding pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
