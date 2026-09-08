using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalAddTileValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var root=new GameObject("Native add tile fixture");
            try
            {
                var pool=root.AddComponent<OriginalPrefabPool>();
                var level=pool.Rent("Game/Level",root.transform).GetComponent<OriginalLevelView>();
                level.Bind(repository.LoadBoard(false,"4b56d_1_1-1"),pool,false,true);level.AdvanceInitialization(.51f);
                var first=level.GetScrew(0);var slots=first.State.Slots;var nut=slots[0].Nut;
                var tile=first.GetTile(0);var bounds=first.Bounds.size;var ready=first.ReadyPosition.localPosition;
                int reads=0;level.AddTile(()=>{reads++;return true;});
                Check(reads==1&&first.State.Capacity==5&&level.GetScrew(1).State.Capacity==4,"Manager adds to first eligible rod only");
                Check(first.State.Slots==slots&&slots.Length==4&&slots[0].Nut==nut,"Capacity changes independently of logic slots");
                Check(first.TileCount==5&&first.GetTile(0)==tile&&first.GetTile(4).Tip.activeSelf,"Reuses existing segments and rents actual fifth prefab");
                Check(!first.GetTile(3).Tip.activeSelf&&first.GetTile(3).Shaft.activeSelf,"Previous top becomes shaft");
                Check(first.Bounds.size==bounds&&first.ReadyPosition.localPosition==ready,"Original fixed collider and aliased anchors remain fixed");
                var snapshot=level.CaptureSnapshot();
                var decoded=OriginalBoardSnapshotJson.Read(OriginalBoardSnapshotJson.Write(snapshot));
                Check(decoded!=null,"Expanded capacity remains serializable");
                level.AddTile(()=>false);
                Check(first.State.Capacity==4&&first.TileCount==5&&first.GetTile(3).Tip.activeSelf,"False mode assigns four without removing pooled segments");
                Check(!first.GetTile(4).Shaft.activeSelf&&!first.GetTile(4).Tip.activeSelf&&!first.GetTile(4).AddHint.activeSelf,"Surplus existing tile keeps native hidden geometry");
                level.BindSaved(decoded,pool,false,true);level.AdvanceInitialization(.51f);
                Check(level.GetScrew(0).State.Capacity==5&&level.GetScrew(0).State.Slots.Length==4&&level.GetScrew(0).TileCount==5,"Saved board restores independent capacity and slot count with actual segments");
                level.Clear();
                Debug.Log("NUT_ADD_TILE_VALIDATION_PASS original first-rod selection, capacity increment versus assign-four, independent slots, real prefab growth/reuse, fixed geometry and snapshot reload; unlock/new-rod and production tool binding pending.");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
