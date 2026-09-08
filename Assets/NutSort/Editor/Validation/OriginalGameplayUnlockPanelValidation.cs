using System;
using System.IO;
using Newtonsoft.Json.Linq;
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
    public static class OriginalGameplayUnlockPanelValidation
    {
        private const string PathName="prefabs/panels/UnlockGameplayPanel",Key="NutSort.UnlockPanelPlay";
        private static OriginalGameScene game;
        private static OriginalGameplayUnlockPanel panel;
        private static OriginalGameplayUnlockPanelHost host;
        private static OriginalUserLocalData user;
        private static OriginalTargetRewardBanner rewardBanner;
        private static int goldHints,progressDisplays,readyCallbacks,initializations;
        private static bool earlyReady;
        private static string stored;
        private static int phase,index,banners,queued,hidden;
        private static double timeout,deadline;
        private static bool gate;
        static OriginalGameplayUnlockPanelValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>(PathName);Check(prefab!=null,"Unlock prefab exists");
            Check(prefab.GetComponentsInChildren<RectTransform>(true).Length==12,"Original twelve-object hierarchy");
            foreach(var t in prefab.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
            var view=prefab.GetComponent<OriginalGameplayUnlockPanel>();
            Check(view.Icons.Length==4 && view.ContinueButton!=null && view.Tip.font!=null,"Four original icons, Button and font");
            foreach(var icon in view.Icons)Check(icon.GetComponent<UnityEngine.UI.Image>().sprite!=null,"Original icon sprite resolved");
            Check(prefab.GetComponentInChildren<OriginalLoopRotation>(true)!=null,"Original glow animation restored");
            Check(new SerializedObject(view).FindProperty("queueDelay").floatValue==2.5f,"Original base Hide queue delay");
            Debug.Log("NUT_GAMEPLAY_UNLOCK_PANEL_VALIDATION_PASS full hierarchy, references, four original sprites, standard Button, font and glow component.");
        }
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Unlock panel validation");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Unlock play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || game.InputBlocked)return;
                if(phase==0)
                {
                    Validate();var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();

                    user=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;user.NewGameplayUnlockIndex=index;
                    if(index==0)
                    {
                        bool available=false;float seen=-1f;
                        Func<UnityEngine.Object> mainProvider=()=>
                        {
                            if(!available)return null;
                            if(seen<0f)seen=Time.time;
                            return startup.MainLevel;
                        };
                        Action ready=()=>{if(seen<0f || Time.time-seen<1.49f)earlyReady=true;readyCallbacks++;};
                        game.ScheduleAfterMainPanel(mainProvider,ready);
                        game.ScheduleAfterMainPanel(mainProvider,ready);
                        Check(readyCallbacks==0,"Main-panel wait is not synchronous");
                        game.ScheduleDelay(.2f,()=>available=true);
                        rewardBanner=startup.TargetReward;
                        stored=PlayerPrefs.GetString(OriginalUserStore.Key);
                        var formatter=new OriginalGoldFormatter(()=>"en-US");
                        rewardBanner.Bind(user,game.Tables,"en",value=>formatter.Format(value),
                            ()=>rewardBanner.transform.parent.TransformPoint(new Vector3(-250,760,0)),
                            ()=>goldHints++,()=>progressDisplays++,()=>{throw new InvalidOperationException("First-level fixture must not save");});
                    }
                    gate=false;game.ModalInputBlocked=true;
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                    var queue=new OriginalPanelActionQueue();queue.Add(()=>queued++);

                    host=new OriginalGameplayUnlockPanelHost(startup.MainLevel.transform.parent,PathName,user,game.Tables,"en",()=>gate,s=>audio.PlaySound(s),callback=>{Check(callback==null && user.NewGameplayUnlockIndex==index,"Banner called before index mutation without completion callback");banners++;rewardBanner.Show(callback);Check(rewardBanner.IsAnimating && rewardBanner.Tip.text.Length>0,"Actual target banner refreshes and starts before index mutation");},queue,game.ScheduleDelay,()=>{hidden++;game.ModalInputBlocked=false;});
                    // Fixture configuration selects each icon without fabricating
                    // server rewards for the first-level banner consumer.
                    var levels=new JArray(900,901,902,903);levels[index]=user.Level+4;
                    user.ServerConfigData=new JObject(new JProperty("LSSGPUL",levels));
                    user.NewGameplayUnlockIndex=index+1;
                    Check(!host.TryShow(game,true) && !host.IsOpen && host.Panel==null,"No eligible scene decision creates no panel");
                    user.NewGameplayUnlockIndex=index;
                    // Explicit branch fixture only; no production reward data or grants.
                    user.ComeOnGold="";user.IsCompleteRecordGuide=true;
                    user.IsCompleteGuidePassStage2Level=true;user.IsGuideGold=false;
                    user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_list\":[null,null,{\"RealLevel\":10,\"Stage2RealLevel\":20}]}");
                    var progress=new OriginalRewardProgress(user,game.Tables,value=>value.ToString());
                    new OriginalSceneInitialization(game,progress,new InitializationUI()).Bind(()=>startup.MainLevel);
                    game.RestartLevel();
                    Check(!host.IsOpen && initializations==index,"Restart does not synchronously complete initialization");
                    Check(!game.IsInitDone && user.PassLevelTime==0,"Actual restart resets and pauses the native elapsed clock");
                    phase=5;deadline=now+3.5;return;
                }
                if(phase==5)
                {
                    if(!host.IsOpen)
                    {
                        Check(now<deadline,"Actual restart continuation reaches unlock panel");
                        Check(!game.IsInitDone && user.PassLevelTime==0,"Board readiness alone cannot advance the clock while awaiting initialization");
                        return;
                    }
                    Check(initializations==index+1,"One continuation per actual restart");
                    panel=host.Panel;
                    for(int i=0;i<4;i++)Check(panel.Icons[i].activeSelf==(i==index),"Actual icon visibility");
                    phase=1;deadline=now+.7;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(game.IsInitDone && user.PassLevelTime>.1f,"Actual Update accumulates scaled time after initialization, including while the unlock modal is open");
                    string path=System.IO.Path.GetFullPath("Library/ValidationCaptures/unlock-"+index+".png");Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);
                    phase=2;deadline=now+.3;return;
                }
                if(phase==2)
                {
                    panel.ContinueButton.onClick.Invoke();Check(!panel.Closing && user.NewGameplayUnlockIndex==index,"Gate denies actual Continue Button");
                    gate=true;panel.ContinueButton.onClick.Invoke();Check(panel.Closing && user.NewGameplayUnlockIndex==index+1,"Continue updates index before delayed close");
                    panel.gameObject.SetActive(false);phase=3;deadline=now+.4;return;
                }
                Check(panel==null && !host.IsOpen && !game.ModalInputBlocked,"Runtime host deregisters and destroys hidden panel after close");
                if(phase!=4)
                {
                    if(++index<4){phase=0;return;}
                    Check(hidden==4,"Hide callback runs for each closed variant");
                    phase=4;deadline=now+2.6;return;
                }
                Check(queued==4,"Every scene-owned delay dispatches after its panel is destroyed");
                Check(readyCallbacks==2 && !earlyReady,"Both independent scene waits respect panel appearance and scaled delay");
                Check(banners==4 && goldHints==4 && progressDisplays==4 && !rewardBanner.IsAnimating,"All four real banner animations finish and invoke both top callbacks");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==stored,"Unlock and first-level banner add no save");
                Debug.Log("NUT_GAMEPLAY_UNLOCK_PANEL_PLAY_PASS actual restart-to-main-panel-wait-to-continuation-to-unlock routing, four current rendered variants, actual Continue Button gate/banner/index/close ordering, hidden close and post-destruction queue dispatch; actual target banner completes to both top callbacks; actual initialization event owns completion state; other UI branches, reward document, destination and top consumers remain fixture boundaries.");Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private sealed class InitializationUI : IOriginalInitializationUI
        {
            public void ShowUnlockPanel(int id,int selected,bool showBanner)
            {
                Check(id==13 && selected==index && showBanner,"Native initialization event routes selected unlock and banner flag");
                Check(!game.IsInitDone,"Unlock is shown before the initialization event marks completion");
                Check(host.Show(selected,showBanner)!=null,"Actual initialization event opens through runtime host");
                initializations++;
            }
            private static void Unexpected(){throw new InvalidOperationException("Unexpected initialization branch in unlock fixture");}
            public void SynchronizeCompletedStage(Action completed){Unexpected();}
            public void ShowPanel(int id){Unexpected();}
            public void CloseAllPanels(){Unexpected();}
            public void ShowTargetRewardBanner(Action completed){Unexpected();}
            public void HideLevelHint(){Unexpected();}
            public void ShowEveryDayGift(){Unexpected();}
            public void PushPlayerGoldHint(){Unexpected();}
            public void CloseRecordGuide(){Unexpected();}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
