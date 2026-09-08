using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalMainBottomPlayValidation
    {
        private const string Key="NutSort.MainBottomPlayValidation";
        private static OriginalGameScene game;
        private static OriginalToolItemDisplay[] views;
        private static OriginalUserLocalData user;
        private static int phase,history;
        private static double deadline,timeout;
        private static string before;
        static OriginalMainBottomPlayValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+45;EditorApplication.update+=Tick; }
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,1040,"Nut Sort tool display validation");
                SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Tool display Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    Transform bottom=main.transform.Find("Bottom");Check(bottom!=null,"Current MainPanel Bottom");
                    user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    user.RevokeCount=7;user.ExchangeCount=0;user.AddScrewCount=3;user.CurrentLevelAddScrewCount=12;
                    user.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":12}");
                    var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelBottom"),bottom.parent,false);
                    root.transform.SetSiblingIndex(bottom.GetSiblingIndex());UnityEngine.Object.Destroy(bottom.gameObject);
                    var panel=root.GetComponent<OriginalMainBottomView>();
                    int opened=0;
                    panel.Bind(user,()=>true,()=>history,()=>{},()=>{},(type,amount,refresh)=>{},(active,delay)=>{},(id,type)=>{},id=>{},()=>true,()=>{},id=>opened+=id);
                    panel.Init();panel.Refresh();panel.Setting.onClick.Invoke();panel.Replay.onClick.Invoke();Check(opened==22,"Current setting/replay button binding");
                    views=new OriginalToolItemDisplay[3];for(int i=0;i<3;i++)views[i]=panel.Items[i].Display;
                    before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;phase=1;deadline=now+.5;return;
                }
                if(now<deadline)return;
                if(phase==1) { Capture("main-bottom-gray-play");phase=2;deadline=now+.3;return; }
                if(phase==2)
                {
                    history=1;user.CurrentLevelAddScrewCount=11;
                    foreach(var view in views)view.RefreshButtonState();phase=3;deadline=now+.3;return;
                }
                if(phase==3) { Capture("main-bottom-active-play");phase=4;deadline=now+.3;return; }
                Check(File.Exists("Library/ValidationCaptures/main-bottom-gray-play.png") && File.Exists("Library/ValidationCaptures/main-bottom-active-play.png"),"Latest captures saved");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"Fixture did not persist inventory");
                Debug.Log("NUT_MAIN_BOTTOM_PLAY_VALIDATION_PASS complete Bottom prefab in real scene, settings/replay callbacks, current gray/active captures and unchanged save; production MainPanel binding pending.");Finish(0);
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
