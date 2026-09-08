using System;
using System.IO;
using System.Collections.Generic;
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
    public static class OriginalUIAnimationValidation
    {
        const string Key="NutSort.UIAnimationValidation";
        static OriginalGameScene game;
        static OriginalGoldItem gold;
        static OriginalCoinItem coin;
        static OriginalTargetRewardBanner banner;
        static int phase,callbacks;
        static double timeout,deadline;
        static string before;
        static Vector3 paused, hintStart, hintPaused;
        static Transform punch, glow, hint;
        static Quaternion punchStart, glowStart, punchPaused, glowPaused;
        static OriginalUIAnimationValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick; }
        }
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>("Prefabs/Panels/CoinItem");
            var first=UnityEngine.Object.Instantiate(prefab);var second=UnityEngine.Object.Instantiate(prefab);
            var a=first.GetComponent<OriginalCoinItem>();var b=second.GetComponent<OriginalCoinItem>();
            try
            {
                var trace=new List<string>();Action<float> next=d=>trace.Add("b");
                OriginalUIAnimationDriver.Register(a,d=>{trace.Add("a");OriginalUIAnimationDriver.Unregister(a);OriginalUIAnimationDriver.Register(b,next);});
                first.SetActive(false);a.enabled=false;
                OriginalUIAnimationDriver.Advance(.1f);Check(string.Join(",",trace)=="a","Hidden/disabled owner advances; callback registrations wait for next global update");
                OriginalUIAnimationDriver.Advance(0);Check(trace.Count==1,"Zero scaled delta preserves registry and does not dispatch");
                OriginalUIAnimationDriver.Advance(.1f);Check(string.Join(",",trace)=="a,b","Deferred registration advances next frame");
                OriginalUIAnimationDriver.Unregister(b);trace.Clear();
                OriginalUIAnimationDriver.Register(a,d=>{trace.Add("a");OriginalUIAnimationDriver.Unregister(b);});
                OriginalUIAnimationDriver.Register(b,next);
                OriginalUIAnimationDriver.Advance(.1f);Check(string.Join(",",trace)=="a","Callback removal suppresses a later owner without shifting the active range");
                OriginalUIAnimationDriver.Unregister(a);OriginalUIAnimationDriver.Register(b,next);
                UnityEngine.Object.DestroyImmediate(second);trace.Clear();OriginalUIAnimationDriver.Advance(.1f);
                Check(trace.Count==0,"Destroyed targets are removed before dispatch");
            }
            finally { UnityEngine.Object.DestroyImmediate(first);if(second!=null)UnityEngine.Object.DestroyImmediate(second); }
            Debug.Log("NUT_UI_ANIMATION_VALIDATION_PASS hidden/disabled owner dispatch, snapshot registration, callback removal, zero delta and destroyed target pruning.");
        }
        public static void RunPlay()
        {
            try { OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode(); }
            catch(Exception e) { Debug.LogException(e);Finish(1); }
        }
        static OriginalUserLocalData Fixture()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.Gold=12.5f;user.Coin=125;user.IsShowCoin=true;
            user.GoldRewardTargetS2CData=JObject.Parse("{\"cal_cfg\":5,\"bear_rates\":\"100\",\"bear_zs_list\":[{\"caliper_psi\":\"1000\",\"psi_value\":\"10\",\"caliper_logs\":3}],\"bear_list\":[null,null,{\"RealLevel\":22,\"Stage2RealLevel\":51}]}");return user;
        }
        static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"UI animation Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    Check(UnityEngine.Object.FindObjectsOfType<OriginalUIAnimationDriver>().Length==1,"Actual scene owns one animation driver");
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    var top=main.GetComponentInChildren<OriginalRewardProgressView>(true).transform.parent;
                    gold=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/GoldItem"),top,false).GetComponent<OriginalGoldItem>();
                    coin=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/CoinItem"),top,false).GetComponent<OriginalCoinItem>();
                    var user=Fixture();var formatter=new OriginalGoldFormatter(()=>"en-US");
                    gold.Bind(user,game.Tables,"en",n=>formatter.Format(n),()=>"US",()=>true,done=>{},id=>{},()=>{});
                    coin.Bind(user,game.Tables,"en",formatter,()=>true,done=>{},id=>{},()=>{});
                    gold.Init();gold.Refresh();coin.Init();coin.Refresh();
                    gold.ShowGoldHintText();hint=gold.GoldHintText.transform.parent;hintStart=hint.localPosition;
                    var hidden=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/HiddenLevel"),top,false).GetComponent<OriginalHiddenLevelView>();
                    var hiddenUser=Fixture();hiddenUser.Level=9;hidden.Bind(hiddenUser,game.Tables,"en");hidden.Init();hidden.Refresh();
                    punch=hidden.GetComponentsInChildren<OriginalRecordPunchRotation>(true)[0].transform;punchStart=punch.localRotation;
                    hidden.gameObject.SetActive(false);
                    var progress=main.GetComponentInChildren<OriginalRewardProgressView>(true);progress.gameObject.SetActive(true);
                    glow=progress.GetComponentInChildren<OriginalLoopRotation>(true).transform;glowStart=glow.localRotation;
                    progress.gameObject.SetActive(false);
                    banner=main.GetComponentInChildren<OriginalTargetRewardBanner>(true);
                    banner.Bind(user,game.Tables,"en",n=>formatter.Format(n),()=>gold.transform.position,()=>callbacks++,()=>callbacks++,()=>{throw new InvalidDataException("Unexpected save");});
                    before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;
                    gold.RefreshGold(true,2.5f);coin.RefreshCoin(true,25);banner.Show(()=>callbacks++);
                    gold.gameObject.SetActive(false);coin.enabled=false;banner.gameObject.SetActive(false);
                    phase=1;deadline=now+.85;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(gold.FloatingPairCount==1 && coin.FloatingPairCount==1 && callbacks==0,"Hidden gold and disabled coin still start delayed gain copies");
                    Check(Quaternion.Angle(punch.localRotation,punchStart)>.01f && Quaternion.Angle(glow.localRotation,glowStart)>1 && (hint.localPosition-hintStart).sqrMagnitude>.001f,"Hidden Punch, glow rotation and hint position continue changing" );
                    Time.timeScale=0;phase=2;deadline=now+.1;return;
                }
                if(phase==2) { punchPaused=punch.localRotation;glowPaused=glow.localRotation;hintPaused=hint.localPosition;paused=gold.transform.localScale;phase=3;deadline=now+.35;return; }
                if(phase==3)
                {
                    Check(gold.transform.localScale==paused && gold.FloatingPairCount==1 && callbacks==0,"Global animation remains scaled while targets are hidden");
                    Check(punch.localRotation==punchPaused && glow.localRotation==glowPaused && hint.localPosition==hintPaused,"All hidden loop tracks pause on scaled zero time" );
                    Time.timeScale=1;phase=4;deadline=now+2.3;return;
                }
                if(phase==4)
                {
                    Check(gold.PulseCount==0 && gold.FloatingPairCount==0 && coin.PulseCount==0 && coin.FloatingPairCount==0,"Hidden and disabled finite animations complete and clean clones");
                    Check(!banner.IsAnimating && callbacks==3 && Mathf.Abs(banner.Target.localScale.x-.2f)<.0001f,"Hidden banner callbacks and native final scale order");
                    gold.gameObject.SetActive(true);coin.enabled=true;banner.gameObject.SetActive(true);
                    Check(gold.transform.localScale==Vector3.one && coin.transform.localScale==Vector3.one,"Reactivation observes completed state");
                    gold.RefreshGold(true,1);coin.RefreshCoin(true,1);banner.Show(()=>callbacks++);
                    UnityEngine.Object.Destroy(gold.gameObject);UnityEngine.Object.Destroy(coin.gameObject);UnityEngine.Object.Destroy(banner.gameObject);
                    phase=5;deadline=now+.9;return;
                }
                Check(callbacks==3,"Destroyed owners never execute their remaining callbacks");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"Animation driver does not change the saved user");
                Debug.Log("NUT_UI_ANIMATION_PLAY_VALIDATION_PASS real scene driver, hidden gold, disabled coin, delayed clone creation, scaled pause, hidden banner callback/final-scale order, cleanup/reactivation and destroyed owner cancellation.");Finish(0);
            }
            catch(Exception e) { Debug.LogException(e);Finish(1); }
        }
        static void Check(bool value,string message) { if(!value)throw new InvalidDataException(message); }
        static void Finish(int code) { Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code); }
    }
}
