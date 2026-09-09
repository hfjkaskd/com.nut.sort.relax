using System;
using Newtonsoft.Json.Linq;
using NutSort.World;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalHiddenCoverValidation
    {
        private static float N(JToken t,string k,float f=0)=>OriginalHiddenCoverBuilder.N(t,k,f);
        private static float Eval(JToken t,float time,Func<JToken,float> f,float setup)=>OriginalFixedScrewValidation.Evaluate(t,time,f,setup);
        public static void Validate()
        {
            var source=OriginalHiddenCoverBuilder.ReadSource();var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(OriginalHiddenCoverBuilder.Folder+"/HiddenCover.prefab");Check(prefab!=null,"Hidden prefab");var root=UnityEngine.Object.Instantiate(prefab);
            try
            {
                var driver=root.GetComponent<OriginalHiddenCoverMesh>();Check(driver.bones.Length==23&&driver.slots.Length==18&&driver.target.sharedMesh.vertexCount==72,"Bounded source geometry");
                Near(driver.unitsPerPixel,.01f,"Source unit scale");
                var uv=driver.target.sharedMesh.uv;
                Near(uv[44].x,318f/512,"Rotated cover lower-left u");Near(uv[44].y,260f/512,"Rotated cover lower-left v");
                Near(uv[46].x,2f/512,"Rotated cover upper-right u");Near(uv[46].y,433f/512,"Rotated cover upper-right v");
                Near(driver.slots[11].corners[0].x,-90.29f,"Cover attachment origin x");Near(driver.slots[11].corners[0].y,-110.79f,"Cover attachment origin y");
                var player=root.GetComponent<Animation>();
                foreach(var animation in (JObject)source["animations"])
                {
                    var clip=UnityEngine.Object.Instantiate(player[animation.Key].clip);clip.wrapMode=WrapMode.ClampForever;Near(clip.length,animation.Key=="wanzheng"?8:1,"Clip duration");
                    var times=new System.Collections.Generic.SortedSet<float>{0,clip.length};
                    foreach(var binding in AnimationUtility.GetCurveBindings(clip))
                    {var keys=AnimationUtility.GetEditorCurve(clip,binding).keys;for(int i=0;i<keys.Length;i++){times.Add(keys[i].time);if(i>0)times.Add((keys[i-1].time+keys[i].time)*.5f);}}
                    foreach(float time in times)
                    {
                        clip.SampleAnimation(root,time);
                        for(int i=0;i<driver.bones.Length;i++)
                        {
                            var data=source["bones"][i];var b=driver.bones[i];var t=animation.Value["bones"]?[(string)data["name"]];
                            Near(b.x,Eval(t?["translate"],time,k=>N(data,"x")+N(k,"x"),N(data,"x")),"x "+animation.Key+" time "+time+" bone "+i);
                            Near(b.y,Eval(t?["translate"],time,k=>N(data,"y")+N(k,"y"),N(data,"y")),"y");
                            Near(b.rotation,Eval(t?["rotate"],time,k=>N(data,"rotation")+N(k,"angle"),N(data,"rotation")),"rotation");
                            Near(b.scaleX,Eval(t?["scale"],time,k=>N(data,"scaleX",1)*N(k,"x",1),N(data,"scaleX",1)),"scale x");
                            Near(b.scaleY,Eval(t?["scale"],time,k=>N(data,"scaleY",1)*N(k,"y",1),N(data,"scaleY",1)),"scale y");
                            Near(b.shearX,Eval(t?["shear"],time,k=>N(data,"shearX")+N(k,"x"),N(data,"shearX")),"shear x");
                            Near(b.shearY,Eval(t?["shear"],time,k=>N(data,"shearY")+N(k,"y"),N(data,"shearY")),"shear y");
                        }
                        for(int i=0;i<18;i++)for(int component=0;component<4;component++)
                        {int c=component;var slot=source["slots"][i];Near(driver.slots[i].color[c],Eval(animation.Value["slots"]?[(string)slot["name"]]?["color"],time,k=>Convert.ToInt32(((string)k["color"]).Substring(c*2,2),16)/255f,1),"Slot color");}
                    }
                    UnityEngine.Object.DestroyImmediate(clip);
                }
                var polygon=root.GetComponentInChildren<SpriteMask>().sprite;Check(polygon.vertices.Length==44&&polygon.triangles.Length==126,"Exact clipping polygon topology");
                var vertices=polygon.vertices;var raw=source["skins"][0]["attachments"]["1234"]["1234"]["vertices"];
                for(int i=0;i<44;i++){Near(vertices[i].x,(float)raw[i*2]*.01f,"Clip x");Near(vertices[i].y,(float)raw[i*2+1]*.01f,"Clip y");}
                float area=0,triArea=0;for(int i=0;i<44;i++)area+=Cross(vertices[i],vertices[(i+1)%44]);var triangles=polygon.triangles;
                for(int i=0;i<triangles.Length;i+=3)triArea+=Mathf.Abs(Cross(vertices[triangles[i+1]]-vertices[triangles[i]],vertices[triangles[i+2]]-vertices[triangles[i]]));Near(Mathf.Abs(area),triArea,"Clip triangulated area");
                var test=driver.bones[0];test.x=3;test.y=4;test.rotation=0;test.scaleX=2;test.scaleY=3;test.shearX=30;test.shearY=-20;test.noScale=true;
                var m=OriginalHiddenCoverMesh.Compose(new OriginalHiddenCoverMesh.Affine{a=4,d=9,x=5,y=6},test);
                Near(m.x,17,"No-scale position inherits parent");Near(m.y,42,"No-scale y");Near(m.a,Mathf.Sqrt(3),"No-scale independent x shear");Near(m.c,1,"No-scale x basis");Near(m.b,3*Mathf.Cos(70*Mathf.Deg2Rad),"Independent y shear");Near(m.d,3*Mathf.Sin(70*Mathf.Deg2Rad),"Y basis");
                m=OriginalHiddenCoverMesh.Compose(new OriginalHiddenCoverMesh.Affine(),test);Near(m.a,0,"Zero-scale finite x");Near(m.d,0,"Zero-scale finite y");
                m=OriginalHiddenCoverMesh.Compose(new OriginalHiddenCoverMesh.Affine{a=4,d=-9},test);Near(m.c,-1,"Reflected parent orientation");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_HIDDEN_COVER_VALIDATION_PASS native clips at every key and interval midpoint, independent shear axes, no-scale and zero/reflected parent transforms, 72 body vertices, exact 44-point clipping polygon and triangulated area.");
        }
        private static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
        private static void Near(float a,float b,string message){Check(Mathf.Abs(a-b)<.0003f,message+": "+a+" expected "+b);}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
