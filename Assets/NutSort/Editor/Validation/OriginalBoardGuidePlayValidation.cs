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
    public static class OriginalBoardGuidePlayValidation
    {
        private const string Key="NutSort.BoardGuidePlay";
        private static double timeout;
        private static float started;
        private static int phase;
        private static OriginalGameScene game;
        private static GameObject hand,correct;
        static OriginalBoardGuidePlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+80;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,1040,"Core board guide");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static ScrewData Rod(params int[] colors)
        {var cells=new CData[4];for(int i=0;i<4;i++)cells[i]=new CData{LP=new LPData{y=i},BIM=i<colors.Length?new BIMData{Id=1,CI=colors[i]}:null};return new ScrewData{Id=1,C=cells};}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Board guide timeout");if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked||!game.Level.AreNutsInitialized)return;
                if(phase==0)
                {
                    hand=game.Level.GetScrew(0).GuideInstance;
                    Check(hand!=null&&hand.name.StartsWith("GuideHand"),"Default startup automatically creates original hand without manual guide binding");
                    Check(hand.GetComponent<Animation>().isPlaying,"Native yoyo starts from prefab");started=Time.time;phase=1;return;
                }
                if(phase==1)
                {
                    if(Time.time-started<.8f)return;
                    var t=hand.transform.Find("Hand");Check(t.localPosition.x>-.4f&&t.localPosition.x<-.3f&&Mathf.Abs(t.localPosition.y-t.localPosition.x-.8f)<.0001f,"Actual linear diagonal hand motion");
                    ScreenCapture.CaptureScreenshot("Library/core-board-guide-hand-current.png");phase=2;return;
                }
                if(phase==2)
                {
                    Check(game.Level.Operate(0).Kind==ScrewOperationKind.Ready,"Actual world selection");
                    Check(game.Level.GetScrew(0).GuideInstance==null,"Selected source hand cleared");correct=game.Level.GetScrew(1).GuideInstance;
                    Check(correct!=null&&correct.name.StartsWith("GuideCorrect"),"Matching destination marked correct after selection");
                    Check(Mathf.Abs(correct.transform.localPosition.y-game.Level.GetScrew(1).ReadyPosition.localPosition.y-1)<.0001f,"Original ready-position plus one marker height");
                    ScreenCapture.CaptureScreenshot("Library/core-board-guide-selected-current.png");phase=3;return;
                }
                if(phase==3)
                {
                    Check(hand==null,"ClearGuide destroys old hand after the frame");
                    game.User.LevelSeed=3;game.Level.RefreshGuide();Check(game.Level.GetScrew(1).GuideInstance==correct,"Source early return preserves existing marker beyond teaching seeds");
                    game.User.LevelSeed=1;
                    game.Level.Bind(new LevelData{B=new[]{Rod(1,2),Rod(),Rod(3),Rod(2)}},UnityEngine.Object.FindObjectOfType<OriginalPrefabPool>(),false,true);
                    phase=4;return;
                }
                if(phase==4)
                {
                    Check(game.Level.GetScrew(0).GuideInstance!=null,"New teaching board creates hand automatically");
                    Check(game.Level.Operate(0).Kind==ScrewOperationKind.Ready,"Fixture selection");
                    Check(game.Level.GetScrew(1).GuideInstance.name.StartsWith("GuideCorrect")&&game.Level.GetScrew(2).GuideInstance.name.StartsWith("GuideError")&&game.Level.GetScrew(3).GuideInstance.name.StartsWith("GuideCorrect"),"Actual empty/mismatched/matching target visuals");
                    started=Time.time;phase=5;return;
                }
                if(phase==5)
                {
                    if(Time.time-started<.8f)return;
                    ScreenCapture.CaptureScreenshot("Library/core-board-guide-targets-current.png");phase=6;return;
                }
                Debug.Log("NUT_BOARD_GUIDE_PLAY_PASS default scene creates native animated world hand; actual world selection updates correct marker at original height; original marker destroyed, source late-seed gate retains markers, rebuilt teaching fixture and real selection render correct/error targets. Default input initialization remains pending; world Operate is invoked explicitly by validation.");Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
