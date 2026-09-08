using System.IO;
using NutSort.World;
using UnityEditor;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalAudioValidation
    {
        public static void Validate()
        {
            var settings = Resources.Load<OriginalAudioSettings>("Configuration/OriginalAudio");
            Check(settings != null && settings.ResourcePrefix == "Audio/" && settings.BgmName == "BGM" && settings.SelectName == "Select" && settings.MoveNameFormat == "Move{0}", "Original audio configuration");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/NutSort/Prefabs/AudioVoice.prefab");
            var voice = prefab.GetComponent<AudioSource>();
            Check(voice.clip == null && !voice.playOnAwake && !voice.loop && voice.spatialBlend == 0 && voice.volume == 1 && voice.pitch == 1, "Original 2D source defaults and lazy clip loading");
            foreach (string name in new[] { "BGM", "Select", "Move1", "Move2", "Move3", "Click" })
            {
                var clip = Resources.Load<AudioClip>(settings.ResourcePrefix + name);
                Check(clip != null && clip.channels == 2 && clip.frequency == (name == "BGM" ? 11000 : 16000) && clip.samples > 0, "Original clip metadata: " + name);
                var importer = (AudioImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip));
                Check(!importer.forceToMono && !importer.loadInBackground && importer.defaultSampleSettings.loadType == (name == "Click" ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad), "Original clip import: " + name);
                Debug.Log("NUT_AUDIO_CLIP " + name + " samples=" + clip.samples + " frequency=" + clip.frequency + " channels=" + clip.channels);
            }
            Debug.Log("NUT_AUDIO_VALIDATION_PASS six original clips, source parameters and configuration.");
        }
        private static void Check(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
    }
}
