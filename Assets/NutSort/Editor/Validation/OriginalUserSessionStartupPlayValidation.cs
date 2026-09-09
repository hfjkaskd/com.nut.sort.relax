using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalUserSessionStartupPlayValidation
    {
        private const string Key="NutSort.UserSessionStartupPlay";
        private static double timeout;private static bool done;
        static OriginalUserSessionStartupPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying||done)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"User session Play timeout");
                var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();var session=audio.UserState;
                // Explicit saved configuration fixture, restored by OriginalPreferenceFixture.
                session.Data.ServerConfigData=new JObject();session.Data.LoginTime=OriginalPlayerGoldHintSchedule.UtcSeconds();session.Data.TodayChallengeTimes=5;session.Data.ComeOnGold=null;game.SaveUserData();
                var prior=game.User;var country=new OriginalUserCountryState(game.Tables.Countries,s=>Debug.LogError(s)){CountryCode="US",Area="USA"};
                Action<bool> held=null;int rewards=0,saves=0;OriginalUserStartup startup=null;
                startup=new OriginalUserStartup(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"),new UnityUserPreferences(),country,
                    ()=>throw new Exception("Existing save must not call SDK identity"),(cb,flag)=>{Check(ReferenceEquals(game.User,startup.Data)&&ReferenceEquals(audio.UserState.Store,startup.Store),"Actual game/audio see startup store before request");held=cb;},
                    ()=>throw new Exception("Saved config must not register"),(cb,flag)=>{Check(session.IsUserInitDone,"Ready before reward request");rewards++;},()=>{game.SaveUserData();saves++;});
                session.StartUser(startup);Check(!ReferenceEquals(prior,game.User)&&ReferenceEquals(game.User,session.Data)&&!session.IsUserInitDone&&held!=null,"Scene now reads published user while request waits");
                held(false);Check(session.IsUserInitDone&&rewards==1&&saves==1,"Held config completion through actual session and scene save");
                bool old=game.User.IsAudio;game.User.IsAudio=!old;Check(audio.AudioEnabled==!old,"Audio reads same new user");game.User.IsAudio=old;audio.Initialize();Check(ReferenceEquals(game.User,startup.Data),"Repeated audio initialization keeps startup user");
                Debug.Log("NUT_USER_SESSION_STARTUP_PLAY_PASS actual scene/audio/session shared startup store, held config callback readiness/reward and scene board save, audio state and repeated initialization; configuration/country are explicit fixtures, SDK excluded and scene automatic bootstrap pending.");done=true;Finish(0);
            }
            catch(Exception error){Debug.LogException(error);done=true;Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
