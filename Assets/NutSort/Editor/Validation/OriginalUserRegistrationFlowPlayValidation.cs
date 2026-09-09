using System;
using System.Collections.Generic;
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
    public static class OriginalUserRegistrationFlowPlayValidation
    {
        private const string Key="NutSort.UserRegistrationFlowPlay";private static double timeout;private static float begin;private static int phase,registrations,identities,saves,rewards;
        private static OriginalMessagePanel panel;private static OriginalUserSession session;private static OriginalUserStartup startup;private static JObject prior;
        private static Action<string,byte[]> registered;private static Action<OriginalGoldRewardInfoResponse> reward;private static readonly List<Action<string,byte[]>> configs=new List<Action<string,byte[]>>();
        static OriginalUserRegistrationFlowPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin("");EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Fresh registration Play timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    session=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>().UserState;
                    // OriginalPreferenceFixture preserves the real save; ensure this
                    // second, explicitly composed startup receives an empty save.
                    PlayerPrefs.SetString(OriginalUserStore.Key,"");PlayerPrefs.Save();prior=OriginalServerConfigRequests.Cached;OriginalServerConfigRequests.Cached=null;
                    Transform canvas=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")canvas=c.transform;Check(canvas!=null,"Actual canvas");
                    panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel"),canvas,false).GetComponent<OriginalMessagePanel>();panel.Bind(id=>game.Tables.Text.GetText(id,"en"));panel.Init();panel.gameObject.SetActive(false);
                    var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");var country=new OriginalUserCountryState(game.Tables.Countries,s=>throw new Exception(s)){CountryCode="US",Area="USA"};
                    var request=new OriginalServerConfigRequests(defaults.ServerConfig,()=>country.CountryInfo,new OriginalServerConfigInitialization(()=>session.Data),
                        (area,callback)=>{Check(area=="USA"&&!panel.gameObject.activeSelf&&session.Data.UserId=="fixture-server-user","Current Area, server ID and hidden message before config request");configs.Add(callback);});
                    var config=new OriginalUserConfigFlow(request.Config,panel.Show);OriginalUserRegistrationFlow registration=null;
                    var rewardFlow=new OriginalGoldRewardInfoFlow(()=>session.Data,
                        (refresh,callback)=>{Check(refresh&&!session.IsUserInitDone,"Fresh reward request refreshes and holds readiness");rewards++;reward=callback;},
                        panel.Show,()=>false,()=>throw new Exception("Original bootstrap has not assigned main panel yet"));
                    // The restored UserMgr registration callback ignores its payload.
                    Action<Action<JObject>,bool> rewardRequest=(callback,refresh)=>rewardFlow.GoldRewardInfo(result=>callback(result.kinetic_data),refresh);
                    Action save=()=>{Check(!session.IsUserInitDone&&reward!=null,"Save follows held reward request");saves++;game.SaveUserData();};
                    startup=new OriginalUserStartup(defaults,new UnityUserPreferences(),country,()=>{identities++;return "explicit-fixture-device";},config.Config,()=>registration.Register(),rewardRequest,save,()=>"Player_A1B2");
                    var registerRequest=new OriginalRegistrationRequests(()=>session.Data,()=>country.CountryInfo,
                        (area,callback)=>{Check(area=="USA"&&!panel.gameObject.activeSelf&&ReferenceEquals(game.User,startup.Data)&&country.CountryInfo.Code=="US","Fresh user published and country initialized before raw registration");registrations++;registered=callback;});
                    registration=new OriginalUserRegistrationFlow(registerRequest.Register,config.Config,rewardRequest,panel.Show,value=>startup.IsInitDone=value,save);
                    session.StartUser(startup);Check(identities==1&&registrations==1&&saves==0&&session.Data.ServerConfigData==null&&!session.IsUserInitDone,"Actual fresh identity then held registration");registered("{\"kinetic_gap\":\"NO-1\"}",null);Check(panel.gameObject.activeSelf&&configs.Count==0,"Registration failure message without config request");begin=Time.time;phase=1;return;
                }
                if(Time.time-begin<1)return;
                if(phase==1){panel.ConfirmButton.onClick.Invoke();Check(registrations==2&&identities==1&&!panel.gameObject.activeSelf,"Confirm retries register without rebuilding identity");registered(OriginalRegistrationRequestsValidation.Success,null);Check(session.Data.UserId=="fixture-server-user","Server ID replaced before config stage");Check(configs.Count==1&&!session.IsUserInitDone,"Registered success waits for real config");configs[0]("",null);Check(panel.gameObject.activeSelf&&rewards==0&&saves==0,"Config failure blocks fresh reward/save");begin=Time.time;phase=2;return;}
                if(phase==2){panel.ConfirmButton.onClick.Invoke();Check(configs.Count==2,"Config confirmation retries only config");configs[1]("{\"LSS260820\":true}",null);Check(rewards==1&&saves==1&&!session.IsUserInitDone&&ReferenceEquals(session.Data.ServerConfigData,OriginalServerConfigRequests.Cached),"Real config initialized then reward requested and saved, still waiting");var saved=JObject.Parse(PlayerPrefs.GetString(OriginalUserStore.Key));Check((bool)saved["ServerConfigData"]["LSS260820"]&&(string)saved["UserId"]=="fixture-server-user","Actual user save contains server ID and new config");begin=Time.time;phase=3;return;}
                if(phase==3){Check(!session.IsUserInitDone,"Elapsed time alone never releases registration readiness");reward(null);Check(!session.IsUserInitDone&&panel.gameObject.activeSelf&&saves==1,"Failed reward response holds fresh initialization behind original retry message");begin=Time.time;phase=4;return;}
                if(phase==4){panel.ConfirmButton.onClick.Invoke();Check(rewards==2&&saves==1&&!session.IsUserInitDone&&!panel.gameObject.activeSelf,"Reward confirmation retries only reward request");
                    // Explicit lower-response initialization fixture; its real InitLss remains pending.
                    session.Data.GoldRewardTargetS2CData=OriginalMainTopViewValidation.MakeUser().GoldRewardTargetS2CData;
                    reward(new OriginalGoldRewardInfoResponse{kinetic_gap="NO-0",kinetic_data=session.Data.GoldRewardTargetS2CData});
                    Check(session.IsUserInitDone&&saves==1&&registrations==2&&!panel.gameObject.activeSelf,"Only successful forced reward result releases registration callback");
                    Debug.Log("NUT_USER_REGISTRATION_FLOW_PLAY_PASS actual fresh store/session/country, register/config/reward MessagePanel retries, raw registration/config parsing and server ID replacement, reward request then save and failed forced reward holding readiness; success callback releases initialization, lower reward Init/transport and identity remain explicit fixtures, production bootstrap pending.");Finish(0);}
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalServerConfigRequests.Cached=prior;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
