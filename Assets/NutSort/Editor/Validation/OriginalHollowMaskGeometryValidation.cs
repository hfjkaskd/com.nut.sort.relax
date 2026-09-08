using System;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalHollowMaskGeometryValidation
    {
        public static void Validate()
        {
            var root=new GameObject("Hollow bounds fixture",typeof(RectTransform));
            var outer=(RectTransform)root.transform;outer.sizeDelta=new Vector2(400,800);
            outer.pivot=new Vector2(.25f,.75f);
            var inner=(RectTransform)new GameObject("Target",typeof(RectTransform)).transform;
            inner.SetParent(outer,false);inner.anchorMin=inner.anchorMax=outer.pivot;
            inner.sizeDelta=new Vector2(100,40);inner.anchoredPosition=new Vector2(20,30);
            try
            {
                var geometry=new OriginalHollowMaskGeometry();
                geometry.CalculateBounds(outer,null);
                Check(geometry.OuterMax==Vector2.zero,"Null target leaves initial outer bounds untouched");
                geometry.CalculateBounds(outer,inner);
                Check(Near(geometry.InnerMin,new Vector2(-30,10)) && Near(geometry.InnerMax,new Vector2(70,50)),"Translated relative inner bounds");
                Check(Near(geometry.OuterMin,new Vector2(-100,-600)) && Near(geometry.OuterMax,new Vector2(300,200)),"Noncentral outer pivot retained");
                using(var helper=new VertexHelper())
                {
                    float meshRadius=25;int meshCount=6;
                    geometry.PopulateMesh(helper,new Color32(1,2,3,128),ref meshRadius,ref meshCount,true);
                    Check(helper.currentVertCount==40 && helper.currentIndexCount==96,"Eight base vertices plus four native six-segment fans");
                    var vertex=new UIVertex();helper.PopulateUIVertex(ref vertex,0);
                    Check((Vector2)vertex.position==geometry.OuterMin && vertex.uv0==Vector4.zero && vertex.color.a==128,"Base vertex keeps simpleVert UV and Graphic tint");
                    helper.PopulateUIVertex(ref vertex,8);
                    Check(Near(vertex.position,geometry.InnerMin) && Near(vertex.uv0,new Vector2(.2f,.75f)),"First fan anchors at inner lower left with inner-size UV mapping");
                    helper.PopulateUIVertex(ref vertex,15);
                    Check(Near(vertex.position,new Vector2(-26,10)),"First corner ends at its lower tangent");
                    helper.PopulateUIVertex(ref vertex,23);
                    Check(Near(vertex.position,new Vector2(-30,46)),"Second corner ends at its left tangent");
                    var rendered=new Mesh();
                    try
                    {
                        helper.FillMesh(rendered);var points=rendered.vertices;var indices=rendered.triangles;
                        Check(indices[0]==0 && indices[1]==1 && indices[2]==4 && indices[24]==8 && indices[25]==9 && indices[26]==10,"Source base and fan triangle order");
                        double area=0;
                        for(int i=0;i<indices.Length;i+=3)
                        {
                            Vector3 a=points[indices[i]],b=points[indices[i+1]],c=points[indices[i+2]];
                            area+=Math.Abs((b.x-a.x)*(c.y-a.y)-(b.y-a.y)*(c.x-a.x))*.5;
                        }
                        double expected=400*800-100*40+4*(16-3*16*Math.Sin(Math.PI/12));
                        Check(Math.Abs(area-expected)<.02,"Mesh covers outer rectangle excluding native polygonal rounded hole");
                    }
                    finally{UnityEngine.Object.DestroyImmediate(rendered);}
                    geometry.PopulateMesh(helper,Color.white,ref meshRadius,ref meshCount,false);
                    Check(helper.currentVertCount==8 && helper.currentIndexCount==6,"Solid mode clears former fans and retains unused inner vertices as source");
                }
                float radius=25;int triangles=6;
                Check(geometry.PrepareCornerRadius(ref radius,ref triangles)==4 && radius==25 && triangles==6,"Native prefab divisor and six segments");
                radius=0;triangles=0;
                Check(geometry.PrepareCornerRadius(ref radius,ref triangles)==50 && triangles==1,"Zero divisor saturates at half width and count repairs to one");
                radius=-4;triangles=-2;
                Check(geometry.PrepareCornerRadius(ref radius,ref triangles)==50 && radius==0 && triangles==1,"Negative radius and count mutate as source");
                radius=float.NaN;triangles=6;
                Check(float.IsNaN(geometry.PrepareCornerRadius(ref radius,ref triangles)),"Unordered division is preserved");
                var child=(RectTransform)new GameObject("Extending child",typeof(RectTransform)).transform;
                child.SetParent(inner,false);child.anchorMin=child.anchorMax=new Vector2(.5f,.5f);
                child.sizeDelta=new Vector2(20,20);child.anchoredPosition=new Vector2(100,0);
                geometry.CalculateBounds(outer,inner);
                Check(Near(geometry.InnerMax,new Vector2(130,50)),"Descendant outside target expands hollow bounds");
                inner.localRotation=Quaternion.Euler(0,0,90);geometry.CalculateBounds(outer,inner);
                Check(Near(geometry.InnerMin,new Vector2(0,-20)) && Near(geometry.InnerMax,new Vector2(40,140)),"Rotated target and descendant are measured in outer coordinates");
                Vector2 remembered=geometry.InnerMax,outerRemembered=geometry.OuterMax;
                UnityEngine.Object.DestroyImmediate(inner.gameObject);outer.sizeDelta=new Vector2(800,1600);
                geometry.CalculateBounds(outer,inner);
                Check(geometry.InnerMax==remembered && geometry.OuterMax==outerRemembered,"Destroyed target retains cached inner and outer bounds");
                Check(OriginalHollowMaskGeometry.GetUV(new Vector2(-100,200),400,800)==new Vector2(.25f,.75f),"UV mapping uses position/dimensions plus one half");
                Check(OriginalHollowMaskGeometry.GetUV(new Vector2(800,-800),400,800)==new Vector2(2.5f,-.5f),"UV values outside unit range are not clamped");
                Vector2 zero=OriginalHollowMaskGeometry.GetUV(Vector2.zero,0,0);
                Check(float.IsNaN(zero.x) && float.IsNaN(zero.y),"Zero dimensions have no invented fallback");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_HOLLOW_MASK_GEOMETRY_VALIDATION_PASS native relative descendant bounds, pivot and rotation, destroyed-target cache, divisor radius/count normalization and UV edge semantics; Graphic mesh binding pending.");
        }
        private static bool Near(Vector2 a,Vector2 b)=>Vector2.Distance(a,b)<.001f;
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
