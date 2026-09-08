using System;
using System.IO;
using System.Security.Cryptography;
using NutSort.Content;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalTablesValidation
    {
        public static void Validate()
        {
            var settings = Resources.Load<OriginalTableSettings>("Configuration/OriginalTables");
            Require(settings != null, "Serialized table archive configuration");
            var asset = Resources.Load<TextAsset>(settings.ResourcePath);
            var data = OriginalTableArchive.Read(asset.bytes, settings);
            Require(data.Count == 4, "Exactly four original tables");
            Hash(data["config.json"], "8d3449624a7171d6e8cacc77c4ef10442387e1e5701a96b896973039795453bf");
            Hash(data["level.json"], "f46580686300b82deed8de209836c016a140cb0894dd167251895cc0c1b55793");
            Hash(data["pay.json"], "5f5def3b0a3d9c91e1b2da082a7cba1774c3c7091b072860dd998c494381d33f");
            Hash(data["text.json"], "3cc600816fd7c66a9c6acd9c4439e2b4812eb028a6803cc7a669bead686b7bf0");
            Resources.UnloadAsset(asset);
            var tables = new OriginalTables(settings);
            Require(tables.LevelCount == 51 && tables.LastLevel.Level == 51 && tables.LastLevel.ShowLevel == 30, "Original level table extent");
            Require(tables.GetShowLevel(1) == 1 && tables.GetShowLevel(4) == 4 && tables.GetShowLevel(5) == 4 && tables.GetShowLevel(6) == 4, "Internal levels 4/5/6 share displayed level 4");
            Require(tables.GetLevelInfo(4, 4).Round == 1 && tables.GetLevelInfo(5, 5).Round == 2 && tables.GetLevelInfo(6, 6).Round == 3, "Original round progression");
            Require(tables.GetShowLevel(52) == 31 && tables.GetShowLevel(100) == 79, "Post-table level progression");
            OriginalLevelInfo overflow = tables.GetLevelInfo(52, 100);
            Require(overflow.Level == 52 && overflow.ShowLevel == 79 && overflow.TotalRound == 0 && overflow.Round == 0, "Overflow retains source current-player-level dependency and default round fields");
            Require(ReferenceEquals(tables.GetLevelInfo(4, 4), tables.GetLevelInfo(4, 100)), "Configured records retain source shared identity");
            Require(((JArray)tables.ReadTable("config.json")["CountryInfos"]).Count == 46, "Original 46 country records");
            Require(((JArray)tables.ReadTable("text.json")["list"]).Count == 187, "Original 187 localized text records");
            Debug.Log("NUT_TABLES_VALIDATION_PASS four payload hashes, source archive chain without reflection, 51 level mappings, rounds, overflow, country and text records.");
        }
        private static void Hash(byte[] bytes, string expected)
        {
            using (var hash = SHA256.Create())
                Require(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant() == expected, "Original table payload hash");
        }
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
    }
}
