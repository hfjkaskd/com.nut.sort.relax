using System.IO;
using System.Security.Cryptography;
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
            CheckPanelAudio("GameLose","abc350184d7491ff6a9d297bbdf4b83516f8de42fd71cad168493b487477e863");
            CheckPanelAudio("Reward_appear","c0238db39f2c4dd39f3dba317ae36be5f6cccefab44bb51f3daacedcf7ba7e8e");
            Debug.Log("NUT_AUDIO_VALIDATION_PASS six original base clips plus original failure/unlock panel audio hashes, native import settings and loadable clips.");
        }
        private static void CheckPanelAudio(string name,string expectedHash)
        {
            var clip=Resources.Load<AudioClip>("Audio/"+name);
            Check(clip!=null&&clip.samples>0,"Core panel clip is loadable: "+name);
            string path=AssetDatabase.GetAssetPath(clip);
            using(var stream=File.OpenRead(path))using(var sha=SHA256.Create())
                Check(System.BitConverter.ToString(sha.ComputeHash(stream)).Replace("-",string.Empty).ToLowerInvariant()==expectedHash,"Original panel audio payload: "+name);
            var importer=(AudioImporter)AssetImporter.GetAtPath(path);
            Check(!importer.forceToMono&&!importer.loadInBackground&&importer.defaultSampleSettings.loadType==AudioClipLoadType.DecompressOnLoad,"Original panel audio import: "+name);
        }
        private static void Check(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
    }
}
