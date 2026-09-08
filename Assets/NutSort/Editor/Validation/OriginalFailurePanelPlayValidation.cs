using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalFailurePanelPlayValidation
    {
        private const string Key="NutSort.FailurePanelPlay";
        private static OriginalGameScene game;
        private static OriginalFailurePanelView panel;
        private static NutSort.Content.OriginalUserLocalData user;
        private static int phase,closed;
        private static double deadline,timeout;
        static OriginalFailurePanelPlayValidation() {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Failure panel validation");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Failure panel play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var old=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/FailPanel"),old.transform.parent,false).GetComponent<OriginalFailurePanelView>();
                    user=OriginalMainTopViewValidation.MakeUser();user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_zs_show\":\"1,250\"}");user.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":12}");user.CurrentLevelAddScrewCount=0;
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                    panel.Bind(user,game.Tables,"en",()=>true,s=>audio.PlaySound(s),game.RestartAfterFailure,()=>game.IsFail=false,()=>{},()=>{closed++;panel.gameObject.SetActive(false);});
                    game.ModalInputBlocked=true;game.IsFail=true;panel.Init();panel.Refresh();phase=1;deadline=now+.6;return;
                }
                if(now<deadline)return;
                if(phase==1){Capture("failure-panel-play");phase=2;deadline=now+.3;return;}
                if(phase==2){user.CurrentLevelAddScrewCount=12;panel.Refresh();phase=3;deadline=now+.3;return;}
                if(phase==3){Capture("failure-panel-gray-play");phase=4;deadline=now+.3;return;}
                Check(File.Exists("Library/ValidationCaptures/failure-panel-play.png") && File.Exists("Library/ValidationCaptures/failure-panel-gray-play.png"),"Captures written");
                panel.Restart.onClick.Invoke();Check(closed==1 && !game.IsFail && game.IsRestarting,"Actual restart button invokes scene reset before close");
                Debug.Log("NUT_FAILURE_PANEL_PLAY_VALIDATION_PASS latest full failure layout and gray state rendered in actual scene, with actual restart Button/scene reset; fixture reward data only.");Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void Capture(string name){string p=Path.GetFullPath("Library/ValidationCaptures/"+name+".png");Directory.CreateDirectory(Path.GetDirectoryName(p));ScreenCapture.CaptureScreenshot(p);}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
