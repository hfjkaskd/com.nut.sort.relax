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
    public static class OriginalMainTopPlayValidation
    {
        private const string Key="NutSort.MainTopPlayValidation";
        private static OriginalGameScene game;
        private static OriginalMainTopView top;
        private static OriginalUserLocalData user;
        private static int phase;
        private static double deadline,timeout;
        private static string before;
        static OriginalMainTopPlayValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+50;EditorApplication.update+=Tick; }
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,1040,"Nut Sort complete Top validation");
                SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Top Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    Transform old=main.GetComponentInChildren<OriginalRewardProgressView>(true).transform.parent;
                    top=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelTop"),old.parent,false).GetComponent<OriginalMainTopView>();
                    top.transform.SetSiblingIndex(old.GetSiblingIndex());UnityEngine.Object.Destroy(old.gameObject);
                    user=OriginalMainTopViewValidation.MakeUser();user.Level=2;
                    var formatter=new OriginalGoldFormatter(()=>"en-US");
                    var item=new OriginalMarqueeItem { IsGold=true,Minimum=7,Maximum=7 };
                    top.Bind(user,game.Tables,"en",formatter,()=>"US",()=>true,done=>done(),done=>done(),id=>{},()=>{},()=>true,()=>item,()=>false,game.ScheduleDelay);
                    top.Init();top.PlayerHint.Init();top.Refresh();
                    before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;
                    phase=1;deadline=now+.5;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(top.Gold.Icon.sprite!=null && top.Hidden.gameObject.activeSelf && !top.Coin.gameObject.activeSelf,"Real bound currency Start and early visibility");
                    Capture("main-top-early-play");phase=2;deadline=now+.3;return;
                }
                if(phase==2)
                {
                    user.Level=52;top.Refresh();top.Progress.Show();phase=3;deadline=now+.5;return;
                }
                if(phase==3)
                {
                    Check(!top.Hidden.gameObject.activeSelf && top.Coin.gameObject.activeSelf && top.Progress.gameObject.activeSelf && top.Level.Group.anchoredPosition.x==374,"Real late-stage coordinated visibility");
                    Capture("main-top-late-play");phase=4;deadline=now+.3;return;
                }
                if(phase==4) { top.PlayerHint.Push();phase=5;deadline=now+.8;return; }
                if(phase==5)
                {
                    Check(top.PlayerHint.transform.localPosition==Vector3.zero,"Bound UTC hint reaches visible position");
                    Capture("main-top-hint-play");phase=6;deadline=now+.3;return;
                }
                foreach(string name in new[]{"main-top-early-play","main-top-late-play","main-top-hint-play"})Check(File.Exists("Library/ValidationCaptures/"+name+".png"),"Latest capture saved");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"No persisted fixture rewards");
                Debug.Log("NUT_MAIN_TOP_PLAY_VALIDATION_PASS full prefab mounted in actual scene, Gold Start after binding, early/late layouts, shared UTC hint and unchanged save; normal startup provider integration pending.");
                Finish(0);
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Capture(string name)
        {
            string path=Path.GetFullPath("Library/ValidationCaptures/"+name+".png");Directory.CreateDirectory(Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
        private static void Finish(int code) { Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code); }
    }
}
