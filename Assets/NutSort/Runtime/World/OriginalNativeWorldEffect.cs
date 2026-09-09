using System;
using UnityEngine;
namespace NutSort.World
{
    // Loads an already authored native animation prefab at first use. No runtime
    // hierarchy construction, JSON evaluation or third-party animation runtime.
    public sealed class OriginalNativeWorldEffect : MonoBehaviour
    {
        [SerializeField] private string prefabPath;
        private Animation player;
        public Animation Player=>player;
        public void Play(string clipName,bool loop)
        {
            if(player==null)
            {
                var prefab=Resources.Load<GameObject>(prefabPath);
                if(prefab==null)throw new InvalidOperationException("Missing native world effect: "+prefabPath);
                player=Instantiate(prefab,transform,false).GetComponent<Animation>();
                if(player==null)throw new InvalidOperationException("Native world effect has no Animation component.");
            }
            var state=player[clipName];
            if(state==null)throw new InvalidOperationException("Missing native effect clip: "+clipName);
            state.wrapMode=loop?WrapMode.Loop:WrapMode.ClampForever;
            state.time=0;player.Play(clipName,PlayMode.StopAll);player.Sample();
        }
    }
}
