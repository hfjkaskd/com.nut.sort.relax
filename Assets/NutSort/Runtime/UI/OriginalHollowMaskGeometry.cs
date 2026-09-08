using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    // UIHollowOutImage geometry shared by its eventual Graphic implementation.
    // Bounds are cached: a missing/destroyed target preserves all four values.
    public sealed class OriginalHollowMaskGeometry
    {
        public Vector2 InnerMax { get; private set; }
        public Vector2 InnerMin { get; private set; }
        public Vector2 OuterMax { get; private set; }
        public Vector2 OuterMin { get; private set; }

        // 0x9B5604 includes descendant RectTransforms in the relative bounds.
        public void CalculateBounds(RectTransform outer, RectTransform inner)
        {
            if (inner == null) return;
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(outer, inner);
            InnerMax = bounds.max;
            InnerMin = bounds.min;
            OuterMax = outer.rect.max;
            OuterMin = outer.rect.min;
        }

        // 0x9B5840: Radius is a width divisor. Preserve comparison/NaN behavior
        // and mutations of invalid settings; no height clamp is present.
        public float PrepareCornerRadius(ref float radius, ref int triangleCount)
        {
            float width = Mathf.Abs(InnerMin.x - InnerMax.x);
            if (radius < 0f) radius = 0f;
            float divided = width / radius;
            float half = width * .5f;
            float result = divided > half ? half : divided;
            if (triangleCount <= 0) triangleCount = 1;
            return result;
        }

        public void PopulateMesh(VertexHelper mesh, Color32 color, ref float radius,
            ref int triangleCount, bool hollow)
        {
            mesh.Clear();
            float corner = PrepareCornerRadius(ref radius, ref triangleCount);
            Add(mesh, OuterMin, color, false);
            Add(mesh, new Vector2(OuterMin.x, OuterMax.y), color, false);
            Add(mesh, OuterMax, color, false);
            Add(mesh, new Vector2(OuterMax.x, OuterMin.y), color, false);
            Add(mesh, InnerMin, color, false);
            Add(mesh, new Vector2(InnerMin.x, InnerMax.y), color, false);
            Add(mesh, InnerMax, color, false);
            Add(mesh, new Vector2(InnerMax.x, InnerMin.y), color, false);
            if (!hollow)
            {
                mesh.AddTriangle(0,1,2);mesh.AddTriangle(2,3,0);
                return;
            }
            mesh.AddTriangle(0,1,4);mesh.AddTriangle(1,4,5);
            mesh.AddTriangle(1,5,2);mesh.AddTriangle(2,5,6);
            mesh.AddTriangle(2,6,3);mesh.AddTriangle(6,3,7);
            mesh.AddTriangle(4,7,3);mesh.AddTriangle(0,4,3);
            float step = (Mathf.PI * .5f) / triangleCount;
            AddCorner(mesh,color,InnerMin,new Vector2(InnerMin.x+corner,InnerMin.y+corner),
                new Vector2(InnerMin.x,InnerMin.y+corner),Mathf.PI,step,corner,triangleCount);
            AddCorner(mesh,color,new Vector2(InnerMin.x,InnerMax.y),new Vector2(InnerMin.x+corner,InnerMax.y-corner),
                new Vector2(InnerMin.x+corner,InnerMax.y),Mathf.PI*.5f,step,corner,triangleCount);
            AddCorner(mesh,color,InnerMax,new Vector2(InnerMax.x-corner,InnerMax.y-corner),
                new Vector2(InnerMax.x,InnerMax.y-corner),0f,step,corner,triangleCount);
            AddCorner(mesh,color,new Vector2(InnerMax.x,InnerMin.y),new Vector2(InnerMax.x-corner,InnerMin.y+corner),
                new Vector2(InnerMax.x-corner,InnerMin.y),Mathf.PI*1.5f,step,corner,triangleCount);
        }

        private void AddCorner(VertexHelper mesh,Color32 color,Vector2 anchor,Vector2 center,
            Vector2 first,float angle,float step,float radius,int triangles)
        {
            int start=mesh.currentVertCount;
            Add(mesh,anchor,color,true);
            Add(mesh,first,color,true);
            for(int i=0;i<triangles;i++)
            {
                angle+=step;
                Add(mesh,new Vector2(center.x+radius*Mathf.Cos(angle),center.y+radius*Mathf.Sin(angle)),color,true);
                mesh.AddTriangle(start,start+i+1,start+i+2);
            }
        }

        private void Add(VertexHelper mesh,Vector2 position,Color32 color,bool mapUV)
        {
            var vertex=UIVertex.simpleVert;
            vertex.position=position;vertex.color=color;
            if(mapUV)vertex.uv0=GetUV(position,Mathf.Abs(InnerMax.x-InnerMin.x),Mathf.Abs(InnerMax.y-InnerMin.y));
            mesh.AddVert(vertex);
        }

        // 0x9B6DE4, per vertex. Compute directly instead of allocating the
        // original one-element input/output arrays in every AddVert call.
        public static Vector2 GetUV(Vector2 position, float width, float height)
        {
            return new Vector2(position.x / width + .5f, position.y / height + .5f);
        }
    }
}
