using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalFixedScrewPlayValidation
    {
        private const string Key="NutSort.FixedScrewPlay";
        private static double timeout;
        private static int phase,done;
        private static float started;
        private static OriginalGameScene game;
        private static OriginalNativeWorldEffect effect;
        static OriginalFixedScrewPlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+80;EditorApplication.update+=Tick;}}
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Core fixed screw break");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static ScrewData Rod(int[] colors,bool fixedRod=false)
        {
            var cells=new CData[4];for(int i=0;i<4;i++)cells[i]=new CData{LP=new LPData{y=i},BIM=i<colors.Length?new BIMData{Id=1,CI=colors[i]}:null};
            return new ScrewData{Id=1,C=cells,OBIM=fixedRod?new[]{new OBIMData{Id=6}}:Array.Empty<OBIMData>()};
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Fixed screw Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    // Mechanism fixture only: actual original world prefab and
                    // manager operation, no production level edits or rewards.
                    game.Level.Bind(new LevelData{B=new[]{Rod(new[]{11}),Rod(new[]{11,11,11},true),Rod(new[]{1,2,1,2}),Rod(Array.Empty<int>())}},
                        UnityEngine.Object.FindObjectOfType<OriginalPrefabPool>(),false,true);game.ResizeCameras(480,1040);phase=1;return;
                }
                if(!game.Level.AreNutsInitialized)return;
                if(phase==1)
                {
                    effect=game.Level.GetScrew(1).GetComponentInChildren<OriginalNativeWorldEffect>(true);
                    // Both mask and fixed components exist; the configured fixed
                    // player is the one already instantiated by Configure.
                    foreach(var candidate in game.Level.GetScrew(1).GetComponentsInChildren<OriginalNativeWorldEffect>(true))
                        if(candidate.Player!=null)effect=candidate;
                    Check(effect!=null&&effect.Player!=null&&effect.Player.IsPlaying("animation"),"Fixed idle plays from real Configure");
                    Check(effect.Player.GetComponentsInChildren<SpriteRenderer>().Length==11,"All fixed source slots instantiated");
                    game.BindMoveCompletion((s,success)=>{Check(!success,"Fixture is not whole-board victory");done++;},()=>{},()=>{},null,id=>throw new Exception("Fixture has empty playable rod"));
                    Check(game.Level.Operate(0).Kind==ScrewOperationKind.Ready,"Real source selection");Time.timeScale=.1f;started=Time.time;
                    Check(game.Level.Operate(1).Kind==ScrewOperationKind.Moved,"Real transfer completes fixed destination");phase=2;return;
                }
                if(phase==2)
                {
                    if(Time.time-started<.06f)return;
                    var old=effect.Player["animation"];var next=effect.Player["animation2"];
                    Check(old.enabled&&next.enabled&&next.weight>0&&next.weight<1,"Both native tracks participate during source 0.2-second mix");
                    var source=OriginalFixedScrewBuilder.ReadSource();var keys=source["animations"]["animation"]["bones"]["bone3"]["translate"];
                    float oldY=OriginalFixedScrewValidation.Evaluate(keys,old.time,k=>((float?)k["y"]??0)*.01f,0);
                    float expected=oldY*old.weight/(old.weight+next.weight);
                    float actual=effect.Player.transform.Find("root/bone3").localPosition.y;
                    Check(Mathf.Abs(actual-expected)<.005f,"Mixed transform follows old animation and new setup-pose weights");
                    Time.timeScale=1;phase=3;return;
                }
                if(phase==3)
                {
                    if(Time.time-started<.5f)return;
                    Check(effect.Player.IsPlaying("animation2")&&!effect.Player["animation"].enabled,"Native break replaces idle after mix");
                    Check(game.Level.GetScrew(1).TypeView.IsDontMoveVisible,"Original fixed cover retained during break");
                    ScreenCapture.CaptureScreenshot("Library/core-fixed-screw-current.png");phase=4;return;
                }
                if(Time.time-started<1.5f)return;
                Check(!game.Level.GetScrew(1).TypeView.IsDontMoveVisible&&done==1,"Original cover hides after 1.34 seconds, independent done event runs once");
                Debug.Log("NUT_FIXED_SCREW_PLAY_PASS actual fixed idle, world nut transfer and completion-triggered break, two native tracks and source-weighted pose during 0.2-second mix, idle removal, current screenshot and 1.34-second cover hide; explicit board fixture, default startup remains pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code)
        {Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
