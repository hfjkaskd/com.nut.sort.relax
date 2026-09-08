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
    public static class OriginalMoveCompletionPlayValidation
    {
        private const string Key="NutSort.MoveCompletionPlay";
        private static double timeout;
        private static float started;
        private static int phase,events;
        private static OriginalGameScene game;
        private static OriginalScrewView target;
        static OriginalMoveCompletionPlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+65;EditorApplication.update+=Tick;}}
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
                Check(EditorApplication.timeSinceStartup<timeout,"Move completion Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    game.User.Level=1;game.User.LevelSeed=0;game.InitLevel(true,false,false);phase=1;return;
                }
                if(phase==1)
                {
                    var level=game.Level;target=level.GetScrew(1);
                    game.BindMoveCompletion((s,success)=>{Check(s==target.State&&success,"Captured actual first-board completion");Check(Time.time-started>=1.99f,"Original two-second game-clock delay");events++;},
                        ()=>throw new Exception("Seed zero cannot hide guide"),()=>throw new Exception("Seed zero cannot push"),s=>throw new Exception("Winning transfer cannot refresh types"),id=>throw new Exception("Winning transfer cannot fail"));
                    int count=game.User.ScrewDoneCount,lucky=game.User.LuckyScrewDoneCount,draw=game.User.LuckyDrawScrewDoneTimes;
                    // Direct gameplay operation fixture, without changing production IsInitDone.
                    level.Operate(0);started=Time.time;var moved=level.Operate(1);
                    Check(moved.Kind==ScrewOperationKind.Moved&&level.Board.IsSuccess,"Actual first-board data transfer completes board");
                    Check(game.User.ScrewDoneCount==count+1&&game.User.LuckyScrewDoneCount==lucky+1&&game.User.LuckyDrawScrewDoneTimes==draw+1,"All counters increment immediately before visual landing");
                    Check(!target.State.IsCanOperator&&events==0,"Operation gate and delayed completion remain separate");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.ScrewDoneCount==count+1&&saved.LuckyScrewDoneCount==lucky+1&&saved.LuckyDrawScrewDoneTimes==draw+1,"Existing operation save includes immediate counters");
                    phase=2;return;
                }
                if(Time.time-started<2.15f)return;
                Check(events==1&&target.State.IsCanOperator&&!target.IsDoneAnimating,"One delayed completion with independently completed cap animation");
                Debug.Log("NUT_MOVE_COMPLETION_PLAY_PASS actual first-board transfer, immediate counters before visual landing, real operation save, two-second captured completion and independent cap/operation-gate release; explicit completion consumer, production reward/success panels pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
