using System;
using UnityEngine;
namespace NutSort.Content
{
    public interface IOriginalUserPreferences
    {
        string GetString(string key, string fallback);
        void SetString(string key, string value);
        void Save();
    }
    public sealed class UnityUserPreferences : IOriginalUserPreferences
    {
        public string GetString(string key,string fallback)=>PlayerPrefs.GetString(key,fallback);
        public void SetString(string key,string value)=>PlayerPrefs.SetString(key,value);
        public void Save()=>PlayerPrefs.Save();
    }
    public sealed class OriginalUserStore
    {
        public const string Key="UserLocalData";
        private readonly IOriginalUserPreferences preferences;
        public OriginalUserLocalData Data { get; }
        public OriginalUserStore(OriginalUserDefaults defaults,IOriginalUserPreferences storage)
        {
            if(defaults==null)throw new ArgumentNullException(nameof(defaults));
            preferences=storage??throw new ArgumentNullException(nameof(storage));
            string json=preferences.GetString(Key,string.Empty);
            Data=string.IsNullOrEmpty(json)?new OriginalUserLocalData(defaults):OriginalUserDataJson.Read(json,defaults);
        }
        // Source SaveData skips a null user. It replaces LevelInfo only when
        // a current board exists, then writes the entire user envelope and flushes.
        // A caller must supply the original board JSON; no new board format here.
        public void SaveData(bool hasCurrentBoard,string currentBoardJson)
        {
            if(Data==null)return;
            if(hasCurrentBoard)Data.LevelInfo=currentBoardJson;
            preferences.SetString(Key,OriginalUserDataJson.Write(Data));
            preferences.Save();
        }
    }
}
