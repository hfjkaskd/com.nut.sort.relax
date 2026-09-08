using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalSuccessFlowValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){Level=1,LevelSeed=2};
            bool succeeded=false,mode=false;int reads=0;var trace=new List<string>();Action pending=null;
            var captured=new OriginalLevelInfo{SubTotalRound=4,SubRound=4};OriginalLevelInfo info=captured;
            var flow=new OriginalSuccessFlow(user,()=>succeeded,()=>{succeeded=true;trace.Add("flag");},()=>{Check(succeeded,"Flag precedes config");reads++;return mode;},
                ()=>{Check(user.LevelSeed==3,"Seed write before restart");trace.Add("restart");},()=>trace.Add("teach"),()=>{trace.Add("table");return info;},
                ()=>trace.Add("gold"),()=>trace.Add("progress"),()=>trace.Add("sound"),()=>trace.Add("effect"),
                (seconds,callback)=>{Check(seconds==.6f,"Source request delay");trace.Add("delay");pending=callback;},.6f,
                level=>{Check(level==captured,"Original table object captured across delay");trace.Add("request");});
            flow.Run();Check(string.Join(",",trace)=="flag,restart,teach","Legacy level-one seed<=2 bypasses normal settlement");
            trace.Clear();flow.Run();Check(trace.Count==0&&reads==1,"Success guard avoids all repeated work");
            succeeded=false;mode=true;flow.Run();Check(string.Join(",",trace)=="flag,table,gold,progress,sound,effect,delay","Normal complete-subround ordering");
            info=new OriginalLevelInfo();trace.Clear();pending();Check(string.Join(",",trace)=="request","Delayed request uses captured level info");
            succeeded=false;info=new OriginalLevelInfo{SubTotalRound=4,SubRound=2};trace.Clear();flow.Run();Check(string.Join(",",trace)=="flag,table,delay","Intermediate subround skips celebration only");
            succeeded=false;info=null;trace.Clear();flow.Run();Check(string.Join(",",trace)=="flag,table,delay","Missing table still schedules request");
            var session=Resources.Load<OriginalSceneSession>("Configuration/OriginalSceneSession");var effects=Resources.Load<OriginalEffectSettings>("Configuration/OriginalEffects");
            Check(session.SuccessRequestDelay==.6f&&session.StageCompleteSound=="StageComplete"&&effects.SuccessLifetime==5,"Native configured constants");
            var prefab=Resources.Load<GameObject>(effects.SuccessPath);var systems=prefab.GetComponentsInChildren<ParticleSystem>(true);
            Check(systems.Length==1&&systems[0].main.playOnAwake,"Original single autoplay particle system");
            Check(systems[0].GetComponent<ParticleSystemRenderer>().sharedMaterial!=null&&Resources.Load<AudioClip>("Audio/StageComplete")!=null,"Actual material and audio resolve");
            Debug.Log("NUT_SUCCESS_FLOW_VALIDATION_PASS success guard/write ordering, legacy first-level seed branch, captured table, equal/intermediate/missing subround paths, configured delay and original effect/audio resources; reward response remains external.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
