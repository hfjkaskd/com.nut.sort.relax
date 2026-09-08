using System;
using System.Collections.Generic;
using System.IO;
using NutSort.Content;
using Newtonsoft.Json.Linq;
using NutSort.UI;
using NutSort.World;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalRecordGuideValidation
    {
        private const string Key="NutSort.RecordGuideValidation";
        private const string PathName="Prefabs/Panels/TXRecordGuidePanel";
        private static OriginalRecordGuidePanel panel;
        private static OriginalGameScene game;
        private static int phase;
        private static double timeout,deadline;
        private static bool started,closed;
        private static OriginalRecordGuidePanelHost host;
        private static OriginalSceneInitialization initialization;
        private static int reentered,queued;
        private static string before;
        static OriginalRecordGuideValidation()
        {
            if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}
        }
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>(PathName);
            Check(prefab!=null,"Original record guide prefab loads");
            foreach(var transform in prefab.GetComponentsInChildren<Transform>(true))
                Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject)==0,"Record hierarchy has no missing scripts: "+transform.name);
            Check(prefab.GetComponentsInChildren<Button>(true).Length==1,"Single standard Start Button");
            Check(prefab.GetComponentsInChildren<ParticleSystem>(true).Length==2,"Both original particle systems preserved");
            Check(prefab.GetComponentsInChildren<OriginalRecordPunchRotation>(true).Length==5,"Five source Get-marker rotation tracks");
            foreach(var image in prefab.GetComponentsInChildren<Image>(true))
                Check(image.sprite!=null || image.gameObject==prefab,"Original sprite reference: "+image.name);
            foreach(var label in prefab.GetComponentsInChildren<TMP_Text>(true))Check(label.font!=null,"Original font: "+label.name);
            var settings=Resources.Load<OriginalRecordGuideSettings>("Configuration/OriginalRecordGuide");
            Check(settings.StartDelay==2 && settings.StartDuration==.5f,"Original delayed Start appearance");
            Check(OriginalRecordGuidePanel.GoldCode("FR")=="DE" && OriginalRecordGuidePanel.GoldCode("CA")=="US" &&
                OriginalRecordGuidePanel.GoldCode("AU")=="US" && OriginalRecordGuidePanel.GoldCode("JP")=="JP" && OriginalRecordGuidePanel.GoldCode("fr")=="fr",
                "Original exact country aliases without case normalization");
            foreach(string code in new[]{"AR","BR","DE","GB","ID","JP","MX","RU","US"})
                Check(Resources.Load<Texture2D>(settings.GoldTexturePrefix+code+"1")!=null,"Original currency texture: "+code);
            var motion=prefab.GetComponentInChildren<OriginalRecordPunchRotation>(true);
            var serialized=new SerializedObject(motion);var curve=serialized.FindProperty("rotation").animationCurveValue;
            Check(curve.length==11 && Mathf.Abs(curve.Evaluate(1f/55f)-10)<.0001f && Mathf.Abs(curve.Evaluate(3f/55f)+9)<.0001f,
                "Native Punch segment endpoint timing and alternating amplitudes");
            Check(Mathf.Abs(curve.Evaluate(.5f/55f)-7.5f)<.0001f && Mathf.Abs(curve.Evaluate(1))<.0001f,"Per-segment OutQuad and terminal zero");
            Debug.Log("NUT_RECORD_GUIDE_VALIDATION_PASS original prefab references, Button, fonts, particle systems, five native Punch tracks, country textures and delayed entry configuration; production lifecycle binding pending.");
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,854,"Nut Sort record guide validation");
                SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Record panel validation timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    Validate();
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    game.ModalInputBlocked=true;before=PlayerPrefs.GetString(OriginalUserStore.Key);
                    var user=game.User;user.IsCompleteRecordGuide=false;user.ComeOnGold="";
                    user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_list\":[null,null,{\"Stage2RealLevel\":20}]}");
                    var progress=new OriginalRewardProgress(user,game.Tables,value=>value.ToString());
                    initialization=new OriginalSceneInitialization(game,progress,new InitializationUI());
                    var queue=new OriginalPanelActionQueue();queue.Add(()=>queued++);
                    host=new OriginalRecordGuidePanelHost(startup.MainLevel.transform.parent,PathName,game.Tables,"en","US",()=>game.IsInitDone,
                        ()=>{started=true;initialization.CompleteRecordGuide();},()=>{},queue,game.ScheduleDelay,
                        ()=>{closed=true;game.ModalInputBlocked=false;});
                    initialization.Bind(()=>startup.MainLevel);
                    game.RestartLevel();phase=5;deadline=now+4;return;
                }
                if(phase==5)
                {
                    if(!host.IsOpen){Check(now<deadline,"Initialization reaches record guide");return;}
                    panel=host.Panel;
                    Check(game.IsInitDone && !game.User.IsCompleteRecordGuide,"Record branch enables gate before guide completion");
                    Check(panel.StartContainer.localScale==Vector3.zero,"Start parent is initially zero scale");
                    Check(panel.CurrencyParticles.sharedMaterial.GetTexture("_MainTex")==Resources.Load<Texture2D>("Atlas/Golds/US1"),"Current US texture applied to actual particle material");
                    panel.enabled=false;
                    phase=1;deadline=now+.8;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(panel.Main.localScale==Vector3.one && panel.StartContainer.localScale.sqrMagnitude<.000001f,"Panel opened while Start still hidden");
                    panel.gameObject.SetActive(false);
                    phase=2;deadline=now+2;return;
                }
                if(phase==2)
                {
                    Check(Vector3.Distance(panel.StartContainer.localScale,Vector3.one)<.00001f,"Start OutBack reaches one after its delay");
                    panel.gameObject.SetActive(true);panel.enabled=true;
                    string path=System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath,"../Library/ValidationCaptures/record-guide.png"));
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);Debug.Log("NUT_RECORD_GUIDE_CAPTURE "+path);
                    phase=3;deadline=now+.2;return;
                }
                if(phase==3)
                {
                    game.User.Level=2; // Explicit next-branch fixture; no reward grant.
                    Canvas.ForceUpdateCanvases();var button=panel.StartButton;var canvas=button.GetComponentInParent<Canvas>();
                    var data=new PointerEventData(EventSystem.current) {position=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,button.transform.position),button=PointerEventData.InputButton.Left};
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);
                    Check(hits.Count>0 && hits[0].gameObject.GetComponentInParent<Button>()==button,"Native UI raycast hits Start");
                    ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerDownHandler);
                    ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);
                    Check(started && panel.Closing && !closed,"Button runs completion callback before delayed close finishes");
                    Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"Record callback adds no save point");
                    panel.gameObject.SetActive(false);
                    phase=4;deadline=now+.4;return;
                }
                Check(closed && panel==null && !host.IsOpen && !game.ModalInputBlocked,"Native close deregisters panel and releases validation host");
                if(phase==4){phase=6;deadline=now+2.6;return;}
                Check(queued==1 && reentered==1 && game.User.IsCompleteRecordGuide,"Record completion re-enters initialization immediately and later advances the queue");
                Debug.Log("NUT_RECORD_GUIDE_PLAY_VALIDATION_PASS real canvas render, disabled component entry, hidden delayed Start and close, country particle texture, native Button raycast/dispatch, close callback order and no added save; full startup lifecycle still pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private sealed class InitializationUI : IOriginalInitializationUI
        {
            public void ShowPanel(int id)
            {
                if(id==34){Check(game.IsInitDone,"Record branch sets completion before panel creation");host.Show();return;}
                Check(id==7 && game.User.GuideIndex==1 && panel.Closing && game.User.IsCompleteRecordGuide,"Start re-enters level-two guide after close starts and completion is written");reentered++;
            }
            public void CloseRecordGuide()
            {
                Check(!game.User.IsCompleteRecordGuide && panel.Closing,"Close precedes completion write");
                host.Close();
            }
            private static void Unexpected(){throw new InvalidDataException("Unexpected record fixture branch");}
            public void SynchronizeCompletedStage(Action completed){Unexpected();}
            public void ShowUnlockPanel(int id,int index,bool banner){Unexpected();}
            public void CloseAllPanels(){Unexpected();}
            public void ShowTargetRewardBanner(Action completed){Unexpected();}
            public void HideLevelHint(){Unexpected();}
            public void ShowEveryDayGift(){Unexpected();}
            public void PushPlayerGoldHint(){Unexpected();}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidDataException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
