using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUserInitializationTailValidation
    {
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var current=new OriginalUserLocalData(defaults);var trace=new List<string>();Action<bool> held=null;Action<JObject> reward=null;
            Action countryHook=null,registerHook=null,configHook=null,initializeHook=null,rewardHook=null;bool initialized=false;JObject seen=null;
            var flow=new OriginalUserInitializationTail(()=>current,()=>{trace.Add("country");countryHook?.Invoke();},
                (cb,flag)=>{Check(!flag,"Original Config flag false");trace.Add("config");held=cb;configHook?.Invoke();},
                ()=>{trace.Add("register");registerHook?.Invoke();},data=>{seen=data;trace.Add("initConfig");initializeHook?.Invoke();},
                value=>{initialized=value;trace.Add("done");},(cb,flag)=>{Check(!flag,"Original reward request flag false");reward=cb;trace.Add("reward");rewardHook?.Invoke();},()=>trace.Add("save"));
            flow.Run();Check(string.Join(",",trace)=="country,register"&&!initialized&&held==null,"No config takes register, no automatic readiness/save");
            trace.Clear();current.ServerConfigData=new JObject();current.ComeOnGold=" ";flow.Run();Check(string.Join(",",trace)=="country,config,save"&&current.GuideIndex==10&&!initialized,"Nonempty whitespace trigger after held Config call");
            var replacement=new JObject{{"fixture",1}};current.ServerConfigData=replacement;trace.Clear();held(false);
            Check(ReferenceEquals(seen,replacement)&&initialized&&string.Join(",",trace)=="initConfig,done,reward,save","Boolean ignored, live config initialized before ready/request/save");
            trace.Clear();reward(null);reward(new JObject());Check(trace.Count==0,"Reward callback is source no-op");
            Action<JObject> first=reward;held(true);Check(ReferenceEquals(first,reward),"Cached reward callback reused");
            trace.Clear();initialized=false;current.ServerConfigData=null;Expect<NullReferenceException>(()=>held(true));Check(trace.Count==0&&!initialized,"Missing live config fails before marking ready");
            current.ServerConfigData=new JObject();initializeHook=()=>throw new InvalidOperationException();trace.Clear();Expect<InvalidOperationException>(()=>held(true));Check(string.Join(",",trace)=="initConfig"&&!initialized,"Config initialization failure prevents readiness");initializeHook=null;
            rewardHook=()=>throw new InvalidOperationException();trace.Clear();Expect<InvalidOperationException>(()=>held(true));Check(initialized&&string.Join(",",trace)=="initConfig,done,reward","Reward request failure retains ready flag but prevents save");rewardHook=null;
            current=new OriginalUserLocalData(defaults);registerHook=()=>current=new OriginalUserLocalData(defaults){ComeOnGold="value"};trace.Clear();flow.Run();Check(current.GuideIndex==10&&string.Join(",",trace)=="country,register,save","Post-register live user determines guide/save");registerHook=null;
            current.ServerConfigData=new JObject();configHook=()=>held(false);trace.Clear();flow.Run();Check(string.Join(",",trace)=="country,config,initConfig,done,reward,save,save","Synchronous callback saves before trailing guide save");configHook=null;
            countryHook=()=>throw new InvalidOperationException();trace.Clear();Expect<InvalidOperationException>(flow.Run);Check(string.Join(",",trace)=="country","Country failure prevents request selection");
            Debug.Log("NUT_USER_INITIALIZATION_TAIL_VALIDATION_PASS country-before-config/register, held and synchronous callbacks, ignored success flag, live config/user, readiness/request/save order, source empty reward callback and failure boundaries; no transport or production startup claim.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
