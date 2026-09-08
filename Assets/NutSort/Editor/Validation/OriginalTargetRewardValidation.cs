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
using UnityEngine.UI;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalTargetRewardValidation
    {
        private const string Key = "NutSort.TargetRewardValidation";
        private static OriginalTargetRewardBanner banner;
        private static OriginalGameScene game;
        private static int phase;
        private static double timeout, deadline;
        private static string before;
        private static readonly List<string> trace = new List<string>();
        static OriginalTargetRewardValidation()
        {
            if (SessionState.GetBool(Key, false)) { timeout = EditorApplication.timeSinceStartup + 60; EditorApplication.update += Tick; }
        }

        public static void Validate()
        {
            var defaults = ScriptableObject.CreateInstance<OriginalUserDefaults>();
            var tables = new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var instance = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanel"));
            try
            {
                var view = instance.GetComponentInChildren<OriginalTargetRewardBanner>(true);
                Check(view != null && view.transform.parent == instance.transform, "Original banner is a direct MainPanel child");
                Check(view.GetComponentsInChildren<Transform>(true).Length == 5, "Five original banner transforms");
                foreach (var child in view.GetComponentsInChildren<Transform>(true))
                    Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject) == 0, "No missing banner scripts");
                foreach (var image in view.GetComponentsInChildren<Image>(true)) Check(image.sprite != null, "Source banner sprites");
                Check(view.Tip.font.name == "zh_Custom SDF" && view.Tip.enableAutoSizing && view.Tip.fontSizeMin == 18 && view.Tip.fontSizeMax == 80, "Original main text font and autosizing");
                Check(view.SubTip.fontSizeMax == 46 && view.SubTip.enableAutoSizing, "Original subtext autosizing");
                var user = new OriginalUserLocalData(defaults) { Level = 1 };
                var events = new List<string>(); int saves = 0, samples = 0;
                Vector3 destination = new Vector3(123, 456, 7);
                var formatter = new OriginalGoldFormatter(() => "en-US");
                view.Bind(user, tables, "en", value => formatter.Format(value), () => { samples++; return destination; },
                    () => events.Add("gold"), () => events.Add("progress"), () => { Check(user.IsCompleteSignIn && view.Tip.text.Length > 0, "Sign-in save follows flag and text writes"); saves++; });
                view.Init(); view.Show(() => { events.Add("callback"); Near(view.Target.position, new Vector3(123, 456, 7), "Callback sees completed world-space movement"); });
                Check(view.Target.localPosition == new Vector3(-2000, 0, 0) && view.Target.localScale == Vector3.one, "Source Init position/scale");
                Check(view.Tip.text == "<color=#FFFF00>Clear this level</color>\nWithdraw all", "First-level banner text");
                Check(!view.SubTip.transform.parent.gameObject.activeSelf && saves == 0, "First level has no round subtip or save");
                view.Advance(.25f); Check(view.Target.localPosition.x > 0 && samples == 0, "OutBack overshoots before entry completes");
                view.Advance(.25f); Near(view.Target.localPosition, Vector3.zero, "Entry completes at local zero");
                Check(samples == 1 && events.Count == 0, "Destination sampled when entry completes");
                destination = new Vector3(-999, -999, -999);
                view.Advance(.75f); instance.transform.position = new Vector3(10, 20, 30);
                Vector3 start = view.Target.position;
                view.Advance(.25f); Near(view.Target.position, start, "Hold does not start flight at exact delay equality");
                view.Advance(0); Near(view.Target.position, start, "Zero scaled delta does not advance tracks");
                view.Advance(.15f);
                Near(view.Target.position, Vector3.Lerp(start, new Vector3(123, 456, 7), .5f), "Flight samples its start after the hold and keeps captured destination");
                Near(view.Target.localScale, Vector3.one * .6f, "Linear half-flight scale");
                view.Advance(.151f);
                Check(string.Join(",", events) == "callback,gold,progress", "Callback precedes both top-area actions");
                Near(view.Target.localPosition, new Vector3(-2000, 0, 0), "Completion resets position");
                Near(view.Target.localScale, Vector3.one * .2f, "Later scale track writes after callback reset");
                Check(!view.IsAnimating, "Completed tracks are released");

                user.Level = 4; view.RefreshSubTip();
                Check(!view.SubTip.transform.parent.gameObject.activeSelf && view.SubTip.text == tables.Text.GetText(150, "en", 4, "1/3"), "Round one text updates even while hidden");
                user.Level = 5; view.RefreshSubTip();
                Check(view.SubTip.transform.parent.gameObject.activeSelf && view.SubTip.text == tables.Text.GetText(150, "en", 4, "2/3"), "Round two shows original subtip");
                user.Level = 52; view.RefreshSubTip();
                Check(!view.SubTip.transform.parent.gameObject.activeSelf && view.SubTip.text == tables.Text.GetText(150, "en", 4, "2/3"), "Zero-round overflow hides without rewriting text");
                user.GoldRewardTargetS2CData = JObject.Parse("{\"cal_cfg\":5,\"bear_list\":[null,null,{\"RealLevel\":2,\"Stage2RealLevel\":3,\"caliper_logs\":3,\"caliper_rank\":20}]}");
                user.TXTargetGold = "100"; view.RefreshTip();
                Check(view.Tip.text == tables.Text.GetText(20, "en", "$100") && saves == 0, "Banner stage four overrides the general status description");
                user.Gold = 100; user.LoginDay = 3; view.RefreshTip(); view.RefreshTip();
                Check(saves == 1 && user.IsCompleteSignIn, "Stage seven saves exactly once on first refresh");
                Check(view.Tip.text == "<color=#FFFF00>20</color> user levels remaining\nWithdraw all", "Only existing numeric color openings are replaced");

                user.Level = 1; events.Clear(); view.Show(() => events.Add("one")); view.Show(() => events.Add("two"));
                view.Advance(.5f); Check(view.IsAnimating, "Concurrent Show calls both survive");
                view.Advance(1); view.Advance(.31f);
                Check(string.Join(",", events) == "one,gold,progress,two,gold,progress" && !view.IsAnimating, "Overlapping invocations preserve their callbacks and registration order");
                events.Clear(); view.Show(); view.Advance(.5f); view.Advance(1.31f);
                Check(string.Join(",", events) == "gold,progress", "Null optional callback still runs both top-area actions");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); UnityEngine.Object.DestroyImmediate(defaults); }
            Debug.Log("NUT_TARGET_REWARD_VALIDATION_PASS original prefab/text, seven-stage consumer, sign-in save, scaled entry/hold/world flight, delayed start capture, callback/scale order and overlapping Show calls; full production host pending.");
        }

        public static void RunPlay()
        {
            try
            {
                OriginalPreferenceFixture.Begin(); EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                PlayModeWindow.SetCustomRenderingResolution(480, 854, "Nut Sort target reward validation");
                SessionState.SetBool(Key, true); EditorApplication.EnterPlaymode();
            }
            catch (Exception error) { Debug.LogException(error); Finish(1); }
        }
        private static void Tick()
        {
            if (!EditorApplication.isPlaying) return;
            try
            {
                double now = EditorApplication.timeSinceStartup; Check(now < timeout, "Target reward Play timeout");
                if (game == null) game = UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if (game == null || game.Level == null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked) return;
                if (phase == 0)
                {
                    banner = UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel.GetComponentInChildren<OriginalTargetRewardBanner>(true);
                    var session = UnityEngine.Object.FindObjectOfType<OriginalUserSession>();
                    var formatter = new OriginalGoldFormatter(() => "en-US");
                    before = PlayerPrefs.GetString(OriginalUserStore.Key);
                    banner.Bind(session.Data, game.Tables, "en", value => formatter.Format(value),
                        () => banner.transform.parent.TransformPoint(new Vector3(-250, 760, 0)),
                        () => trace.Add("gold"), () => trace.Add("progress"), () => { throw new InvalidDataException("First level must not save"); });
                    game.ModalInputBlocked = true; banner.Show(() => trace.Add("callback"));
                    phase = 1; deadline = now + .7; return;
                }
                if (now < deadline) return;
                if (phase == 1)
                {
                    Near(banner.Target.localPosition, Vector3.zero, "Actual frame entry reaches center during hold");
                    Check(trace.Count == 0 && banner.IsAnimating, "First level does not skip the banner delay");
                    string path = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/ValidationCaptures/target-reward.png"));
                    Directory.CreateDirectory(Path.GetDirectoryName(path)); ScreenCapture.CaptureScreenshot(path);
                    Debug.Log("NUT_TARGET_REWARD_CAPTURE " + path); Time.timeScale = 0;
                    phase = 2; deadline = now + .4; return;
                }
                if (phase == 2)
                {
                    Check(trace.Count == 0, "Paused time scale holds actual banner tracks");
                    Near(banner.Target.localPosition, Vector3.zero, "Paused banner remains centered");
                    Time.timeScale = 1; phase = 3; deadline = now + 1.4; return;
                }
                Check(string.Join(",", trace) == "callback,gold,progress" && !banner.IsAnimating, "Actual frame completion order");
                Near(banner.Target.localPosition, new Vector3(-2000, 0, 0), "Actual frame reset position");
                Near(banner.Target.localScale, Vector3.one * .2f, "Actual frame final scale ordering");
                Check(PlayerPrefs.GetString(OriginalUserStore.Key) == before, "First-level banner does not add a save");
                game.ModalInputBlocked = false;
                Debug.Log("NUT_TARGET_REWARD_PLAY_VALIDATION_PASS current MainPanel canvas, visible first-level hold, scaled pause/resume, native motion completion/callback order and no added first-level save; top-area callbacks isolated."); Finish(0);
            }
            catch (Exception error) { Debug.LogException(error); Finish(1); }
        }
        private static void Near(Vector3 a, Vector3 b, string message) { Check(Vector3.Distance(a, b) < .001f, message + ": " + a + " / " + b); }
        private static void Check(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
        private static void Finish(int code)
        {
            Time.timeScale = 1; OriginalPreferenceFixture.Restore(); SessionState.SetBool(Key, false);
            EditorApplication.update -= Tick; EditorApplication.Exit(code);
        }
    }
}
