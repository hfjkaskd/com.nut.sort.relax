using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalSceneToolsPlayValidation
    {
        private const string Key="NutSort.SceneToolsPlay";
        private static double timeout,deadline;
        private static int phase;
        private static OriginalGameScene game;
        static OriginalSceneToolsPlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+75;EditorApplication.update+=Tick;}}
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Scene tools timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    game.User.Level=3;game.User.LevelSeed=0;game.User.AddScrewCount=3;
                    game.InitLevel(true,false,false);phase=1;return;
                }
                if(phase==1)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    var main=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete"),startup.MainLevel.transform.parent,false).GetComponent<OriginalMainPanelView>();
                    startup.MainLevel.gameObject.SetActive(false);
                    var fixture=OriginalMainTopViewValidation.MakeUser();var user=game.User;
                    user.ServerConfigData=fixture.ServerConfigData;user.GoldRewardTargetS2CData=fixture.GoldRewardTargetS2CData;user.UserLssInfo=fixture.UserLssInfo;user.TXTargetGold=fixture.TXTargetGold;
                    OriginalMainPanelValidation.BindFixture(main,user,game.Tables,game.ScheduleDelay,(active,delay)=>main.SetExchangeState(game,active,delay));
                    bool allow=false,mode=true;int audio=0,panels=0,refreshes=0;
                    var items=new OriginalItemManager(user,game.SaveUserData,()=>{refreshes++;main.Bottom.Refresh();},(a,b,c)=>throw new Exception("Unexpected gold"),(a,b)=>throw new Exception("Unexpected coin"));
                    main.Bottom.BindScene(game,items,()=>12,()=>mode,(active,delay)=>main.SetExchangeState(game,active,delay),
                        (panel,type)=>{Check(panel==4&&type==4,"Native acquisition panel arguments");panels++;},id=>throw new Exception("Unexpected tip"),()=>allow,()=>audio++,id=>throw new Exception("Unexpected panel"));
                    main.Init();main.Bottom.Refresh();
                    var button=main.Bottom.GetOtherItem(4).Display.Click;
                    button.onClick.Invoke();Check(user.AddScrewCount==3&&audio==0,"UI click gate prevents tool and audio");
                    var level=game.Level;int count=level.Board.Screws.Length;var locked=level.GetScrew(count-1);
                    Check(locked.State.IsLocked&&locked.State.Capacity==0,"Original level-three locked screw");
                    Physics.SyncTransforms();var screen=game.WorldCamera.WorldToScreenPoint(locked.Bounds.bounds.center);
                    Check(game.TryOperateAtScreenPoint(screen).Kind==ScrewOperationKind.Ignored&&user.AddScrewCount==3,"Production init gate remains closed before explicit fixture release");
                    // Isolated input fixture; this is not a production initialization completion.
                    game.IsInitDone=true;
                    var outcome=game.TryOperateAtScreenPoint(screen);
                    Check(outcome.Kind==ScrewOperationKind.AddScrewRequested&&!outcome.OriginalReturnValue,"World ray dispatches native false-return branch");
                    Check(!locked.State.IsLocked&&locked.State.Capacity==1&&user.AddScrewCount==2&&user.CurrentLevelAddScrewCount==1&&audio==0,"World unlock consumes through shared flow without UI audio or UI gate");
                    Check(main.Bottom.GetOtherItem(4).Display.Value.text=="2"&&refreshes==1,"Real bottom display refresh after world click");
                    allow=true;mode=false;button.onClick.Invoke();
                    Check(locked.State.Capacity==4&&user.AddScrewCount==1&&user.CurrentLevelAddScrewCount==5&&audio==1,"Button shares live-mode flow and expands eligible rod");
                    button.onClick.Invoke();
                    Check(level.Board.Screws.Length==count+1&&level.GetScrew(count).TileCount==4&&user.AddScrewCount==0&&user.CurrentLevelAddScrewCount==9&&audio==2,"Same button adds whole prefab rod once no unlock candidate remains");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.AddScrewCount==0&&saved.CurrentLevelAddScrewCount==9&&saved.LevelInfo==OriginalBoardSnapshotJson.Write(level.CaptureSnapshot()),"Real user and full changed board persisted");
                    button.onClick.Invoke();Check(panels==1&&audio==3&&level.Board.Screws.Length==count+1,"No-stock branch requests native panel without extra rod");
                    Check(game.GetComponent<OriginalGameplayEffects>().ActiveCount==3,"Three actual unlock/add effects from three uses");
                    phase=2;deadline=Time.time+3.4;return;
                }
                if(Time.time<deadline)return;
                Check(game.GetComponent<OriginalGameplayEffects>().ActiveCount==0,"All actual effects released on game clock");
                Debug.Log("NUT_SCENE_TOOLS_PLAY_PASS shared real scene flow for locked world ray and complete bottom Button, independent UI/init gates, live mode, unlock/expand/new-rod branches, actual effects, item display, inventory/board save and no-stock panel request; isolated configuration and input fixture, default startup remains pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
