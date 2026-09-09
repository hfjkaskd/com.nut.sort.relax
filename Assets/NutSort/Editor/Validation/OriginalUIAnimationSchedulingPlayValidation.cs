using System;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalUIAnimationSchedulingPlayValidation
    {
        private const string Key="NutSort.AnimationSchedulingPlay";
        private static double timeout,start;private static int phase,scaled,unscaled;private static float previousScale;private static bool changedScale;
        static OriginalUIAnimationSchedulingPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+90;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Scheduling Play timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    previousScale=Time.timeScale;changedScale=true;Time.timeScale=0;
                    OriginalUIAnimationDriver.Schedule(.3f,false,()=>scaled++);OriginalUIAnimationDriver.Schedule(.3f,true,()=>unscaled++);
                    start=EditorApplication.timeSinceStartup;phase=1;return;
                }
                if(EditorApplication.timeSinceStartup-start<.7)return;
                if(phase==1)
                {
                    if(unscaled==0)return;
                    Check(unscaled==1&&scaled==0&&Time.timeScale==0,"Actual scene Update runs unscaled callback during Time.timeScale zero");
                    Time.timeScale=1;start=EditorApplication.timeSinceStartup;phase=2;return;
                }
                if(scaled==0)return;
                Check(scaled==1&&unscaled==1,"Actual scaled timer resumes after restoring scaled progression");
                Debug.Log("NUT_UI_ANIMATION_SCHEDULING_PLAY_PASS real scene Update with Time.timeScale=0 executes only unscaled timer; scaled timer completes after resume, callbacks exactly once; no production payment or new visual claim.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){if(changedScale)Time.timeScale=previousScale;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
