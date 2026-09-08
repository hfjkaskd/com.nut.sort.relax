using System;
using System.IO;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalLevelViewValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var holder = new GameObject("Original level integration validation");
            try
            {
                var pool = holder.AddComponent<OriginalPrefabPool>();
                var view = pool.Rent("Game/Level",holder.transform).GetComponent<OriginalLevelView>();
                var data = repository.LoadBoard(false,"4b56d_1_1-1");
                view.Bind(data,pool,false,true);
                Check(view.NutViewCount==0,"Source rods precede delayed nuts");
                Near(view.GetScrew(0).transform.position,new Vector3(-1.1f,0,0),"First source position");
                Near(view.GetScrew(1).transform.position,new Vector3(1.1f,0,0),"Second source position");
                view.AdvanceInitialization(.49f);
                Check(view.NutViewCount==0,"No premature nut spawn");
                view.AdvanceInitialization(.011f);
                Check(view.AreNutsInitialized && view.NutViewCount==4,"Exactly four original first-board nuts");
                foreach (ScrewState screw in view.Board.Screws)
                    foreach (NutSlot slot in screw.Slots)
                        if (slot.Nut!=null)
                        {
                            var nut=view.GetNut(slot.Nut);
                            Check(nut.transform.parent==view.GetScrew(screw.Index).GetTile(slot.Coordinate.y).transform,"Fixed-slot hierarchy");
                            nut.AdvanceMotion(2f);
                            Near(nut.transform.localPosition,Vector3.zero,"Entry lands inside its configured tile");
                        }
                int complete=0,effect=0,spark=0;
                view.ScrewCompleted += state => {Check(state.IsCanOperator,"Completion gate released before notification");complete++;};
                view.DoneEffectRequested += screw => effect++;
                view.SparkRequested += nut => spark++;
                var moving=view.GetNut(view.Board.Screws[1].Slots[0].Nut);
                Check(view.Operate(1).Kind==ScrewOperationKind.Ready,"Core selection integrated");
                moving.AdvanceMotion(.2f);
                var move=view.Operate(0);
                Check(move.Kind==ScrewOperationKind.Moved && view.Board.IsSuccess,"Actual board state transfers");
                moving.AdvanceMotion(0f);
                moving.AdvanceMotion(.2f);
                moving.AdvanceMotion(.2f);
                var target=view.GetScrew(0);
                Check(target.IsDoneAnimating && !target.State.IsCanOperator && complete==0 && spark==1,"Landing starts cap animation without early release");
                target.AdvanceDone(.15f);
                float outCirc=Mathf.Sqrt(.75f);
                Near(target.Cap.localScale,Vector3.one*(1.5f*outCirc),"Original OutCirc growth midpoint");
                Near(target.Cap.localPosition,new Vector3(0,1.58f+2f*outCirc,0),"Original OutCirc rise midpoint");
                target.AdvanceDone(.15f);
                Check(effect==1 && complete==0,"Effect requested at peak before completion");
                Near(target.Cap.localScale,Vector3.one,"Original refresh at peak resets scale before return tween initialization");
                target.AdvanceDone(.1f);
                Near(target.Cap.localPosition,new Vector3(0,3.58f-2f*(1f-outCirc),0),"Original InCirc return midpoint");
                target.AdvanceDone(.101f);
                Check(complete==1 && target.State.IsCanOperator && !target.IsDoneAnimating,"Complete animation releases destination exactly once");
                Near(target.Cap.localPosition,new Vector3(0,1.58f,0),"Cap returned to source height");
                view.Bind(data,pool,true,false);
                view.AdvanceInitialization(1.49f);
                Check(view.NutViewCount==0,"Long-entry delay preserved");
                view.Clear();view.AdvanceInitialization(1f);
                Check(view.NutViewCount==0 && view.Board==null,"Clearing cancels pending board initialization");
                view.Bind(data,pool,true,false);view.AdvanceInitialization(1.501f);
                Check(view.NutViewCount==4,"Long-entry board can be reused");
                view.Clear();pool.Return(view.gameObject);
            }
            finally { UnityEngine.Object.DestroyImmediate(holder); }
            Debug.Log("NUT_LEVEL_VIEW_VALIDATION_PASS native level/position/screw/nut prefab chain, delayed initialization, entry, first-board selection/transfer, cap completion and board reuse; main startup and peripheral event consumers remain pending.");
        }
        private static void Check(bool value,string message) { if(!value) throw new InvalidDataException(message); }
        private static void Near(Vector3 a,Vector3 b,string message) {Check((a-b).sqrMagnitude<.000001f,message+": "+a);}
    }
}
