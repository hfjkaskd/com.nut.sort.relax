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
    public static class OriginalHiddenLevelValidation
    {
        const string Key="NutSort.HiddenLevelValidation";
        static OriginalHiddenLevelView view;
        static OriginalGameScene game;
        static OriginalUserLocalData user;
        static int phase;
        static double deadline,timeout;
        static string before;
        static float alpha;
        static OriginalHiddenLevelValidation()
        {
            if(SessionState.GetBool(Key,false)) { timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick; }
        }
        static OriginalUserLocalData Fixture()
        {
            var data=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            data.GoldRewardTargetS2CData=JObject.Parse("{\"bear_list\":[null,null,{\"StartLevel\":7,\"RealLevel\":22,\"Stage2StartShowLevel\":30,\"Stage2StartRealLevel\":47,\"Stage2RealLevel\":51}]}");
            return data;
        }
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/HiddenLevel"));
            try
            {
                var v=instance.GetComponent<OriginalHiddenLevelView>();var data=Fixture();v.Bind(data,tables,"en");
                Check(instance.GetComponentsInChildren<Transform>(true).Length==38,"Source hierarchy");
                foreach(var t in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"Missing source script");
                Check(instance.GetComponentsInChildren<OriginalRecordPunchRotation>(true).Length==5,"Five source Get punches");
                Check(v.SubRounds.GetComponent<HorizontalLayoutGroup>()!=null,"Source sub-round layout");
                v.Init();Check(!v.SubRoundTemplate.activeSelf,"Init hides template only");
                data.Level=1;v.Refresh();Check(!instance.activeSelf,"Level one hides owner");
                data.Level=2;v.Refresh();Check(instance.activeSelf && v.Progress.fillAmount==.25f,"Second early level position");
                Check(!v.Levels[0].Get.activeSelf && v.Levels[1].Get.activeSelf && !v.Levels[2].Get.activeSelf && v.Levels[3].Get.activeSelf,"Native Get conditions use actual level and skip third marker");
                data.Level=5;v.Refresh();Check(v.Round4.text=="2/3" && v.Round4.transform.parent.gameObject.activeSelf && !v.Round5.transform.parent.gameObject.activeSelf,"Fourth displayed level round two");
                data.Level=7;v.Refresh();Check(v.Progress.fillAmount==1 && v.Level5.text=="5" && !v.Round5.transform.parent.gameObject.activeSelf,"StartLevel equality fills bar and hides round five");
                data.Level=8;v.Refresh();Check(v.Round5.text=="2/5" && v.Round5.transform.parent.gameObject.activeSelf,"Fifth level rounds");
                data.Level=9;v.Refresh();Check(v.DotCount==4 && v.SubRounds.activeSelf && v.Round5.text=="3/5","Four sub-round clones");
                data.Level=10;v.Refresh();Check(v.DotCount==4,"Sub-round objects reused");
                data.Level=13;v.Refresh();Check(v.DotCount==5,"Original increase to five dots");
                data.Level=23;v.Refresh();Check(!instance.activeSelf && !v.SubRounds.activeSelf && v.Level5.text=="30","Hidden owner still refreshes round labels and sub-round visibility");
                data.IsCompleteGuidePassStage2Level=true;v.Refresh();Check(instance.activeSelf && Mathf.Abs(v.Progress.fillAmount-.05f)<.0001f,"Stage-two guide completion permits late milestones");
                Check(v.Levels[0].Level.text=="5" && v.Levels[3].Level.text=="20" && v.Level5.text=="30","Late marker labels");
                data.Level=40;v.Refresh();Check(Mathf.Abs(v.Progress.fillAmount-.825f)<.0001f,"Last interval spans ten displayed levels");
                data.Level=51;v.Progress.fillAmount=.42f;v.Refresh();Check(v.Progress.fillAmount==.42f && v.Round5.text=="5/5","All completed markers retain prior bar value");
                data.Level=52;v.Refresh();Check(!instance.activeSelf,"Post-stage-two owner hidden");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("NUT_HIDDEN_LEVEL_VALIDATION_PASS original prefab, two milestone branches, Get states, inclusive boundaries, round ordering, sub-round reuse/growth and retained fill.");
        }
        public static void RunPlay()
        {
            try { OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,854,"Nut Sort hidden level");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode(); }
            catch(Exception e) { Debug.LogException(e);Finish(1); }
        }
        static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    var top=main.GetComponentInChildren<OriginalRewardProgressView>(true).transform.parent;
                    view=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/HiddenLevel"),top,false).GetComponent<OriginalHiddenLevelView>();
                    user=Fixture();user.Level=9;view.Bind(user,game.Tables,"en");view.Init();view.Refresh();
                    before=PlayerPrefs.GetString(OriginalUserStore.Key);game.ModalInputBlocked=true;phase=1;deadline=now+.5;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Check(view.DotCount==4 && view.Round5.text=="3/5","Actual round-three view");
                    Capture("hidden-level-rounds");Time.timeScale=0;phase=2;deadline=now+.1;return;
                }
                if(phase==2) { alpha=CurrentDot().color.a;phase=3;deadline=now+.3;return; }
                if(phase==3)
                {
                    Check(Mathf.Abs(CurrentDot().color.a-alpha)<.00001f,"Scaled blink pause");
                    Time.timeScale=1;user.Level=13;view.Refresh();Check(view.DotCount==5,"Actual clone growth");phase=4;deadline=now+.3;return;
                }
                if(phase==4)
                {
                    user.Level=40;user.IsCompleteGuidePassStage2Level=true;view.Refresh();phase=5;deadline=now+.3;return;
                }
                if(phase==5) { Capture("hidden-level-milestones");phase=6;deadline=now+.2;return; }
                Check(PlayerPrefs.GetString(OriginalUserStore.Key)==before,"View never saves synthetic progress");
                Debug.Log("NUT_HIDDEN_LEVEL_PLAY_VALIDATION_PASS actual MainPanel placement, sub-round clones, scaled blink pause/resume, late milestone refresh and current captures; production lifecycle pending.");Finish(0);
            }
            catch(Exception e) { Debug.LogException(e);Finish(1); }
        }
        static Image CurrentDot()
        {
            foreach(Transform child in view.SubRoundTemplate.transform.parent)
                if(child.gameObject!=view.SubRoundTemplate)return child.Find("Icon").GetComponent<Image>();
            throw new InvalidDataException("Missing cloned dot");
        }
        static void Capture(string name)
        {
            string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures/"+name+".png"));Directory.CreateDirectory(Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);Debug.Log("NUT_HIDDEN_LEVEL_CAPTURE "+path);
        }
        static void Check(bool value,string message) { if(!value)throw new InvalidDataException(message); }
        static void Finish(int code) { Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code); }
    }
}
