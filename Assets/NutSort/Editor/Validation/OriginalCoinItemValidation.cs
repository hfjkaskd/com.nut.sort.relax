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
    public static class OriginalCoinItemValidation
    {
        private const string Key="NutSort.CoinItemValidation";
        private static OriginalCoinItem coin;
        private static OriginalGameScene game;
        private static OriginalUserLocalData user;
        private static Action response;
        private static int phase,requests,opened,sounds,images;
        private static double timeout,deadline;
        private static Vector3 pausedScale;
        private static string before;
        static OriginalCoinItemValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+60; EditorApplication.update+=Tick; }
        }
        private static OriginalUserLocalData Fixture()
        {
            var data=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            data.Coin=125; data.Level=5;
            data.GoldRewardTargetS2CData=JObject.Parse("{\"cal_cfg\":5,\"bear_rates\":\"100\",\"bear_zs_list\":[{\"caliper_psi\":\"1000\",\"psi_value\":\"10\",\"caliper_logs\":3,\"caliper_rank\":20}]}");
            return data;
        }
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/CoinItem"));
            try
            {
                var v=instance.GetComponent<OriginalCoinItem>(); var data=Fixture();
                Check(v!=null && instance.GetComponentsInChildren<Transform>(true).Length==8,"Original coin hierarchy");
                foreach(var t in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
                Check(instance.GetComponentsInChildren<Button>(true).Length==1 && instance.GetComponentsInChildren<OriginalButtonFeedback>(true).Length==0,"Standard Button, no disabled press tween");
                Check(instance.GetComponentsInChildren<OriginalLoopPosition>(true).Length==1,"Native hint bobbing");
                Check(v.Icon.sprite==AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/atlas/items/Coin.asset"),"Original coin sprite");
                var formatter=new OriginalGoldFormatter(()=>"en-US"); var trace=new List<string>(); bool gate=true; Action complete=null;
                v.Bind(data,tables,"en",formatter,()=>gate,done=>{trace.Add("request");complete=done;},id=>trace.Add("panel:"+id),()=>trace.Add("sound"));
                v.Value.text="untouched"; v.Init();
                Check(v.Value.text=="untouched" && v.HintText.transform.parent.gameObject.activeSelf,"Init updates hint but not amount");
                string hint=tables.Text.GetText(9,"en","$8.75","$10").Replace("\n"," ");
                Check(v.HintText.text==hint,"Original shortfall hint and red tags");
                v.Refresh();Check(v.Value.text=="125" && instance.activeSelf && v.PulseCount==0,"Normal coin refresh");
                data.Coin=1000;v.RefreshCoin();
                Check(v.Value.text=="1000" && !v.HintText.transform.parent.gameObject.activeSelf && v.HintText.text==hint,"Stage five hides and retains prior text, even without animation");
                data.Coin=125;data.Level=4;v.Refresh();
                Check(!instance.activeSelf && v.HintText.transform.parent.gameObject.activeSelf,"Refresh updates hint before whole-item visibility");
                data.IsShowCoin=true;v.Refresh();Check(instance.activeSelf,"Saved visibility reactivates coin");
                gate=false;v.Click.onClick.Invoke();Check(trace.Count==0,"Click gate");
                gate=true;v.Click.onClick.Invoke();Check(string.Join(",",trace)=="request,sound","Request before audio, response deferred");
                complete();Check(string.Join(",",trace)=="request,sound,panel:19","Coin callback panel 19");
                v.Init();trace.Clear();v.Click.onClick.Invoke();Check(trace.Count==2,"Listener replacement");
                v.Bind(data,tables,"ja",formatter,()=>true,done=>{},id=>{},()=>{});
                foreach(var label in instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
                    if(label.name=="label_185")Check(label.text==tables.Text.GetText(185,"ja"),"Original localized cash button label");
                v.RefreshCoin(true,25.5f);v.Advance(.6f);Check(v.FloatingPairCount==0,"Source pulse delay");
                v.Advance(.03f);Check(v.FloatingPairCount==1 && Mathf.Abs(v.transform.localScale.x-(1+.15f*Mathf.Sin(Mathf.PI*.25f)))<.0001f,"OutSine pulse and one pair");
                bool found=false;
                foreach(var label in instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
                    if(label!=v.Value && label.text=="+25.50") { found=true;Check(label.transform.localScale==Vector3.one*.7f,"Gain scale"); }
                Check(found,"Gain uses coin number formatting");
                v.Advance(.7f);Check(v.PulseCount==0 && v.FloatingPairCount==1 && v.Icon.transform.parent.localScale==Vector3.one,"Twelve loops finish before float fade");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("NUT_COIN_ITEM_VALIDATION_PASS native prefab, Button, sprites, hint/visibility order, localized labels, deferred panel 19, OutSine pulse and coin gain formatting.");
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin(); EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,854,"Nut Sort coin item validation");
                SessionState.SetBool(Key,true); EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error);Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup; Check(now<timeout,"CoinItem Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    var progress=main.GetComponentInChildren<OriginalRewardProgressView>(true);
                    coin=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/CoinItem"),progress.transform.parent,false).GetComponent<OriginalCoinItem>();
                    user=Fixture(); var formatter=new OriginalGoldFormatter(()=>"en-US");
                    coin.Bind(user,game.Tables,"en",formatter,()=>true,
                        done=>{requests++;response=done;},id=>{Check(id==19,"Source panel ID");opened++;},()=>sounds++);
                    coin.Init();coin.Refresh(); images=coin.GetComponentsInChildren<Image>(true).Length;
                    before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;
                    phase=1;deadline=now+2.1;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(coin.HintText.transform.parent.gameObject.activeSelf,"Coin shortfall shows actual hint");
                    Check(coin.Icon.sprite==AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/atlas/items/Coin.asset"),"Original coin icon");
                    Capture("coin-item");
                    phase=7;deadline=now+.15;return;
                }
                if(phase==7)
                {
                    Canvas.ForceUpdateCanvases();var button=coin.Click;var canvas=button.GetComponentInParent<Canvas>();
                    var data=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,button.transform.position),button=PointerEventData.InputButton.Left};
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);
                    Check(hits.Count>0 && hits[0].gameObject.GetComponentInParent<Button>()==button,"Native coin Button raycast");
                    ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);
                    Check(requests==1 && sounds==1 && opened==0,"Native Button waits for request result");response();Check(opened==1,"Deferred local callback opens original panel route");
                    user.Coin=150;coin.RefreshCoin(true,25f);phase=2;deadline=now+.8;return;
                }
                if(phase==2)
                {
                    Check(coin.Value.text=="150" && coin.FloatingPairCount==1 && coin.PulseCount==1,"Actual delayed reward presentation");
                    Capture("coin-item-gain");Time.timeScale=0;phase=3;deadline=now+.1;return;
                }
                if(phase==3){pausedScale=coin.transform.localScale;phase=4;deadline=now+.3;return;}
                if(phase==4)
                {
                    Check(coin.transform.localScale==pausedScale && coin.FloatingPairCount==1,"Scaled reward animation pauses");
                    Time.timeScale=1;phase=5;deadline=now+2.2;return;
                }
                Check(coin.PulseCount==0 && coin.FloatingPairCount==0 && coin.transform.localScale==Vector3.one,"Native animation cleanup");
                Check(coin.GetComponentsInChildren<Image>(true).Length==images,"Floating icon destroyed after fade");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"Coin view does not grant or save currency");
                game.ModalInputBlocked=false;
                Debug.Log("NUT_COIN_ITEM_PLAY_VALIDATION_PASS actual native coin prefab, shortfall hint and Button request boundary, delayed gain copies/pulse, pause and fade cleanup; production host and SDK service remain excluded.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Capture(string name)
        {
            string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures/"+name+".png"));
            Directory.CreateDirectory(Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);Debug.Log("NUT_COIN_ITEM_CAPTURE "+path);
        }
        private static void Check(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
        private static void Finish(int code){Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
