using System;
using System.Collections.Generic;
using System.IO;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalLockedScrewValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var layout = Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            var session = Resources.Load<OriginalSceneSession>("Configuration/OriginalSceneSession");
            Check(session.LockedScrewStartLevel==3,"Original fresh-level lock threshold");
            var data=repository.LoadBoard(false,"4b56d_1_1-1");
            for(int playerLevel=1;playerLevel<=3;playerLevel++)
            {
                var board=new OriginalBoardState(data,layout,playerLevel>=session.LockedScrewStartLevel);
                Check(board.Screws.Length==data.B.Length+(playerLevel==3?1:0),"Locked rod begins at player level three");
            }
            var holder=new GameObject("Original fresh locked screw validation");
            OriginalLevelView view=null;
            try
            {
                var pool=holder.AddComponent<OriginalPrefabPool>();
                view=pool.Rent("Game/Level",holder.transform).GetComponent<OriginalLevelView>();
                view.Bind(data,pool,false,true,true);
                var locked=view.Board.Screws[2];
                Check(locked.Id==view.Board.Screws[1].Id && locked.Index==2 && locked.Capacity==0 && locked.Slots.Length==4,
                    "Source appends repeated Id, next index, zero capacity and previous-capacity empty slots");
                Check(locked.IsLocked && locked.IsCanOperator && locked.Masks.Length==0 && locked.IsNull && !locked.IsDone,
                    "Source empty locked state and completion semantics");
                for(int i=0;i<locked.Slots.Length;i++)Check(locked.Slots[i].Nut==null && locked.Slots[i].Coordinate==new Vector3Int(0,i,0),"Original empty slot coordinates");
                Check(view.GetScrew(2).TileCount==0 && view.GetScrew(2).TypeView.IsShortLockVisible && !view.GetScrew(2).TypeView.IsFullLockVisible,
                    "Zero-capacity native prefab displays source LSSAB lock");
                for(int i=0;i<3;i++)Check((view.GetScrew(i).transform.position-layout.Position(3,i)).sqrMagnitude<.000001f,"Appended lock participates in board layout");
                view.AdvanceInitialization(.501f);
                Check(view.NutViewCount==4,"Locked slots do not instantiate nuts");
                var click=view.Operate(2);
                Check(click.Kind==ScrewOperationKind.AddScrewRequested && !click.SaveRequested,"Original lock routes request without automatic grant or save");
                string saved=OriginalBoardSnapshotJson.Write(view.CaptureSnapshot());
                view.BindSaved(OriginalBoardSnapshotJson.Read(saved),pool,false,false);
                Check(view.Board.Screws.Length==3 && view.Board.Screws[2].Capacity==0 && view.Board.Screws[2].Slots.Length==4,
                    "Restoration retains exactly one lock and its independent slot/capacity data");
                Check(view.GetScrew(2).TypeView.IsFullLockVisible && !view.GetScrew(2).TypeView.IsShortLockVisible,"Other LSSAB variant uses original lock structure");
                view.Bind(data,pool,false,true,true);
                Check(view.Board.Screws.Length==3,"Repeated fresh build does not accumulate lock rods");
            }
            finally {if(view!=null)view.Clear();UnityEngine.Object.DestroyImmediate(holder);}
            Debug.Log("NUT_LOCKED_SCREW_VALIDATION_PASS original level-three fresh lock insertion, independent zero capacity/four empty slots, layout, native lock variants, request-only click, snapshot restoration and repeat rebuild; reward/SDK grant remains excluded.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidDataException(message);}
    }
}
