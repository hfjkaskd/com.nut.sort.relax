using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalGoldItemValidation
    {
        private const string Key="NutSort.GoldItemValidation";
        private static OriginalGoldItem gold;
        private static OriginalGameScene game;
        private static OriginalUserLocalData user;
        private static Action response;
        private static int phase,requests,opened,sounds,images;
        private static double timeout,deadline;
        private static Vector3 pausedScale;
        private static string before;
        static OriginalGoldItemValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+60; EditorApplication.update+=Tick; }
        }
        private static OriginalUserLocalData Fixture()
        {
            var data=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            data.Gold=12.5f; data.TXTargetGold="100";
            data.GoldRewardTargetS2CData=JObject.Parse("{\"cal_cfg\":5,\"bear_list\":[null,null,{\"RealLevel\":5,\"Stage2RealLevel\":10,\"Stage2StartShowLevel\":4,\"caliper_logs\":3,\"caliper_rank\":20}]}");
            return data;
        }
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/GoldItem"));
            try
            {
                var v=instance.GetComponent<OriginalGoldItem>(); var data=Fixture();
                Check(v!=null && instance.GetComponentsInChildren<Transform>(true).Length==8,"Original GoldItem hierarchy");
                foreach(var t in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
                Check(instance.GetComponentsInChildren<Button>(true).Length==1 && instance.GetComponentsInChildren<OriginalButtonFeedback>(true).Length==0,"One standard Button without the source-disabled press tween");
                Check(instance.GetComponentsInChildren<OriginalLoopPosition>(true).Length==1,"Source hint bobbing");
                var trace=new List<string>(); bool gate=true; Action complete=null; string country="US";
                var formatter=new OriginalGoldFormatter(()=>"en-US");
                v.Bind(data,tables,"en",x=>formatter.Format(x),()=>country,()=>gate,
                    done=>{trace.Add("request");complete=done;},id=>trace.Add("panel:"+id),()=>trace.Add("sound"),()=>trace.Add("top"));
                v.Value.text="untouched"; v.Init();
                Check(v.Value.text=="untouched" && !v.GoldHintText.transform.parent.gameObject.activeSelf && string.Join(",",trace)=="top","Init binds/hides/refreshes hint but does not refresh amount");
                Check(v.GoldHintText.text=="<color=#FF0000>Clear this level</color> Withdraw all","Gold hint keeps source red text and removes only newline");
                trace.Clear(); v.Refresh(); Check(v.Value.text=="$12.50" && trace.Count==0 && v.PulseCount==0,"Normal refresh only updates amount");
                data.Level=10; v.ShowGoldHintText(); Check(v.GoldHintText.transform.parent.gameObject.activeSelf,"Inclusive hint boundary shows");
                data.Level=11; v.ShowGoldHintText(); Check(!v.GoldHintText.transform.parent.gameObject.activeSelf,"Post-boundary show hides");
                data.Level=1; v.RefreshHint(); Check(!v.GoldHintText.transform.parent.gameObject.activeSelf,"Hint refresh does not show early-level bubble");
                v.ShowGoldHintText(); Check(v.GoldHintText.transform.parent.gameObject.activeSelf,"Explicit show reveals early bubble");
                trace.Clear(); gate=false; v.Click.onClick.Invoke(); Check(trace.Count==0,"Blocked clicks do not request or play sound");
                gate=true; v.Click.onClick.Invoke(); Check(string.Join(",",trace)=="request,sound","Request occurs before sound; panel waits for response");
                complete(); Check(string.Join(",",trace)=="request,sound,panel:18","Original response panel ID");
                v.Init(); trace.Clear(); v.Click.onClick.Invoke(); Check(string.Join(",",trace)=="request,sound","Re-init replaces listeners");
                var currency=instance.GetComponentInChildren<OriginalGoldImage>(true);
                foreach(string code in new[]{"US","CA","FR","JP","BR","AR","MX","RU","ID","GB"})
                {
                    country=code; currency.Apply();
                    var sprite=Resources.Load<Sprite>("Atlas/Golds/"+OriginalRecordGuidePanel.GoldCode(code)+"1");
                    Check(v.Icon.sprite==sprite && sprite!=null,"Original country sprite alias: "+code);
                }
                country="US";currency.Apply();
                v.Bind(data,tables,"ja",x=>formatter.Format(x),()=>country,()=>gate,done=>complete=done,id=>{},()=>{},()=>trace.Add("top"));
                foreach(var label in instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
                    if(label.name=="label_185")Check(label.text==tables.Text.GetText(185,"ja"),"Source panel label pass localizes Draw");
                v.Bind(data,tables,"en",x=>formatter.Format(x),()=>country,()=>gate,done=>complete=done,id=>{},()=>{},()=>trace.Add("top"));
                trace.Clear(); v.RefreshGold(true,2.5f); Check(string.Join(",",trace)=="top" && v.PulseCount==1 && v.FloatingPairCount==0,"Animated refresh updates hints immediately but delays the gain copy");
                v.Advance(.6f); Check(v.FloatingPairCount==0,"Gain clone waits through delay equality");
                v.Advance(.03f); Check(v.FloatingPairCount==1,"Exactly one gain pair on first play");
                Check(Mathf.Abs(v.transform.localScale.x-(1+.15f*Mathf.Sin(Mathf.PI*.25f)))<.0001f,"Original OutSine pulse");
                var labels=instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
                bool found=false; foreach(var label in labels)if(label!=v.Value && label.text=="+$2.50") { found=true; Check(label.transform.localScale==Vector3.one*.7f,"Float scale"); }
                Check(found,"Captured added gold text");
                v.Advance(.7f); Check(v.PulseCount==0 && v.FloatingPairCount==1 && v.transform.localScale==Vector3.one,"Twelve Yoyo loops reset the owner before the two-second float ends");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("NUT_GOLD_ITEM_VALIDATION_PASS source prefab/Button/currency sprites, hint boundary and text, refresh scope, deferred panel request, delayed OutSine pulse and floating gain; SDK request and production host excluded.");
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin(); EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,854,"Nut Sort gold item validation");
                SessionState.SetBool(Key,true); EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup; Check(now<timeout,"GoldItem Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    var progress=main.GetComponentInChildren<OriginalRewardProgressView>(true);
                    gold=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/GoldItem"),progress.transform.parent,false).GetComponent<OriginalGoldItem>();
                    user=Fixture(); var formatter=new OriginalGoldFormatter(()=>"en-US");
                    progress.Bind(user,game.Tables,"en",x=>formatter.Format(x));
                    gold.Bind(user,game.Tables,"en",x=>formatter.Format(x),()=>"US",()=>true,
                        done=>{requests++;response=done;},id=>{Check(id==18,"Source panel ID");opened++;},()=>sounds++,
                        ()=>{progress.Refresh();main.Refresh(game.Tables,user.Level,"en",false,progress.IsShow);});
                    gold.Init();gold.Refresh(); images=gold.GetComponentsInChildren<Image>(true).Length;
                    before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;
                    var banner=main.GetComponentInChildren<OriginalTargetRewardBanner>();
                    banner.Bind(user,game.Tables,"en",x=>formatter.Format(x),()=>gold.transform.position,gold.ShowGoldHintText,progress.Show,()=>{throw new InvalidDataException("No first-level save");});
                    banner.Show();phase=1;deadline=now+2.1;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(gold.GoldHintText.transform.parent.gameObject.activeSelf,"Real banner completion shows real GoldItem hint");
                    Check(gold.Icon.sprite==Resources.Load<Sprite>("Atlas/Golds/US1"),"Country icon applied in Start");
                    Capture("gold-item");
                    phase=7;deadline=now+.15;return;
                }
                if(phase==7)
                {
                    Canvas.ForceUpdateCanvases();var button=gold.Click;var canvas=button.GetComponentInParent<Canvas>();
                    var data=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,button.transform.position),button=PointerEventData.InputButton.Left};
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);
                    Check(hits.Count>0 && hits[0].gameObject.GetComponentInParent<Button>()==button,"Native gold Button raycast");
                    ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);
                    Check(requests==1 && sounds==1 && opened==0,"Native Button waits for request result");response();Check(opened==1,"Deferred local callback opens original panel route");
                    user.Gold=15;gold.RefreshGold(true,2.5f);phase=2;deadline=now+.8;return;
                }
                if(phase==2)
                {
                    Check(gold.Value.text=="$15" && gold.FloatingPairCount==1 && gold.PulseCount==1,"Actual delayed reward presentation");
                    Capture("gold-item-gain");Time.timeScale=0;phase=3;deadline=now+.1;return;
                }
                if(phase==3){pausedScale=gold.transform.localScale;phase=4;deadline=now+.3;return;}
                if(phase==4)
                {
                    Check(gold.transform.localScale==pausedScale && gold.FloatingPairCount==1,"Scaled reward animation pauses");
                    Time.timeScale=1;phase=5;deadline=now+2.2;return;
                }
                Check(gold.PulseCount==0 && gold.FloatingPairCount==0 && gold.transform.localScale==Vector3.one,"Native animation cleanup");
                Check(gold.GetComponentsInChildren<Image>(true).Length==images,"Floating icon destroyed after fade");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"Gold view does not grant or save currency");
                game.ModalInputBlocked=false;
                Debug.Log("NUT_GOLD_ITEM_PLAY_VALIDATION_PASS actual banner destination/hint/progress chain, country sprite Start, native Button request boundary, delayed gain copies/pulse, pause and fade cleanup; production host and SDK service remain excluded.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Capture(string name)
        {
            string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures/"+name+".png"));
            Directory.CreateDirectory(Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);Debug.Log("NUT_GOLD_ITEM_CAPTURE "+path);
        }
        private static void Check(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
        private static void Finish(int code){Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
