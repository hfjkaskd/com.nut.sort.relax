using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalAudioPlayer : MonoBehaviour
    {
        [SerializeField] private OriginalAudioSettings settings;
        [SerializeField] private AudioSource background;
        [SerializeField] private AudioSource voicePrefab;
        private sealed class ClipEntry { public AudioClip Clip; public string ObjectName; }
        private struct Voice { public AudioSource Source; public int StartFrame; }
        private readonly Dictionary<string, ClipEntry> clips = new Dictionary<string, ClipEntry>(StringComparer.Ordinal);
        private readonly Stack<AudioSource> available = new Stack<AudioSource>();
        private readonly List<Voice> active = new List<Voice>();
        private OriginalLevelView level;
        private bool initialized;
        public bool AudioEnabled { get; private set; }
        public AudioSource Background => background;
        public int ActiveVoiceCount => active.Count;
        public int CachedClipCount => clips.Count;
        public event Action<string> SoundStarted;

        private void Awake() { Initialize(); }
        public void Initialize()
        {
            if (initialized) return;
            if (settings == null || background == null || voicePrefab == null)
                throw new InvalidOperationException("Original audio prefab references are incomplete.");
            AudioEnabled = settings.DefaultEnabled;
            initialized = true;
        }

        public void Bind(OriginalLevelView value)
        {
            Unbind(); level = value;
            level.SelectionSoundRequested += PlaySelection;
            level.MoveSoundRequested += PlayMove;
        }
        public void Unbind()
        {
            if (level == null) return;
            level.SelectionSoundRequested -= PlaySelection;
            level.MoveSoundRequested -= PlayMove;
            level = null;
        }
        private void PlaySelection() { PlaySound(settings.SelectName); }
        private void PlayMove(int count) { PlaySound(string.Format(CultureInfo.InvariantCulture, settings.MoveNameFormat, count)); }

        private ClipEntry GetClip(string name)
        {
            if (clips.TryGetValue(name, out ClipEntry entry)) return entry;
            string path = settings.ResourcePrefix + name;
            entry = new ClipEntry { Clip = Resources.Load<AudioClip>(path), ObjectName = "audio_" + name };
            clips.Add(name, entry);
            if (entry.Clip == null) Debug.LogError("GetTextAsset not find path: " + path);
            return entry;
        }

        public AudioSource PlaySound(string name)
        {
            Initialize();
            if (!AudioEnabled) return null;
            ClipEntry entry = GetClip(name);
            if (entry.Clip == null) return null;
            AudioSource source = null;
            while (available.Count > 0 && source == null) source = available.Pop();
            if (source == null) source = Instantiate(voicePrefab, transform, false);
            source.gameObject.name = entry.ObjectName;
            source.gameObject.SetActive(true);
            source.clip = entry.Clip;
            source.Play();
            active.Add(new Voice { Source = source, StartFrame = Time.frameCount });
            SoundStarted?.Invoke(name);
            return source;
        }

        public void PlaySound(string name, float delay) { StartCoroutine(PlayDelayed(name, delay)); }
        private IEnumerator PlayDelayed(string name, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySound(name); // Check the audio setting when the delay expires.
        }

        public void PlayBgm()
        {
            Initialize();
            if (!AudioEnabled) return;
            if (background.clip == null) background.clip = GetClip(settings.BgmName).Clip;
            background.Play();
        }
        public void SetAudioEnabled(bool value)
        {
            Initialize(); AudioEnabled = value;
            if (value) PlayBgm(); else background.Pause();
            // Source leaves already-playing short effects alone.
        }

        private void LateUpdate()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Voice voice = active[i];
                if (Time.frameCount <= voice.StartFrame || voice.Source.isPlaying) continue;
                voice.Source.Stop();
                voice.Source.clip = null;
                voice.Source.gameObject.SetActive(false);
                available.Push(voice.Source);
                active.RemoveAt(i);
            }
        }
        private void OnDestroy() { Unbind(); }
    }
}
