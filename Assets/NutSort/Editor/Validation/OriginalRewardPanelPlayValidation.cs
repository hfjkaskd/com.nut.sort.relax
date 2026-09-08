using System;
using System.Collections.Generic;
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
    public static class OriginalRewardPanelPlayValidation
    {
        private const string Key="NutSort.RewardPanelPlay";
        private static double timeout;
        private static float start;
        private static int phase,flies,queued,hidden,revoke,exchange;
        private static OriginalRewardPanelHost host;
        private static OriginalGameScene game;
        static OriginalRewardPanelPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"RewardPanel Play timeout");
                if(phase==0)
                {
                    game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                    Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;
                    Check(parent!=null,"Actual scene UI canvas");
                    revoke=game.User.RevokeCount;exchange=game.User.ExchangeCount;
                    var items=new OriginalItemManager(game.User,game.SaveUserData,()=>{},(v,a,b)=>throw new InvalidOperationException("Unexpected cash"),(v,a)=>throw new InvalidOperationException("Unexpected coin"));
                    var factory=new OriginalRewardItemFactory(Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem"),new OriginalGoldFormatter(()=>"en-US"),()=>"US");
                    var queue=new OriginalPanelActionQueue();queue.Add(()=>queued++);
                    host=new OriginalRewardPanelHost(parent,"Prefabs/Panels/RewardPanel",factory,items,(type,icon)=>
                    {
                        Check(host.IsOpen&&icon!=null&&Time.time-start>=.99f,"Actual panel/icon alive at delayed fly dispatch");
                        Check(type==2?game.User.RevokeCount==revoke+2:game.User.ExchangeCount==exchange+3,"Inventory addition precedes fly dispatch");flies++;
                    },game.ScheduleDelay,queue,()=>hidden++);
                    start=Time.time;
                    host.Show(new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=2,Count=2},new OriginalItemInfo{ItemType=3,Count=3}}});
                    Check(host.IsOpen&&flies==0&&host.Panel.Views.Count==2,"Registry owns actual panel before delayed collect");
                    Directory.CreateDirectory("Library/ValidationCaptures");phase=1;return;
                }
                float elapsed=Time.time-start;
                if(phase==1)
                {
                    if(elapsed<.2f)return;
                    Check(flies==0&&game.User.RevokeCount==revoke,"No early inventory update");
                    Canvas.ForceUpdateCanvases();ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/reward-panel-current.png");phase=2;return;
                }
                if(phase==2)
                {
                    if(elapsed<1.8f)return;
                    Check(flies==2&&hidden==1&&!host.IsOpen&&queued==0,"Actual timers grant and hide then wait for base queue delay");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.RevokeCount==revoke+2&&saved.ExchangeCount==exchange+3,"Actual item manager persists tool rewards through scene save");phase=3;return;
                }
                if(queued==0)return;
                Check(queued==1&&elapsed>=3.99f,"Queue continuation follows one plus half plus two-and-half seconds");
                Check(File.Exists("Library/ValidationCaptures/reward-panel-current.png"),"Latest actual panel capture exists");
                Debug.Log("NUT_REWARD_PANEL_PLAY_PASS actual RewardPanel prefab/registry, configured layout, one-second item-manager tool grants and saves, fly dispatch with live icons, half-second close/destruction and 2.5-second queue continuation; explicit tool fixture and flight consumer, actual flight implementation/default startup pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
