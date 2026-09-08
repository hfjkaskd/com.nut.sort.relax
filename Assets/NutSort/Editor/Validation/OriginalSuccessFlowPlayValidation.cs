using System;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalSuccessFlowPlayValidation
    {
        private const string Key="NutSort.SuccessFlowPlay";
        private static double timeout;
        private static float started;
        private static int phase,requests,sounds;
        private static OriginalGameScene game;
        private static OriginalGameplayEffects effects;
        private static ParticleSystem particle;
        static OriginalSuccessFlowPlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+70;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Success Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    var main=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete"),startup.MainLevel.transform.parent,false).GetComponent<OriginalMainPanelView>();
                    var fixture=OriginalMainTopViewValidation.MakeUser();var user=game.User;
                    user.ServerConfigData=fixture.ServerConfigData;user.GoldRewardTargetS2CData=fixture.GoldRewardTargetS2CData;user.UserLssInfo=fixture.UserLssInfo;user.TXTargetGold=fixture.TXTargetGold;
                    OriginalMainPanelValidation.BindFixture(main,user,game.Tables,game.ScheduleDelay,(a,b)=>main.SetExchangeState(game,a,b));main.Init();
                    OriginalLevelInfo table=null;
                    for(int i=1;i<=game.Tables.LevelCount;i++){var candidate=game.Tables.GetLevelInfo(i,i);if(candidate.SubRound==candidate.SubTotalRound){table=candidate;break;}}
                    Check(table!=null,"Actual table has completed subround");user.Level=table.Level;user.IsAudio=true;
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();audio.SoundStarted+=name=>{if(name=="StageComplete")sounds++;};
                    effects=game.GetComponent<OriginalGameplayEffects>();
                    main.Top.Progress.gameObject.SetActive(true);
                    game.BindSuccess(()=>true,()=>throw new Exception("Normal path cannot teach"),main.Top.Gold.HideGoldHintText,main.Top.Progress.Hide,main.transform.parent,
                        captured=>{Check(captured==table&&Time.time-started>=.59f,"Delayed request retains source table");requests++;});
                    started=Time.time;game.Success();game.Success();
                    Check(game.IsSucceed&&sounds==1&&effects.ActiveCount==1&&requests==0&&!main.Top.Progress.gameObject.activeSelf,"Actual success flag, UI hide, single sound/effect and delayed request");
                    foreach(var ps in main.transform.parent.GetComponentsInChildren<ParticleSystem>())if(ps.gameObject.name=="SuccessEffect(Clone)")particle=ps;
                    Check(particle!=null&&particle.isPlaying,"Original victory particles autoplay under UI parent");
                    phase=1;return;
                }
                if(phase==1)
                {
                    if(Time.time-started<.9f)return;
                    Check(requests==1,"Exactly one delayed request, no fabricated grant");
                    game.InitLevel(true,false,false);Check(particle!=null&&effects.ActiveCount==1,"UI victory effect survives board clear");phase=2;return;
                }
                if(Time.time-started<5.3f)return;
                Check(particle==null&&effects.ActiveCount==0,"Actual effect destroyed after five game seconds");
                Debug.Log("NUT_SUCCESS_FLOW_PLAY_PASS actual success state, source main progress hide, StageComplete audio, autoplay victory prefab, single delayed captured-table request, board-clear survival and five-second cleanup; explicit configuration/request consumer, production completion/reward panels pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
