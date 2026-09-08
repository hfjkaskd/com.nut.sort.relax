using System;
using System.Collections.Generic;
using System.IO;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalSelectionValidation
    {
        private sealed class LastValueRandom : ILevelRandom
        {
            public readonly List<Vector2Int> Ranges = new List<Vector2Int>();
            public int Range(int minimumInclusive, int maximumExclusive)
            {
                Ranges.Add(new Vector2Int(minimumInclusive, maximumExclusive));
                return maximumExclusive - 1;
            }
        }

        public static void Validate(OriginalLevelRepository repository, OriginalContentSettings settings)
        {
            Check(repository, settings, 1, 1, false);
            Check(repository, settings, 2, 5, false);
            Check(repository, settings, 216, 219, false);
            Check(repository, settings, 217, 220, true);
            Check(repository, settings, 296, 299, true);

            var random = new LastValueRandom();
            var selector = new OriginalLevelSelector(repository, settings, random);
            var state = new LevelProgressState { Level = 297, LevelSeed = 0 };
            LevelSelection selected = selector.Select(state, false, 1);
            Require(selected.Loop && selected.LevelId == 299, "Beyond final loop entry rerolls into original range.");
            Require(random.Ranges.Count == 1 && random.Ranges[0] == new Vector2Int(220, 300), "Original loop random bounds.");

            random.Ranges.Clear();
            state = new LevelProgressState { Level = 1, LevelSeed = 2 };
            selected = selector.Select(state, true, 2);
            Require(selected.LevelId == 4 && !selected.Loop, "Original special first-level remap.");
            Require(state.LevelId == 4 && state.LevelSeed == 2 && state.IsRandomLevelSeed, "Seed overflow is sticky without overwriting persisted seed.");
            Require(random.Ranges.Count == 1 && random.Ranges[0] == new Vector2Int(0, 1), "Overflow random bound comes from selected original index.");

            state = new LevelProgressState { Level = 2, LevelSeed = 0 };
            selected = selector.Select(state, true, 2);
            Require(selected.LevelId == 6, "Original special next-level remap uses start + 4.");

            random.Ranges.Clear();
            state = new LevelProgressState { Level = 1, LevelSeed = 0, IsRandomLevelSeed = true };
            selected = selector.Select(state, false, 1);
            Require(selected.SeedIndex == 3 && state.LevelSeed == 0 && state.IsRandomLevelSeed, "Previously random seed remains random.");
            Require(random.Ranges[0] == new Vector2Int(0, 4), "First original index has four seeds.");
            Require(repository.LoadBoard(selected.Loop, selected.Seed).B != null, "Selected original payload loads.");
            Debug.Log("NUT_SELECTION_VALIDATION_PASS primary boundaries, loop bounds, special guide mapping, overflow and sticky random seed verified.");
        }

        private static void Check(OriginalLevelRepository repository, OriginalContentSettings settings, int level, int id, bool loop)
        {
            var random = new LastValueRandom();
            var state = new LevelProgressState { Level = level, LevelSeed = 0 };
            LevelSelection result = new OriginalLevelSelector(repository, settings, random).Select(state, false, 1);
            Require(result.LevelId == id && state.LevelId == id && result.Loop == loop, "Original level mapping " + level);
            Require(random.Ranges.Count == 0, "In-range sequential seed must not consume RNG.");
            Require(repository.LoadBoard(loop, result.Seed).B.Length > 0, "Mapped original board exists.");
        }

        private static void Require(bool result, string detail)
        {
            if (!result) throw new InvalidDataException(detail);
        }
    }
}
