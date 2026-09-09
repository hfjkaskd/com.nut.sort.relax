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
    public static class OriginalServerConfigRequestsPlayValidation
    {
        private const string Key="NutSort.ServerConfigRequestsPlay";private static double timeout;private static bool done;
        private static JObject prior;
        static OriginalServerConfigRequestsPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying||done)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Server config Play timeout");
                var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                var session=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>().UserState;
                session.Data.ServerConfigData=new JObject();session.Data.LoginTime=0;session.Data.TodayChallengeTimes=0;session.Data.ComeOnGold=null;game.SaveUserData();
                prior=OriginalServerConfigRequests.Cached;OriginalServerConfigRequests.Cached=null;
                Transform canvas=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")canvas=c.transform;Check(canvas!=null,"Actual canvas");
                var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel"),canvas,false).GetComponent<OriginalMessagePanel>();panel.Bind(id=>game.Tables.Text.GetText(id,"en"));panel.Init();panel.gameObject.SetActive(false);
                var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");var pending=new List<Action<string,byte[]>>();int randoms=0,rewards=0,saves=0;
                var country=new OriginalUserCountryState(game.Tables.Countries,s=>throw new Exception(s)){CountryCode="US",Area="USA"};
                var requests=new OriginalServerConfigRequests(defaults.ServerConfig,()=>country.CountryInfo,new OriginalServerConfigInitialization(()=>session.Data,random:(a,b)=>{randoms++;return a;}),
                    (area,callback)=>{Check(area=="USA","Initialized user country Area before request");pending.Add(callback);});
                var flow=new OriginalUserConfigFlow(requests.Config,panel.Show);
                var startup=new OriginalUserStartup(defaults,new UnityUserPreferences(),country,()=>throw new Exception("Saved identity"),flow.Config,()=>throw new Exception("Saved config"),
                    (callback,flag)=>{Check(session.IsUserInitDone&&ReferenceEquals(session.Data.ServerConfigData,OriginalServerConfigRequests.Cached),"Published config and readiness before reward");rewards++;},()=>{saves++;game.SaveUserData();});
                session.StartUser(startup);Check(!session.IsUserInitDone&&pending.Count==1&&(int)session.Data.ServerConfigData["LSSLR1"]==30,"Saved default config while real request pending");
                pending[0]("{\"lsslr1\":17,\"LSS260820\":true}",null);
                Check(session.IsUserInitDone&&rewards==1&&saves==1&&randoms==1&&session.Data.TodayChallengeTimes==5&&(int)game.User.ServerConfigData["LSSLR1"]==17&&(bool)game.User.ServerConfigData["LSS260820"],"Raw response through parse/cache/Init/boolean/user tail/save");
                var cached=OriginalServerConfigRequests.Cached;requests.Config(value=>Check(value&&ReferenceEquals(cached,OriginalServerConfigRequests.Cached),"Cached immediate success"));Check(pending.Count==1,"Cached config avoids new transport");
                OriginalServerConfigRequests.Cached=null;bool finished=false;flow.Config(value=>finished=value,true);pending[1]("",null);Check(panel.gameObject.activeSelf&&!finished,"Empty raw response reaches original message failure branch");
                panel.ConfirmButton.onClick.Invoke();Check(!panel.gameObject.activeSelf&&pending.Count==3,"Standard confirmation initiates another actual config request");pending[2]("{\"LSSLR1\":18}",null);
                Check(finished&&(int)game.User.ServerConfigData["LSSLR1"]==18&&!panel.gameObject.activeSelf,"Retry raw response initializes live user before completion");
                Debug.Log("NUT_SERVER_CONFIG_REQUESTS_PLAY_PASS actual saved user/session/country, held raw config response to native defaults/shared cache/Init/bool/user startup readiness/reward/save, cached reuse and MessagePanel confirmation retry; raw transport and reward calls are explicit fixtures, default production bootstrap pending.");done=true;Finish(0);
            }
            catch(Exception error){Debug.LogException(error);done=true;Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalServerConfigRequests.Cached=prior;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
