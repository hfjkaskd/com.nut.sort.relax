using System;
using System.IO;
using UnityEngine;

namespace NutSort.Content
{
    public sealed class OriginalLevelRepository
    {
        private readonly OriginalContentSettings settings;
        public LevelDataConfig Primary { get; private set; }
        public LevelDataConfig Loop { get; private set; }

        public OriginalLevelRepository(OriginalContentSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            this.settings = settings;
        }

        // Original InitLevelConfig/InitLoopLevelConfig, RVA 0x9FBAE4 / 0x9FBC04.
        public void Initialize()
        {
            LevelDataConfig primary = OriginalLevelJson.ReadIndex(ReadJson(settings.PrimaryIndexPath));
            LevelDataConfig loop = OriginalLevelJson.ReadIndex(ReadJson(settings.LoopIndexPath));
            if (primary?.LevelDataInfos == null || loop?.LevelDataInfos == null)
                throw new InvalidDataException("Original level index is missing LevelDataInfos.");
            Primary = primary;
            Loop = loop;
        }

        public string ReadJson(string resourcePath)
        {
            TextAsset asset = Resources.Load<TextAsset>(resourcePath);
            if (asset == null) throw new FileNotFoundException("Original TextAsset not found: " + resourcePath, resourcePath);
            try { return OriginalContentCodec.Decrypt(asset.text, settings.Key, settings.IV); }
            finally { Resources.UnloadAsset(asset); }
        }

        // The caller supplies the original selected seed. Selection/progression is a separate rule.
        public LevelData LoadBoard(bool loop, string seed)
        {
            if (string.IsNullOrEmpty(seed) || seed.IndexOfAny(new[] { '/', '\\', '.' }) >= 0)
                throw new ArgumentException("A board seed must be a resource name.", nameof(seed));
            string directory = loop ? settings.LoopBoardDirectory : settings.PrimaryBoardDirectory;
            LevelData board = OriginalLevelJson.ReadBoard(ReadJson(directory + "/" + seed));
            if (board?.B == null) throw new InvalidDataException("Original board is missing B.");
            return board;
        }
    }
}
