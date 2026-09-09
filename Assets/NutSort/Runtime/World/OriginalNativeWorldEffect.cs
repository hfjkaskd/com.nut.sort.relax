using System;
using UnityEngine;
namespace NutSort.World
{
    // Loads an already authored native animation prefab at first use. No runtime
    // hierarchy construction, JSON evaluation or third-party animation runtime.
    public sealed class OriginalNativeWorldEffect : MonoBehaviour
    {
        [SerializeField] private string prefabPath;
        [SerializeField] private float mixDuration;
        private Animation player;
        private string currentClip;
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
            state.time=0;
            if(player.isPlaying&&currentClip!=clipName&&mixDuration>0)
                player.CrossFade(clipName,mixDuration,PlayMode.StopAll);
            else player.Play(clipName,PlayMode.StopAll);
            currentClip=clipName;player.Sample();
        }
    }
}
