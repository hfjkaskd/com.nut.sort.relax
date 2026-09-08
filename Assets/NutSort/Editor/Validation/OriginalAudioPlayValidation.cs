using System;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalAudioPlayValidation
    {
        private const string Key = "NutSort.AudioPlayValidation";
        private static OriginalAudioPlayer player;
        private static AudioSource voice;
        private static readonly float[] pcm = new float[1024];
        private static int phase, started, beforeDelay;
        private static double deadline, timeout;
        private static bool bgmOutput, voiceOutput;
        static OriginalAudioPlayValidation()
        {
            if (SessionState.GetBool(Key, false)) { timeout = EditorApplication.timeSinceStartup + 60; EditorApplication.update += Tick; }
        }
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Key, true); EditorApplication.EnterPlaymode();
        }
        private static bool HasOutput(AudioSource source)
        {
            source.GetOutputData(pcm, 0);
            foreach (float sample in pcm) if (Mathf.Abs(sample) > .00001f) return true;
            return false;
        }
        private static void Tick()
        {
            if (!EditorApplication.isPlaying) return;
            try
            {
                double now = EditorApplication.timeSinceStartup;
                Check(now < timeout, "Audio play validation timeout: phase=" + phase);
                if (player == null) player = UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                if (player == null) return;
                if (phase == 0)
                {
                    if (!player.Background.isPlaying) return;
                    bgmOutput |= HasOutput(player.Background);
                    if (!bgmOutput) return;
                    Check(player.AudioEnabled && player.Background.loop && player.Background.spatialBlend == 0, "Startup BGM source");
                    player.SoundStarted += name => started++;
                    voice = player.PlaySound("Move3"); Check(voice != null, "Short audio started");
                    phase = 1; deadline = now + .25; return;
                }
                if (phase == 1) voiceOutput |= HasOutput(voice);
                if (now < deadline) return;
                switch (phase)
                {
                    case 1:
                        Check(voiceOutput && voice.isPlaying, "Short effect produces real PCM output");
                        player.SetAudioEnabled(false);
                        Check(!player.Background.isPlaying && voice.isPlaying, "Switch pauses BGM without stopping active effects");
                        int cache = player.CachedClipCount;
                        Check(player.PlaySound("Select") == null && player.CachedClipCount == cache, "Disabled sound never loads a clip");
                        player.PlaySound("Click", .1f); beforeDelay = started;
                        phase = 2; deadline = now + 1.6; break;
                    case 2:
                        Check(started == beforeDelay && player.ActiveVoiceCount == 0 && !voice.gameObject.activeSelf, "Delayed sound rechecks disabled state; finished voice returns to pool");
                        player.SetAudioEnabled(true); Check(player.Background.isPlaying, "BGM Play on reenable");
                        var reused = player.PlaySound("Select");
                        Check(reused == voice && reused.clip.name == "Select" && !reused.loop && reused.volume == 1 && reused.pitch == 1, "Voice reused with new clip and original defaults");
                        phase = 3; deadline = now + .85; break;
                    case 3:
                        Check(player.ActiveVoiceCount == 0, "Reused voice expires");
                        Time.timeScale = 0; beforeDelay = started; player.PlaySound("Click", .1f);
                        phase = 4; deadline = now + .3; break;
                    case 4:
                        Check(started == beforeDelay, "Delay obeys scaled time");
                        Time.timeScale = 1; phase = 5; deadline = now + .5; break;
                    case 5:
                        Check(started == beforeDelay + 1, "Delay plays after scaled time resumes");
                        player.PlaySound("Move2"); var second = player.PlaySound("Move1");
                        Check(player.ActiveVoiceCount == 2 && second != voice, "Overlapping effects use separate voices");
                        phase = 6; deadline = now + 1.4; break;
                    case 6:
                        Check(player.ActiveVoiceCount == 0 && player.Background.isPlaying, "Overlapping effects expire independently of BGM");
                        Debug.Log("NUT_AUDIO_PLAY_VALIDATION_PASS real BGM/effect PCM, startup, pause/re-enable, active-effect preservation, disabled cache gate, scaled delayed callback, pool reuse and overlapping voices.");
                        Finish(0); break;
                }
            }
            catch (Exception error) { Debug.LogException(error); Finish(1); }
        }
        private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
        private static void Finish(int code)
        {
            Time.timeScale = 1; SessionState.SetBool(Key, false); EditorApplication.update -= Tick; EditorApplication.Exit(code);
        }
    }
}
