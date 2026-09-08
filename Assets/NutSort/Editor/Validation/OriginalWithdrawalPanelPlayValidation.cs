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
    public static class OriginalWithdrawalPanelPlayValidation
    {
        private const string Key="NutSort.WithdrawalPanelPlay";
        private static double timeout;private static float start;private static int phase;
        private static OriginalGameScene game;private static Transform parent;
        private static OriginalWithdrawalPanel panel;
        private static OriginalWithdrawalPanelValidation.Services host;
        static OriginalWithdrawalPanelPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"TXPanel validation");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Show(bool early)
        {
            var user=game.User;user.Level=early?2:20;user.GuideIndex=0;user.Level1Gold=5;
            user.ServerConfigData=JObject.Parse(@"{""LSS260820"":true}");
            user.GoldRewardTargetS2CData=JObject.Parse(@"{""cal_cfg"":5,""bear_list"":[{""psi_value"":""5""},{""psi_value"":""10""},{""RealLevel"":5,""Stage2RealLevel"":10,""caliper_logs"":3,""caliper_rank"":8}]}");
            user.Gold=5;user.TXTargetGold=early?"":"10";user.ComeOnGold=early?"":"10";user.LoginDay=1;user.IsGuideGold=false;
            OriginalWithdrawalPanel.IsPlayGoldTween=!early;
            panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/TXPanel"),parent,false).GetComponent<OriginalWithdrawalPanel>();
            host=new OriginalWithdrawalPanelValidation.Services{User=user,Tables=game.Tables,Allowed=true};
            var format=new OriginalGoldFormatter(()=>"en-US");panel.Bind(user,game.Tables,"en",v=>format.Format(v),host);
            panel.Init(early?new object[]{1}:Array.Empty<object>());panel.Refresh();start=Time.time;
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"TXPanel Play timeout");
                if(phase==0)
                {
                    game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;
                    Check(parent!=null,"Actual canvas exists");Directory.CreateDirectory("Library/ValidationCaptures");Show(true);phase=1;return;
                }
                if(phase==1)
                {
                    if(Time.time-start<.6f)return;
                    Check(panel.ProgressLabel.text=="1/1"&&panel.Main.localScale==Vector3.one,"Actual early panel open");
                    ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-early-current.png");phase=2;return;
                }
                if(phase==2){if(Time.time-start<.9f)return;panel.WithdrawalButton.onClick.Invoke();Check(host.Panels.Contains(20),"Actual Button route");start=Time.time;phase=3;return;}
                if(phase==3){if(Time.time-start<.4f)return;Check(host.Closes==1,"Actual close animation callback");UnityEngine.Object.Destroy(panel.gameObject);Show(false);phase=4;return;}
                if(phase==4)
                {
                    if(Time.time-start<1.7f)return;
                    var format=new OriginalGoldFormatter(()=>"en-US");Check(panel.ProgressLabel.text=="1/3"&&panel.GoldLabel.text==format.Format(10)&&host.Saves==1,"Actual pending tween finishes and displays later login progress");
                    ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-later-current.png");phase=5;return;
                }
                if(Time.time-start<2)return;
                Debug.Log("NUT_WITHDRAWAL_PANEL_PLAY_PASS actual original TXPanel, native button/close flow, actual pending gold tween, progress and PlayerInfo instantiation; two current captures; explicit fixture services, production host pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalWithdrawalPanel.IsPlayGoldTween=false;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
