using System;
using System.IO;
using NutSort.Content;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalUserPlayValidation
    {
        private const string Active="NutSort.UserPlayValidation";
        private const string Fixture="{\"IsAudio\":false,\"IsVibrate\":false,\"RevokeCount\":3,\"LevelInfo\":\"preserve-original-board-string\",\"UserName\":\"2030-01-02T03:04:05Z\"}";
        private static string Backup=>Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationBackups/UserLocalData.bin"));
        private static int phase;
        private static double deadline,timeout;
        private static OriginalAudioPlayer audio;
        static OriginalUserPlayValidation()
        {
            if(SessionState.GetBool(Active,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}
        }
        public static void Run()
        {
            try
            {
            // Recover an interrupted previous validation before taking a new backup.
            Restore();Directory.CreateDirectory(Path.GetDirectoryName(Backup));
            using(var writer=new BinaryWriter(File.Create(Backup)))
            {
                writer.Write(PlayerPrefs.HasKey(OriginalUserStore.Key));
                writer.Write(PlayerPrefs.GetString(OriginalUserStore.Key,string.Empty));
            }
            PlayerPrefs.SetString(OriginalUserStore.Key,Fixture);PlayerPrefs.Save();
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Active,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"User state play timeout");
                if(audio==null)audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                if(audio==null)return;
                if(phase==0){phase=1;deadline=now+3;return;}
                if(now<deadline)return;
                if(phase==1)
                {
                    var user=audio.UserState.Data;
                    Check(!audio.AudioEnabled&&!user.IsVibrate&&user.RevokeCount==3&&user.LevelInfo=="preserve-original-board-string"&&user.UserName=="2030-01-02T03:04:05Z","Actual scene loads source user preference fields");
                    Check(!audio.Background.isPlaying&&audio.Background.clip==null&&audio.CachedClipCount==0,"Persisted mute prevents BGM and StageStart from loading or playing");
                    audio.SetAudioEnabled(true);
                    Check(user.IsAudio&&ReferenceEquals(user,audio.UserState.Store.Data),"Audio toggles shared user object");
                    Check(PlayerPrefs.GetString(OriginalUserStore.Key)==Fixture,"Toggle does not autosave or rewrite board data");
                    phase=2;deadline=now+.3;return;
                }
                Check(audio.Background.isPlaying,"Reenable starts real BGM");
                audio.SetAudioEnabled(false);Check(!audio.UserState.Data.IsAudio&&!audio.Background.isPlaying,"Disable mutates shared state and pauses BGM");
                audio.UserState.Store.SaveData(false,null);
                var restored=new OriginalUserStore(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"),new UnityUserPreferences());
                Check(!restored.Data.IsAudio&&!restored.Data.IsVibrate&&restored.Data.RevokeCount==3&&restored.Data.LevelInfo=="preserve-original-board-string","Actual PlayerPrefs full-envelope save/reload preserves board and settings");
                Restore();
                Debug.Log("NUT_USER_PLAY_VALIDATION_PASS actual source-key load, persisted mute gates BGM/StageStart, shared state, no toggle autosave, explicit whole-envelope PlayerPrefs save/reload; original preference restored.");
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Restore()
        {
            if(!File.Exists(Backup))return;
            bool existed;string value;
            using(var reader=new BinaryReader(File.OpenRead(Backup))){existed=reader.ReadBoolean();value=reader.ReadString();}
            if(existed)PlayerPrefs.SetString(OriginalUserStore.Key,value);else PlayerPrefs.DeleteKey(OriginalUserStore.Key);
            PlayerPrefs.Save();File.Delete(Backup);
        }
        private static void Finish(int code)
        {
            Restore();SessionState.SetBool(Active,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);
        }
        private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    }
}
