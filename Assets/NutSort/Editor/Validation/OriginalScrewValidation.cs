using System;
using System.IO;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalScrewValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var settings = Resources.Load<OriginalScrewSettings>("Configuration/OriginalScrew");
            var world = Resources.Load<OriginalWorldSettings>("Configuration/OriginalWorld");
            var holder = new GameObject("Original screw validation");
            try
            {
                var pool = holder.AddComponent<OriginalPrefabPool>();
                var instance = pool.Rent(settings.PrefabPath, holder.transform);
                var screw = instance.GetComponent<OriginalScrewView>();
                foreach (Transform item in instance.GetComponentsInChildren<Transform>(true))
                    Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(item.gameObject) == 0, "Missing script: " + item.name);
                foreach (MeshFilter mesh in instance.GetComponentsInChildren<MeshFilter>(true))
                    Check(mesh.sharedMesh != null, "Missing screw mesh: " + mesh.name);
                foreach (SpriteRenderer sprite in instance.GetComponentsInChildren<SpriteRenderer>(true))
                    Check(sprite.sprite != null, "Missing screw sprite: " + sprite.name);
                Near(instance.transform.localScale, new Vector3(1.4f,1.4f,1.4f), "Original prefab root scale");
                var source = repository.LoadBoard(false, "4b56d_1_1-1");
                var state = new ScrewState(source.B[0], 0);
                screw.Bind(state, pool, world, settings, true);
                Check(screw.TileCount == 4 && screw.ReadyPosition == screw.InitialPosition, "Four slots and original shared anchor");
                Near(screw.ReadyPosition.localPosition, new Vector3(0,1.94f,0), "Initial-position write wins original alias");
                Near(screw.Bounds.center, new Vector3(0,.72f,0), "Fixed source collider center");
                Near(screw.Bounds.size, new Vector3(1,1.44f,1), "Fixed source collider size");
                Near(screw.Cap.localPosition, new Vector3(0,1.58f,0), "Original cap height");
                Near(screw.Cap.localScale, Vector3.zero, "Incomplete cap hidden by scale");
                for (int i=0;i<4;i++) Near(screw.GetTile(i).transform.localPosition,new Vector3(0,i*.36f,0),"Tile coordinate");
                state.IsLocked=true; screw.RefreshType(true);
                Check(screw.TypeView.IsShortLockVisible && !screw.TypeView.IsFullLockVisible,"LSSAB true lock variant");
                screw.RefreshType(false);
                Check(!screw.TypeView.IsShortLockVisible && screw.TypeView.IsFullLockVisible,"LSSAB false lock variant");
                state.IsLocked=false; screw.RefreshType(false);
                Check(!screw.TypeView.IsShortLockVisible && !screw.TypeView.IsFullLockVisible,"Unlocked hides both variants");
                var mask = new ScrewMaskState { Type=ScrewType.Mask, Object=new MaskObjectState { Color=11 } };
                screw.TypeView.Configure(mask,state,settings,true);
                Check(screw.TypeView.IsMaskVisible && screw.TypeView.MaskSprite==Resources.Load<Sprite>("Game/Spirte/12"),"Mask object color maps to original sprite");
                mask.Object.IsShow=false; screw.TypeView.Configure(mask,state,settings,true);
                Check(!screw.TypeView.IsMaskVisible,"Revealed mask disappears");
                int requests=0;
                screw.TypeView.AnimationRequested += (target,clip,loop) => { Check(target!=null && loop,"Source animation target and loop");requests++; };
                screw.TypeView.Configure(new ScrewMaskState {Type=ScrewType.DontMove},state,settings,true);
                Check(screw.TypeView.IsDontMoveVisible && requests==1,"Incomplete fixed screw requests idle animation");
                state.Slots[3].Nut=new NutState { Color=11,Type=NutType.Normal };
                screw.RefreshCap();
                Near(screw.Cap.localScale,Vector3.one,"Completed source cap restored");
                screw.TypeView.Configure(new ScrewMaskState {Type=ScrewType.DontMove},state,settings,true);
                Check(!screw.TypeView.IsDontMoveVisible && requests==1,"Completed fixed screw hides stone and stops requesting idle");
                screw.TypeView.Configure(new ScrewMaskState {Type=ScrewType.Hidden,IsShow=true},state,settings,true);
                Check(screw.TypeView.IsHiddenVisible && requests==2,"Hidden cover request");
                var oldTile=screw.GetTile(3);
                screw.Release();pool.Return(instance);
                var second=pool.Rent(settings.PrefabPath,holder.transform).GetComponent<OriginalScrewView>();
                Check(second==screw,"Whole screw prefab reused");
                var shortData=new ScrewData { Id=1,C=new[] {new CData {LP=new LPData()},new CData {LP=new LPData {y=1}}} };
                second.Bind(new ScrewState(shortData,0),pool,world,settings,false);
                Check(second.TileCount==2 && second.GetTile(0)==oldTile,"Slot pool reused after board release");
                Check(second.GetTile(1).Tip.activeSelf && second.GetTile(1).AddHint.activeSelf,"Short screw tip and add hint");
                Near(second.ReadyPosition.localPosition,new Vector3(0,1.94f,0),"Capacity does not change original anchor height");
                second.Release();pool.Return(second.gameObject);
            }
            finally { UnityEngine.Object.DestroyImmediate(holder); }
            Debug.Log("NUT_SCREW_VALIDATION_PASS original prefab structure, fixed geometry, aliased anchors, tile reuse, cap, LSSAB lock variants and mask state verified; hidden-cover skeletal animations remain pending; mask smoke now has native assets.");
        }
        private static void Check(bool value,string message) { if (!value) throw new InvalidDataException(message); }
        private static void Near(Vector3 a,Vector3 b,string message) { Check((a-b).sqrMagnitude<.000001f,message+": "+a); }
    }
}
