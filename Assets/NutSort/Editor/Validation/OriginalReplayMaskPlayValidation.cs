using System;
using NutSort.UI;
using NutSort.Content;
using NutSort.Gameplay;
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
        private static LocalGameplayController local;
        private static double timeout;
        private static float deadline;
        static OriginalReplayMaskPlayValidation()
        {
            if(SessionState.GetBool(Key,false))
            { timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick; }
        }
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin("{\"Level\":2}");
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
                    replay=startup.Replay;local=startup.LocalGameplay;
                    mask=(GameObject)new SerializedObject(startup).FindProperty("clickMask").objectReferenceValue;
                    Check(!mask.activeSelf && replay.Panel==null,"Normal startup starts without click mask or replay panel");
                    Check(replay.BeginClick() && replay.BeginClick(),"Direct repeated callbacks each acquire, without a timer gate");
                    Check(mask.activeSelf && !game.ModalInputBlocked,"Real serialized mask blocks UI without becoming a registered modal");
                    replay.enabled=false;
                    deadline=Time.time+.35f;phase=1;return;
                }
                if(Time.time<deadline)return;
                if(phase==1)
                {
                    Check(!mask.activeSelf && !game.ModalInputBlocked,"Scene scheduler releases both acquisitions while controller disabled");
                    replay.enabled=true;local.Bottom.Replay.onClick.Invoke();
                    Check(replay.Panel!=null && mask.activeSelf && game.ModalInputBlocked,"Actual main Button opens replay and acquires mask");
                    replay.Panel.enabled=false;
                    deadline=Time.time+.35f;phase=2;return;
                }
                if(phase==2)
                {
                    Check(!mask.activeSelf && replay.Panel!=null && game.ModalInputBlocked,"Expired click mask retains existing modal panel block");
                    Check(replay.Panel.Main.localScale==Vector3.one,"Global opening animation completes while panel component disabled");
                    replay.Panel.enabled=true;
                    replay.Panel.ContinueButton.onClick.Invoke();
                    Check(replay.Panel.Closing && mask.activeSelf,"Actual Continue Button acquires mask before starting close");
                    replay.Panel.gameObject.SetActive(false);
                    Check(replay.Panel!=null && mask.activeSelf && game.ModalInputBlocked,"Hidden closing panel retains outstanding click acquisition");
                    deadline=Time.time+.35f;phase=3;return;
                }
                if(phase==3)
                {
                    Check(replay.Panel==null && !mask.activeSelf && !game.ModalInputBlocked,"Global close destroys hidden panel and restores gameplay");
                    local.Bottom.Replay.onClick.Invoke();
                    deadline=Time.time+.35f;phase=4;return;
                }
                if(phase==4)
                {
                    seed=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data.LevelSeed;
                    replay.Panel.ReplayButton.onClick.Invoke();
                    Check(replay.Panel.Closing && game.IsRestarting && game.Level.Board==null,
                        "Actual Replay Button starts close and immediately clears board for reconstruction");
                    deadline=Time.time+.35f;phase=5;return;
                }
                Check(replay.Panel==null && !mask.activeSelf && !game.ModalInputBlocked && !game.IsRestarting,
                    "Normal replay completes close, mask release and actual scene reconstruction");
                var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                Check(UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data.LevelSeed==seed&&saved.LevelSeed==seed&&saved.LevelInfo==OriginalBoardSnapshotJson.Write(game.Level.CaptureSnapshot()),"Actual replay retains seed and local board checkpoint");
                Debug.Log("NUT_REPLAY_MASK_PLAY_VALIDATION_PASS normal startup binding, repeated acquisition, disabled-controller release, disabled-panel opening and inactive-panel close, actual main/continue/replay Buttons, modal retention, post-close release and scene restart with unchanged seed and local board checkpoint; counted UI shield is separate from registered modal state.");
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
