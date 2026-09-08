using System;
using System.IO;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalMainPanelPlayValidation
    {
        private const string Key="NutSort.MainPanelPlayValidation";
        private static OriginalGameScene game;
        private static OriginalMainPanelView panel;
        private static int phase,cancels;
        private static double deadline,timeout;
        private static string before;
        static OriginalMainPanelPlayValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+50;EditorApplication.update+=Tick; }
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,1040,"Nut Sort full MainPanel validation");
                SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"MainPanel Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var old=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete"),old.transform.parent,false).GetComponent<OriginalMainPanelView>();
                    panel.transform.SetSiblingIndex(old.transform.GetSiblingIndex());UnityEngine.Object.Destroy(old.gameObject);
                    var user=OriginalMainTopViewValidation.MakeUser();
                    OriginalMainPanelValidation.BindFixture(panel,user,game.Tables,game.ScheduleDelay,(active,delay)=>{Check(!active && delay==.5f,"Exchange cancellation route");cancels++;});
                    panel.Init();panel.Refresh();panel.Top.Progress.Show();
                    game.ModalInputBlocked=true;before=PlayerPrefs.GetString(NutSort.Content.OriginalUserStore.Key);
                    phase=1;deadline=now+.6;return;
                }
                if(now<deadline)return;
                if(phase==1) { Capture("main-panel-complete-play");phase=2;deadline=now+.3;return; }
                if(phase==2) { panel.ExchangeMask.gameObject.SetActive(true);phase=3;deadline=now+.3;return; }
                if(phase==3) { Capture("main-panel-exchange-mask-play");panel.ExchangeMask.onClick.Invoke();Check(cancels==1,"Actual bound mask callback");phase=4;deadline=now+.3;return; }
                Check(File.Exists("Library/ValidationCaptures/main-panel-complete-play.png") && File.Exists("Library/ValidationCaptures/main-panel-exchange-mask-play.png"),"Latest captures saved");
                Check(PlayerPrefs.GetString(NutSort.Content.OriginalUserStore.Key)==before,"Fixture did not persist rewards");
                Debug.Log("NUT_MAIN_PANEL_PLAY_VALIDATION_PASS full prefab parent initialization in real scene, current combined layout and exchange mask render/callback; normal bootstrap and core exchange execution pending.");Finish(0);
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Capture(string name)
        {
            string path=Path.GetFullPath("Library/ValidationCaptures/"+name+".png");Directory.CreateDirectory(Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
        private static void Finish(int code) { OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code); }
    }
}
