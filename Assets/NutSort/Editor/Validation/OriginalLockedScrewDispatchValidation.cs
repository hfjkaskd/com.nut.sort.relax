using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalLockedScrewDispatchValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var root=new GameObject("Locked dispatch fixture");
            try
            {
                var pool=root.AddComponent<OriginalPrefabPool>();
                var level=pool.Rent("Game/Level",root.transform).GetComponent<OriginalLevelView>();
                level.Bind(repository.LoadBoard(false,"4b56d_1_1-1"),pool,false,true,true);level.AdvanceInitialization(.51f);
                int calls=0,observed=0;
                level.BindAddScrew(()=>calls++);
                level.OperationApplied+=(operation,target)=>{if(operation.Kind==ScrewOperationKind.AddScrewRequested){Check(calls==observed+1,"Synchronous manager call before operation observer");observed++;}};
                Check(level.Operate(2,true).Kind==ScrewOperationKind.Ignored&&calls==0,"Exchange mode ignores locked rod");
                var result=level.Operate(2);
                Check(result.Kind==ScrewOperationKind.AddScrewRequested&&!result.OriginalReturnValue&&!result.SaveRequested&&calls==1,"Native false result with synchronous manager dispatch");
                var saved=level.CaptureSnapshot();level.BindSaved(saved,pool,false,true);level.AdvanceInitialization(.51f);
                level.Operate(2);Check(calls==2,"Manager binding survives board rebuild");
                level.BindAddScrew(()=>throw new InvalidOperationException("fixture manager failure"));
                bool failed=false;try{level.Operate(2);}catch(InvalidOperationException e){failed=e.Message=="fixture manager failure";}
                Check(failed&&observed==2,"Manager failure propagates before operation observer");
                level.Clear();
                Debug.Log("NUT_LOCKED_SCREW_DISPATCH_VALIDATION_PASS native locked/exchange guards, synchronous manager call, false return and no common save, retained binding across board rebuild, exception ordering; real inventory/UI scene path covered in Play.");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
