using System;
using System.Collections.Generic;
using System.IO;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalMarqueeRequestsPlayValidation
    {
        private const string Key="NutSort.MarqueeRequestsPlay";
        private static double timeout;private static float begin,initialX;private static int phase;
        private static OriginalMainTopView top;private static OriginalMarqueeRequests requests;
        private static OriginalMarqueeResponse prior,published;private static string saved;
        static OriginalMarqueeRequestsPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,1040,"Nut Sort PMD response validation");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Marquee response Play timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting||!game.Level.AreNutsInitialized||game.InputBlocked)return;
                    prior=OriginalMarqueeRequests.Cached;OriginalMarqueeRequests.Cached=null;saved=PlayerPrefs.GetString(OriginalUserStore.Key);
                    var pending=new List<Action<string,byte[]>>();requests=new OriginalMarqueeRequests((version,done)=>{Check(version=="-1","Native version argument");pending.Add(done);},(a,b)=>3);
                    requests.Initialize();Check(pending.Count==1&&OriginalMarqueeRequests.Cached==null,"Held startup response");
                    var main=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().MainLevel;
                    Transform old=main.GetComponentInChildren<OriginalRewardProgressView>(true).transform.parent;
                    top=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelTop"),old.parent,false).GetComponent<OriginalMainTopView>();top.transform.SetSiblingIndex(old.GetSiblingIndex());UnityEngine.Object.Destroy(old.gameObject);
                    var user=OriginalMainTopViewValidation.MakeUser();user.Level=52;
                    top.Bind(user,game.Tables,"en",new OriginalGoldFormatter(()=>"en-US"),()=>"US",()=>true,done=>done(),done=>done(),id=>{},()=>{},()=>false,requests.Next,()=>false,game.ScheduleDelay);
                    top.Init();top.PlayerHint.Init();top.Refresh();game.ModalInputBlocked=true;
                    top.Marquee.StartLaunch();Check(top.Marquee.Template.IsReady&&pending.Count==2,"Missing response prevents launch and requests again");
                    pending[0](OriginalMarqueeRequestsValidation.Success,null);published=OriginalMarqueeRequests.Cached;
                    Check(published.kinetic_data.Items[0].IsGold,"Actual successful receive initialized cash rows");
                    top.Marquee.StartLaunch();Check(!top.Marquee.Template.IsReady&&!string.IsNullOrEmpty(top.Marquee.Template.Tip.text),"Actual response selector supplies visible item");
                    initialX=top.Marquee.Template.transform.localPosition.x;begin=Time.time;phase=1;return;
                }
                if(phase==1){if(Time.time-begin<6)return;Check(top.Marquee.Template.transform.localPosition.x<initialX&&top.Marquee.Template.HeadImage.sprite!=null,"Actual item travels with payment icon");top.PlayerHint.Push();begin=Time.time;phase=2;return;}
                if(phase==2){if(Time.time-begin<.8f)return;Check(top.PlayerHint.transform.localPosition==Vector3.zero,"Same response provider drives player hint");Directory.CreateDirectory("Library/ValidationCaptures");ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/marquee-response-current.png");begin=Time.time;phase=3;return;}
                if(phase==3){if(Time.time-begin<.5f)return;Check(ReferenceEquals(published,OriginalMarqueeRequests.Cached)&&PlayerPrefs.GetString(OriginalUserStore.Key)==saved,"Shared response retained without fixture rewards persisted");Debug.Log("NUT_MARQUEE_REQUESTS_PLAY_PASS held startup PMD response to native JsonUtility/shared cache/Init/selector and actual main Top marquee travel plus player hint; transport and unrelated system actions are explicit fixtures, production bootstrap composition pending.");Finish(0);}
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalMarqueeRequests.Cached=prior;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
