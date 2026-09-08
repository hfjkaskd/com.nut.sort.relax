using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NutSort.Content;
using UnityEditor;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalContentValidation
    {
        [Serializable] private sealed class Manifest { public Entry[] entries; }
        [Serializable] private sealed class Entry
        {
            public string resourcePath;
            public string sha256;
            public bool isIndex;
            public int entryCount;
            public int cellCount;
            public int filledCount;
            public string levelId;
        }

        // This is an offline validation entry point, not an alternative game initialization path.
        public static void Run()
        {
            try
            {
                OriginalPreferenceFixture.Begin();
                var settings = Resources.Load<OriginalContentSettings>("Configuration/OriginalContent");
                Require(settings != null, "Serialized original content settings must load.");
                var repository = new OriginalLevelRepository(settings);
                repository.Initialize();
                Require(repository.Primary.LevelDataInfos.Length == 219, "Original primary count 219.");
                Require(repository.Loop.LevelDataInfos.Length == 80, "Original loop count 80.");
                Require(repository.Primary.LevelDataInfos[0].Id == 1, "Primary starts at 1.");
                Require(repository.Loop.LevelDataInfos[0].Id == 220, "Loop starts at 220.");
                Require(repository.Loop.LevelDataInfos[79].Id == 299, "Loop ends at 299.");

                string manifestPath = Path.Combine(Application.dataPath, "NutSort/Editor/Validation/content-manifest.json");
                Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(manifestPath));
                Require(manifest.entries.Length == 1476, "All 1476 original encrypted resources are covered.");
                int boards = 0;
                foreach (Entry entry in manifest.entries)
                {
                    string json = repository.ReadJson(entry.resourcePath);
                    Require(Hash(json) == entry.sha256, "Decoded SHA-256 mismatch: " + entry.resourcePath);
                    if (entry.isIndex) continue;
                    LevelData board = OriginalLevelJson.ReadBoard(json);
                    Require(board.B.Length == entry.entryCount, "Screw count mismatch: " + entry.resourcePath);
                    Require(board.LId == entry.levelId, "Level identity mismatch: " + entry.resourcePath);
                    int cells = 0;
                    int nuts = 0;
                    foreach (ScrewData screw in board.B)
                    {
                        cells += screw.C.Length;
                        foreach (CData cell in screw.C) if (cell.BIM != null) nuts++;
                    }
                    Require(cells == entry.cellCount, "Cell count mismatch: " + entry.resourcePath);
                    Require(nuts == entry.filledCount, "Empty/filled cell mismatch: " + entry.resourcePath);
                    boards++;
                }
                Require(boards == 1474, "All 1474 original board payloads parse.");
                ValidateSeedReferences(repository, repository.Primary, false);
                ValidateSeedReferences(repository, repository.Loop, true);

                LevelData first = repository.LoadBoard(false, "4b56d_1_1-1");
                Require(first.CC == 1 && first.UCCC == 0 && first.B.Length == 2, "Original first board layout.");
                Require(first.B[0].C.Length == 4 && first.B[1].C.Length == 4, "Original first board capacities.");
                int filled = 0;
                foreach (ScrewData screw in first.B)
                    foreach (CData cell in screw.C)
                        if (cell.BIM != null)
                        {
                            Require(cell.BIM.Id == 1 && cell.BIM.CI == 11 && cell.BIM.V, "Original first board nut values.");
                            filled++;
                        }
                Require(filled == 4, "Missing BIM must remain an empty slot, not a default nut.");
                Expect<FormatException>(() => OriginalContentCodec.Decrypt("invalid base64!", settings.Key, settings.IV));
                Expect<FileNotFoundException>(() => repository.ReadJson("LevelConfig/missing-evidence"));
                Expect<ArgumentException>(() => repository.LoadBoard(false, "../invalid"));
                OriginalSelectionValidation.Validate(repository, settings);
                OriginalTablesValidation.Validate();
                OriginalMainLevelValidation.Validate();
                OriginalLayoutValidation.Validate();
                OriginalGameplayValidation.Validate(repository);
                OriginalRenderingValidation.Validate();
                OriginalPrefabValidation.Validate();
                OriginalNutMotionValidation.Validate(repository);
                OriginalTransferValidation.Validate(repository);
                OriginalScrewValidation.Validate(repository);
                OriginalLevelViewValidation.Validate(repository);
                OriginalExchangeScrewValidation.Validate(repository);
                OriginalExchangeOperationValidation.Validate(repository);
                OriginalAddScrewFlowValidation.Validate();
                OriginalAddTileValidation.Validate(repository);
                OriginalUnlockScrewValidation.Validate(repository);
                OriginalAddNullScrewValidation.Validate(repository);
                OriginalSceneRowsValidation.Validate(repository);
                OriginalLockedScrewDispatchValidation.Validate(repository);
                OriginalMoveCompletionValidation.Validate(repository);
                OriginalSuccessFlowValidation.Validate();
                OriginalSuccessResponseValidation.Validate();
                OriginalSuccessSettlementValidation.Validate();
                OriginalRewardItemValidation.Validate();
                OriginalSuccessPanelFlowValidation.Validate();
                OriginalSuccessRewardValidation.Validate();
                OriginalRewardGetFlowValidation.Validate();
                OriginalSuccessPanelValidation.Validate();
                OriginalRewardPanelValidation.Validate();
                OriginalToolRewardFlightValidation.Validate();
                OriginalCurrencyRewardFlightValidation.Validate();
                OriginalEffectsValidation.Validate();
                OriginalAudioValidation.Validate();
                OriginalUserValidation.Validate();
                OriginalBoardSnapshotValidation.Validate(repository);
                OriginalLockedScrewValidation.Validate(repository);
                OriginalResumeValidation.Validate(repository);
                OriginalGameplayUnlockValidation.Validate();
                OriginalGameplayUnlockPanelValidation.Validate();
                OriginalGameplayUnlockDisplayValidation.Validate();
                OriginalGameplayUnlockContinueValidation.Validate();
                OriginalGameplayUnlockConfigValidation.Validate();
                OriginalInitializationWaitValidation.Validate();
                OriginalInitializationContinuationValidation.Validate();
                OriginalInitializationFlowValidation.Validate();
                OriginalEveryDayGiftValidation.Validate();
                OriginalTeachingFlowValidation.Validate();
                OriginalNewbieGuideValidation.Validate();
                OriginalNewbieMaskPlacementValidation.Validate();
                OriginalNewbieSuccessGuideValidation.Validate();
                OriginalWithdrawalGuideValidation.Validate();
                OriginalGuideRoutingValidation.Validate();
                OriginalCoinGuideValidation.Validate();
                OriginalGoldEntryGuideValidation.Validate();
                OriginalWithdrawalStageGuideValidation.Validate();
                OriginalTargetGuideReturnValidation.Validate();
                OriginalGuideTargetRequestValidation.Validate();
                OriginalWithdrawalPanelFlowValidation.Validate();
                OriginalWithdrawalClaimValidation.Validate();
                OriginalWithdrawalHeaderValidation.Validate();
                OriginalWithdrawalPendingGoldValidation.Validate();
                OriginalWithdrawalGoldTweenValidation.Validate();
                OriginalWithdrawalEarlyProgressValidation.Validate();
                OriginalWithdrawalLaterProgressValidation.Validate();
                OriginalPlayerInfoValidation.Validate();
                OriginalSimpleMarqueeValidation.Validate();
                OriginalWithdrawalRefreshValidation.Validate();
                OriginalWithdrawalPunchValidation.Validate();
                OriginalWithdrawalPanelValidation.Validate();
                OriginalWithdrawalLoadingValidation.Validate();
                OriginalWithdrawalUserInfoValidation.Validate();
                OriginalGuideBranchAdapterValidation.Validate();
                OriginalNewbieGuideHostValidation.Validate();
                OriginalNewbieInteractionValidation.Validate();
                OriginalNewbieCompletionValidation.Validate();
                OriginalGuideTargetCompletionValidation.Validate();
                OriginalGuideTargetPanelValidation.Validate();
                OriginalHollowMaskGeometryValidation.Validate();
                OriginalDeadlockValidation.Validate();
                OriginalRecordGuideValidation.Validate();
                OriginalRewardProgressValidation.Validate();
                OriginalCoinProgressValidation.Validate();
                OriginalMarqueeDataValidation.Validate();
                OriginalMarqueeTextValidation.Validate();
                OriginalMarqueeNameValidation.Validate();
                OriginalPayChannelsValidation.Validate();
                OriginalMarqueeItemValidation.Validate();
                OriginalMarqueeLauncherValidation.Validate();
                OriginalMainTopFlowValidation.Validate();
                OriginalMainTopViewValidation.Validate();
                OriginalToolItemDisplayValidation.Validate();
                OriginalToolItemControllerValidation.Validate();
                OriginalMainBottomValidation.Validate();
                OriginalMainPanelValidation.Validate();
                OriginalItemManagerValidation.Validate();
                OriginalRevokeFlowValidation.Validate();
                OriginalRecordRevokeValidation.Validate(repository);
                OriginalBoardRevokeIntegrationValidation.Validate(repository);
                OriginalFailureFlowValidation.Validate();
                OriginalFailureRestartValidation.Validate();
                OriginalReplayRestartValidation.Validate();
                OriginalFailurePanelDataValidation.Validate();
                OriginalFailureSparkleValidation.Validate();
                OriginalPunchPathValidation.Validate();
                OriginalPunchRotationValidation.Validate();
                OriginalFailurePanelValidation.Validate();
                OriginalPanelRegistryValidation.Validate();
                OriginalPanelActionQueueValidation.Validate();
                OriginalPanelCloseAllValidation.Validate();
                OriginalCountedMaskValidation.Validate();
                OriginalButtonGateValidation.Validate();
                OriginalPlayerGoldHintScheduleValidation.Validate();
                OriginalPlayerGoldHintTextValidation.Validate();
                OriginalHintIconsValidation.Validate();
                OriginalPlayerGoldHintViewValidation.Validate();
                OriginalTargetRewardValidation.Validate();
                OriginalRewardProgressViewValidation.Validate();
                OriginalGoldItemValidation.Validate();
                OriginalCoinItemValidation.Validate();
                OriginalHiddenLevelValidation.Validate();
                OriginalUIAnimationValidation.Validate();
                OriginalReplayValidation.Validate();
                OriginalSceneValidation.Validate();
                Debug.Log("NUT_CONTENT_VALIDATION_PASS resources=1476 boards=1474 primary=219 loop=80; original first board and seed references verified.");
                OriginalPreferenceFixture.Restore();
                EditorApplication.Exit(0);
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                OriginalPreferenceFixture.Restore();
                EditorApplication.Exit(1);
            }
        }

        private static void ValidateSeedReferences(OriginalLevelRepository repository, LevelDataConfig index, bool loop)
        {
            foreach (LevelDataInfo level in index.LevelDataInfos)
            {
                Require(level.Seeds != null && level.Seeds.Length != 0, "Level has original seeds.");
                foreach (string seed in level.Seeds) Require(repository.LoadBoard(loop, seed).B != null, "Referenced board loads.");
            }
        }

        private static string Hash(string value)
        {
            using (SHA256 algorithm = SHA256.Create())
            {
                byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
                var result = new StringBuilder(64);
                for (int i = 0; i < hash.Length; i++) result.Append(hash[i].ToString("x2"));
                return result.ToString();
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }

        private static void Expect<T>(Action operation) where T : Exception
        {
            try { operation(); }
            catch (T) { return; }
            throw new InvalidDataException("Expected " + typeof(T).Name);
        }
    }
}
