using System;
using System.Collections.Generic;
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
    public static class OriginalSuccessGuideHostPlayValidation
    {
        private const string Key="NutSort.SuccessGuideHostPlay";
        private static double timeout;
        private static float start;
        private static int phase,targets,ads,requests,targetRequests,initializations;
        private static OriginalGuideTargetPanelHost target;
        private static OriginalGuideTargetRequest targetRequest;
        private static Action<JObject> targetResponse;
        private static int withdrawalPanels;
        private static OriginalWithdrawalPanelHost withdrawal;
        private static OriginalWithdrawalPanelValidation.Services withdrawalServices;
        private static OriginalGameScene game;
        private static OriginalSuccessPanelHost success;
        private static OriginalNewbieGuideHost guide;
        private static OriginalCountedMask mask;
        private static bool masked;
        private static Action<object> previousCallback;
        private sealed class Panels:IOriginalGuidePanels
        {
            public bool HasPanel=>success.IsOpen||guide.IsOpen||target.IsOpen||(withdrawal!=null&&withdrawal.IsOpen);
            public OriginalGuideSuccessBinding GetSuccessGuideTarget()=>success.GetSuccessGuideTarget();
            public OriginalGuideButtonBinding GetWithdrawal()=>withdrawal.GetWithdrawalGuideTarget();
            public void ShowPanel(int id){Check(id==7,"Guide panel dispatch");guide.Show();}
            public void ShowTargetPanel(int id,int level){Check(id==35&&level==1&&!guide.IsOpen,"Reopened guide advances to target completion after close");targets++;target.Show(level);}
            public void ShowUnlockPanel(int id,int index,bool banner){throw new InvalidOperationException("Unexpected unlock branch");}
        }
        private sealed class Requests:IOriginalGuideRequests
        {
            public void TargetGoldInfo(bool showMask)
            {
                Check(!showMask&&game.User.GuideIndex==1&&game.User.IsCompleteRecordGuide&&initializations==1&&target.Panel.Closing,"Target request follows initialization and precedes guide increment/save");
                targetRequests++;targetRequest.Run(showMask);
            }
            public void CoinGoldInfo(bool showMask){throw new InvalidOperationException("Unexpected coin request");}
            public void EntryGoldInfo(bool showMask){throw new InvalidOperationException("Unexpected entry request");}
        }
        static OriginalSuccessGuideHostPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Success guide host timeout");
                if(phase==0)
                {
                    game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                    Transform parent=null;foreach(var canvas in UnityEngine.Object.FindObjectsOfType<Canvas>())if(canvas.name=="UICanvas")parent=canvas.transform;
                    Check(parent!=null,"Actual UI canvas");game.User.Level=2;game.User.GuideIndex=77;
                    previousCallback=OriginalNewbieGuideView.CallbackAction;
                    targetRequest=new OriginalGuideTargetRequest(game.User,(showMask,cb)=>{Check(!showMask,"Original unmasked request");targetResponse=cb;},
                        (id,level)=>{Check(id==18&&level==game.User.Level-1,"Live-level withdrawal panel dispatch");withdrawalPanels++;withdrawal.Show(new object[]{level});});
                    var panels=new Panels();mask=new OriginalCountedMask(v=>masked=v,game.ScheduleDelay);
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                    var ui=new OriginalSceneGuideUI(game,()=>null,mask,panels,new Requests());
                    guide=new OriginalNewbieGuideHost(parent,"Prefabs/Panels/NewbieGuidePanel",game.User,game.Tables,"en",()=>true,()=>audio.PlaySound("Click"),v=>game.IsCanOperatorScrew=v,()=>false,game.ScheduleDelay,ui);
                    var factory=new OriginalRewardItemFactory(Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem"),new OriginalGoldFormatter(()=>"en-US"),()=>"US");
                    game.User.ServerConfigData=JObject.Parse(@"{""LSS260820"":true}");
                    game.User.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[{""psi_value"":""5""},{""psi_value"":""10""},{""RealLevel"":5,""Stage2RealLevel"":10}]}");
                    withdrawal=new OriginalWithdrawalPanelHost(parent,"Prefabs/Panels/TXPanel",game.User,game.Tables,"en",v=>new OriginalGoldFormatter(()=>"en-US").Format(v),close=>
                    {
                        Check(withdrawal.IsOpen,"TXPanel registration precedes service binding/init");
                        withdrawalServices=new OriginalWithdrawalPanelValidation.Services{User=game.User,Tables=game.Tables,Allowed=true,CloseAction=close,Delay=game.ScheduleDelay,
                            PanelShown=(id,args)=>{if(id==7)panels.ShowPanel(id);}};
                        return withdrawalServices;
                    });
                    game.User.Level1Gold=5;
                    target=new OriginalGuideTargetPanelHost(parent,"Prefabs/Panels/TXGuideTargetCompletePanel",game.User,game.Tables,"en",()=>true,s=>audio.PlaySound(s),()=>"US",value=>new OriginalGoldFormatter(()=>"en-US").Format(value),
                        (banner,first)=>{Check(banner&&!first&&game.User.IsCompleteRecordGuide&&target.Panel.Closing&&game.User.GuideIndex==1,"Native initialization arguments and pre-callback order");initializations++;},
                        new OriginalPanelActionQueue(),game.ScheduleDelay);
                    success=new OriginalSuccessPanelHost(parent,"Prefabs/Panels/SuccessPanel",factory,game.Tables,"en",
                        ()=>game.CreateSuccessPanelFlow(()=>123,panels.ShowPanel),
                        get=>new OriginalRewardGetFlow(game.User,()=>1,()=>0,cb=>ads++,(cb,b)=>ads++,()=>456,v=>{},get),
                        (more,cb)=>requests++,(id,info)=>throw new InvalidOperationException("No reward panel in early guide"),
                        (flow,baseHide)=>flow.Hide(baseHide,()=>false,()=>false,()=>{},()=>throw new InvalidOperationException("Guide must suppress legacy restart")),
                        ()=>true,s=>audio.PlaySound(s),game.ScheduleDelay,new OriginalPanelActionQueue());
                    success.Show(new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=0,Count=5}}});
                    Check(!guide.IsOpen&&success.IsOpen,"Guide waits for actual opening tween");start=Time.time;phase=1;return;
                }
                if(phase==1)
                {
                    if(Time.time-start<.6f)return;
                    Check(guide.IsOpen&&game.User.GuideIndex==0&&success.IsOpen,"Actual SuccessPanel opening resets guide index and creates guide host");
                    var binding=success.GetSuccessGuideTarget();
                    Check(binding.MoreGetButton==success.Panel.MoreButton&&guide.Panel.ContinueButton.image.rectTransform.sizeDelta==binding.MoreGetButton.image.rectTransform.sizeDelta,"Guide uses actual SuccessPanel button size");
                    guide.Panel.ContinueButton.onClick.Invoke();
                    Check(!guide.IsOpen&&success.IsOpen&&game.User.GuideIndex==1&&masked&&ads==0&&requests==0,"Guide invokes early GetCallback, closes guide, acquires mask and leaves success closing without ads");
                    start=Time.time;phase=2;return;
                }
                if(phase==2)
                {
                    if(Time.time-start<.35f)return;
                    Check(!success.IsOpen&&!guide.IsOpen&&targets==0&&masked,"Success closes before delayed guide reopen");phase=3;return;
                }
                if(phase==3)
                {
                    if(Time.time-start<.8f)return;
                    Check(targets==1&&!guide.IsOpen&&masked&&OriginalNewbieGuideView.CallbackAction!=null,"Delayed reopen enters case one and installs target completion callback");phase=4;return;
                }
                if(Time.time-start<1.7f)return;
                if(phase==4)
                {
                    Check(!masked&&ads==0&&requests==0&&target.IsOpen&&target.Panel.Tip.text=="$5","Actual target-complete panel displays saved first-stage gold after mask release");
                    target.Panel.ContinueButton.onClick.Invoke();
                    Check(targetRequests==1&&initializations==1&&game.User.GuideIndex==2&&OriginalNewbieGuideView.CallbackAction==null,"Actual target button invokes and clears captured guide completion");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.GuideIndex==2&&saved.IsCompleteRecordGuide,"Actual scene save records callback progression");start=Time.time;phase=5;return;
                }
                if(phase==5)
                {
                    Check(!target.IsOpen&&targetRequests==1&&ads==0&&requests==0&&withdrawalPanels==0&&targetResponse!=null,"Target panel closes while actual target response remains pending");
                    targetResponse(null);Check(withdrawalPanels==1&&withdrawal.IsOpen&&withdrawal.Panel.ProgressLabel.text=="1/1","Response opens real registered TXPanel and refreshes actual fields");
                    Check(!guide.IsOpen,"Withdrawal guide waits for panel opening");start=Time.time;phase=6;return;
                }
                if(phase==6)
                {
                    Check(guide.IsOpen&&game.User.GuideIndex==2&&withdrawal.IsOpen,"Real TXPanel opening enters actual withdrawal guide");
                    var binding=withdrawal.GetWithdrawalGuideTarget();
                    Check(binding.Button==withdrawal.Panel.WithdrawalButton&&guide.Panel.ContinueButton.image.rectTransform.sizeDelta==binding.Button.image.rectTransform.sizeDelta,"Guide uses real withdrawal control dimensions");
                    guide.Hide();OriginalNewbieGuideView.CallbackAction=null;
                    int before=withdrawal.Panel.PlayerParent.childCount;withdrawal.Refresh();Check(withdrawal.Panel.PlayerParent.childCount==before+1,"Host refresh calls actual panel and appends PlayerInfo");
                    var first=withdrawal.Panel;Check(withdrawal.Show(Array.Empty<object>())==null&&ReferenceEquals(first,withdrawal.Panel),"Duplicate registration keeps original panel");
                    withdrawalServices.OnlineTimeHint=true;withdrawal.Panel.CloseButton.onClick.Invoke();
                    Check(withdrawalServices.Panels.Contains(30)&&!withdrawalServices.OnlineTimeHint&&withdrawal.IsOpen,"Native close hint routes before animated registry removal");
                    start=Time.time;phase=7;return;
                }
                Check(!withdrawal.IsOpen&&withdrawalServices.Closes==1&&withdrawalServices.HiddenCount==1,"Animated close hides and removes real registry entry");
                game.User.GuideIndex=77;withdrawal.Show(new object[]{1});Check(withdrawal.IsOpen&&withdrawal.Panel.PlayerParent.childCount==1,"Reopen creates a fresh original panel");withdrawal.Hide();
                Debug.Log("NUT_SUCCESS_GUIDE_HOST_PLAY_PASS real SuccessPanel -> guide -> target completion -> saved progression -> held target response -> registered TXPanel -> native opening -> actual withdrawal guide target; real refresh/duplicate/animated close/reopen ownership; initialization and transport/services remain explicit fixtures.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalNewbieGuideView.CallbackAction=previousCallback;OriginalWithdrawalPanel.IsPlayGoldTween=false;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
