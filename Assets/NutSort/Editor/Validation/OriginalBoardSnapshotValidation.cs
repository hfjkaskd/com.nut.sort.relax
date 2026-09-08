using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalBoardSnapshotValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var layout = Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            Check(layout != null, "Original layout exists");
            string fixture = @"{""ScrewInfos"":[{""Id"":17,""Index"":0,""NutMaxCount"":7,
                ""Coordinate"":{""x"":99,""y"":-2},""NutInfos"":[
                {""NutPos"":{""x"":2,""y"":0,""z"":3},""NutData"":{""NutType"":2,""NutColor"":11,""V"":false}},
                {""NutPos"":{""x"":2,""y"":1,""z"":3},""NutData"":null}],
                ""ScrewMaskDatas"":[{""ScrewType"":4,""NutColor"":6,""Obj"":{""Id"":5,""TA"":2,""NutColor"":12,""IsShow"":false},""IsShow"":true},
                {""ScrewType"":7,""NutColor"":0,""Obj"":null,""IsShow"":false}],""IsLocked"":true}],
                ""OperatorInfos"":[{""FromScrewIndex"":2,""ToScrewIndex"":0,""NutCount"":3}]}";
            var snapshot = OriginalBoardSnapshotJson.Read(fixture);
            EqualJson(fixture, OriginalBoardSnapshotJson.Write(snapshot), "Independent source-shaped fixture preserves all fields and nulls");
            var restored = snapshot.Restore(layout);
            Check(restored.LevelId == null && restored.Screws[0].Capacity == 7 && restored.Screws[0].Slots.Length == 2,
                "Capacity is independent of slot count; no invented level identifier");
            Check(restored.Screws[0].Coordinate == layout.Coordinate(1,0), "Source runtime initialization replaces saved screw coordinate");
            Check(restored.Screws[0].Slots[0].Coordinate == new Vector3Int(2,0,3), "Fixed nut position is preserved");
            Check(restored.Screws[0].Slots[0].Nut.Type == NutType.Hidden && !restored.Screws[0].Slots[0].Nut.V &&
                restored.Screws[0].Slots[1].Nut == null && restored.Screws[0].IsLocked, "Hidden nut, V, empty slot and lock restored");
            Check(!restored.Screws[0].Masks[0].Object.IsShow && !restored.Screws[0].Masks[1].IsShow,
                "Independent mask and object visibility restored");
            restored.Screws[0].Masks[0].Object.IsShow = true;
            Check(!snapshot.ScrewInfos[0].ScrewMaskDatas[0].Obj.IsShow, "Restoration owns its mutable state");

            var defaults = OriginalBoardSnapshotJson.Read(@"{""ScrewInfos"":[{""ScrewMaskDatas"":[{""Obj"":{}}]}]}");
            Check(defaults.OperatorInfos.Count == 0 && defaults.ScrewInfos[0].NutInfos.Count == 0 &&
                defaults.ScrewInfos[0].NutMaxCount == 0 && defaults.ScrewInfos[0].Coordinate == null &&
                defaults.ScrewInfos[0].ScrewMaskDatas[0].IsShow && defaults.ScrewInfos[0].ScrewMaskDatas[0].Obj.IsShow,
                "Source constructor defaults, including true mask visibility");
            var duplicates = OriginalBoardSnapshotJson.Read(@"{/*before*/""screwinfos"":[{""Id"":1,""id"":2,""Id"":3,""Index"":/*value*/0,""IsReady"":true}],""Level"":{""ignored"":[]}}");
            Check(duplicates.ScrewInfos[0].Id == 3, "Property order and case variants obey last assignment");
            string clean = OriginalBoardSnapshotJson.Write(duplicates);
            Check(!clean.Contains("IsReady") && !clean.Contains("Level\""), "Unknown runtime fields are discarded");
            EqualJson(@"{""ScrewInfos"":null,""OperatorInfos"":null}", OriginalBoardSnapshotJson.Write(
                OriginalBoardSnapshotJson.Read(@"{""ScrewInfos"":null,""OperatorInfos"":null}")), "Explicit null lists preserved");
            Check(OriginalBoardSnapshotJson.Read("null") == null && OriginalBoardSnapshotJson.Read("  ") == null,
                "Source null and empty input semantics");
            ExpectFailure(() => OriginalBoardSnapshotJson.Read("{broken"));
            ExpectFailure(() => OriginalBoardSnapshotJson.Read("{} {}"));
            ExpectFailure(() => OriginalBoardSnapshotJson.Read(@"{""ScrewInfos"":[{""Id"":null}]}"));
            ExpectFailure(() => OriginalBoardSnapshotJson.Read(@"{""ScrewInfos"":[{""Id"":2147483648}]}"));
            ExpectFailure(() => OriginalBoardSnapshotJson.Read(@"{""ScrewInfos"":{}}"));
            ExpectFailure(() => OriginalBoardSnapshotJson.Read(@"{""ScrewInfos"":null}").Restore(layout));

            var history = new List<OriginalMoveRecord>();
            int boards = 0;
            foreach (LevelDataInfo entry in repository.Primary.LevelDataInfos)
                foreach (string seed in entry.Seeds) { RoundTrip(repository.LoadBoard(false, seed), layout, history); boards++; }
            foreach (LevelDataInfo entry in repository.Loop.LevelDataInfos)
                foreach (string seed in entry.Seeds) { RoundTrip(repository.LoadBoard(true, seed), layout, history); boards++; }
            Check(boards > 0, "Repository seeds validated");
            ValidateView(repository);
            Debug.Log("NUT_BOARD_SNAPSHOT_VALIDATION_PASS source field envelope, nulls/defaults, masks/locks, numeric enums, runtime exclusions, seed round trips=" + boards + "; native prefab restore and compact operation history; automatic startup/save routing remains pending.");
        }

        private static void RoundTrip(LevelData data, OriginalLayoutSettings layout, List<OriginalMoveRecord> history)
        {
            var board = new OriginalBoardState(data, layout);
            string json = OriginalBoardSnapshotJson.Write(OriginalBoardSnapshot.Capture(board, history));
            var snapshot = OriginalBoardSnapshotJson.Read(json);
            var restored = snapshot.Restore(layout);
            EqualJson(json, OriginalBoardSnapshotJson.Write(OriginalBoardSnapshot.Capture(restored, snapshot.OperatorInfos)),
                "Every source board preserves its serialized state");
        }

        private static void ValidateView(OriginalLevelRepository repository)
        {
            var holder = new GameObject("Snapshot native prefab validation");
            OriginalLevelView view = null;
            try
            {
                var pool = holder.AddComponent<OriginalPrefabPool>();
                view = pool.Rent("Game/Level", holder.transform).GetComponent<OriginalLevelView>();
                LevelData first = repository.LoadBoard(false,"4b56d_1_1-1");
                view.Bind(first,pool,false,true);
                view.AdvanceInitialization(.501f);
                Check(view.Operate(1).Kind == ScrewOperationKind.Ready, "Select original right nut");
                string selectedJson = OriginalBoardSnapshotJson.Write(view.CaptureSnapshot());
                Check(!selectedJson.Contains("IsReady") && !selectedJson.Contains("IsCanOperator") && !selectedJson.Contains("LevelId"),
                    "Transient state and internal level identity never enter the envelope");
                var loaded = OriginalBoardSnapshotJson.Read(selectedJson);
                view.BindSaved(loaded,pool,false,true);
                Check(view.NutViewCount == 0 && !view.AreNutsInitialized, "Restore uses original delayed prefab initialization");
                view.AdvanceInitialization(.49f);
                Check(view.NutViewCount == 0, "No early restored nut");
                view.AdvanceInitialization(.011f);
                Check(view.NutViewCount == 4 && !view.Board.Screws[1].IsReadyMove && view.Board.Screws[1].IsCanOperator,
                    "Restored board starts unselected and operable");
                Check(view.Operate(1).Kind == ScrewOperationKind.Ready, "Fresh selection after restore");
                Check(view.Operate(0).Kind == ScrewOperationKind.Moved, "Restored board accepts original winning transfer");
                var captured = view.CaptureSnapshot();
                Check(captured.OperatorInfos.Count == 1 && captured.OperatorInfos[0].FromScrewIndex == 1 &&
                    captured.OperatorInfos[0].ToScrewIndex == 0 && captured.OperatorInfos[0].NutCount == 1,
                    "History stores source indices and transferred count");
                Check(!view.Board.Screws[0].IsCanOperator, "Capture occurs during animation lock");
                view.BindSaved(OriginalBoardSnapshotJson.Read(OriginalBoardSnapshotJson.Write(captured)),pool,true,true);
                Check(view.Board.IsSuccess && view.Board.Screws[0].IsCanOperator, "Saved data survives animation cancellation without serializing the gate");
                view.AdvanceInitialization(1.49f);
                Check(view.NutViewCount == 0, "Restored long-entry delay");
                view.AdvanceInitialization(.011f);
                Check(view.NutViewCount == 4 && view.GetScrew(0).Cap.localScale == Vector3.one, "Saved complete rod initializes its native cap");
                Check(view.CaptureSnapshot().OperatorInfos.Count == 1, "History retained through view rebind");
                view.Bind(first,pool,false,true);
                Check(view.CaptureSnapshot().OperatorInfos.Count == 0, "Fresh replay clears history");
                Check(captured.OperatorInfos.Count == 1, "Earlier capture is independent of replay");
            }
            finally
            {
                if (view != null) view.Clear();
                UnityEngine.Object.DestroyImmediate(holder);
            }
        }

        private static void EqualJson(string a, string b, string message) => Check(JToken.DeepEquals(JToken.Parse(a),JToken.Parse(b)),message);
        private static void Check(bool value,string message) { if (!value) throw new InvalidDataException(message); }
        private static void ExpectFailure(Action action)
        {
            try { action(); }
            catch (JsonException) { return; }
            catch (ArgumentException) { return; }
            catch (OverflowException) { return; }
            catch (InvalidDataException) { return; }
            throw new InvalidDataException("Invalid snapshot must not silently become a fresh board.");
        }
    }
}
