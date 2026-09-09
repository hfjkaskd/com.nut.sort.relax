using UnityEngine;
namespace NutSort.World
{
    // Native AnimationClip parameters. Geometry uses two independent shear axes.
    public sealed class OriginalEffectBone : MonoBehaviour
    {
        public float x,y,rotation,scaleX=1,scaleY=1,shearX,shearY;
        public int parentIndex=-1;
        public bool noScale;
    }
}
