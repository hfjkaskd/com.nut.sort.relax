using System;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalReplayMaskPlayValidation
    {
        private const string Key="NutSort.ReplayMaskPlay";
        private static OriginalGameScene game;
        private static OriginalReplayController replay;
        private static GameObject mask;
        private static int phase, seed;
        private static string saved;
        private static double timeout;
        private static float deadline;
        static OriginalReplayMaskPlayValidation()
        {
            if(SessionState.GetBool(Key,false))
            { timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick; }
        }
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin();
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Replay mask play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || !game.Level.AreNutsInitialized || game.InputBlocked)return;
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    replay=startup.Replay;
                    mask=(GameObject)new SerializedObject(startup).FindProperty("clickMask").objectReferenceValue;
                    Check(!mask.activeSelf && replay.Panel==null,"Normal startup starts without click mask or replay panel");
                    Check(replay.BeginClick() && replay.BeginClick(),"Direct repeated callbacks each acquire, without a timer gate");
                    Check(mask.activeSelf && game.ModalInputBlocked,"Real serialized mask blocks during acquisitions");
                    replay.enabled=false;
                    deadline=Time.time+.35f;phase=1;return;
                }
                if(Time.time<deadline)return;
                if(phase==1)
                {
                    Check(!mask.activeSelf && !game.ModalInputBlocked,"Scene scheduler releases both acquisitions while controller disabled");
                    replay.enabled=true;replay.ReplayButton.onClick.Invoke();
                    Check(replay.Panel!=null && mask.activeSelf && game.ModalInputBlocked,"Actual main Button opens replay and acquires mask");
                    deadline=Time.time+.35f;phase=2;return;
                }
                if(phase==2)
                {
                    Check(!mask.activeSelf && replay.Panel!=null && game.ModalInputBlocked,"Expired click mask retains existing modal panel block");
                    replay.Panel.ContinueButton.onClick.Invoke();
                    Check(replay.Panel.Closing && mask.activeSelf,"Actual Continue Button acquires mask before starting close");
                    replay.Panel.Advance(.3f);
                    Check(replay.Panel==null && mask.activeSelf && game.ModalInputBlocked,"Panel destruction retains outstanding click acquisition");
                    deadline=Time.time+.35f;phase=3;return;
                }
                if(phase==3)
                {
                    Check(!mask.activeSelf && !game.ModalInputBlocked,"Last scene-owned release restores gameplay after close");
                    replay.ReplayButton.onClick.Invoke();
                    deadline=Time.time+.35f;phase=4;return;
                }
                if(phase==4)
                {
                    seed=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data.LevelSeed;
                    saved=PlayerPrefs.GetString(NutSort.Content.OriginalUserStore.Key);
                    replay.Panel.ReplayButton.onClick.Invoke();
                    Check(replay.Panel.Closing && game.IsRestarting && game.Level.Board==null,
                        "Actual Replay Button starts close and immediately clears board for reconstruction");
                    deadline=Time.time+.35f;phase=5;return;
                }
                Check(replay.Panel==null && !mask.activeSelf && !game.ModalInputBlocked && !game.IsRestarting,
                    "Normal replay completes close, mask release and actual scene reconstruction");
                Check(UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data.LevelSeed==seed &&
                    PlayerPrefs.GetString(NutSort.Content.OriginalUserStore.Key)==saved,"Actual replay retains seed and does not add a save");
                Debug.Log("NUT_REPLAY_MASK_PLAY_VALIDATION_PASS normal startup binding, repeated acquisition, disabled-controller release, actual main/continue/replay Buttons, modal retention, post-close release and scene restart without seed/save change.");
                Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code)
        {
            OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);
            EditorApplication.update-=Tick;EditorApplication.Exit(code);
        }
    }
}
