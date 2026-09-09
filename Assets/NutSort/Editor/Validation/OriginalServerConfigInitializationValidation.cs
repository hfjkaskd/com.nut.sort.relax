using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalServerConfigInitializationValidation
    {
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var day=new DateTime(2026,9,8,23,0,0,DateTimeKind.Utc);long stamp=OriginalPlayerGoldHintSchedule.SecondsFromUtc(day);
            var current=new OriginalUserLocalData(defaults){LoginTime=stamp,TodayChallengeTimes=2,TodayPassLevelCount=9,LoginDay=4};
            var trace=new List<string>();Action nowHook=null,secondsHook=null,randomHook=null;int randomValue=5;
            var init=new OriginalServerConfigInitialization(()=>current,()=>{trace.Add("date");nowHook?.Invoke();return day;},()=>{trace.Add("seconds");secondsHook?.Invoke();return stamp;},
                (a,b)=>{trace.Add(a+":"+b);randomHook?.Invoke();return a==5?randomValue:a;});
            var data=new JObject{{"fixture",1}};init.Initialize(data);
            Check(ReferenceEquals(current.ServerConfigData,data)&&string.Join(",",trace)=="date"&&current.TodayChallengeTimes==2,"Same UTC day attaches config and retains positive challenge count");
            current.TodayChallengeTimes=0;trace.Clear();init.Initialize(data);Check(current.TodayChallengeTimes==5&&string.Join(",",trace)=="date,5:11","Same day nonpositive count replenishes only challenges");
            current.LoginTime=stamp-86400;current.UserLssInfo=new JObject();trace.Clear();init.Initialize(data);
            Check(current.LoginTime==stamp&&current.TodayChallengeTimes==5&&(int)current.UserLssInfo["QueueCount"]==8000&&(int)current.UserLssInfo["PassRate"]==90&&string.Join(",",trace)=="date,seconds,8000:9000,90:96,5:11","Cross-day login, original queue, challenge order");
            Check(current.TodayPassLevelCount==9&&current.LoginDay==4,"No invented daily counter reset");
            current.LoginTime=stamp-86400;current.UserLssInfo=null;trace.Clear();init.Initialize(data);Check(string.Join(",",trace)=="date,seconds,5:11","Null account skips queue without creating account");
            current.LoginTime=stamp-86400;randomValue=0;trace.Clear();init.Initialize(data);Check(string.Join(",",trace)=="date,seconds,5:11,5:11","Second live nonpositive guard retained even after day reset");randomValue=5;
            var original=current;original.LoginTime=stamp-86400;secondsHook=()=>current=new OriginalUserLocalData(defaults){TodayChallengeTimes=7};trace.Clear();init.Initialize(data);
            Check(original.LoginTime==stamp&&current.LoginTime==0&&current.TodayChallengeTimes==5,"Captured login target, later current user refreshed");secondsHook=null;
            current.LoginTime=stamp;current.TodayChallengeTimes=0;original=current;randomHook=()=>current=new OriginalUserLocalData(defaults){TodayChallengeTimes=9};init.Initialize(data);Check(original.TodayChallengeTimes==5&&current.TodayChallengeTimes==9,"Challenge assignment retains target across random callback");randomHook=null;
            current.LoginTime=long.MaxValue;trace.Clear();Expect<ArgumentOutOfRangeException>(()=>init.Initialize(data));Check(ReferenceEquals(current.ServerConfigData,data)&&trace.Count==0,"Config assignment precedes timestamp failure and clock read");
            current.LoginTime=stamp-86400;nowHook=()=>throw new InvalidOperationException();trace.Clear();Expect<InvalidOperationException>(()=>init.Initialize(data));Check(string.Join(",",trace)=="date","Date clock failure prevents mutations");nowHook=null;
            secondsHook=()=>throw new InvalidOperationException();trace.Clear();Expect<InvalidOperationException>(()=>init.Initialize(data));Check(current.LoginTime==stamp-86400&&string.Join(",",trace)=="date,seconds","Clock failure prevents login assignment");secondsHook=null;
            // Compose the actual initializer with the restored held Config callback.
            current=new OriginalUserLocalData(defaults){ServerConfigData=data,LoginTime=stamp-86400};Action<bool> held=null;bool ready=false;int saves=0;
            var tail=new OriginalUserInitializationTail(()=>current,()=>{},(cb,flag)=>held=cb,()=>throw new Exception(),init.Initialize,
                value=>{Check(current.LoginTime==stamp&&current.TodayChallengeTimes==5,"Ready only after actual config initialization");ready=value;},(cb,flag)=>{Check(ready,"Reward after ready");},()=>saves++);
            tail.Run();Check(!ready&&saves==0,"Request remains held");held(false);Check(ready&&saves==1&&ReferenceEquals(current.ServerConfigData,data),"Actual config init composed without synthetic network success");
            Debug.Log("NUT_SERVER_CONFIG_INITIALIZATION_VALIDATION_PASS config attachment, UTC day comparison, native queue/random ordering, challenge guards, unchanged other counters, live/captured users and failure boundaries; actual held initialization-tail composition, SDK excluded.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
    }
}
