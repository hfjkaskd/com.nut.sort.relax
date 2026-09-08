using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalBoardRevokeIntegrationValidation
    {
        private sealed class Preferences : IOriginalUserPreferences
        {
            public string Json;
            public readonly List<string> Trace = new List<string>();
            public string GetString(string key,string value) => Json ?? value;
            public void SetString(string key,string value) { Json=value;Trace.Add("write"); }
            public void Save() { Trace.Add("flush"); }
        }
        public static void Validate(OriginalLevelRepository repository)
        {
            var root=new GameObject("Board revoke button integration");
            try
            {
                var pool=root.AddComponent<OriginalPrefabPool>();
                var level=pool.Rent("Game/Level",root.transform).GetComponent<OriginalLevelView>();
                level.Bind(repository.LoadBoard(false,"4b56d_1_1-1"),pool,false,true);
                level.AdvanceInitialization(.51f);
                foreach(var screw in level.Board.Screws)
                    foreach(var slot in screw.Slots)
                        if(slot.Nut!=null)level.GetNut(slot.Nut).AdvanceMotion(2f);
                NutState moving=level.Board.Screws[1].Slots[0].Nut;
                var nut=level.GetNut(moving);
                level.Operate(1);nut.AdvanceMotion(.2f);level.Operate(0);
                nut.AdvanceMotion(0);nut.AdvanceMotion(.2f);nut.AdvanceMotion(.2f);
                level.GetScrew(0).AdvanceDone(.3f);level.GetScrew(0).AdvanceDone(.201f);
                Check(level.MoveHistoryCount==1,"Forward operation produced actual live history");
                var prefs=new Preferences();
                var store=new OriginalUserStore(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"),prefs);
                store.Data.RevokeCount=2;store.Data.ServerConfigData=JObject.Parse("{\"LSSLSMAC\":12}");
                var bottomObject=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelBottom"),root.transform);
                var bottom=bottomObject.GetComponent<OriginalMainBottomView>();
                var manager=new OriginalItemManager(store.Data,()=>
                {
                    Check(level.MoveHistoryCount==0 && level.Board.Screws[1].Slots[0].Nut==moving,
                        "History removed and board restored before save");
                    Check(!level.Board.Screws[0].IsCanOperator && nut.IsTransferring,
                        "Save is immediate while reverse motion is pending");
                    store.SaveData(true,OriginalBoardSnapshotJson.Write(level.CaptureSnapshot()));
                },()=>{prefs.Trace.Add("refresh");bottom.Refresh();},(v,a,b)=>{},(v,a)=>{});
                Action revoke=()=>level.Revoke(type=>
                {
                    Check(type==2 && level.MoveHistoryCount==0,"Gray refresh follows live removal");
                    prefs.Trace.Add("gray");bottom.RefreshOtherItemButtonState(type);
                },()=>{throw new InvalidOperationException("Restored first board must not fail");});
                bottom.Bind(store.Data,()=>level.Board!=null,()=>level.MoveHistoryCount,
                    ()=>{},revoke,manager.AddTool,(active,delay)=>{},(panel,type)=>{},
                    id=>prefs.Trace.Add("tip"),()=>true,()=>prefs.Trace.Add("audio"),id=>{});
                bottom.Init();bottom.Refresh();
                bottom.GetOtherItem(2).Display.Click.onClick.Invoke();
                Check(string.Join(",",prefs.Trace)=="gray,write,flush,refresh,audio" && store.Data.RevokeCount==1,
                    "Actual Button -> record -> history -> gray -> inventory -> save -> display -> audio");
                var saved=JObject.Parse(prefs.Json);
                Check((int)saved["RevokeCount"]==1,"Inventory saved in same user payload");
                var restored=OriginalBoardSnapshotJson.Read((string)saved["LevelInfo"]);
                Check(restored.OperatorInfos.Count==0,"Persisted history excludes reversed record");
                var replay=pool.Rent("Game/Level",root.transform).GetComponent<OriginalLevelView>();
                replay.BindSaved(restored,pool,false,true);replay.AdvanceInitialization(.51f);
                Check(replay.MoveHistoryCount==0 && replay.Board.Screws[1].Slots[0].Nut.Color==moving.Color &&
                    !replay.Board.IsSuccess,"Saved undo resumes as original playable board");
                for(int i=0;i<12;i++)nut.AdvanceMotion(.2f);
                Check(level.Board.Screws[0].IsCanOperator && nut.transform.parent==level.GetScrew(1).GetTile(0).transform,
                    "Reverse animation finishes independently of save");
                prefs.Trace.Clear();bottom.GetOtherItem(2).Display.Click.onClick.Invoke();
                Check(store.Data.RevokeCount==1 && string.Join(",",prefs.Trace)=="tip,audio",
                    "Next click sees empty live history and does not consume or save");
                replay.Clear();pool.Return(replay.gameObject);level.Clear();pool.Return(level.gameObject);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
            Debug.Log("NUT_BOARD_REVOKE_INTEGRATION_VALIDATION_PASS real Bottom Button/native board/history/item manager/user store/save resume and delayed landing, followed by empty-history rejection without consumption.");
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
