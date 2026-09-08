using System;
using System.Collections.Generic;
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
    public static class OriginalCurrencyRewardFlightPlayValidation
    {
        private const string Key="NutSort.CurrencyFlightPlay";
        private static double timeout;
        private static float start;
        private static int phase,sounds;
        private static OriginalCurrencyRewardFlight flight;
        private static OriginalRewardPanelHost host;
        private static OriginalGameScene game;
        static OriginalCurrencyRewardFlightPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Currency flight Play timeout");
                if(phase==0)
                {
                    game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                    Transform parent=null,top=null;
                    foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>()){if(c.name=="UICanvas")parent=c.transform;if(c.name=="TopUICanvas")top=c.transform;}
                    Check(parent!=null&&top!=null,"Actual panel/top canvases");
                    var hud=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelTop"),parent,false).GetComponent<OriginalMainTopView>();
                    var formatter=new OriginalGoldFormatter(()=>"en-US");
                    hud.Bind(game.User,game.Tables,"en",formatter,()=>"US",()=>false,done=>{},done=>{},id=>{},()=>{},()=>false,()=>null,()=>true,game.ScheduleDelay);
                    var pool=UnityEngine.Object.FindObjectOfType<OriginalPrefabPool>();
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();game.User.IsAudio=true;
                    flight=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/CurrencyRewardFlight"),game.transform,false).GetComponent<OriginalCurrencyRewardFlight>();
                    flight.Bind(pool,top,type=>type==0?hud.Gold.Icon.transform:hud.Coin.Icon.transform,()=>"US",game.ScheduleDelay,
                        (name,delay)=>{sounds++;audio.PlaySound(name,delay);},null);
                    var items=new OriginalItemManager(game.User,game.SaveUserData,()=>{},(value,a,b)=>game.User.Gold=value,(value,a)=>game.User.Coin=value);
                    var factory=new OriginalRewardItemFactory(Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem"),formatter,()=>"US");
                    host=new OriginalRewardPanelHost(parent,"Prefabs/Panels/RewardPanel",factory,items,flight.Fly,game.ScheduleDelay,new OriginalPanelActionQueue());
                    Action<JObject> response=null;int closed=0;
                    var get=game.CreateSuccessReward((more,done)=>response=done,(id,info)=>{Check(id==8,"Real reward host route");host.Show(info);},()=>closed++);
                    get(false);response(JObject.Parse(@"{""kinetic_data"":{""hg_amt"":""5"",""hg_psi"":""100"",""hg_zs_amt"":""3"",""hg_zs_psi"":""200.25""}}"));
                    Check(closed==1&&host.IsOpen&&flight.ActiveCount==0,"Actual claim consumer displays panel before delayed collection");
                    start=Time.time;phase=1;return;
                }
                float elapsed=Time.time-start;
                if(phase==1)
                {
                    if(elapsed<1.25f)return;
                    Check(flight.ActiveCount==12&&flight.TrailCount==12&&sounds==2,"Six cash and six coin icons/trails with per-call sound dispatch");
                    Check(game.User.Gold==100&&game.User.Coin==200.25,"Existing item manager reaches explicit balance consumers before flight");
                    Directory.CreateDirectory("Library/ValidationCaptures");phase=2;return;
                }
                if(phase==2)
                {
                    if(elapsed<1.8f)return;
                    Check(!host.IsOpen&&flight.ActiveCount>0,"Flight survives actual reward panel close");
                    ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/currency-reward-flight-current.png");phase=3;return;
                }
                if(elapsed<2.9f)return;
                Check(flight.ActiveCount==0&&flight.TrailCount==0,"All currency and star leases return after staggered travel");
                Check(File.Exists("Library/ValidationCaptures/currency-reward-flight-current.png"),"Current flight capture exists");
                Debug.Log("NUT_CURRENCY_REWARD_FLIGHT_PLAY_PASS actual claim adapter/RewardPanel host and HUD targets, six cash plus six coin pooled icons/stars, audio dispatch, panel-independent staggered travel and complete return; explicit response/balance fixtures, production startup pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
