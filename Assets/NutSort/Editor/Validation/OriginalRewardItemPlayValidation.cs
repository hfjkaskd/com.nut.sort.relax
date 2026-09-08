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
    public static class OriginalRewardItemPlayValidation
    {
        private const string Key="NutSort.RewardItemPlay";
        private const string Capture="Library/ValidationCaptures/reward-item-current.png";
        private static double timeout,deadline;
        private static int phase;
        private static OriginalRewardItemView item;
        private static Transform glow;
        private static Quaternion initial,paused;
        static OriginalRewardItemPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Reward item Play timeout");
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                    Transform parent=null;
                    foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;
                    Check(parent!=null,"Actual scene UI canvas");
                    var factory=new OriginalRewardItemFactory(Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem"),new OriginalGoldFormatter(()=>"en-US"),()=>"US");
                    item=factory.GenerateItem(new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=0,Count=12.5f}}},parent,true)[0];
                    glow=item.GetComponentInChildren<OriginalLoopRotation>(true).transform;initial=glow.localRotation;
                    Check(item.Count.text=="$12.50"&&!item.Max.activeSelf,"Real prefab initialization");
                    deadline=EditorApplication.timeSinceStartup+.8;phase=1;return;
                }
                if(EditorApplication.timeSinceStartup<deadline)return;
                if(phase==1)
                {
                    Check(Quaternion.Angle(initial,glow.localRotation)>15,"Autoplay rotation advanced on real frame clock");
                    var particles=item.GetComponentsInChildren<ParticleSystem>(true);
                    Check(particles.Length>0,"Original big reward particle hierarchy");
                    bool playing=false;foreach(var p in particles)playing|=p.isPlaying;
                    Check(playing,"Source playOnAwake particles are running");
                    Directory.CreateDirectory("Library/ValidationCaptures");ScreenCapture.CaptureScreenshot(Capture);
                    Time.timeScale=0;deadline=EditorApplication.timeSinceStartup+.1;phase=2;return;
                }
                if(phase==2){paused=glow.localRotation;deadline=EditorApplication.timeSinceStartup+.3;phase=3;return;}
                Check(Quaternion.Angle(paused,glow.localRotation)<.001f,"Rotation freezes on scaled time");
                Check(File.Exists(Capture)&&new FileInfo(Capture).Length>1000,"Current project screenshot captured");
                Time.timeScale=1;
                Debug.Log("NUT_REWARD_ITEM_PLAY_PASS actual scene canvas and generated big reward, cash icon/text, native Max hiding, autoplay rotation/particles and scaled-clock pause; explicit item fixture, not the production SuccessPanel. Current capture: "+Capture);
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
        private static void Finish(int code){Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
