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
    public static class OriginalExchangeStatePlayValidation
    {
        private const string Key="NutSort.ExchangeStatePlay";
        private static double timeout,deadline;
        private static int phase;
        private static OriginalGameScene game;
        private static OriginalMainPanelView main;
        private static GameObject background,topCamera;
        static OriginalExchangeStatePlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
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
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Exchange state timeout");
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(startup==null||startup.MainLevel==null||game==null||game.IsRestarting||game.InputBlocked)return;
                    var serialized=new SerializedObject(game);
                    background=(GameObject)serialized.FindProperty("background").objectReferenceValue;
                    topCamera=((Camera)serialized.FindProperty("TopGameCamera").objectReferenceValue).gameObject;
                    Check(background!=null&&background.name=="BG"&&!topCamera.activeSelf,"Serialized source scene references");
                    main=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete"),startup.MainLevel.transform.parent,false).GetComponent<OriginalMainPanelView>();
                    startup.MainLevel.gameObject.SetActive(false);
                    // Explicit main-view data fixture, independent of production request/initialization.
                    var fixture=OriginalMainTopViewValidation.MakeUser();var user=game.User;user.ExchangeCount=2;
                    user.ServerConfigData=fixture.ServerConfigData;user.GoldRewardTargetS2CData=fixture.GoldRewardTargetS2CData;
                    user.UserLssInfo=fixture.UserLssInfo;user.TXTargetGold=fixture.TXTargetGold;
                    OriginalMainPanelValidation.BindFixture(main,user,game.Tables,game.ScheduleDelay,(active,delay)=>main.SetExchangeState(game,active,delay));
                    main.Init();
                    main.Bottom.GetOtherItem(3).Display.Click.onClick.Invoke();
                    Check(game.IsExchanging&&main.ExchangeMask.gameObject.activeSelf&&topCamera.activeSelf&&!background.activeSelf,"Tool Button switches flag and all scene visuals");
                    Check(!game.Level.GetScrew(0).gameObject.activeSelf&&!game.Level.GetScrew(1).gameObject.activeSelf,"Source first-board uniform rods hidden");
                    Check(user.ExchangeCount==2,"Entering exchange does not consume stock");
                    phase=1;deadline=now+.2;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    main.ExchangeMask.onClick.Invoke();
                    Check(game.IsExchanging&&!main.ExchangeMask.gameObject.activeSelf&&!topCamera.activeSelf&&background.activeSelf,"Cancel changes visuals immediately but defers operation flag");
                    Check(game.Level.GetScrew(0).gameObject.activeSelf&&game.Level.GetScrew(1).gameObject.activeSelf,"Cancel restores rods immediately");
                    phase=2;deadline=now+.15;return;
                }
                if(phase==2)
                {
                    Check(game.IsExchanging,"Original half-second cancel delay has not elapsed");
                    main.SetExchangeState(game,true);
                    phase=3;deadline=now+.65;return;
                }
                Check(!game.IsExchanging&&main.ExchangeMask.gameObject.activeSelf,"Earlier delayed cancel survives later entry and only writes operation flag");
                main.SetExchangeState(game,true);
                Check(!game.IsExchanging,"Matching mask activeSelf returns without repairing flag");
                main.SetExchangeState(game,false);
                Check(!main.ExchangeMask.gameObject.activeSelf&&!topCamera.activeSelf&&background.activeSelf,"Explicit immediate exit restores visuals");
                // Explicit mixed-color operation fixture, not a production stage or initialization grant.
                var rod=game.Level.Board.Screws[0];var old=(NutSlot[])rod.Slots.Clone();
                rod.Slots[1].Nut.Color=rod.Slots[0].Nut.Color+1;
                game.IsInitDone=true;
                var items=new OriginalItemManager(game.User,game.SaveUserData,main.Bottom.Refresh,
                    (a,b,c)=>throw new Exception("Unexpected gold"),(a,b)=>throw new Exception("Unexpected coin"));
                game.BindExchange(items,id=>Check(id==10,"Only native failure panel may be requested"));
                main.SetExchangeState(game,true);
                Physics.SyncTransforms();
                var outcome=game.TryOperateAtScreenPoint(game.WorldCamera.WorldToScreenPoint(game.Level.GetScrew(0).Bounds.bounds.center));
                Check(outcome.Kind==ScrewOperationKind.ExchangeRequested&&rod.Slots[0]==old[2],"Actual screen ray dispatches bound scene exchange");
                Check(game.User.ExchangeCount==1&&main.Bottom.GetOtherItem(3).Display.Value.text=="1","Real scene item manager decrements stock and refreshes actual Bottom");
                var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                Check(saved.ExchangeCount==1&&saved.LevelInfo==game.User.LevelInfo&&!string.IsNullOrEmpty(saved.LevelInfo),"Exchange persists reordered board and inventory through existing save envelope");
                Check(game.IsExchanging&&main.ExchangeMask.gameObject.activeSelf,"Original exchange operation does not exit mode");
                Debug.Log("NUT_EXCHANGE_STATE_PLAY_PASS real tool and mask Buttons, native scene references, immediate visual changes, delayed operation flag, retained older callback, activeSelf early return and unchanged stock; actual ray-dispatched exchange, scene inventory/board save and Bottom refresh; default main startup binding pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
