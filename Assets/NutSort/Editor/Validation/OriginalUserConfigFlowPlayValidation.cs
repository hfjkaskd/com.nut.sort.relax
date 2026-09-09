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
    public static class OriginalUserConfigFlowPlayValidation
    {
        private const string Key="NutSort.UserConfigFlowPlay";private static double timeout;private static float begin;private static int phase,completed;
        private static OriginalMessagePanel panel;private static OriginalUserConfigFlow flow;private static readonly List<Action<bool>> pending=new List<Action<bool>>();
        static OriginalUserConfigFlowPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"User config Play timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform canvas=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")canvas=c.transform;Check(canvas!=null,"Actual canvas");
                    panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel"),canvas,false).GetComponent<OriginalMessagePanel>();panel.Bind(id=>game.Tables.Text.GetText(id,"en"));panel.Init();panel.gameObject.SetActive(false);
                    flow=new OriginalUserConfigFlow(done=>{Check(!panel.gameObject.activeSelf,"Hide before request");pending.Add(done);},panel.Show);
                    var session=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>().UserState;
                    session.Data.ServerConfigData=new JObject();session.Data.LoginTime=OriginalPlayerGoldHintSchedule.UtcSeconds();session.Data.TodayChallengeTimes=5;session.Data.ComeOnGold=null;game.SaveUserData();
                    var country=new OriginalUserCountryState(game.Tables.Countries,s=>throw new Exception(s)){CountryCode="US",Area="USA"};int rewards=0;
                    var startup=new OriginalUserStartup(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"),new UnityUserPreferences(),country,
                        ()=>throw new Exception("Saved user"),flow.Config,()=>throw new Exception("Saved config"),(done,flag)=>{Check(session.IsUserInitDone,"Ready before reward");rewards++;},game.SaveUserData);
                    session.StartUser(startup);Check(!session.IsUserInitDone&&pending.Count==1,"Actual startup waits on config flow");pending[0](false);
                    Check(session.IsUserInitDone&&rewards==1&&!panel.gameObject.activeSelf,"Startup nonpopup failure proceeds with saved config through original tail");
                    flow.Config(value=>{Check(value,"Retry final result");completed++;},true);pending[1](false);Check(panel.gameObject.activeSelf&&completed==0,"Popup failure holds caller");begin=Time.time;phase=1;return;
                }
                if(Time.time-begin<1)return;
                if(phase==1){Directory.CreateDirectory("Library/ValidationCaptures");ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/config-retry-current.png");begin=Time.time;phase=2;return;}
                if(phase==2){panel.ConfirmButton.onClick.Invoke();Check(!panel.gameObject.activeSelf&&pending.Count==3&&completed==0,"Real Button starts retry");pending[2](false);Check(panel.gameObject.activeSelf,"Retry failure reopens dialog");begin=Time.time;phase=3;return;}
                if(phase==3){panel.ConfirmButton.onClick.Invoke();pending[3](true);Check(completed==1&&!panel.gameObject.activeSelf,"Retry success releases original caller");Debug.Log("NUT_USER_CONFIG_FLOW_PLAY_PASS actual user session startup/nonpopup failed config continuation, original MessagePanel localization/standard Button/repeated retry and final success; boolean request results are explicit fixtures, production transport composition pending.");Finish(0);}
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
