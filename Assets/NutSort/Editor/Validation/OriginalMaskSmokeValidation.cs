using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalMaskSmokeValidation
    {
        public static void Validate()
        {
            var source=OriginalMaskSmokeBuilder.ReadSource();
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(OriginalMaskSmokeBuilder.PrefabPath);
            Check(prefab!=null,"Converted smoke prefab exists");
            var instance=UnityEngine.Object.Instantiate(prefab);
            try
            {
                var clip=instance.GetComponent<Animation>().clip;
                Check(clip.legacy&&Mathf.Abs(clip.length-(float)source["duration"])<.00001f,"Native clip duration");
                Check(AnimationUtility.GetCurveBindings(clip).Length==136,"All 40 bone scale and 96 slot color curves");
                var renderers=instance.GetComponentsInChildren<SpriteRenderer>();Check(renderers.Length==24,"All source slots");
                var sprite=renderers[0].sprite;var texture=sprite.texture;
                Check(texture.width==64&&texture.height==64&&texture.format==TextureFormat.RGBA32,"Original decoded texture dimensions and format");
                var pixels=texture.GetPixels32();var rgba=(JArray)source["pmaPixelsTopDown"];
                for(int y=0;y<64;y++)for(int x=0;x<64;x++)
                {
                    int i=(y*64+x)*4;var pixel=pixels[(63-y)*64+x];
                    Check(pixel.r==(int)rgba[i]&&pixel.g==(int)rgba[i+1]&&pixel.b==(int)rgba[i+2]&&pixel.a==(int)rgba[i+3],"Source PMA texels remain byte-exact");
                }
                float u0=1,v0=1,u1=0,v1=0;
                foreach(var uv in sprite.uv){u0=Mathf.Min(u0,uv.x);v0=Mathf.Min(v0,uv.y);u1=Mathf.Max(u1,uv.x);v1=Mathf.Max(v1,uv.y);}
                Near(u0,2f/94,"Atlas u min");Near(u1,92f/94,"Atlas u max");Near(v0,2f/81,"Atlas v min");Near(v1,79f/81,"Atlas v max");
                foreach(float time in new[]{0f,.08335f,.25f,.5667f,.9667f,1.1333f})
                {
                    clip.SampleAnimation(instance,time);
                    foreach(var bone in (JObject)source["boneTimelines"])
                    {
                        var t=instance.transform.Find("root/"+bone.Key);var keys=(JArray)bone.Value["scale"];
                        Near(t.localScale.x,Evaluate(keys,time,k=>(float?)k["x"]??1),"Bone x "+bone.Key);
                        Near(t.localScale.y,Evaluate(keys,time,k=>(float?)k["y"]??1),"Bone y "+bone.Key);
                    }
                    int index=0;
                    foreach(var slot in (JArray)source["slots"])
                    {
                        string bone=(string)slot["bone"],name=(string)slot["name"];
                        var renderer=instance.transform.Find((bone=="root"?"root":"root/"+bone)+"/"+name).GetComponent<SpriteRenderer>();
                        Check(renderer.sortingOrder==index++,"Original slot order");
                        Check(renderer.sharedMaterial==renderers[0].sharedMaterial&&renderer.sharedMaterial.shader.name=="NutSort/Original Premultiplied Sprite","Shared native PMA material");
                        for(int c=0;c<4;c++)
                        {
                            int component=c;float expected=Evaluate((JArray)slot["colors"],time,k=>Convert.ToInt32(((string)k["color"]).Substring(component*2,2),16)/255f);
                            Near(renderer.color[c],expected,"Slot color "+name);
                        }
                    }
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(instance);}
            Debug.Log("NUT_MASK_SMOKE_VALIDATION_PASS source-exact PMA pixels and normalized atlas UV, 21 bones/24 slots, 136 native curves sampled against source linear/stepped timelines, original slot order and shared material.");
        }
        private static float Evaluate(JArray keys,float time,Func<JToken,float> value)
        {
            int i=0;while(i+1<keys.Count&&time>=((float?)keys[i+1]["time"]??0))i++;
            if(i+1==keys.Count||(string)keys[i]["curve"]=="stepped")return value(keys[i]);
            float a=(float?)keys[i]["time"]??0,b=(float?)keys[i+1]["time"]??0;
            return Mathf.Lerp(value(keys[i]),value(keys[i+1]),Mathf.Clamp01((time-a)/(b-a)));
        }
        private static void Near(float a,float b,string message){Check(Mathf.Abs(a-b)<.00005f,message+": "+a+" expected "+b);}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
