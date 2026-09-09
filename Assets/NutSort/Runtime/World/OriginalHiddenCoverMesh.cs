using UnityEngine;
namespace NutSort.World
{
    // Bounded world mesh deformation, not UI or a skeleton interpreter. Static
    // attachments, UVs, hierarchy and clips are authored in the native prefab.
    public sealed class OriginalHiddenCoverMesh : MonoBehaviour
    {
        public OriginalEffectBone[] bones;
        public OriginalEffectSlot[] slots;
        public MeshFilter target;
        public float unitsPerPixel;
        private Mesh mesh;
        private Vector3[] vertices;
        private Color[] colors;
        private Affine[] world;
        public struct Affine
        {
            public float a,b,c,d,x,y;
            public static Affine Identity=>new Affine{a=1,d=1};
            public Vector2 Point(Vector2 p)=>new Vector2(a*p.x+b*p.y+x,c*p.x+d*p.y+y);
        }
        public static Affine Compose(Affine parent,OriginalEffectBone bone)
        {
            var result=new Affine{x=parent.a*bone.x+parent.b*bone.y+parent.x,y=parent.c*bone.x+parent.d*bone.y+parent.y};
            float angle=bone.rotation*Mathf.Deg2Rad;
            if(bone.noScale)
            {
                float qx=parent.a*Mathf.Cos(angle)+parent.b*Mathf.Sin(angle);
                float qy=parent.c*Mathf.Cos(angle)+parent.d*Mathf.Sin(angle);
                float length=Mathf.Sqrt(qx*qx+qy*qy);
                float factor=length>0.00001f?1/length:length;
                qx*=factor;qy*=factor;
                float sign=parent.a*parent.d-parent.b*parent.c<0?-1:1;
                parent=new Affine{a=qx,c=qy,b=-qy*sign,d=qx*sign};
                angle=0;
            }
            float rx=angle+bone.shearX*Mathf.Deg2Rad,ry=angle+(90+bone.shearY)*Mathf.Deg2Rad;
            float ax=Mathf.Cos(rx)*bone.scaleX,ay=Mathf.Sin(rx)*bone.scaleX;
            float bx=Mathf.Cos(ry)*bone.scaleY,by=Mathf.Sin(ry)*bone.scaleY;
            result.a=parent.a*ax+parent.b*ay;result.c=parent.c*ax+parent.d*ay;
            result.b=parent.a*bx+parent.b*by;result.d=parent.c*bx+parent.d*by;
            return result;
        }
        public void RefreshMesh()
        {
            if(mesh==null)
            {
                mesh=Instantiate(target.sharedMesh);mesh.MarkDynamic();target.sharedMesh=mesh;
                vertices=new Vector3[slots.Length*4];colors=new Color[vertices.Length];world=new Affine[bones.Length];
            }
            for(int i=0;i<bones.Length;i++)world[i]=Compose(bones[i].parentIndex<0?Affine.Identity:world[bones[i].parentIndex],bones[i]);
            for(int i=0;i<slots.Length;i++)
            {
                var slot=slots[i];var matrix=world[slot.boneIndex];
                for(int j=0;j<4;j++){vertices[i*4+j]=matrix.Point(slot.corners[j])*unitsPerPixel;colors[i*4+j]=slot.color;}
            }
            mesh.vertices=vertices;mesh.colors=colors;mesh.RecalculateBounds();
        }
        private void LateUpdate()=>RefreshMesh();
        private void OnDestroy(){if(mesh!=null)Destroy(mesh);}
    }
}
