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
    public static class OriginalMaskSmokeBuilder
    {
        public const string SourcePath="Assets/NutSort/Editor/AnimationConversion/OriginalMaskSmokeSource.json";
        public const string Folder="Assets/Resources/Game/NativeEffects/MaskSmoke";
        public const string PrefabPath=Folder+"/MaskSmoke.prefab";
        public static JObject ReadSource()=>JObject.Parse(File.ReadAllText(SourcePath));
        public static void Run()
        {
            try{Build();Debug.Log("NUT_MASK_SMOKE_BUILD_PASS native texture, sprite, 21-bone/24-slot prefab and original linear/stepped animation clip.");EditorApplication.Exit(0);}
            catch(Exception error){Debug.LogException(error);EditorApplication.Exit(1);}
        }
        private static void Build()
        {
            var source=ReadSource();Directory.CreateDirectory(Folder);AssetDatabase.Refresh();
            int width=(int)source["textureWidth"],height=(int)source["textureHeight"];
            var values=(JArray)source["pmaPixelsTopDown"];var pixels=new Color[width*height];
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)
            {
                int offset=(y*width+x)*4;float alpha=(int)values[offset+3];
                // Keep PMA texels intact: unpremultiplying before bilinear
                // sampling changes edge colors. The authored native ShaderLab
                // pass applies slot alpha exactly once after texture sampling.
                pixels[(height-1-y)*width+x]=new Color(
                    (int)values[offset]/255f,(int)values[offset+1]/255f,(int)values[offset+2]/255f,alpha/255f);
            }
            var texture=new Texture2D(width,height,TextureFormat.RGBA32,false,false){name="MaskSmokePremultiplied",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
            texture.SetPixels(pixels);texture.Apply(false,false);
            texture=Store(texture,Folder+"/MaskSmokeTexture.asset");
            var region=(JArray)source["region"];float sx=width/(float)source["atlasWidth"],sy=height/(float)source["atlasHeight"];
            var rect=new Rect((float)region[0]*sx,height-((float)region[1]+(float)region[3])*sy,(float)region[2]*sx,(float)region[3]*sy);
            var sprite=Sprite.Create(texture,rect,new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);sprite.name="yan1";
            sprite=Store(sprite,Folder+"/MaskSmokeSprite.asset");
            var shader=Shader.Find("NutSort/Original Premultiplied Sprite");
            if(shader==null)throw new InvalidOperationException("Native PMA sprite shader missing");
            var material=Store(new Material(shader){name="MaskSmoke"},Folder+"/MaskSmoke.mat");
            var root=new GameObject("MaskSmoke");
            try
            {
                root.layer=6;root.AddComponent<SortingGroup>();
                var bones=new Dictionary<string,Transform>();var clip=new AnimationClip{name="animation",legacy=true,frameRate=30,wrapMode=WrapMode.ClampForever};
                foreach(var bone in (JArray)source["bones"])
                {
                    string name=(string)bone["name"];var transform=new GameObject(name).transform;transform.gameObject.layer=6;
                    transform.SetParent(bone["parent"]==null?root.transform:bones[(string)bone["parent"]],false);
                    transform.localPosition=new Vector3((float?)bone["x"]??0,(float?)bone["y"]??0,0)*.01f;bones.Add(name,transform);
                    var timelines=source["boneTimelines"][name];
                    if(timelines==null)continue;
                    foreach(var channel in ((JObject)timelines).Properties())
                    {
                        if(channel.Name!="scale")throw new InvalidOperationException("Unsupported smoke bone channel "+channel.Name);
                        string path=AnimationUtility.CalculateTransformPath(transform,root.transform);
                        SetCurve(clip,path,typeof(Transform),"m_LocalScale.x",(JArray)channel.Value,k=>(float?)k["x"]??1);
                        SetCurve(clip,path,typeof(Transform),"m_LocalScale.y",(JArray)channel.Value,k=>(float?)k["y"]??1);
                    }
                }
                int order=0;
                foreach(var slot in (JArray)source["slots"])
                {
                    var transform=new GameObject((string)slot["name"]).transform;transform.gameObject.layer=6;
                    transform.SetParent(bones[(string)slot["bone"]],false);
                    transform.localPosition=new Vector3((float)slot["x"],(float)slot["y"],0)*.01f;
                    transform.localScale=new Vector3((float)slot["width"]/rect.width,(float)slot["height"]/rect.height,1);
                    var renderer=transform.gameObject.AddComponent<SpriteRenderer>();renderer.sprite=sprite;renderer.sharedMaterial=material;renderer.sortingOrder=order++;
                    renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
                    var colors=(JArray)slot["colors"];string path=AnimationUtility.CalculateTransformPath(transform,root.transform);
                    for(int c=0;c<4;c++)
                    {
                        int component=c;string property=new[]{"r","g","b","a"}[c];
                        SetCurve(clip,path,typeof(SpriteRenderer),"m_Color."+property,colors,k=>Channel((string)k["color"],component));
                    }
                }
                clip=Store(clip,Folder+"/animation.anim");
                var animation=root.AddComponent<Animation>();animation.playAutomatically=false;animation.cullingType=AnimationCullingType.AlwaysAnimate;
                animation.AddClip(clip,"animation");animation.clip=clip;clip.SampleAnimation(root,0);
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            var screw=PrefabUtility.LoadPrefabContents("Assets/Resources/Game/Screw.prefab");
            try
            {
                var view=screw.GetComponentInChildren<OriginalScrewTypeView>(true);var fields=new SerializedObject(view);
                var target=(GameObject)fields.FindProperty("MaskSpine").objectReferenceValue;
                var player=target.GetComponent<OriginalNativeWorldEffect>();if(player==null)player=target.AddComponent<OriginalNativeWorldEffect>();
                var playerFields=new SerializedObject(player);playerFields.FindProperty("prefabPath").stringValue="Game/NativeEffects/MaskSmoke/MaskSmoke";playerFields.ApplyModifiedPropertiesWithoutUndo();
                fields.FindProperty("maskEffect").objectReferenceValue=player;fields.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(screw,"Assets/Resources/Game/Screw.prefab");
            }
            finally{PrefabUtility.UnloadPrefabContents(screw);}
            AssetDatabase.SaveAssets();
        }
        private static float Channel(string value,int index)=>Convert.ToInt32(value.Substring(index*2,2),16)/255f;
        private static void SetCurve(AnimationClip clip,string path,Type type,string property,JArray frames,Func<JToken,float> value)
        {
            var keys=new Keyframe[frames.Count];
            for(int i=0;i<keys.Length;i++)keys[i]=new Keyframe((float?)frames[i]["time"]??0,value(frames[i]));
            for(int i=0;i<keys.Length;i++)
            {
                var key=keys[i];
                if(i>0)key.inTangent=Slope(frames,keys,i-1);
                if(i+1<keys.Length)key.outTangent=Slope(frames,keys,i);
                keys[i]=key;
            }
            AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve(path,type,property),new AnimationCurve(keys));
        }
        private static float Slope(JArray frames,Keyframe[] keys,int i)
        {
            var curve=frames[i]["curve"];
            if(curve!=null)
            {
                if((string)curve!="stepped")throw new InvalidOperationException("Unsupported smoke interpolation");
                return float.PositiveInfinity;
            }
            return (keys[i+1].value-keys[i].value)/(keys[i+1].time-keys[i].time);
        }
        private static T Store<T>(T value,string path) where T:UnityEngine.Object
        {
            var existing=AssetDatabase.LoadAssetAtPath<T>(path);
            if(existing==null){AssetDatabase.CreateAsset(value,path);return value;}
            EditorUtility.CopySerialized(value,existing);UnityEngine.Object.DestroyImmediate(value);return existing;
        }
    }
}
