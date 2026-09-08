using System;
using System.IO;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalNewbieGuideHostValidation
    {
        private const string Key="NutSort.GuideHostPlay",PathName="prefabs/panels/NewbieGuidePanel";
        private static double timeout,deadline;
        private static int phase,scheduled;
        private static OriginalNewbieGuideHost host;
        private static OriginalGameScene game;
        private static bool canOperate;
        static OriginalNewbieGuideHostValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Validate()
        {
            var parent=new GameObject("Guide host fixture");var previous=OriginalNewbieGuideView.CallbackAction;
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){Level=1,LevelSeed=0,GuideIndex=4};
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                bool operate=false;int schedules=0;
                var local=new OriginalNewbieGuideHost(parent.transform,PathName,user,tables,"en",()=>true,()=>{},v=>operate=v,()=>false,
                    (a,b)=>schedules++,new OriginalSceneGuideUI(null,()=>null,null,null,null));
                var view=local.Show(false);Check(local.IsOpen && view==local.Panel && operate && !view.ShowBanner,"Registered guide initializes then teaches");
                Check(view.Tip.text==tables.Text.GetText(70,"en") && view.transform.Find("main")==null,"Actual teaching and native absence of panel tween target");
                user.Level=2;user.GuideIndex=0;local.Refresh();Check(!operate && local.IsOpen,"Refresh preserves cached no-op index four");
                Action<object> callback=v=>{};OriginalNewbieGuideView.CallbackAction=callback;
                Check(schedules==0 && OriginalNewbieGuideView.CallbackAction==callback,"Teaching refresh does not schedule or clear global callback");
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;UnityEngine.Object.DestroyImmediate(parent);}
            Debug.Log("NUT_NEWBIE_GUIDE_HOST_VALIDATION_PASS register/init/refresh, real teaching, cached index; actual close covered by Play fixture; production startup binding pending.");
        }
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Guide host validation");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Guide host timeout");
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(startup==null || startup.MainLevel==null || game==null || game.IsRestarting || game.InputBlocked)return;
                    game.User.Level=1;game.User.LevelSeed=0;
                    host=new OriginalNewbieGuideHost(startup.MainLevel.transform.parent,PathName,game.User,game.Tables,"en",()=>true,()=>{},v=>canOperate=v,()=>false,
                        (delay,action)=>{scheduled++;game.ScheduleDelay(delay,action);},new OriginalSceneGuideUI(game,()=>null,null,null,null));
                    host.Show();Check(canOperate && host.IsOpen,"Actual host displays teaching");phase=1;deadline=now+.7;return;
                }
                if(now<deadline)return;
                if(phase==1){Check(host.Panel.Tip.text==game.Tables.Text.GetText(70,"en"),"First teaching text before capture");Capture("guide-host-teach-0");phase=2;deadline=now+.3;return;}
                if(phase==2){game.User.LevelSeed=3;host.Refresh();phase=3;deadline=now+.4;return;}
                if(phase==3){Check(host.Panel.Tip.text==game.Tables.Text.GetText(151,"en"),"Refresh displays final teaching text");Capture("guide-host-teach-3");phase=4;deadline=now+.3;return;}
                var previous=OriginalNewbieGuideView.CallbackAction;Action<object> retained=value=>{};
                OriginalNewbieGuideView.CallbackAction=retained;
                host.Panel.Close();Check(!host.IsOpen && scheduled==0 && OriginalNewbieGuideView.CallbackAction==retained,"Real close removes registration without queue or callback clearing");
                OriginalNewbieGuideView.CallbackAction=previous;
                Debug.Log("NUT_NEWBIE_GUIDE_HOST_PLAY_PASS actual prefab host initial/final teaching and close; production startup and operation-gate wiring remain pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Capture(string name){Directory.CreateDirectory("Library/ValidationCaptures");ScreenCapture.CaptureScreenshot(System.IO.Path.GetFullPath("Library/ValidationCaptures/"+name+".png"));}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
