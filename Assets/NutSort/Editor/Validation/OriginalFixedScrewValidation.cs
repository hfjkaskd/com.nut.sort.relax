using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalFixedScrewValidation
    {
        private static float N(JToken token,string field,float fallback=0)=>(float?)token[field]??fallback;
        public static float Evaluate(JToken timeline,float time,Func<JToken,float> value,float setup)
        {
            if(timeline==null||((JArray)timeline).Count==0||time<N(timeline[0],"time"))return setup;
            int i=0;while(i+1<((JArray)timeline).Count&&time>=N(timeline[i+1],"time"))i++;
            if(i+1==((JArray)timeline).Count||(string)timeline[i]["curve"]=="stepped")return value(timeline[i]);
            float a=N(timeline[i],"time"),b=N(timeline[i+1],"time");return Mathf.Lerp(value(timeline[i]),value(timeline[i+1]),(time-a)/(b-a));
        }
        public static void Validate()
        {
            var source=OriginalFixedScrewBuilder.ReadSource();var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(OriginalFixedScrewBuilder.Folder+"/FixedScrew.prefab");
            Check(prefab!=null,"Native fixed screw prefab exists");var instance=UnityEngine.Object.Instantiate(prefab);
            try
            {
                Check(instance.GetComponentsInChildren<SpriteRenderer>().Length==11,"Source slot count");var player=instance.GetComponent<Animation>();
                foreach(var animation in (JObject)source["animations"])
                {
                    var clip=player[animation.Key].clip;Near(clip.length,animation.Key=="animation"?8:1.3333f,"Source duration");
                    foreach(float time in new[]{0f,.15f,.3f,.36665f,.4667f,.6f,.8f,1.3333f,1.5f,1.8333f,6.3333f})
                    {
                        float sample=Mathf.Min(time,clip.length);clip.SampleAnimation(instance,sample);
                        foreach(var bone in (JArray)source["bones"])
                        {
                            string name=(string)bone["name"];var t=instance.transform.Find(name=="root"?"root":"root/"+name);var keys=animation.Value["bones"]?[name];
                            Near(t.localPosition.x,Evaluate(keys?["translate"],sample,k=>(N(bone,"x")+N(k,"x"))*.01f,N(bone,"x")*.01f),"Translation x "+name);
                            Near(t.localPosition.y,Evaluate(keys?["translate"],sample,k=>(N(bone,"y")+N(k,"y"))*.01f,N(bone,"y")*.01f),"Translation y "+name);
                            Near(t.localScale.x,Evaluate(keys?["scale"],sample,k=>N(bone,"scaleX",1)*N(k,"x",1),N(bone,"scaleX",1)),"Setup-relative scale x "+name);
                            Near(t.localScale.y,Evaluate(keys?["scale"],sample,k=>N(bone,"scaleY",1)*N(k,"y",1),N(bone,"scaleY",1)),"Setup-relative scale y "+name);
                            float angle=Evaluate(keys?["rotate"],sample,k=>N(bone,"rotation")+N(k,"angle"),N(bone,"rotation"));Near(Mathf.DeltaAngle(t.localEulerAngles.z,angle),0,"Rotation "+name);
                        }
                        int index=0;
                        foreach(var slot in (JArray)source["slots"])
                        {
                            string bone=(string)slot["bone"],name=(string)slot["name"];var renderer=instance.transform.Find((bone=="root"?"root":"root/"+bone)+"/"+name+"/region").GetComponent<SpriteRenderer>();
                            Check(renderer.sortingOrder==index++,"Source slot ordering");
                            for(int component=0;component<4;component++)
                            {
                                int c=component;Func<JToken,float> color=k=>Convert.ToInt32(((string)k["color"]).Substring(c*2,2),16)/255f;
                                float setup=Convert.ToInt32(((string)slot["color"]).Substring(c*2,2),16)/255f;
                                Near(renderer.color[c],Evaluate(animation.Value["slots"]?[name]?["color"],sample,color,setup),"Slot color "+name);
                            }
                        }
                    }
                }
                ValidateRegions(source,instance);
            }
            finally{UnityEngine.Object.DestroyImmediate(instance);}
            Debug.Log("NUT_FIXED_SCREW_VALIDATION_PASS native idle/break durations, source translations/scales/angles/colors sampled before and between keys, slot order, rotated/unrotated region corner UV mapping and geometry; transition mixing has separate Play coverage.");
        }
        private static void ValidateRegions(JObject source,GameObject root)
        {
            foreach(var slot in (JArray)source["slots"])
            {
                JToken region=null;foreach(var candidate in (JArray)source["regions"])if((string)candidate["name"]==(string)slot["region"]){region=candidate;break;}
                bool rot=(bool)region["rotated"];string bone=(string)slot["bone"];
                var quad=root.transform.Find((bone=="root"?"root":"root/"+bone)+"/"+(string)slot["name"]+"/region");var sprite=quad.GetComponent<SpriteRenderer>().sprite;
                float u0=(float)region["xy"][0]/(float)source["atlasWidth"],u1=u0+(float)region["size"][rot?1:0]/(float)source["atlasWidth"];
                float v1=1-(float)region["xy"][1]/(float)source["atlasHeight"],v0=v1-(float)region["size"][rot?0:1]/(float)source["atlasHeight"];
                var vertices=sprite.vertices;var uv=sprite.uv;
                for(int i=0;i<vertices.Length;i++)
                {
                    var position=quad.localRotation*Vector3.Scale(vertices[i],quad.localScale);bool right=position.x>0,top=position.y>0;
                    float u=rot?(top?u0:u1):(right?u1:u0),v=rot?(right?v1:v0):(top?v1:v0);
                    Near(uv[i].x,u,"Region corner u");Near(uv[i].y,v,"Region corner v");
                    Near(Mathf.Abs(position.x),N(slot,"width")*.005f,"Attachment width");Near(Mathf.Abs(position.y),N(slot,"height")*.005f,"Attachment height");
                }
            }
        }
        private static void Near(float a,float b,string message){Check(Mathf.Abs(a-b)<.0001f,message+": "+a+" expected "+b);}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
