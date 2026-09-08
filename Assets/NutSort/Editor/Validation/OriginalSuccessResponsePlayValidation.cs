using System;
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
    public static class OriginalSuccessResponsePlayValidation
    {
        private const string Key="NutSort.SuccessResponsePlay";
        private static double timeout;
        private static int phase,requests,panels,settlements;
        private static float requestedAt;
        private static OriginalGameScene game;
        private static Action<JObject> deliver;
        private static Action continueSettlement;
        private static JObject received;
        static OriginalSuccessResponsePlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+75;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Success response timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();OriginalLevelInfo intermediate=null;
                    for(int i=1;i<=game.Tables.LevelCount;i++){var info=game.Tables.GetLevelInfo(i,i);if(info.SubTotalRound>info.SubRound){intermediate=info;break;}}
                    Check(intermediate!=null,"Actual table intermediate subround");game.User.Level=intermediate.Level;
                    game.BindSuccessResponse(()=>true,()=>throw new Exception("Unexpected teaching"),()=>{},()=>{},startup.MainLevel.transform.parent,
                        callback=>{Check(Time.time-requestedAt>=.59f,"Actual source request delay");deliver=callback;requests++;},
                        (id,callback)=>{Check(id==43,"Actual special panel request ID");panels++;continueSettlement=callback;},response=>{received=response;settlements++;});
                    requestedAt=Time.time;game.Success();phase=1;return;
                }
                if(phase==1)
                {
                    if(deliver==null)return;
                    Check(requests==1,"One actual delayed request");game.User.Level=6;
                    deliver(null);deliver=null;
                    Check(game.IsRestarting&&!game.IsSucceed&&panels==0&&settlements==0,"Captured intermediate table triggers real InitLevel despite changed current level");
                    phase=2;return;
                }
                if(phase==2)
                {
                    var six=game.Tables.GetLevelInfo(6,6);Check(six.SubTotalRound<=six.SubRound,"Actual level-six settlement branch");
                    game.User.IsCompleteCoinNewPeopleReward=false;requestedAt=Time.time;game.Success();phase=3;return;
                }
                if(deliver==null)return;
                var payload=new JObject{{"fixture",true}};game.User.Level=1;deliver(payload);deliver=null;
                Check(requests==2&&panels==1&&settlements==0&&!game.User.IsCompleteCoinNewPeopleReward,"Captured level six defers settlement without changing live reward flag");
                continueSettlement();Check(settlements==1&&received==payload,"Special-panel continuation receives exact response reference");
                Debug.Log("NUT_SUCCESS_RESPONSE_PLAY_PASS real delayed success request composition, actual-table intermediate InitLevel and flag reset, captured level-six panel 43 routing despite current-level changes, unchanged reward flag and deferred exact response; explicit transport/panel consumers, real reward panel still pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
