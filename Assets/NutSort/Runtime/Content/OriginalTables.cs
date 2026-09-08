using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Content
{
    public sealed class OriginalLevelInfo
    {
        public int Level, ShowLevel, TotalRound, Round, SubTotalRound, SubRound;
    }

    public sealed class OriginalTables
    {
        private readonly Dictionary<string, byte[]> payloads;
        private readonly OriginalLevelInfo[] levels;
        public int LevelCount => levels.Length;
        public OriginalLevelInfo LastLevel => levels[levels.Length - 1];

        public OriginalTables(OriginalTableSettings settings)
        {
            var asset = Resources.Load<TextAsset>(settings.ResourcePath);
            if (asset == null) throw new FileNotFoundException("Original table archive missing.", settings.ResourcePath);
            try { payloads = OriginalTableArchive.Read(asset.bytes, settings); }
            finally { Resources.UnloadAsset(asset); }
            var rows = (JArray)ReadTable("level.json")["list"];
            levels = new OriginalLevelInfo[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                JToken row = rows[i];
                levels[i] = new OriginalLevelInfo { Level = (int)row["level"], ShowLevel = (int)row["showLevel"],
                    TotalRound = (int)row["totalRound"], Round = (int)row["round"],
                    SubTotalRound = (int)row["subTotalRound"], SubRound = (int)row["subRound"] };
            }
        }

        public JObject ReadTable(string name) => JObject.Parse(Encoding.UTF8.GetString(payloads[name]));

        public OriginalLevelInfo GetLevelInfo(int requestedLevel, int playerLevel)
        {
            // The source overflow branch uses the current user's level for
            // ShowLevel, even when the queried level differs from that value.
            if (requestedLevel >= levels.Length + 1)
                return new OriginalLevelInfo { Level = requestedLevel, ShowLevel = playerLevel - LastLevel.Level + LastLevel.ShowLevel };
            for (int i = 0; i < levels.Length; i++) if (levels[i].Level == requestedLevel) return levels[i];
            Debug.LogError("LevelTable not levelInfo: " + requestedLevel);
            return null;
        }

        public int GetShowLevel(int playerLevel)
        {
            OriginalLevelInfo info = GetLevelInfo(playerLevel, playerLevel);
            return info != null ? info.ShowLevel : playerLevel - LastLevel.Level + LastLevel.ShowLevel;
        }
    }
}
