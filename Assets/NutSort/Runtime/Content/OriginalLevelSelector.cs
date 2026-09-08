using System;
using System.IO;

namespace NutSort.Content
{
    [Serializable]
    public sealed class LevelProgressState
    {
        // Original UserLocalData fields at offsets 0x2C, 0x30, 0x34, 0x3C.
        public int Level;
        public int LevelId;
        public int LevelSeed;
        public bool IsRandomLevelSeed;
    }

    public interface ILevelRandom { int Range(int minimumInclusive, int maximumExclusive); }

    public sealed class UnityLevelRandom : ILevelRandom
    {
        public static readonly UnityLevelRandom Instance = new UnityLevelRandom();
        private UnityLevelRandom() { }
        public int Range(int minimumInclusive, int maximumExclusive) => UnityEngine.Random.Range(minimumInclusive, maximumExclusive);
    }

    public readonly struct LevelSelection
    {
        public readonly bool Loop;
        public readonly int LevelId;
        public readonly int SeedIndex;
        public readonly string Seed;
        public LevelSelection(bool loop, int levelId, int seedIndex, string seed)
        {
            Loop = loop; LevelId = levelId; SeedIndex = seedIndex; Seed = seed;
        }
    }

    public sealed class OriginalLevelSelector
    {
        private readonly OriginalLevelRepository repository;
        private readonly OriginalContentSettings settings;
        private readonly ILevelRandom random;

        public OriginalLevelSelector(OriginalLevelRepository repository, OriginalContentSettings settings, ILevelRandom random)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.settings = settings != null ? settings : throw new ArgumentNullException(nameof(settings));
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        // Original GetLevelData RVA 0x9FCE64. LSS260820/LSSSHSLV are original server fields,
        // not assumed country or AB labels. No server calls are made here.
        public LevelSelection Select(LevelProgressState state, bool LSS260820, int LSSSHSLV)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            int id = state.Level;
            if (id > settings.FirstLevelId) id += settings.NormalLevelOffset;
            bool loop = id > settings.FinalPrimaryId;
            if (loop)
            {
                if (id >= settings.LoopMaxIdExclusive) id = random.Range(settings.LoopMinId, settings.LoopMaxIdExclusive);
            }
            else if (LSS260820)
            {
                if (id == settings.FirstLevelId) id = LSSSHSLV + state.LevelSeed;
                else if (id == settings.FirstLevelId + settings.GuideContinuationOffset) id = LSSSHSLV + settings.GuideContinuationOffset;
            }

            state.LevelId = id;
            LevelDataConfig catalog = loop ? repository.Loop : repository.Primary;
            if (catalog?.LevelDataInfos == null) throw new InvalidOperationException("Initialize original indexes before selecting a level.");
            LevelDataInfo entry = null;
            for (int i = 0; i < catalog.LevelDataInfos.Length; i++)
                if (catalog.LevelDataInfos[i].Id == id) { entry = catalog.LevelDataInfos[i]; break; }
            if (entry?.Seeds == null || entry.Seeds.Length == 0) throw new InvalidDataException("Original index has no seeds for level " + id);

            int seed = state.LevelSeed;
            if (seed >= entry.Seeds.Length) state.IsRandomLevelSeed = true;
            if (state.IsRandomLevelSeed) seed = random.Range(0, entry.Seeds.Length);
            // The original random branch does not overwrite persisted LevelSeed.
            return new LevelSelection(loop, id, seed, entry.Seeds[seed]);
        }
    }
}
