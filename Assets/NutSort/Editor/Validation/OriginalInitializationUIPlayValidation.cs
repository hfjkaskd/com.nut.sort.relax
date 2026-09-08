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
    public static class OriginalInitializationUIPlayValidation
    {
        private const string Key="NutSort.InitializationUIPlay";
        private static double timeout,deadline;
        private static int phase;
        private static OriginalGameScene game;
        private static OriginalMainPanelView main;
        private static OriginalRecordGuidePanelHost record;
        private static OriginalNewbieGuideHost guide;
        private sealed class Panels:IOriginalInitializationPanels
        {
            public bool HasPanel=>record.IsOpen||guide.IsOpen;
            public void ShowPanel(int id){Check(id==7,"Expected teaching after banner");guide.Show();}
            public void ShowUnlockPanel(int id,int index,bool banner)=>throw new InvalidOperationException("Unexpected unlock in first-level fixture");
            public void CloseAll(){if(record.IsOpen)record.Hide();if(guide.IsOpen)guide.Hide();}
        }
        static OriginalInitializationUIPlayValidation()
        {if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Initialization UI chain");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Initialization UI timeout");
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(startup==null||startup.MainLevel==null||game==null||game.IsRestarting||game.InputBlocked)return;
                    // Explicit saved/server-data fixture. No production request response or grant is synthesized.
                    var fixture=OriginalMainTopViewValidation.MakeUser();var user=game.User;
                    user.Level=1;user.LevelSeed=0;user.IsCompleteRecordGuide=false;user.IsGuideGold=false;user.ComeOnGold=null;
                    user.GoldRewardTargetS2CData=fixture.GoldRewardTargetS2CData;user.ServerConfigData=fixture.ServerConfigData;
                    user.UserLssInfo=fixture.UserLssInfo;user.TXTargetGold=fixture.TXTargetGold;
                    var parent=startup.MainLevel.transform.parent;startup.MainLevel.gameObject.SetActive(false);
                    main=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete"),parent,false).GetComponent<OriginalMainPanelView>();
                    OriginalMainPanelValidation.BindFixture(main,user,game.Tables,game.ScheduleDelay,(a,d)=>{});
                    var formatter=new OriginalGoldFormatter(()=>"en-US");
                    main.TargetReward.Bind(user,game.Tables,"en",v=>formatter.Format(v),()=>main.Top.Gold.transform.position,
                        main.Top.Gold.ShowGoldHintText,main.Top.Progress.Show,game.SaveUserData);
                    main.Init();main.Refresh();
                    var ui=new OriginalInitializationUI(game,()=>main,()=>record,new Panels(),done=>throw new InvalidOperationException("Unexpected synchronization for first level"));
                    var initialization=new OriginalSceneInitialization(game,new OriginalRewardProgress(user,game.Tables,v=>formatter.Format(v)),ui);
                    record=new OriginalRecordGuidePanelHost(parent,"prefabs/panels/TXRecordGuidePanel",game.Tables,"en","US",()=>game.IsInitDone,
                        initialization.CompleteRecordGuide,()=>{},new OriginalPanelActionQueue(),game.ScheduleDelay,null);
                    guide=new OriginalNewbieGuideHost(parent,"prefabs/panels/NewbieGuidePanel",user,game.Tables,"en",()=>game.IsInitDone,()=>{},
                        value=>game.IsCanOperatorScrew=value,()=>false,game.ScheduleDelay,new OriginalSceneGuideUI(game,()=>main,null,null,null));
                    initialization.Bind(()=>main);game.InitLevel(true,true,true);
                    Check(!game.IsInitDone&&!record.IsOpen,"No synthetic ready state before initialization wait");
                    phase=1;return;
                }
                game.ModalInputBlocked=record.IsOpen||guide.IsOpen;
                if(phase==1)
                {
                    if(!record.IsOpen)return;
                    Check(game.IsInitDone&&!game.User.IsCompleteRecordGuide,"Native initialization sets ready before record guide");
                    phase=2;deadline=now+1;return;
                }
                if(now<deadline)return;
                if(phase==2)
                {
                    Check(Operate().Kind==ScrewOperationKind.Ignored,"Record modal blocks ready board");
                    record.Panel.StartButton.onClick.Invoke();
                    Check(record.Panel.Closing&&game.User.IsCompleteRecordGuide&&main.TargetReward.IsAnimating,"Actual start Button closes record and reenters banner branch");
                    phase=3;return;
                }
                if(!guide.IsOpen)return;
                Check(!record.IsOpen&&game.IsInitDone&&game.IsCanOperatorScrew,"Real banner callback opens teaching after record close");
                Check(guide.Panel.Tip.text==game.Tables.Text.GetText(70,"en"),"First native teaching tip");
                Check(Operate().Kind==ScrewOperationKind.Ready,"Teaching override permits actual screen ray without manual initialization readiness");
                // CompleteRecordGuide reenters with firstInit=false, so the first-init hint push is intentionally absent.
                Check(main.Top.PlayerHint.LastShowTime==-1,"Record return retains native firstInit=false");
                Debug.Log("NUT_INITIALIZATION_UI_PLAY_PASS real initialization wait, record prefab Button, record completion, target banner animation callback, teaching prefab and camera-ray operation without manually setting IsInitDone; explicit data and panel-dispatch fixture, production startup wiring pending.");
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static ScrewOperation Operate()
        {
            Physics.SyncTransforms();return game.TryOperateAtScreenPoint(game.WorldCamera.WorldToScreenPoint(game.Level.GetScrew(1).Bounds.bounds.center));
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
