using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalAddNullScrewValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var root=new GameObject("Append screw fixture");
            try
            {
                var pool=root.AddComponent<OriginalPrefabPool>();
                var level=pool.Rent("Game/Level",root.transform).GetComponent<OriginalLevelView>();
                var data=repository.LoadBoard(false,"4b56d_1_1-1");
                // Isolated equal-row fixture reuses original cell definitions.
                var first=data.B[0];data.B=new[]{first,first,first,first,first,first};
                level.Bind(data,pool,false,true);level.AdvanceInitialization(.51f);
                var layout=Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
                var original=new OriginalScrewView[6];var positions=new Vector3[6];
                for(int i=0;i<6;i++){original[i]=level.GetScrew(i);positions[i]=original[i].transform.parent.position;}
                var slots=original[0].State.Slots;var nut=slots[0].Nut;slots[0].IsReady=true;
                int nuts=level.NutViewCount,reads=0,cameras=0,effects=0;
                level.UnlockRequested+=view=>{Check(cameras==effects+1,"Camera precedes initialized effect target");Check(view.TileCount==view.State.Capacity,"Segments exist before effect");effects++;};
                Action camera=()=>{cameras++;Check(level.Board.Screws.Length==6+cameras,"Data precedes camera");Check(effects==cameras-1,"Effect follows camera");};
                var added=level.AddNullScrew(()=>{reads++;return true;},camera);
                Check(reads==1&&added.State.Capacity==1&&added.State.Slots.Length==4,"One live read; previous capacity controls slots independently");
                Check(added.State.Coordinate==new Vector2Int(1,3)&&added.State.Id==original[5].State.Id&&added.State.Index==original[5].State.Index+1,"Last equal row, repeated ID, incremented index");
                for(int i=0;i<6;i++)
                {
                    Check(level.GetScrew(i)==original[i]&&level.Board.Screws[i]==original[i].State,"Existing state and prefab references preserved");
                    Check(original[i].transform.parent.position==(i<3?positions[i]:layout.Position(2,4,original[i].State.Coordinate)),"Only selected row recenters");
                }
                var second=level.AddNullScrew(()=>false,camera);
                Check(second.State.Coordinate==new Vector2Int(0,3)&&second.State.Capacity==4&&second.State.Slots.Length==1,"Next smallest row; previous one-capacity rod gives one slot");
                Check(!second.State.IsLocked&&second.State.Masks.Length==0&&second.State.Slots[0].Nut==null,"Unlocked empty unmasked rod");
                Check(level.NutViewCount==nuts&&slots==original[0].State.Slots&&slots[0].Nut==nut&&slots[0].IsReady,"Existing nuts and ready state survive append");
                var snapshot=OriginalBoardSnapshotJson.Read(OriginalBoardSnapshotJson.Write(level.CaptureSnapshot()));
                level.BindSaved(snapshot,pool,false,true);level.AdvanceInitialization(.51f);
                Check(level.Board.Screws.Length==8&&level.GetScrew(6).State.Coordinate==new Vector2Int(1,3)&&level.GetScrew(7).State.Coordinate==new Vector2Int(0,3),"Appended coordinates survive save/load");
                Check(level.GetScrew(6).State.Capacity==1&&level.GetScrew(6).State.Slots.Length==4&&level.GetScrew(7).State.Capacity==4&&level.GetScrew(7).State.Slots.Length==1,"Independent capacity/slots survive save/load");
                level.Clear();
                Debug.Log("NUT_ADD_NULL_SCREW_VALIDATION_PASS native row tie-break and local recenter, stable existing objects/readiness, independent capacity/slots, camera-before-effect and snapshot restore; explicit six-rod fixture.");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
