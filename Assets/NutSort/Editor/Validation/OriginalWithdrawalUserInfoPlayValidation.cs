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
    public static class OriginalWithdrawalUserInfoPlayValidation
    {
        private const string Key="NutSort.WithdrawalUserInfoPlay";
        private static double timeout;private static float start;private static int phase;
        private static OriginalWithdrawalUserInfoPanel panel;
        private static OriginalWithdrawalUserInfoPanelValidation.Services services;
        static OriginalWithdrawalUserInfoPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Withdrawal account form");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Form Play timeout");
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform parent=null;foreach(var canvas in UnityEngine.Object.FindObjectsOfType<Canvas>())if(canvas.name=="UICanvas")parent=canvas.transform;
                    Check(parent!=null,"Actual canvas");game.User.UserLssInfo=null;
                    services=new OriginalWithdrawalUserInfoPanelValidation.Services();panel=OriginalWithdrawalUserInfoPanelValidation.Create(parent,game.User,game.Tables,services);
                    Directory.CreateDirectory("Library/ValidationCaptures");start=Time.time;phase=1;return;
                }
                if(phase==1)
                {
                    if(Time.time-start<.7f)return;
                    ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-user-form-current.png");phase=2;return;
                }
                if(phase==2)
                {
                    if(Time.time-start<1)return;
                    panel.GetButton.onClick.Invoke();Check(services.Tip!=0&&services.Saves==0&&!panel.Closing,"Actual blank form rejects without closing");
                    panel.ChannelButton(2).onClick.Invoke();Check(panel.Flow.GetTypeIndex==2,"Actual PayPal selection");
                    panel.Name="Fixture User";panel.Email="fixture@example.test";panel.Number="12345";
                    ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-user-filled-current.png");phase=3;return;
                }
                if(phase==3)
                {
                    if(Time.time-start<1.4f)return;
                    panel.GetButton.onClick.Invoke();Check(services.Panel==21&&services.Saves==1&&panel.Closing,"Actual form routes confirmation and saves fixture state");start=Time.time;phase=4;return;
                }
                if(Time.time-start<.4f)return;
                Check(services.Closes==1,"Animated close completes");
                Debug.Log("NUT_WITHDRAWAL_USER_INFO_PLAY_PASS current original form visual captures, blank rejection, real channel Button, TMP input submission and confirmation dispatch/animated close; services and user data are explicit fixtures, no payment or production host.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
