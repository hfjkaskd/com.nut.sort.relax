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
    public static class OriginalMarqueePlayValidation
    {
        private const string Key="NutSort.MarqueePlayValidation";
        private static OriginalGameScene game;
        private static OriginalMarqueeLauncher launcher;
        private static int phase,liveLevel,queries,widthCallbacks;
        private static bool pausedCallback,hiddenCallback;
        private static double deadline,timeout;
        private static Vector3 pausedPosition;
        private static string before;
        static OriginalMarqueePlayValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+65;EditorApplication.update+=Tick; }
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,1040,"Nut Sort marquee validation");
                SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Marquee Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    var progress=main.GetComponentInChildren<OriginalRewardProgressView>(true);
                    launcher=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PMDBullet"),progress.transform.parent,false).GetComponent<OriginalMarqueeLauncher>();
                    var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    var text=new OriginalMarqueeText(user,game.Tables,()=>"Player_AI29",n=>"$7",(a,b)=>7);
                    var data=new OriginalMarqueeItem { IsGold=true,Minimum=5,Maximum=9 };
                    liveLevel=3;
                    launcher.Bind(()=>liveLevel,()=>{queries++;return data;},view=>view.Bind(text,game.Tables.PayChannels,"en",()=>"US",(seconds,done)=>game.ScheduleDelay(seconds,()=>{done();widthCallbacks++;}),null,(a,b)=>2),(a,b)=>a);
                    launcher.Init();Check(launcher.PoolCount==1,"Actual tall-screen aspect gate");
                    liveLevel=4;before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;
                    Time.timeScale=0;game.ScheduleDelay(.01f,()=>pausedCallback=true);
                    phase=1;deadline=now+.3;return;
                }
                if(phase==1)
                {
                    if(now<deadline)return;
                    Check(!pausedCallback && queries==0,"Main-owner delay and initial launcher clock pause");
                    Time.timeScale=4;phase=2;return;
                }
                if(phase==2)
                {
                    if(queries<2 || widthCallbacks<1)return;
                    Check(queries==2 && pausedCallback,"Actual Update launches after scaled initial wait");
                    Check(Mathf.Abs(launcher.Template.Rect.sizeDelta.x-launcher.Template.Tip.rectTransform.sizeDelta.x-140)<.01f,"Real coroutine applies text layout width");
                    launcher.enabled=false;launcher.gameObject.SetActive(false);
                    game.ScheduleDelay(.05f,()=>hiddenCallback=true);
                    Time.timeScale=0;phase=3;deadline=now+.1;return;
                }
                if(now<deadline)return;
                if(phase==3) { pausedPosition=launcher.Template.transform.localPosition;phase=4;deadline=now+.3;return; }
                if(phase==4)
                {
                    Check(launcher.Template.transform.localPosition==pausedPosition && !hiddenCallback,"Hidden travel and main-owner callback pause");
                    Time.timeScale=4;phase=5;deadline=now+.4;return;
                }
                if(phase==5)
                {
                    Check(hiddenCallback && launcher.Template.transform.localPosition.x<pausedPosition.x,"Main owner coroutine and item travel continue with launcher hidden");
                    launcher.gameObject.SetActive(true);phase=6;deadline=now+.6;return;
                }
                if(phase==6)
                {
                    string path=Path.GetFullPath("Library/ValidationCaptures/marquee-play.png");Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ScreenCapture.CaptureScreenshot(path);phase=7;deadline=now+.2;return;
                }
                if(!launcher.Template.IsReady)return;
                Check(queries==2 && launcher.PoolCount==1,"Disabled launcher does not spawn while travel completes");
                Check(launcher.Template.transform.localPosition.x==-launcher.Rect.sizeDelta.x*.5f && launcher.Template.gameObject.activeSelf,"Completion retains active item at end");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"No currency grant or save");
                Debug.Log("NUT_MARQUEE_PLAY_VALIDATION_PASS actual scaled launch, main-game coroutine layout, paused and hidden callbacks/travel, disabled spawning and completion; production Top binding pending.");
                Finish(0);
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Check(bool condition,string message) { if(!condition)throw new InvalidOperationException(message); }
        private static void Finish(int code) { Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code); }
    }
}
