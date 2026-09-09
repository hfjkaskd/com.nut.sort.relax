using System;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalBootLoadingPlayValidation
    {
        private const string Key="NutSort.BootLoadingPlay";private static double timeout;private static int phase;private static float start;private static OriginalBootLoadingPanel panel;
        static OriginalBootLoadingPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,1040,"Original loading page");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Loading Play timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;Check(parent!=null,"Actual canvas");
                    panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/BootLoadingPanel"),parent,false).GetComponent<OriginalBootLoadingPanel>();panel.SetState(true);start=Time.time;phase=1;return;
                }
                if(phase==1){if(Time.time-start<2.2f)return;Check(panel.Percent.text=="90%"&&panel.Marker.anchoredPosition.x==675,"Actual update reaches source cap");ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/boot-loading-current.png");start=Time.time;phase=2;return;}
                if(Time.time-start<.4f)return;
                if(phase==2){panel.SetState(false);Check(panel.gameObject.activeSelf,"Hide does not instantly remove loading page");start=Time.time;phase=3;return;}
                if(Time.time-start<1)return;Check(!panel.gameObject.activeSelf&&panel.Percent.text=="100%","Real tween completes and scaled delayed hide executes");
                Debug.Log("NUT_BOOT_LOADING_PLAY_PASS actual source-derived page on scene canvas, Start/logo size, progress cap, percentage and marker, global completion and delayed inactive state; automatic bootstrap not yet connected.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
