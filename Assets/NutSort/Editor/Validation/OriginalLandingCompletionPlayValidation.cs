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
    public static class OriginalLandingCompletionPlayValidation
    {
        private const string Key="NutSort.LandingCompletionPlay";
        private static double timeout;
        private static float movedAt;
        private static int phase,scenario,events;
        private static OriginalGameScene game;
        private static NutMoveBatch batch;
        static OriginalLandingCompletionPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+80;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static ScrewData Rod(params int[] colors)
        {var cells=new CData[4];for(int i=0;i<4;i++)cells[i]=new CData{LP=new LPData{y=i},BIM=i<colors.Length?new BIMData{Id=1,CI=colors[i]}:null};return new ScrewData{Id=1,C=cells,OBIM=Array.Empty<OBIMData>()};}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Landing completion timeout");if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    game.IsSkipLevel=false;game.User.Level=1;game.User.LevelSeed=0;game.User.ScrewDoneCount=0;events=0;
                    var data=scenario==3?new LevelData{B=new[]{Rod(1),Rod(1,1,1),Rod(1,2),Rod()}}:
                        new LevelData{B=new[]{Rod(1,2,2),Rod(),Rod(2,1)}};
                    game.Level.Bind(data,UnityEngine.Object.FindObjectOfType<OriginalPrefabPool>(),false,true);
                    game.BindMoveCompletion((target,success)=>
                    {
                        Check(success,"Skip path reports captured true");Check(target.IsCanOperator,"Operation gate is reopened before completion event");
                        Check(target==batch.Destination,"Original target survives callback");events++;
                    },()=>throw new Exception("Early seed hide"),()=>{},null,id=>throw new Exception("Unexpected failure"));
                    phase=1;return;
                }
                if(!game.Level.AreNutsInitialized)return;
                if(phase==3)
                {
                    Check(game.IsSkipLevel&&game.Level.Board.Screws.Length==2,"Saved unsolved board uses manager skip success and rebuilds original first board");
                    Debug.Log("NUT_LANDING_COMPLETION_PLAY_PASS real native transfers: normal incomplete landing, skip disabled/enabled during flight, operation gate before immediate DoneEvent, no completion counters on incomplete rod, repeated landing guard, full rod captured skip success through original two-second callback with no duplicate landing event; skipped saved board rebuilds without resetting manager flag.");Finish(0);return;
                }
                if(phase==1)
                {
                    Check(!game.Level.Board.IsSuccess&&!game.IsSuccess(),"Fixture starts unsolved");
                    game.IsSkipLevel=scenario==1||scenario==3;
                    Check(game.Level.Operate(0).Kind==ScrewOperationKind.Ready,"Actual selection");var moved=game.Level.Operate(1);Check(moved.Kind==ScrewOperationKind.Moved,"Actual transfer");batch=moved.Move;
                    Check(batch.Transfers.Length==(scenario==3?1:2),"Staggered multi-nut landing fixture");
                    Check(!batch.Destination.IsCanOperator&&events==0,"No early release or completion");
                    if(scenario==1)game.IsSkipLevel=false;
                    if(scenario==2)game.IsSkipLevel=true;
                    if(scenario==3)game.IsSkipLevel=false;
                    movedAt=Time.time;phase=2;return;
                }
                if(scenario==3)
                {
                    if(Time.time-movedAt<2.4f)return;
                    Check(events==1&&game.User.ScrewDoneCount==1,"Full destination uses only captured two-second completion, not landing skip");
                    Check(!game.IsSuccess()&&!game.Level.Board.IsSuccess,"Cleared skip flag exposes still-unsolved board");
                }
                else
                {
                    if(!batch.Destination.IsCanOperator||Time.time-movedAt<1.2f)return;
                    Check(events==(scenario==2?1:0),"Landing reads current skip flag, not transfer-time snapshot");
                    Check(game.User.ScrewDoneCount==0,"Incomplete landing does not increment full-screw counters");
                    Check(!batch.RequiresDoneAnimation&&!batch.Destination.IsDone,"Incomplete target does not run cap animation");
                    Check(!batch.TryCompleteMovement(batch.Transfers.Length-1),"Repeated completion cannot restart cap or dispatch another event");
                    if(scenario==2)Check(game.IsSuccess()&&!game.Level.Board.IsSuccess,"Manager skip success short-circuits physical board result");
                }
                scenario++;phase=0;if(scenario<4)return;
                game.SaveUserData();game.IsSkipLevel=true;game.InitLevel(false,false,false);phase=3;
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){if(game!=null)game.IsSkipLevel=false;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
