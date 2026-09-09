using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.World;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using Unity.Collections;
namespace NutSort.Validation
{
    public static class OriginalHiddenCoverBuilder
    {
        public const string SourcePath="Assets/NutSort/Editor/AnimationConversion/OriginalHiddenCoverSource.json";
        public const string Folder="Assets/Resources/Game/NativeEffects/HiddenCover";
        public static JObject ReadSource()=>JObject.Parse(File.ReadAllText(SourcePath));
        public static float N(JToken t,string k,float fallback=0)=>(float?)t?[k]??fallback;
        private static T Store<T>(T value,string path) where T:UnityEngine.Object=>OriginalMaskSmokeBuilder.Store(value,path);
        public static void Run(){EditorApplication.update+=BuildOnUpdate;}
        private static void BuildOnUpdate()
        {EditorApplication.update-=BuildOnUpdate;try{Build();Debug.Log("NUT_HIDDEN_COVER_BUILD_PASS");EditorApplication.Exit(0);}catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
        private static void Curve(AnimationClip clip,string path,Type type,string property,JToken source,Func<JToken,float> value,float setup)
            =>OriginalFixedScrewBuilder.Curve(clip,path,type,property,source,value,setup);
        private static Color Tint(string value)
        {ColorUtility.TryParseHtmlString("#"+(value??"ffffffff"),out var color);return color;}
        private static void Build()
        {
            var data=ReadSource();Directory.CreateDirectory(Folder);AssetDatabase.Refresh();
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="HiddenCoverPma",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
            if(!ImageConversion.LoadImage(texture,Convert.FromBase64String((string)data["pngBase64"])))throw new InvalidDataException("Hidden texture");
            texture=Store(texture,Folder+"/Texture.asset");
            var material=new Material(Shader.Find("NutSort/Original Premultiplied Sprite")){name="HiddenCover",mainTexture=texture};material=Store(material,Folder+"/Material.mat");
            var root=new GameObject("HiddenCover");root.layer=6;
            try
            {
                root.AddComponent<SortingGroup>();var driver=root.AddComponent<OriginalHiddenCoverMesh>();
                var boneData=(JArray)data["bones"];var bones=new OriginalEffectBone[boneData.Count];var indices=new Dictionary<string,int>();
                for(int i=0;i<bones.Length;i++)
                {
                    var b=boneData[i];string name=(string)b["name"];var go=new GameObject(name);go.layer=6;go.transform.SetParent(root.transform,false);
                    var state=go.AddComponent<OriginalEffectBone>();state.x=N(b,"x");state.y=N(b,"y");state.rotation=N(b,"rotation");state.scaleX=N(b,"scaleX",1);state.scaleY=N(b,"scaleY",1);state.shearX=N(b,"shearX");state.shearY=N(b,"shearY");
                    state.parentIndex=b["parent"]==null?-1:indices[(string)b["parent"]];string mode=(string)b["transform"]??"normal";
                    if(mode!="normal"&&mode!="noScale")throw new InvalidDataException("Unsupported transform "+mode);state.noScale=mode=="noScale";bones[i]=state;indices.Add(name,i);
                }
                var slots=(JArray)data["slots"];var attachments=data["skins"][0]["attachments"];
                var states=new OriginalEffectSlot[18];var uv=new Vector2[72];var triangles=new int[108];var positions=new Vector3[72];
                for(int i=0;i<18;i++)
                {
                    var slot=slots[i];string name=(string)slot["name"],attachment=(string)slot["attachment"];var a=attachments[name][attachment];
                    var go=new GameObject("slot"+i);go.layer=6;go.transform.SetParent(root.transform,false);var state=go.AddComponent<OriginalEffectSlot>();states[i]=state;
                    state.boneIndex=indices[(string)slot["bone"]];state.color=Tint((string)slot["color"]);state.corners=new Vector2[4];
                    var region=data["regions"][(string)a["path"]??attachment];bool rotated=(bool)region["rotated"];
                    float u0=N(region,"x")/512,v1=1-N(region,"y")/512;
                    float u1=u0+N(region,rotated?"height":"width")/512,v0=v1-N(region,rotated?"width":"height")/512;
                    for(int j=0;j<4;j++)
                    {
                        bool right=j==2||j==3,top=j==1||j==2;
                        var point=new Vector2((right?1:-1)*N(a,"width")*.5f*N(a,"scaleX",1),(top?1:-1)*N(a,"height")*.5f*N(a,"scaleY",1));
                        float angle=N(a,"rotation")*Mathf.Deg2Rad;
                        state.corners[j]=new Vector2(Mathf.Cos(angle)*point.x-Mathf.Sin(angle)*point.y+N(a,"x"),Mathf.Sin(angle)*point.x+Mathf.Cos(angle)*point.y+N(a,"y"));
                        uv[i*4+j]=rotated?new Vector2(top?u0:u1,right?v1:v0):new Vector2(right?u1:u0,top?v1:v0);
                    }
                    int start=i*4,t=i*6;triangles[t]=start;triangles[t+1]=start+1;triangles[t+2]=start+2;triangles[t+3]=start;triangles[t+4]=start+2;triangles[t+5]=start+3;
                }
                var mesh=new Mesh{name="HiddenCoverRegions",vertices=positions,uv=uv,triangles=triangles};mesh=Store(mesh,Folder+"/Regions.asset");
                var body=new GameObject("Body");body.layer=6;body.transform.SetParent(root.transform,false);var filter=body.AddComponent<MeshFilter>();filter.sharedMesh=mesh;
                var renderer=body.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.sortingOrder=0;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
                driver.bones=bones;driver.slots=states;driver.target=filter;driver.unitsPerPixel=.01f;
                // Only the last slot is clipped; all body slots precede the source clip.
                var glowSlot=slots[19];string glowName=(string)glowSlot["name"];var ga=attachments[glowName][(string)glowSlot["attachment"]];
                var glow=new GameObject("Glow");glow.layer=6;glow.transform.SetParent(root.transform,false);
                var gr=data["regions"][(string)glowSlot["attachment"]];var sprite=Sprite.Create(texture,new Rect(N(gr,"x"),512-N(gr,"y")-N(gr,"height"),N(gr,"width"),N(gr,"height")),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);
                sprite.name="Glow";sprite=Store(sprite,Folder+"/Glow.asset");var glowRenderer=glow.AddComponent<SpriteRenderer>();glowRenderer.sprite=sprite;glowRenderer.sharedMaterial=material;glowRenderer.sortingOrder=19;glowRenderer.maskInteraction=SpriteMaskInteraction.VisibleInsideMask;
                var clipSlot=slots[18];var ca=attachments[(string)clipSlot["name"]][(string)clipSlot["attachment"]];var raw=(JArray)ca["vertices"];var polygon=new Vector2[raw.Count/2];
                if((string)ca["end"]!=(string)clipSlot["name"]||indices[(string)clipSlot["bone"]]!=22)throw new InvalidDataException("Changed clipping scope");
                for(int i=0;i<polygon.Length;i++)polygon[i]=new Vector2((float)raw[i*2],(float)raw[i*2+1])*.01f;
                Vector2 min=polygon[0],max=min;foreach(var point in polygon){min=Vector2.Min(min,point);max=Vector2.Max(max,point);}float extent=Mathf.Max(max.x-min.x,max.y-min.y);
                var white=new Texture2D(1,1,TextureFormat.RGBA32,false){name="MaskWhite"};white.SetPixel(0,0,Color.white);white.Apply();white=Store(white,Folder+"/MaskWhite.asset");
                var maskSprite=Sprite.Create(white,new Rect(0,0,1,1),-min/extent,1/extent,0,SpriteMeshType.FullRect);maskSprite.name="ClipPolygon";SetPolygon(maskSprite,polygon);if(maskSprite.vertices.Length!=polygon.Length)throw new InvalidOperationException("Native polygon override failed");maskSprite=Store(maskSprite,Folder+"/ClipPolygon.asset");
                var maskGo=new GameObject("GlowClip");maskGo.layer=6;maskGo.transform.SetParent(root.transform,false);var mask=maskGo.AddComponent<SpriteMask>();mask.sprite=maskSprite;mask.isCustomRangeActive=true;mask.backSortingOrder=18;mask.frontSortingOrder=20;
                var player=root.AddComponent<Animation>();player.playAutomatically=false;player.cullingType=AnimationCullingType.AlwaysAnimate;
                foreach(var entry in (JObject)data["animations"])
                {
                    var clip=new AnimationClip{name=entry.Key,legacy=true,frameRate=30,wrapMode=entry.Key=="wanzheng"?WrapMode.Loop:WrapMode.ClampForever};
                    for(int i=0;i<bones.Length;i++)
                    {
                        var b=boneData[i];string name=(string)b["name"];var t=entry.Value["bones"]?[name];
                        string[] properties={"x","y","rotation","scaleX","scaleY","shearX","shearY"};string[] timelines={"translate","translate","rotate","scale","scale","shear","shear"};string[] keys={"x","y","angle","x","y","x","y"};
                        for(int j=0;j<properties.Length;j++)
                        {int k=j;bool scale=k==3||k==4;float setup=N(b,properties[k],scale?1:0);Curve(clip,name,typeof(OriginalEffectBone),properties[k],t?[timelines[k]],f=>scale?setup*N(f,keys[k],1):setup+N(f,keys[k]),setup);}
                    }
                    for(int i=0;i<18;i++)
                    {
                        var slot=slots[i];string name=(string)slot["name"];var timeline=entry.Value["slots"]?[name];
                        if(timeline?["attachment"]!=null)foreach(var frame in (JArray)timeline["attachment"])if((string)frame["name"]!=(string)slot["attachment"])throw new InvalidDataException("Changed attachment");
                        for(int j=0;j<4;j++){int c=j;Curve(clip,"slot"+i,typeof(OriginalEffectSlot),"color."+new[]{"r","g","b","a"}[c],timeline?["color"],f=>Tint((string)f["color"])[c],Tint((string)slot["color"])[c]);}
                    }
                    var gb=boneData[21];var gt=entry.Value["bones"]?[(string)gb["name"]];
                    Curve(clip,"Glow",typeof(Transform),"m_LocalPosition.x",gt?["translate"],f=>(N(gb,"x")+N(f,"x")+N(ga,"x"))*.01f,(N(gb,"x")+N(ga,"x"))*.01f);
                    Curve(clip,"Glow",typeof(Transform),"m_LocalPosition.y",gt?["translate"],f=>(N(gb,"y")+N(f,"y")+N(ga,"y"))*.01f,(N(gb,"y")+N(ga,"y"))*.01f);
                    for(int j=0;j<4;j++){int c=j;Curve(clip,"Glow",typeof(SpriteRenderer),"m_Color."+new[]{"r","g","b","a"}[c],entry.Value["slots"]?[glowName]?["color"],f=>Tint((string)f["color"])[c],Tint((string)glowSlot["color"])[c]);}
                    clip=Store(clip,Folder+"/"+entry.Key+".anim");player.AddClip(clip,entry.Key);if(entry.Key=="wanzheng")player.clip=clip;
                }
                player.clip.SampleAnimation(root,0);
                // Save posed native mesh; runtime clones the shared mesh once.
                var matrices=new OriginalHiddenCoverMesh.Affine[bones.Length];
                for(int i=0;i<bones.Length;i++)matrices[i]=OriginalHiddenCoverMesh.Compose(bones[i].parentIndex<0?OriginalHiddenCoverMesh.Affine.Identity:matrices[bones[i].parentIndex],bones[i]);
                var colors=new Color[72];for(int i=0;i<18;i++)for(int j=0;j<4;j++){positions[i*4+j]=matrices[states[i].boneIndex].Point(states[i].corners[j])*.01f;colors[i*4+j]=states[i].color;}
                mesh.vertices=positions;mesh.colors=colors;mesh.RecalculateBounds();EditorUtility.SetDirty(mesh);
                PrefabUtility.SaveAsPrefabAsset(root,Folder+"/HiddenCover.prefab");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            var screw=PrefabUtility.LoadPrefabContents("Assets/Resources/Game/Screw.prefab");
            try
            {
                var view=screw.GetComponentInChildren<OriginalScrewTypeView>(true);var fields=new SerializedObject(view);var target=(GameObject)fields.FindProperty("HiddenSpine").objectReferenceValue;
                var effect=target.GetComponent<OriginalNativeWorldEffect>();if(effect==null)effect=target.AddComponent<OriginalNativeWorldEffect>();var config=new SerializedObject(effect);
                config.FindProperty("prefabPath").stringValue="Game/NativeEffects/HiddenCover/HiddenCover";config.FindProperty("mixDuration").floatValue=.2f;config.ApplyModifiedPropertiesWithoutUndo();fields.FindProperty("hiddenEffect").objectReferenceValue=effect;fields.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.SaveAsPrefabAsset(screw,"Assets/Resources/Game/Screw.prefab");
            }
            finally{PrefabUtility.UnloadPrefabContents(screw);}AssetDatabase.SaveAssets();
        }
        private static void SetPolygon(Sprite sprite,Vector2[] polygon)
        {
            sprite.SetVertexCount(polygon.Length);
            var pointData=new Vector3[polygon.Length];var uvData=new Vector2[polygon.Length];
            for(int i=0;i<polygon.Length;i++){pointData[i]=polygon[i];uvData[i]=new Vector2(.5f,.5f);}
            using(var positions=new NativeArray<Vector3>(pointData,Allocator.Temp))
            using(var uv=new NativeArray<Vector2>(uvData,Allocator.Temp))
            using(var indices=new NativeArray<ushort>(Triangulate(polygon),Allocator.Temp))
            {
                sprite.SetVertexAttribute(VertexAttribute.Position,positions);
                sprite.SetVertexAttribute(VertexAttribute.TexCoord0,uv);
                sprite.SetIndices(indices);
            }
        }
        private static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
        public static ushort[] Triangulate(Vector2[] points)
        {
            var remaining=new List<int>();float area=0;for(int i=0;i<points.Length;i++)area+=Cross(points[i],points[(i+1)%points.Length]);
            for(int i=0;i<points.Length;i++)remaining.Add(area>0?i:points.Length-1-i);
            var result=new List<ushort>();
            while(remaining.Count>3)
            {
                bool found=false;
                for(int i=0;i<remaining.Count;i++)
                {
                    int a=remaining[(i+remaining.Count-1)%remaining.Count],b=remaining[i],c=remaining[(i+1)%remaining.Count];
                    if(Cross(points[b]-points[a],points[c]-points[b])<=0)continue;bool inside=false;
                    foreach(int k in remaining)if(k!=a&&k!=b&&k!=c&&Cross(points[b]-points[a],points[k]-points[a])>=0&&Cross(points[c]-points[b],points[k]-points[b])>=0&&Cross(points[a]-points[c],points[k]-points[c])>=0){inside=true;break;}
                    if(inside)continue;result.Add((ushort)a);result.Add((ushort)b);result.Add((ushort)c);remaining.RemoveAt(i);found=true;break;
                }
                if(!found)throw new InvalidDataException("Non-simple clipping polygon");
            }
            foreach(int i in remaining)result.Add((ushort)i);return result.ToArray();
        }
    }
}
