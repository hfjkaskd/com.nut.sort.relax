using System;
using System.Collections.Generic;
using System.Globalization;
using NutSort.Gameplay;
using NutSort.World;
using NutSort.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalBootstrapHostPlayValidation
    {
        private const string Key="NutSort.BootstrapHostPlay";private static double timeout;private static int phase,oldFps;private static float begin,mainAt;
        private static CultureInfo oldCulture,oldUiCulture;private static OriginalBootstrapHost host;private static Actions actions;
        private sealed class Actions:IOriginalBootstrapActions
        {
            public string Code;public bool UserDone,Level;public readonly List<string> Trace=new List<string>();
            public string CountryCode=>Code;public bool IsUserInitDone=>UserDone;public bool HasLevelInfo=>Level;
            public void SetLoading(bool value)=>throw new Exception("Host must use real loading view");
            public void InitializeUI(){Check(Application.targetFrameRate==60&&CultureInfo.CurrentCulture.Name=="en-US"&&CultureInfo.CurrentUICulture.Name=="en-US","Native Start settings before initialization");Trace.Add("ui");}
            public void InitializeTables()=>Trace.Add("tables");public void InitializeSdkBoundary()=>Trace.Add("sdkBoundary");
            public void InitializePool()=>Trace.Add("pool");public void InitializeUser()=>Trace.Add("user");
            public void InitializeAudio()=>Trace.Add("audio");public void InitializeScene()=>Trace.Add("scene");
            public void ShowAndAssignMainPanel(){Trace.Add("main");mainAt=Time.time;}
            public void InitializeRequests(){Check(Time.time-mainAt>=.49f&&!host.Loading.Flow.IsShow&&host.gameObject.activeSelf,"Source .5 scaled delay before request and host survives");Trace.Add("requests");}
            public void PlayBgm()=>Trace.Add("bgm");
        }
        static OriginalBootstrapHostPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Bootstrap host timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;Check(parent!=null,"Actual canvas");
                    oldFps=Application.targetFrameRate;oldCulture=CultureInfo.CurrentCulture;oldUiCulture=CultureInfo.CurrentUICulture;
                    host=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/OriginalBootstrapHost"),parent,false).GetComponent<OriginalBootstrapHost>();actions=new Actions();host.Bind(actions);begin=Time.time;phase=1;return;
                }
                if(Time.time-begin<.6f)return;
                if(phase==1){Check(string.Join(",",actions.Trace)=="ui,tables,sdkBoundary,pool"&&host.Loading.gameObject.activeSelf,"Country wait holds real loading");actions.Code="US";begin=Time.time;phase=2;return;}
                if(phase==2){Check(actions.Trace[actions.Trace.Count-1]=="audio"&&!actions.Trace.Contains("scene"),"Audio initialized while user wait holds scene");actions.UserDone=true;begin=Time.time;phase=3;return;}
                if(phase==3){Check(actions.Trace[actions.Trace.Count-1]=="scene"&&!actions.Trace.Contains("main"),"LevelInfo wait holds main");actions.Level=true;begin=Time.time;phase=4;return;}
                if(phase==4){if(Time.time-begin<1.5f||host.Loading.gameObject.activeSelf)return;Check(string.Join(",",actions.Trace)=="ui,tables,sdkBoundary,pool,user,audio,scene,main,requests,bgm"&&!host.Loading.gameObject.activeSelf&&host.gameObject.activeSelf,"Full actual coroutine and view lifecycle");
                    Debug.Log("NUT_BOOTSTRAP_HOST_PLAY_PASS actual serialized host Start, 60FPS/en-US cultures, native coroutine waits/frame boundaries, real loading view and scaled .5 main delay, request/BGM order and host survival; system action ports are explicit fixtures, production scene auto-entry pending.");Finish(0);}
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){if(oldCulture!=null){Application.targetFrameRate=oldFps;CultureInfo.CurrentCulture=oldCulture;CultureInfo.CurrentUICulture=oldUiCulture;}OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
