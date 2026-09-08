using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalRewardProgressViewValidation
    {
        private const string Key = "NutSort.RewardProgressViewValidation";
        private static OriginalRewardProgressView view;
        private static OriginalGameScene game;
        private static Transform glow;
        private static Quaternion rotation;
        private static int phase;
        private static double timeout, deadline;
        private static string before;
        static OriginalRewardProgressViewValidation()
        {
            if (SessionState.GetBool(Key, false)) { timeout = EditorApplication.timeSinceStartup + 60; EditorApplication.update += Tick; }
        }
        private static OriginalUserLocalData Fixture()
        {
            var data = new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            data.Level = 11; data.TXTargetGold = "100"; data.Gold = 42.5f;
            data.GoldRewardTargetS2CData = JObject.Parse("{\"cal_cfg\":5,\"bear_list\":[null,null,{\"RealLevel\":5,\"Stage2RealLevel\":10,\"Stage2StartShowLevel\":4,\"caliper_logs\":3,\"caliper_rank\":20}]}");
            return data;
        }
        public static void Validate()
        {
            var tables = new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var instance = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanel"));
            try
            {
                var v = instance.GetComponentInChildren<OriginalRewardProgressView>(true);
                Check(v != null && v.transform.parent.name == "Top" && !v.gameObject.activeSelf, "Source inactive TX_JD under Top");
                Check(v.GetComponentsInChildren<Transform>(true).Length == 8, "Original eight-transform hierarchy");
                foreach (var t in v.GetComponentsInChildren<Transform>(true)) Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) == 0, "No missing scripts");
                Check(v.GetComponentsInChildren<OriginalLoopRotation>(true).Length == 1, "Source glow rotation is native");
                foreach (var image in v.GetComponentsInChildren<Image>(true)) Check(image.sprite != null, "Source image references");
                Check(v.Bar.type == Image.Type.Filled && v.Bar.fillMethod == Image.FillMethod.Horizontal, "Native Image fill mode");
                Check(((RectTransform)v.transform).sizeDelta == new Vector2(737,154.49f), "Original container size");
                var data = Fixture(); var formatter = new OriginalGoldFormatter(() => "en-US");
                v.Bind(data, tables, "en", gold => formatter.Format(gold));
                v.Show(); Check(!v.gameObject.activeSelf, "Show must respect the untouched IsShow flag");
                data.Level = 10; v.Refresh(); Check(!v.IsShow && !v.gameObject.activeSelf, "Inclusive stage-two boundary hides");
                data.Level = 11; v.Refresh();
                Check(v.IsShow && !v.gameObject.activeSelf && v.ProgressValue.text == "4/4" && v.Tip.text == tables.Text.GetText(24,"en"), "Unfinished gold guide substitutes stage-two display progress while hiding");
                Near(v.Bar.fillAmount,1,"Guide full progress");
                v.Show(); Check(v.gameObject.activeSelf, "Show may reveal a hidden but eligible guide view");
                v.Hide(); Check(v.IsShow && !v.gameObject.activeSelf, "Hide preserves eligibility");
                data.IsGuideGoldComplete = true; v.Refresh();
                Check(!v.gameObject.activeSelf && v.ProgressValue.text == "$42.50/$100", "Refresh updates without activating");
                Near(v.Bar.fillAmount,.425f,"Gold fraction");
                Check(v.GoldValue.text == tables.Text.GetText(160,"en") && !v.Tip.text.Contains("\n") && v.Tip.text.Contains("a3ff8a"), "Original heading and recolored single-line description");
                v.Show(); v.Init(); Check(v.IsShow && !v.gameObject.activeSelf, "Init hides without clearing eligibility");
                data.Gold = 100; data.LoginDay = 1; data.TodayPassLevelCount = 2; v.Refresh();
                Check(v.ProgressValue.text == "$100/$100" && v.Tip.text == tables.Text.GetText(24,"en"), "Stage five before target guide retains currency progress");
                data.IsGuideGoldTargetComplete = true; v.Refresh(); Near(v.Bar.fillAmount,.4f,"Daily fraction");
                Check(v.ProgressValue.text == "2/5", "Daily counters");
                data.TodayPassLevelCount = 5; v.Refresh(); Near(v.Bar.fillAmount,1f/3,"Login fraction");
                Check(v.ProgressValue.text == "1/3", "Login counters");
                data.IsGuideGoldTargetComplete = false; v.Refresh();
                Check(v.ProgressValue.text == "$100/$100" && v.Tip.text == tables.Text.GetText(24,"en"), "Stage six shares the pending-target-guide substitution");
                data.LoginDay = 3; data.UserLevel = 25; v.Refresh();
                Check(v.ProgressValue.text == "25/20", "Rank numerator uses UserLevel without text clamping");
                Near(v.Bar.fillAmount,1,"Unity Image clamps overfilled rank");
                data.Level = 10; string prior = v.ProgressValue.text; v.Refresh();
                Check(!v.IsShow && !v.gameObject.activeSelf && v.ProgressValue.text == prior, "Early-level hide does not refresh existing labels");
                data.Level = 11; data.Gold = 0; data.IsGuideGoldComplete = false;
                data.GoldRewardTargetS2CData["bear_list"][2]["Stage2StartShowLevel"] = 0; v.Refresh();
                Check(float.IsNaN(v.Bar.fillAmount) && v.ProgressValue.text == "0/0", "Original zero/zero calculation is retained");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("NUT_REWARD_PROGRESS_VIEW_VALIDATION_PASS source prefab, independent visibility/eligibility, stage boundaries, gold/daily/login/rank display, guide substitutions, native fill and zero denominator behavior; production reward host pending.");
        }
        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin(); EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480,854,"Nut Sort reward progress validation");
                SessionState.SetBool(Key,true); EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error); Finish(1); }
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup; Check(now<timeout,"Progress Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    view=main.GetComponentInChildren<OriginalRewardProgressView>(true);
                    var data=Fixture(); data.IsGuideGoldComplete=true;
                    var formatter=new OriginalGoldFormatter(()=>"en-US");
                    view.Bind(data,game.Tables,"en",gold=>formatter.Format(gold)); view.Refresh();
                    var motion=view.GetComponentInChildren<OriginalLoopRotation>(true); glow=motion.transform;
                    before=PlayerPrefs.GetString(OriginalUserStore.Key); game.ModalInputBlocked=true;
                    var banner=main.GetComponentInChildren<OriginalTargetRewardBanner>();
                    banner.Bind(data,game.Tables,"en",gold=>formatter.Format(gold),()=>view.transform.position,()=>{},view.Show,()=>{throw new InvalidDataException("Stage four does not save");});
                    banner.Show(); phase=1; deadline=now+2.2; return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(view.gameObject.activeSelf && view.ProgressValue.text=="$42.50/$100","Banner completion shows actual configured progress view");
                    string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures/reward-progress.png"));
                    Directory.CreateDirectory(Path.GetDirectoryName(path)); ScreenCapture.CaptureScreenshot(path); Debug.Log("NUT_REWARD_PROGRESS_CAPTURE "+path);
                    rotation=glow.localRotation; phase=2; deadline=now+.3; return;
                }
                if(phase==2)
                {
                    Check(Quaternion.Angle(rotation,glow.localRotation)>10,"Glow rotates in actual frames");
                    Time.timeScale=0; phase=3; deadline=now+.1; return;
                }
                if(phase==3) { rotation=glow.localRotation; phase=4; deadline=now+.3; return; }
                if(phase==4)
                {
                    Check(Quaternion.Angle(rotation,glow.localRotation)<.01f,"Glow uses scaled time");
                    Time.timeScale=1; view.Hide(); phase=5; deadline=now+.4; return;
                }
                if(phase==5) { view.Show(); phase=6; deadline=now+.05; return; }
                Check(Quaternion.Angle(rotation,glow.localRotation)>20,"Hidden interval preserves continuing glow phase");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"Isolated visual fixture introduces no save");
                game.ModalInputBlocked=false;
                Debug.Log("NUT_REWARD_PROGRESS_VIEW_PLAY_VALIDATION_PASS real banner-to-progress callback, original current canvas/render, glow rotation, scaled pause and hidden-phase continuation; synthetic reward values and isolated gold destination, production host pending."); Finish(0);
            }
            catch(Exception error) { Debug.LogException(error); Finish(1); }
        }
        private static void Near(float a,float b,string message) { Check(Mathf.Abs(a-b)<.00001f,message); }
        private static void Check(bool condition,string message) { if(!condition)throw new InvalidDataException(message); }
        private static void Finish(int code) { Time.timeScale=1; OriginalPreferenceFixture.Restore(); SessionState.SetBool(Key,false); EditorApplication.update-=Tick; EditorApplication.Exit(code); }
    }
}
