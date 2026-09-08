using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUserValidation
    {
        private sealed class MemoryPreferences : IOriginalUserPreferences
        {
            public string Value;
            public int Writes, Flushes;
            public string GetString(string key,string fallback) { Check(key==OriginalUserStore.Key,"Original read key");return Value??fallback; }
            public void SetString(string key,string value) { Check(key==OriginalUserStore.Key,"Original write key");Value=value;Writes++; }
            public void Save() { Flushes++; }
        }
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            Check(defaults!=null&&defaults.Level==1&&defaults.LevelId==1&&defaults.RevokeCount==5&&defaults.IsAudio&&defaults.IsVibrate,"Original constructor defaults");
            var empty=new MemoryPreferences();var store=new OriginalUserStore(defaults,empty);
            Check(store.Data.Level==1&&store.Data.LevelId==1&&store.Data.RevokeCount==5&&store.Data.IsAudio&&store.Data.IsVibrate,"Empty preferences construct source defaults");
            Check(store.Data.UserId==null&&store.Data.UserName==null&&store.Data.Gold==0&&store.Data.Coin==0&&store.Data.ServerConfigData==null,"No invented SDK profile, money or server response");
            Check(empty.Writes==0&&empty.Flushes==0,"Loading never silently writes a save");
            string fixture=File.ReadAllText(Path.Combine(Application.dataPath,"NutSort/Editor/Validation/user-data-fixture.json"));
            var data=OriginalUserDataJson.Read(fixture,defaults);
            var expected=JObject.Parse(fixture);var actual=JObject.Parse(OriginalUserDataJson.Write(data));
            Check(expected.Count==62&&JToken.DeepEquals(expected,actual),"All 62 top-level fields round-trip with nested documents and exact strings");
            Check(data.RegisterTime==9007199254740993L&&data.LoginTime==long.MaxValue,"64-bit timestamps avoid double precision loss");
            int level=data.Level,revoke=data.RevokeCount;string board=data.LevelInfo;bool audio=data.IsAudio;
            data.Init();Check(data.CurrentLevelAddScrewCount==0&&data.Level==level&&data.RevokeCount==revoke&&data.LevelInfo==board&&data.IsAudio==audio,"Source Init resets only CurrentLevelAddScrewCount");
            var mixed=OriginalUserDataJson.Read("{\"IsAudio\":true,\"isaudio\":false,\"IsAudio\":true,\"LEVEL\":7,\"unknown\":123}",defaults);
            Check(mixed.IsAudio&&mixed.Level==7&&mixed.RevokeCount==5&&JObject.Parse(OriginalUserDataJson.Write(mixed))["unknown"]==null,"Duplicate/case-varied field order, omitted defaults and unknown fields");
            Check(!OriginalUserDataJson.Read("{\"IsAudio\":/*comment*/false}",defaults).IsAudio,"Json.NET comments remain accepted");
            Check(OriginalUserDataJson.Read("null",defaults)==null&&OriginalUserDataJson.Read("   ",defaults)==null,"Nonempty null/whitespace documents do not become new users");
            foreach(string invalid in new[]{"{","[]","{} {}","null true","{\"Level\":null}","{\"IsAudio\":[]}"})
            {
                bool failed=false;try{OriginalUserDataJson.Read(invalid,defaults);}catch(Exception){failed=true;}
                Check(failed,"Malformed source user input must fail: "+invalid);
            }
            var prefs=new MemoryPreferences{Value=fixture};store=new OriginalUserStore(defaults,prefs);
            store.Data.IsAudio=false;Check(prefs.Value==fixture&&prefs.Writes==0,"Audio changes memory without creating a separate preference or autosaving");
            string savedBoard=store.Data.LevelInfo;store.SaveData(false,"ignored");
            Check(prefs.Writes==1&&prefs.Flushes==1&&OriginalUserDataJson.Read(prefs.Value,defaults).LevelInfo==savedBoard,"No-current-board save retains original LevelInfo string and flushes once");
            string replacement="{\"ScrewInfos\":[],\"OperatorInfos\":[]}";store.SaveData(true,replacement);
            Check(prefs.Writes==2&&prefs.Flushes==2&&OriginalUserDataJson.Read(prefs.Value,defaults).LevelInfo==replacement,"Current-board JSON is nested as a string, not a new save format");
            var nullPrefs=new MemoryPreferences{Value="null"};new OriginalUserStore(defaults,nullPrefs).SaveData(false,null);
            Check(nullPrefs.Writes==0&&nullPrefs.Flushes==0,"Source null-user SaveData is a no-op");
            Debug.Log("NUT_USER_VALIDATION_PASS 62 original top-level fields, defaults, exact 64-bit values, nested document preservation, parsing rules, reset scope and original save envelope/key/flush semantics.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidDataException(message);}
    }
}
