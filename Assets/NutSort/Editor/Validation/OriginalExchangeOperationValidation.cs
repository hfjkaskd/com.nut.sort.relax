using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalExchangeOperationValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var holder=new GameObject("Exchange operation fixture");
            try
            {
                var pool=holder.AddComponent<OriginalPrefabPool>();
                var view=pool.Rent("Game/Level",holder.transform).GetComponent<OriginalLevelView>();
                var data=repository.LoadBoard(false,"4b56d_1_1-1");
                data.B[0].C[0].BIM.Id=2;data.B[0].C[0].BIM.CI=1;
                data.B[0].C[1].BIM.CI=2;data.B[0].C[2].BIM.CI=2;
                data.B[1].C[0].BIM.CI=3; // No matching destination after rotation.
                view.Bind(data,pool,false,true);view.AdvanceInitialization(.51f);
                var rod=view.Board.Screws[0];var old=(NutSlot[])rod.Slots.Clone();
                foreach(var slot in rod.Slots)if(slot.Nut!=null)view.GetNut(slot.Nut).AdvanceMotion(2);
                old[2].IsReady=true;
                var trace=new List<string>();
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){ExchangeCount=2};
                var items=new OriginalItemManager(user,()=>
                {
                    Check(rod.Slots[0]==old[1]&&rod.Slots[1]==old[2]&&rod.Slots[2]==old[0],"Whole slot objects rotate top group to bottom in original order");
                    Check(rod.Slots[2].Nut.Type==NutType.Normal&&user.ExchangeCount==1,"Reveal and consumption precede save");trace.Add("save");
                },()=>trace.Add("refresh"),(a,b,c)=>throw new Exception("Unexpected gold"),(a,b)=>throw new Exception("Unexpected coin"));
                int attempts=0;view.MoveAttempted+=()=>attempts++;
                view.BindExchange(target=>view.Exchange(target,items.AddTool,()=>trace.Add("fail")));
                var result=view.Operate(0,true);
                Check(result.Kind==ScrewOperationKind.ExchangeRequested&&!result.OriginalReturnValue&&!result.SaveRequested,"Native exchange returns false without normal transfer save");
                Check(string.Join(",",trace)=="save,refresh,fail"&&attempts==0&&view.MoveHistoryCount==0,"Item save/refresh precedes actual deadlock, without move count or history");
                Check(rod.Slots[3]==old[3],"Unoccupied tail slot remains untouched");
                for(int i=0;i<3;i++)
                {
                    var slot=rod.Slots[i];var nut=view.GetNut(slot.Nut);
                    Check(slot.Coordinate==new Vector3Int(0,i,0)&&!slot.IsReady,"New source NutPos and cleared ready flag");
                    Check(nut.transform.parent==view.GetScrew(0).GetTile(i).transform&&nut.transform.localPosition==Vector3.zero,"Actual original nut transforms snap to new tile");
                }
                Check(view.GetNut(old[0].Nut).HiddenInstance==null,"New top hidden cover returned to pool");
                // A failed item callback occurs after reordering and prevents deadlock dispatch.
                var before=(NutSlot[])rod.Slots.Clone();bool failed=false;
                try{view.Exchange(rod,(t,n,f)=>throw new InvalidOperationException("consume"),()=>throw new Exception("Unexpected fail"));}
                catch(InvalidOperationException){failed=true;}
                Check(failed&&rod.Slots[0]==before[2],"Consumption failure retains completed reordering");
                view.Clear();
                Debug.Log("NUT_EXCHANGE_OPERATION_VALIDATION_PASS actual prefab top-group rotation, slot identity/coordinates, transform refresh, hidden reveal, inventory save/refresh and error ordering; default startup and complete exchange UI binding pending.");
            }
            finally{UnityEngine.Object.DestroyImmediate(holder);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
