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
    public static class OriginalScrewTypeRefreshPlayValidation
    {
        private const string Key="NutSort.ScrewTypeRefreshPlay";
        private static double timeout,paused;
        private static float movedAt;
        private static int phase,done;
        private static OriginalGameScene game;
        static OriginalScrewTypeRefreshPlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+80;EditorApplication.update+=Tick;}}
        public static void Run()
        {OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static ScrewData Rod(int[] colors,params OBIMData[] masks)
        {
            var cells=new CData[4];
            for(int i=0;i<4;i++)cells[i]=new CData{LP=new LPData{y=i},BIM=i<colors.Length?new BIMData{Id=1,CI=colors[i]}:null};
            return new ScrewData{Id=1,C=cells,OBIM=masks};
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Mask core Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    // Explicit mechanics fixture, rendered through the actual world
                    // prefabs. No changes to production configuration or save files.
                    game.Level.Bind(new LevelData{B=new[]{
                        Rod(new[]{11}),Rod(new[]{11,11,11}),
                        Rod(new[]{1,2,1,2},new OBIMData{Id=4,Obj=new OBIMObjData{CI=11}}),
                        Rod(Array.Empty<int>()),Rod(new[]{1,2,1,2},new OBIMData{Id=7}),
                        Rod(new[]{1,2,1,2},new OBIMData{Id=7})}},
                        UnityEngine.Object.FindObjectOfType<OriginalPrefabPool>(),false,true);
                    game.ResizeCameras(480,1040);phase=1;return;
                }
                if(!game.Level.AreNutsInitialized)return;
                if(phase==1)
                {
                    game.BindMoveCompletion((s,success)=>{Check(!success,"Fixture completion is not whole-board victory");done++;},
                        ()=>{},()=>{},null,id=>throw new Exception("Remaining empty rod prevents failure"));
                    Check(game.Level.Operate(0).Kind==ScrewOperationKind.Ready,"Actual selection");
                    movedAt=Time.time;
                    Check(game.Level.Operate(1).Kind==ScrewOperationKind.Moved,"Actual completed rod transfer");
                    Check(!game.Level.Board.Screws[2].IsColorMask&&!game.Level.Board.Screws[4].IsHidden&&game.Level.Board.Screws[5].IsHidden,
                        "Scene binding automatically applies mask and adjacency rules without external refresh callback");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    var restored=OriginalBoardSnapshotJson.Read(saved.LevelInfo).Restore(Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout"));
                    Check(!restored.Screws[2].IsColorMask&&!restored.Screws[4].IsHidden&&restored.Screws[5].IsHidden,"Actual save retains revealed and unrevealed masks");
                    phase=2;return;
                }
                if(phase==2)
                {
                    if(Time.time-movedAt<1.1f)return;
                    Check(done==1&&game.Level.GetScrew(2).TypeView.IsMaskBreakVisible,"Delayed completion and mask break callback run independently");
                    Check(game.Level.GetScrew(2).TypeView.IsMaskVisible,"Mask still visible before total 1.7-second timeline");
                    Check(!game.Level.GetScrew(4).TypeView.IsHiddenVisible&&game.Level.GetScrew(5).TypeView.IsHiddenVisible,"Only adjacent hidden cover breaks");
                    Time.timeScale=0;paused=EditorApplication.timeSinceStartup;phase=3;return;
                }
                if(EditorApplication.timeSinceStartup-paused<.9)return;
                Check(!game.Level.GetScrew(2).TypeView.IsMaskVisible&&done==1,"Unscaled mask hide continues while game clock is paused");
                Debug.Log("NUT_SCREW_TYPE_REFRESH_PLAY_PASS real world transfer, automatic scene mask/adjacency consumer with null observer, actual saved mask states, source scaled mask callback and independent-time cover hide while paused; explicit board fixture, skeletal assets and default startup still pending.");
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code)
        {Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
