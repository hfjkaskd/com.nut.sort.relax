using System;
using System.IO;
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
    public static class OriginalGuideTargetPanelValidation
    {
        private const string Key="NutSort.GuideTargetPlay",PathName="prefabs/panels/TXGuideTargetCompletePanel";
        private static double deadline,timeout;
        private static int phase,initialized,callbacks,sounds,hidden,queued;
        private static bool gate;
        private static OriginalGuideTargetPanelHost host;
        static OriginalGuideTargetPanelValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Validate()
        {
            var p=Resources.Load<GameObject>(PathName);Check(p!=null,"Target prefab loads");
            Check(p.GetComponentsInChildren<RectTransform>(true).Length==12,"All 12 original RectTransforms");
            foreach(var t in p.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
            Check(p.GetComponentsInChildren<Transform>(true).Length==14,"All 14 original nodes");
            Check(p.GetComponentsInChildren<Button>(true).Length==1,"Original standard Button");
            Check(p.GetComponentsInChildren<ParticleSystem>(true).Length==2,"Both original particle effects");
            var v=p.GetComponent<OriginalGuideTargetPanel>();Check(v.ContinueButton!=null && v.Tip.font!=null,"Button and original gold font");
            Check(p.GetComponentsInChildren<OriginalButtonFeedback>(true).Length==1,"Button press feedback");
            Check(p.GetComponentsInChildren<OriginalGuideLoopMotion>(true).Length==1 && p.GetComponentsInChildren<OriginalLoopRotation>(true).Length==1,"Hand and glow animations");
            var gold=new SerializedObject(p.GetComponentInChildren<OriginalGoldImage>(true));
            Check(gold.FindProperty("goldType").intValue==3 && Resources.Load<Sprite>("Atlas/Golds/US3")!=null,"Original US type-three gold resource");
            Check(new SerializedObject(v).FindProperty("queueDelay").floatValue==2.5f,"Native hide queue delay");
            Debug.Log("NUT_GUIDE_TARGET_PANEL_VALIDATION_PASS 14 source nodes, gold font/resource, standard Button, two particles and animation references.");
        }
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Target completion validation");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Target play timeout");
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                    if(startup==null || startup.MainLevel==null || game==null || game.IsRestarting || game.InputBlocked)return;
                    Validate();var user=game.User;
                    // Isolated fixture amounts; no server response, reward grant or SDK success is simulated.
                    user.Level1Gold=12;user.Level2Gold=34;user.Gold=56;user.IsCompleteRecordGuide=false;
                    var queue=new OriginalPanelActionQueue();queue.Add(()=>queued++);
                    var formatter=new OriginalGoldFormatter(()=>"en-US");
                    host=new OriginalGuideTargetPanelHost(startup.MainLevel.transform.parent,PathName,user,game.Tables,"en",()=>gate,
                        sound=>{Check(sound=="Click" && callbacks==1,"Click sound follows continuation");sounds++;},()=>"US",value=>formatter.Format(value),
                        (banner,first)=>{Check(host.Panel.Closing && user.IsCompleteRecordGuide && banner && !first,"Close and flag precede initialization");initialized++;},
                        queue,game.ScheduleDelay,()=>hidden++);
                    OriginalNewbieGuideView.CallbackAction=value=>{Check(value==null && initialized==1,"Guide callback follows initialization with null payload");callbacks++;};
                    host.Show(1);Check(host.Panel.Tip.text=="$12","Actual stage-one text");
                    phase=1;deadline=now+1;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    Directory.CreateDirectory("Library/ValidationCaptures");ScreenCapture.CaptureScreenshot(System.IO.Path.GetFullPath("Library/ValidationCaptures/guide-target-complete.png"));
                    host.Panel.ContinueButton.onClick.Invoke();Check(!host.Panel.Closing && initialized==0 && sounds==0,"Rejected gate leaves panel unchanged");
                    phase=2;deadline=now+.3;return;
                }
                if(phase==2)
                {
                    gate=true;host.Panel.ContinueButton.onClick.Invoke();
                    Check(host.Panel.Closing && initialized==1 && callbacks==1 && sounds==1,"Standard Button reaches exact continuation");
                    phase=3;deadline=now+.6;return;
                }
                if(phase==3){Check(!host.IsOpen && hidden==1 && queued==0,"Closing destroys registered panel before delayed queue");phase=4;deadline=now+2.6;return;}
                Check(queued==1,"Hide releases queued action once");
                Debug.Log("NUT_GUIDE_TARGET_PANEL_PLAY_PASS actual prefab amount, gated standard Button, close/flag/init/callback/audio order, animated registry removal and delayed queue; isolated fixture, production routing remains incomplete.");Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalNewbieGuideView.CallbackAction=null;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
