using System;
using System.IO;
using NutSort.World;
using NutSort.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    // Offline authoring only. Runtime uses these serialized scene/prefab objects.
    public static class OriginalAudioAuthoring
    {
        public static void Run()
        {
            try
            {
                const string config = "Assets/Resources/Configuration/OriginalAudio.asset";
                var settings = AssetDatabase.LoadAssetAtPath<OriginalAudioSettings>(config);
                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<OriginalAudioSettings>();
                    AssetDatabase.CreateAsset(settings, config);
                }
                settings.DefaultEnabled = true; settings.ResourcePrefix = "Audio/";
                settings.BgmName = "BGM"; settings.SelectName = "Select"; settings.MoveNameFormat = "Move{0}";
                EditorUtility.SetDirty(settings);
                Directory.CreateDirectory("Assets/NutSort/Prefabs"); AssetDatabase.Refresh();
                var voiceObject = new GameObject("AudioVoice");
                var voice = voiceObject.AddComponent<AudioSource>();
                voice.playOnAwake = false; voice.loop = false; voice.spatialBlend = 0;
                var voicePrefab = PrefabUtility.SaveAsPrefabAsset(voiceObject, "Assets/NutSort/Prefabs/AudioVoice.prefab");
                UnityEngine.Object.DestroyImmediate(voiceObject);
                var scene = EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                OriginalAudioPlayer player = null; OriginalGameScene game = null; OriginalStartupFlow flow = null;
                foreach (var root in scene.GetRootGameObjects())
                {
                    if (player == null) player = root.GetComponentInChildren<OriginalAudioPlayer>(true);
                    if (game == null) game = root.GetComponentInChildren<OriginalGameScene>(true);
                    if (flow == null) flow = root.GetComponentInChildren<OriginalStartupFlow>(true);
                }
                if (game == null || flow == null) throw new InvalidDataException("Original startup/game scene missing.");
                if (player == null)
                {
                    var root = new GameObject("AudioMgr");
                    player = root.AddComponent<OriginalAudioPlayer>();
                    var child = new GameObject("bgm-ui_01"); child.transform.SetParent(root.transform, false);
                    var bgm = child.AddComponent<AudioSource>(); bgm.playOnAwake = false; bgm.loop = true; bgm.spatialBlend = 0;
                    Set(player, "background", bgm);
                }
                Set(player, "settings", settings); Set(player, "voicePrefab", voicePrefab.GetComponent<AudioSource>());
                Set(game, "audioPlayer", player); Set(flow, "audioPlayer", player);
                EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
                OriginalAudioValidation.Validate();
                Debug.Log("NUT_AUDIO_AUTHORING_PASS serialized scene audio, source prefab and settings.");
                EditorApplication.Exit(0);
            }
            catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
        }
        private static void Set(UnityEngine.Object target, string name, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target); serialized.FindProperty(name).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
