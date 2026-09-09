using System;
using Newtonsoft.Json.Linq;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalWithdrawalTooFastPlayValidation
    {
        private const string Key="NutSort.WithdrawalTooFastPlay";
        private static double timeout;private static float start;private static int phase,resumes,hides,sounds;
        private static OriginalWithdrawalTooFastHost host;
        static OriginalWithdrawalTooFastPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+120;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,1040,"Withdrawal too-fast hint");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Too-fast Play timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;
                    Check(parent!=null,"Actual source canvas");
                    game.User.GoldRewardTargetS2CData=new JObject{{"bear_list",new JArray(new JObject(),new JObject(),new JObject{{"Stage2StartShowLevel",12}})}};
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                    host=new OriginalWithdrawalTooFastHost(parent,"Prefabs/Panels/TXTooFastHintPanel",game.User,game.Tables,"en",()=>true,s=>{sounds++;audio.PlaySound(s);},()=>7,
                        ()=>{Check(host.IsOpen&&host.Panel.Closing&&hides==0,"Resolve live stage during close");return ()=>resumes++;},()=>hides++,game.ScheduleDelay,()=>{});
                    host.Show(Array.Empty<object>());start=Time.time;phase=1;return;
                }
                if(Time.time-start<.8f)return;
                if(phase==1)
                {
                    Check(host.Panel.LevelValue.text=="6/12"&&host.Panel.VideoValue.text=="10/10"&&host.Panel.LevelFill.fillAmount==.5f,"Actual view display");
                    ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-too-fast-current.png");start=Time.time;phase=2;return;
                }
                if(phase==2){host.Panel.SureButton.onClick.Invoke();Check(resumes==1&&sounds==1&&host.IsOpen&&host.Panel.Closing,"Actual Button resumes before hide");start=Time.time;phase=3;return;}
                Check(!host.IsOpen&&hides==1&&resumes==1,"Actual host animated removal");
                Debug.Log("NUT_WITHDRAWAL_TOO_FAST_PLAY_PASS current source prefab on actual canvas, localized values, standard Button, live resolver before animated hide; resume target is explicit fixture, no stage23 production or SDK claim.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
