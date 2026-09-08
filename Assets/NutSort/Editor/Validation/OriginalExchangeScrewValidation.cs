using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalExchangeScrewValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var holder=new GameObject("Exchange screw fixture");
            try
            {
                var pool=holder.AddComponent<OriginalPrefabPool>();
                var view=pool.Rent("Game/Level",holder.transform).GetComponent<OriginalLevelView>();
                view.Bind(repository.LoadBoard(false,"4b56d_1_1-1"),pool,false,true);
                view.AdvanceInitialization(.51f);
                var first=view.Board.Screws[0];var second=view.Board.Screws[1];
                Check(first.IsNutColorSame&&second.IsNutColorSame,"Partial same-color rods are uniform without requiring fullness");
                view.SetScrewState(false);
                Check(!view.GetScrew(0).gameObject.activeSelf&&!view.GetScrew(1).gameObject.activeSelf,"Exchange hides uniform rods");
                view.SetScrewState(true);
                first.Slots[1].Nut.Color=first.Slots[0].Nut.Color+1;
                Check(!first.IsNutColorSame,"Mixed colors remain eligible");
                foreach(var slot in first.Slots)if(slot.Nut!=null)view.GetNut(slot.Nut).AdvanceMotion(2);
                Check(view.Operate(0).Kind==ScrewOperationKind.Ready,"Real mixed-rod selection");
                NutSlot top=null;foreach(var slot in first.Slots)if(slot.IsReady)top=slot;
                Check(top!=null,"Selected top slot");
                var nut=view.GetNut(top.Nut);nut.AdvanceMotion(.3f);
                int sparks=0;view.SparkRequested+=n=>sparks++;
                view.SetScrewState(false);
                Check(view.GetScrew(0).gameObject.activeSelf&&!top.IsReady,"Eligible rod stays active and selected nut reverts");
                nut.AdvanceMotion(.3f);Check(sparks==1,"Revert reaches actual landing spark callback");
                // An inactive eligible rod is left untouched by activeSelf early return.
                view.GetScrew(0).gameObject.SetActive(false);top.IsReady=true;
                view.SetScrewState(false);Check(!view.GetScrew(0).gameObject.activeSelf&&top.IsReady,"Matching activeSelf does not re-evaluate eligibility or selection");
                holder.SetActive(false);view.SetScrewState(true);
                Check(view.GetScrew(0).gameObject.activeSelf&&top.IsReady,"Restore uses activeSelf and does not revert selection");
                holder.SetActive(true);
                // Native comparison starts at one and does not normalize a malformed hole at zero.
                var saved=first.Slots[0].Nut;first.Slots[0].Nut=null;
                bool failed=false;try{bool unused=first.IsNutColorSame;}catch(NullReferenceException){failed=true;}
                Check(failed,"Missing first nut with later filled slots preserves native failure");first.Slots[0].Nut=saved;
                view.Clear();
                Debug.Log("NUT_EXCHANGE_SCREW_VALIDATION_PASS native uniform-color predicate, activeSelf gate, mixed-rod visibility, selected-nut reversion and actual landing callback; full exchange mode camera/mask scheduling pending.");
            }
            finally{UnityEngine.Object.DestroyImmediate(holder);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
