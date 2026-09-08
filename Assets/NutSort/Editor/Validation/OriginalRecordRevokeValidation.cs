using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalRecordRevokeValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var from = Screw(5, 0, 0, 0);
            var to = Screw(9, 2, 3, 4);
            var callbacks = new List<Action>();
            var trace = new List<int>();
            NutState top = to.Slots[2].Nut;
            NutState middle = to.Slots[1].Nut;
            top.Type = NutType.Hidden;
            var record = new OriginalMoveRecord { FromScrewIndex=5, ToScrewIndex=9, NutCount=2 };
            var flow = new OriginalRecordRevoke(index => index==5 ? from : to,
                state => { throw new Exception("Unexpected revert"); },
                (target, transfer, index, callback) =>
                {
                    Check(target==from && transfer.Source.Nut==transfer.Nut && transfer.Destination.Nut==transfer.Nut,
                        "Visual receives destination assignment before source clear");
                    trace.Add(index); callbacks.Add(callback);
                }, state => trace.Add(state.Index));
            Check(flow.Revoke(record),"Accepted reverse record");
            Check(from.Slots[0].Nut==top && from.Slots[1].Nut==middle && top.Type==NutType.Hidden,
                "Highest occupied first; no color filtering or hidden reveal");
            Check(to.Slots[1].Nut==null && to.Slots[2].Nut==null && !to.IsCanOperator && from.IsCanOperator,
                "Immediate state transfer locks original destination only");
            Check(string.Join(",",trace)=="0,1,5,9","Stagger then original-from/original-to caps");
            callbacks[0]();Check(!to.IsCanOperator,"Earlier landing does not unlock");
            callbacks[1]();Check(to.IsCanOperator,"Final landing unlocks original destination");
            record.NutCount=0;callbacks.Clear();trace.Clear();
            Check(flow.Revoke(record) && !to.IsCanOperator && callbacks.Count==0 && string.Join(",",trace)=="5,9",
                "Zero count retains source lock and refreshes both caps");
            to.IsCanOperator=true;

            var holder = new GameObject("Record revoke prefab validation");
            try
            {
                var pool=holder.AddComponent<OriginalPrefabPool>();
                var level=pool.Rent("Game/Level",holder.transform).GetComponent<OriginalLevelView>();
                level.Bind(repository.LoadBoard(false,"4b56d_1_1-1"),pool,false,true);
                level.AdvanceInitialization(.51f);
                foreach(var screw in level.Board.Screws)
                    foreach(var slot in screw.Slots)
                        if(slot.Nut!=null)level.GetNut(slot.Nut).AdvanceMotion(2f);
                var moved=level.Board.Screws[1].Slots[0].Nut;
                var nut=level.GetNut(moved);
                level.Operate(1);nut.AdvanceMotion(.2f);level.Operate(0);
                nut.AdvanceMotion(0f);nut.AdvanceMotion(.2f);nut.AdvanceMotion(.2f);
                level.GetScrew(0).AdvanceDone(.3f);level.GetScrew(0).AdvanceDone(.201f);
                Check(level.Board.Screws[0].IsCanOperator,"Forward completion finished");
                int done=0;level.ScrewCompleted+=state=>done++;
                var history=level.CaptureSnapshot().OperatorInfos;
                Check(level.RevokeRecord(history[0]),"Actual prefab reverse accepted");
                Check(level.Board.Screws[1].Slots[0].Nut==moved && !level.Board.Screws[0].IsCanOperator,
                    "Live board restored before animation");
                Check(level.CaptureSnapshot().OperatorInfos.Count==1,"Record does not remove manager history itself");
                for(int i=0;i<12;i++)nut.AdvanceMotion(.2f);
                Check(nut.BoundSlot==level.Board.Screws[1].Slots[0] &&
                    nut.transform.parent==level.GetScrew(1).GetTile(0).transform && nut.transform.localPosition.sqrMagnitude<.000001f,
                    "Existing native nut prefab lands on reverse destination slot");
                Check(level.Board.Screws[0].IsCanOperator && done==0 && !level.GetScrew(1).IsDoneAnimating,
                    "Reverse landing unlocks without forward completion event or cap tween");
                level.Clear();pool.Return(level.gameObject);
            }
            finally { UnityEngine.Object.DestroyImmediate(holder); }
            Debug.Log("NUT_RECORD_REVOKE_VALIDATION_PASS reverse ordering, hidden identity, stagger callbacks, zero count, cap order and actual board prefab reverse landing without done animation.");
        }
        private static ScrewState Screw(int index,params int[] colors)
        {
            var data=new ScrewData { C=new CData[colors.Length] };
            for(int i=0;i<colors.Length;i++)data.C[i]=new CData { LP=new LPData {y=i}, BIM=colors[i]==0 ? null : new BIMData {Id=1,CI=colors[i]} };
            return new ScrewState(data,index);
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
