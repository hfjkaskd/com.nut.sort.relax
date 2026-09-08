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
    public static class OriginalPlayerGoldHintPlayValidation
    {
        private const string Key="NutSort.PlayerHintPlayValidation";
        private static OriginalGameScene game;
        private static OriginalPlayerGoldHintView view;
        private static int phase,calls;
        private static double deadline,timeout;
        private static string before;
        static OriginalPlayerGoldHintPlayValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+50;EditorApplication.update+=Tick; }
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,1040,"Nut Sort player hint validation");
                SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Hint Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    var progress=main.GetComponentInChildren<OriginalRewardProgressView>(true);
                    view=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PlayerGoldGetHint"),progress.transform.parent,false).GetComponent<OriginalPlayerGoldHintView>();
                    var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.Level=3;
                    var item=new OriginalMarqueeItem { IsGold=true,Minimum=5,Maximum=9 };
                    var schedule=new OriginalPlayerGoldHintSchedule(()=>JObject.Parse("{\"LSSUPT\":[20,30]}"),()=>false,()=>item,view.PresentPush,()=>100,(a,b)=>20);
                    view.Bind(schedule,new OriginalPlayerGoldHintText(user,game.Tables,n=>"$7",()=>"Player_AI29",(a,b)=>7),new OriginalPlayerGoldHintIcons(game.Tables.PayChannels,()=>"US",null,(a,b)=>2),()=>"en",()=>game.Tables.ChannelInfos.GetChannelInfo("PayPal"));
                    view.Init();view.Push();before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;
                    Time.timeScale=0;phase=1;deadline=now+.3;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(view.LastShowTime==120 && view.transform.localPosition.y==500,"UTC polling continues while scaled motion pauses");
                    Time.timeScale=1;phase=2;deadline=now+.7;return;
                }
                if(phase==2)
                {
                    Check(view.transform.localPosition==Vector3.zero,"Real frame entrance completes");
                    string path=Path.GetFullPath("Library/ValidationCaptures/player-gold-hint-play.png");Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ScreenCapture.CaptureScreenshot(path);phase=3;deadline=now+.3;return;
                }
                if(phase==3)
                {
                    Check(File.Exists("Library/ValidationCaptures/player-gold-hint-play.png"),"Current Play capture saved");
                    view.gameObject.SetActive(false);phase=4;deadline=now+3.5;return;
                }
                if(phase==4)
                {
                    Check(view.transform.localPosition==new Vector3(0,500,0),"Global motion completes while owner is inactive");
                    view.gameObject.SetActive(true);view.Show(()=>calls++);phase=5;deadline=now+1.8;return;
                }
                Check(calls==1 && view.transform.localPosition==new Vector3(0,500,0),"Actual callback fires after return");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"No currency grant or save");
                Debug.Log("NUT_PLAYER_GOLD_HINT_PLAY_VALIDATION_PASS actual UTC Update during timeScale zero, scaled entrance, current capture, hidden exit and callback completion; production Top binding pending.");
                Finish(0);
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
        private static void Finish(int code) { Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code); }
    }
}
