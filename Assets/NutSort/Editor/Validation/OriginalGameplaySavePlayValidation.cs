using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalGameplaySavePlayValidation
    {
        private const string Active = "NutSort.GameplaySavePlayValidation";
        private const string Fixture = "{\"ScrewMoveCount\":40,\"LevelInfo\":\"untouched-before-operation\",\"IsAudio\":false,\"RevokeCount\":3}";
        private static OriginalGameScene game;
        private static int phase;
        private static double deadline, timeout;
        private static string saved;

        static OriginalGameplaySavePlayValidation()
        {
            if (SessionState.GetBool(Active,false))
            { timeout=EditorApplication.timeSinceStartup+60; EditorApplication.update+=Tick; }
        }

        public static void Run()
        {
            try
            {
                OriginalPreferenceFixture.Begin(Fixture);
                EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                SessionState.SetBool(Active,true);
                EditorApplication.EnterPlaymode();
            }
            catch(Exception error) { Debug.LogException(error); Finish(1); }
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying) return;
            try
            {
                double now=EditorApplication.timeSinceStartup;
                Check(now<timeout,"Gameplay save validation timeout phase="+phase);
                if (game==null) game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if (game==null || game.Level==null || !game.Level.AreNutsInitialized || game.InputBlocked || now<deadline) return;
                var user=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;
                if (phase==0)
                {
                    Check(PlayerPrefs.GetString(OriginalUserStore.Key)==Fixture,"Startup has not rewritten the stored board");
                    Check(game.Level.Operate(1).Kind==ScrewOperationKind.Ready,"Select right screw");
                    Check(game.Level.Operate(1).Kind==ScrewOperationKind.Reverted,"Same screw cancels selection");
                    Check(game.Level.Operate(1).Kind==ScrewOperationKind.Ready,"Select again");
                    Check(PlayerPrefs.GetString(OriginalUserStore.Key)==Fixture && user.ScrewMoveCount==40,
                        "Selection and cancellation do not count or save");
                    // Controlled non-matching fixture uses the real operation and
                    // scene save consumer, without replacing their implementation.
                    NutState top=game.Level.Board.Screws[0].Slots[2].Nut;
                    top.Color=6; game.Level.GetNut(top).RefreshVisual();
                    var result=game.Level.Operate(0);
                    Check(result.Kind==ScrewOperationKind.Reverted && result.SaveRequested,"Mismatched target reaches original save point");
                    var store=Reload();
                    var snapshot=OriginalBoardSnapshotJson.Read(store.Data.LevelInfo);
                    Check(store.Data.ScrewMoveCount==41 && user.ScrewMoveCount==41 && snapshot.OperatorInfos.Count==0,
                        "Rejected attempt increments persisted cumulative count without adding move history");
                    Check(snapshot.ScrewInfos[0].NutInfos[2].NutData.NutColor==6 && snapshot.ScrewInfos[1].NutInfos[0].NutData!=null,
                        "Failed move stores the unchanged occupied slots");
                    Check(!store.Data.IsAudio && store.Data.RevokeCount==3,"Automatic save preserves other user fields");
                    saved=PlayerPrefs.GetString(OriginalUserStore.Key);
                    phase=1; deadline=now+.4; return;
                }
                if (phase==1)
                {
                    Check(saved==PlayerPrefs.GetString(OriginalUserStore.Key),"Animation frames do not continuously rewrite preferences");
                    NutState top=game.Level.Board.Screws[0].Slots[2].Nut;
                    top.Color=11; game.Level.GetNut(top).RefreshVisual();
                    Check(game.Level.Operate(1).Kind==ScrewOperationKind.Ready,"Select source after mismatch");
                    var move=game.Level.Operate(0);
                    Check(move.Kind==ScrewOperationKind.Moved && !game.Level.Board.Screws[0].IsCanOperator,"Transfer starts with animation gate locked");
                    var store=Reload();
                    var snapshot=OriginalBoardSnapshotJson.Read(store.Data.LevelInfo);
                    Check(store.Data.ScrewMoveCount==42 && snapshot.OperatorInfos.Count==1 && snapshot.OperatorInfos[0].NutCount==1,
                        "Successful move saves count and compact record before animation ends");
                    Check(snapshot.ScrewInfos[1].NutInfos[0].NutData==null && snapshot.ScrewInfos[0].NutInfos[3].NutData.NutColor==11,
                        "Saved positions already reflect the transfer");
                    Check(JObject.Parse(PlayerPrefs.GetString(OriginalUserStore.Key))["LevelInfo"].Type==JTokenType.String,
                        "Board remains a nested JSON string in the original envelope");
                    saved=PlayerPrefs.GetString(OriginalUserStore.Key);
                    phase=2; deadline=now+1; return;
                }
                if (phase==2)
                {
                    Check(saved==PlayerPrefs.GetString(OriginalUserStore.Key),"Completion animation does not duplicate this save");
                    game.RestartLevel(); phase=3; deadline=now+1.1; return;
                }
                Check(!game.IsRestarting && user.ScrewMoveCount==42,"Replay retains cumulative attempts");
                game.Level.Operate(1);game.Level.Operate(0);
                var reloaded=Reload();
                Check(reloaded.Data.ScrewMoveCount==43 && OriginalBoardSnapshotJson.Read(reloaded.Data.LevelInfo).OperatorInfos.Count==1,
                    "Save subscription survives rebuild exactly once; fresh history replaces previous board history");
                Debug.Log("NUT_GAMEPLAY_SAVE_PLAY_VALIDATION_PASS real PlayerPrefs operation saves, selection exclusions, rejected/successful attempts, immediate transferred slots, original string envelope, preserved preferences, replay cumulative counter and single subscription; prior preferences restored on exit.");
                Finish(0);
            }
            catch(Exception error) { Debug.LogException(error); Finish(1); }
        }

        private static OriginalUserStore Reload() => new OriginalUserStore(
            Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"), new UnityUserPreferences());
        private static void Check(bool value,string message) { if(!value)throw new Exception(message); }
        private static void Finish(int code)
        {
            OriginalPreferenceFixture.Restore();
            SessionState.SetBool(Active,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);
        }
    }
}
