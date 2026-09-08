using System;
using NutSort.Content;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUnlockScrewValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var root=new GameObject("Unlock fixture");
            try
            {
                var pool=root.AddComponent<OriginalPrefabPool>();
                var view=pool.Rent("Game/Level",root.transform).GetComponent<OriginalLevelView>();
                view.Bind(repository.LoadBoard(false,"4b56d_1_1-1"),pool,false,true,true);view.AdvanceInitialization(.51f);
                var locked=view.GetScrew(2);int reads=0,requests=0;bool mode=true;
                Check(locked.State.IsLocked&&locked.State.Capacity==0&&locked.TypeView.IsShortLockVisible,"Original reserved lock fixture");
                view.UnlockRequested+=screw=>
                {
                    Check(screw==locked&&!screw.State.IsLocked&&!screw.TypeView.IsShortLockVisible&&!screw.TypeView.IsFullLockVisible,"Effect follows flag and lock visuals");
                    Check(screw.State.Capacity==0&&reads==0,"Effect precedes capacity write; unlocked lock refresh skips mode reads");
                    requests++;mode=false;
                };
                Check(view.Unlock(()=>{reads++;return mode;}),"First eligible reserved rod unlocks");
                Check(reads==1&&requests==1&&locked.State.Capacity==4&&locked.TileCount==4,"Post-effect mode is live and expands actual prefab");
                Check(locked.State.Slots.Length==4&&view.GetScrew(0).State.Capacity==4,"Slots and earlier full rods unchanged");
                Check(!view.Unlock(()=>throw new Exception("No mode read without eligible rod")),"No eligible rod returns false");
                var settings=Resources.Load<OriginalEffectSettings>("Configuration/OriginalEffects");
                var effect=Resources.Load<GameObject>(settings.UnlockPath);
                Check(settings.UnlockLifetime==3&&effect!=null,"Original unlock resource and caller lifetime");
                var systems=effect.GetComponentsInChildren<ParticleSystem>();Check(systems.Length==3,"Three native systems");
                foreach(var ps in systems)
                {
                    var renderer=ps.GetComponent<ParticleSystemRenderer>();
                    Check(renderer.sharedMaterial!=null&&renderer.sharedMaterial.shader!=null,"Resolved original particle dependencies");
                }
                view.Clear();
                Debug.Log("NUT_UNLOCK_SCREW_VALIDATION_PASS first eligible rod, lock-before-effect-before-add ordering, live mode, native prefab expansion and three-system unlock resource; actual effect lifetime covered in Play.");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
