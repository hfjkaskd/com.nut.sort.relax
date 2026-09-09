using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using NutSort.World;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
namespace NutSort.Validation
{
    public static class OriginalFixedScrewBuilder
    {
        public const string SourcePath="Assets/NutSort/Editor/AnimationConversion/OriginalFixedScrewSource.json";
        public const string Folder="Assets/Resources/Game/NativeEffects/FixedScrew";
        public static JObject ReadSource()=>JObject.Parse(File.ReadAllText(SourcePath));
        public static void Run()
        {
            try{Build();Debug.Log("NUT_FIXED_SCREW_BUILD_PASS native idle and break clips, 11 bones/slots, original atlas rotation and prefab binding.");EditorApplication.Exit(0);}
            catch(Exception error){Debug.LogException(error);EditorApplication.Exit(1);}
        }
        private static float Number(JToken t,string key,float fallback=0)=>(float?)t[key]??fallback;
        private static float ColorValue(string c,int component)=>Convert.ToInt32(c.Substring(component*2,2),16)/255f;
        private static T Store<T>(T value,string path) where T:UnityEngine.Object=>OriginalMaskSmokeBuilder.Store(value,path);
        private static void Build()
        {
            var source=ReadSource();Directory.CreateDirectory(Folder);AssetDatabase.Refresh();
            int w=(int)source["textureWidth"],h=(int)source["textureHeight"];var rgba=(JArray)source["pmaPixelsTopDown"];var pixels=new Color32[w*h];
            for(int y=0;y<h;y++)for(int x=0;x<w;x++)
            {
                int i=(y*w+x)*4;pixels[(h-1-y)*w+x]=new Color32((byte)(int)rgba[i],(byte)(int)rgba[i+1],(byte)(int)rgba[i+2],(byte)(int)rgba[i+3]);
            }
            var texture=new Texture2D(w,h,TextureFormat.RGBA32,false,false){name="FixedScrewPma",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};texture.SetPixels32(pixels);texture.Apply();
            texture=Store(texture,Folder+"/Texture.asset");
            var shader=Shader.Find("NutSort/Original Premultiplied Sprite");if(shader==null)throw new InvalidOperationException("PMA shader missing");
            var material=Store(new Material(shader){name="FixedScrew"},Folder+"/Material.mat");
            var sprites=new Dictionary<string,Sprite>();var rotated=new Dictionary<string,bool>();
            foreach(var region in (JArray)source["regions"])
            {
                string name=(string)region["name"];bool rotation=(bool)region["rotated"];float sx=w/(float)source["atlasWidth"],sy=h/(float)source["atlasHeight"];
                float width=(float)region["size"][rotation?1:0]*sx,height=(float)region["size"][rotation?0:1]*sy;
                var rect=new Rect((float)region["xy"][0]*sx,h-(float)region["xy"][1]*sy-height,width,height);
                var sprite=Sprite.Create(texture,rect,new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);sprite.name=name;
                sprites.Add(name,Store(sprite,Folder+"/"+name+".asset"));rotated.Add(name,rotation);
            }
            var root=new GameObject("FixedScrew");
            try
            {
                root.layer=6;root.AddComponent<SortingGroup>();var bones=new Dictionary<string,Transform>();
                foreach(var bone in (JArray)source["bones"])
                {
                    string name=(string)bone["name"];var t=new GameObject(name).transform;t.gameObject.layer=6;
                    t.SetParent(bone["parent"]==null?root.transform:bones[(string)bone["parent"]],false);
                    t.localPosition=new Vector3(Number(bone,"x"),Number(bone,"y"),0)*.01f;
                    t.localScale=new Vector3(Number(bone,"scaleX",1),Number(bone,"scaleY",1),1);
                    t.localRotation=Quaternion.Euler(0,0,Number(bone,"rotation"));bones.Add(name,t);
                }
                var slotPaths=new Dictionary<string,string>();int order=0;
                foreach(var slot in (JArray)source["slots"])
                {
                    string name=(string)slot["name"],region=(string)slot["region"];var t=new GameObject(name).transform;t.gameObject.layer=6;t.SetParent(bones[(string)slot["bone"]],false);
                    t.localPosition=new Vector3(Number(slot,"x"),Number(slot,"y"),0)*.01f;t.localRotation=Quaternion.Euler(0,0,Number(slot,"rotation"));t.localScale=new Vector3(Number(slot,"scaleX",1),Number(slot,"scaleY",1),1);
                    var quad=new GameObject("region").transform;quad.gameObject.layer=6;quad.SetParent(t,false);var sprite=sprites[region];bool rot=rotated[region];
                    quad.localScale=new Vector3(Number(slot,rot?"height":"width")/sprite.rect.width,Number(slot,rot?"width":"height")/sprite.rect.height,1);
                    quad.localRotation=Quaternion.Euler(0,0,rot?-90:0);
                    var renderer=quad.gameObject.AddComponent<SpriteRenderer>();renderer.sprite=sprite;renderer.sharedMaterial=material;renderer.sortingOrder=order++;renderer.shadowCastingMode=ShadowCastingMode.Off;
                    slotPaths.Add(name,AnimationUtility.CalculateTransformPath(quad,root.transform));
                }
                var player=root.AddComponent<Animation>();player.playAutomatically=false;player.cullingType=AnimationCullingType.AlwaysAnimate;
                foreach(var animation in (JObject)source["animations"])
                {
                    var clip=new AnimationClip{name=animation.Key,legacy=true,frameRate=30,wrapMode=animation.Key=="animation"?WrapMode.Loop:WrapMode.ClampForever};
                    foreach(var bone in (JArray)source["bones"])
                    {
                        string name=(string)bone["name"],path=AnimationUtility.CalculateTransformPath(bones[name],root.transform);var timeline=animation.Value["bones"]?[name];
                        Curve(clip,path,typeof(Transform),"m_LocalPosition.x",timeline?["translate"],k=>(Number(bone,"x")+Number(k,"x"))*.01f,Number(bone,"x")*.01f);
                        Curve(clip,path,typeof(Transform),"m_LocalPosition.y",timeline?["translate"],k=>(Number(bone,"y")+Number(k,"y"))*.01f,Number(bone,"y")*.01f);
                        Curve(clip,path,typeof(Transform),"m_LocalScale.x",timeline?["scale"],k=>Number(bone,"scaleX",1)*Number(k,"x",1),Number(bone,"scaleX",1));
                        Curve(clip,path,typeof(Transform),"m_LocalScale.y",timeline?["scale"],k=>Number(bone,"scaleY",1)*Number(k,"y",1),Number(bone,"scaleY",1));
                        Curve(clip,path,typeof(Transform),"localEulerAnglesRaw.z",timeline?["rotate"],k=>Number(bone,"rotation")+Number(k,"angle"),Number(bone,"rotation"));
                        Curve(clip,path,typeof(Transform),"localEulerAnglesRaw.x",null,k=>0,0);
                        Curve(clip,path,typeof(Transform),"localEulerAnglesRaw.y",null,k=>0,0);
                    }
                    foreach(var slot in (JArray)source["slots"])
                    {
                        string name=(string)slot["name"];
                        for(int component=0;component<4;component++)
                        {
                            int c=component;Curve(clip,slotPaths[name],typeof(SpriteRenderer),"m_Color."+new[]{"r","g","b","a"}[c],animation.Value["slots"]?[name]?["color"],k=>ColorValue((string)k["color"],c),ColorValue((string)slot["color"],c));
                        }
                    }
                    clip=Store(clip,Folder+"/"+animation.Key+".anim");player.AddClip(clip,animation.Key);if(animation.Key=="animation")player.clip=clip;
                }
                player.clip.SampleAnimation(root,0);PrefabUtility.SaveAsPrefabAsset(root,Folder+"/FixedScrew.prefab");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            var screw=PrefabUtility.LoadPrefabContents("Assets/Resources/Game/Screw.prefab");
            try
            {
                var view=screw.GetComponentInChildren<OriginalScrewTypeView>(true);var fields=new SerializedObject(view);var target=(GameObject)fields.FindProperty("DontMoveSpine").objectReferenceValue;
                var effect=target.GetComponent<OriginalNativeWorldEffect>();if(effect==null)effect=target.AddComponent<OriginalNativeWorldEffect>();var config=new SerializedObject(effect);
                config.FindProperty("prefabPath").stringValue="Game/NativeEffects/FixedScrew/FixedScrew";config.FindProperty("mixDuration").floatValue=(float)source["defaultMix"];config.ApplyModifiedPropertiesWithoutUndo();
                fields.FindProperty("dontMoveEffect").objectReferenceValue=effect;fields.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.SaveAsPrefabAsset(screw,"Assets/Resources/Game/Screw.prefab");
            }
            finally{PrefabUtility.UnloadPrefabContents(screw);}
            AssetDatabase.SaveAssets();
        }
        internal static void Curve(AnimationClip clip,string path,Type type,string property,JToken source,Func<JToken,float> value,float setup)
        {
            var keys=new JArray();
            // Before the first key Spine uses setup pose, not the first value.
            if(source==null||((JArray)source).Count==0||(float?)source[0]["time"]>0)
                keys.Add(new JObject{["time"]=0,["value"]=setup,["curve"]="stepped"});
            if(source!=null)foreach(var frame in (JArray)source)
            {
                var key=new JObject{["time"]=(float?)frame["time"]??0,["value"]=value(frame)};
                if(frame["curve"]!=null)key["curve"]=frame["curve"].DeepClone();keys.Add(key);
            }
            OriginalMaskSmokeBuilder.SetCurve(clip,path,type,property,keys,k=>(float)k["value"]);
        }
    }
}
