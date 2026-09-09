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
    public static class OriginalWithdrawalStageZeroPlayValidation
    {
        private const string Key="NutSort.WithdrawalStageZeroPlay";
        private static double timeout;private static float start;private static int phase,saves,hides,returns;
        private static OriginalWithdrawalStageZeroHost stage;private static OriginalWithdrawalTooFastHost hint;
        static OriginalWithdrawalStageZeroPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+150;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,1040,"Withdrawal first-stage chain");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Stage-zero Play timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;Check(parent!=null,"Actual canvas");
                    game.User.UserLssInfo=null;game.User.GoldRewardTargetS2CData=new JObject{{"bear_list",new JArray(new JObject(),new JObject(),new JObject{{"Stage2StartShowLevel",12}})}};
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                    stage=new OriginalWithdrawalStageZeroHost(parent,"Prefabs/Panels/TXProgress0Panel",game.User,game.Tables,"en",()=>100,v=>"fixture",(t,f)=>"09/08/2026 12:00",(a,b)=>a,()=>false,()=>7,
                        ()=>saves++,v=>throw new Exception("Guide fixture routes UI18"),()=>true,s=>audio.PlaySound(s),
                        (id,args)=>{if(id==33){Check(!hint.IsOpen,"Single initial hint");hint.Show(args);}else{Check(id==18&&stage.Panel.Closing,"Native return route");returns++;}},()=>hides++,game.ScheduleDelay,()=>{});
                    hint=new OriginalWithdrawalTooFastHost(parent,"Prefabs/Panels/TXTooFastHintPanel",game.User,game.Tables,"en",()=>true,s=>audio.PlaySound(s),()=>7,
                        ()=>{Check(stage.IsOpen&&hint.Panel.Closing,"Resolve actual live stage during hint close");return stage.Panel.PlayProgress;},()=>{},game.ScheduleDelay,()=>{});
                    stage.Show(new object[]{true});start=Time.time;phase=1;return;
                }
                if(phase==1){if(!hint.IsOpen)return;start=Time.time;phase=2;return;}
                if(Time.time-start<.8f)return;
                if(phase==2){Check(stage.Panel.SureButton.transform.localScale==Vector3.zero,"Source waits for hint");ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-stage-zero-hint-current.png");start=Time.time;phase=3;return;}
                if(phase==3){hint.Panel.SureButton.onClick.Invoke();Check(stage.IsOpen&&hint.Panel.Closing,"Actual hint resumes retained stage");start=Time.time;phase=4;return;}
                if(phase==4){if(Time.time-start<4)return;Check(!hint.IsOpen&&stage.Panel.SureButton.transform.localScale==Vector3.one&&stage.Panel.Steps.Find("3/Loading").gameObject.activeSelf,"Actual resumed animation");ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-stage-zero-current.png");start=Time.time;phase=5;return;}
                if(phase==5){stage.Panel.SureButton.onClick.Invoke();Check(returns==1&&stage.IsOpen,"Actual Sure routes before removal");start=Time.time;phase=6;return;}
                Check(!stage.IsOpen&&hides==1&&saves==2,"Actual stage host removal and initial saves");
                Debug.Log("NUT_WITHDRAWAL_STAGE_ZERO_PLAY_PASS actual stage23 first animation to hint33 and Button return to actual stage23 resume, final Sure UI18 route and animated removal; clock/data/UI18 destination are explicit fixtures, no production/SDK claim.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
