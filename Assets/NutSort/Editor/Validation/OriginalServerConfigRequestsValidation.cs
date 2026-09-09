using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalServerConfigRequestsValidation
    {
        public static void Validate()
        {
            var userDefaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");var defaults=userDefaults.ServerConfig;
            var data=OriginalServerConfigJson.Read("{}",defaults);
            Check(data.Count==18&&(int)data["LSSLR1"]==30&&(int)data["LSSLR2"]==4&&(int)data["LSSLRFP"]==3&&(int)data["LSSLRFV"]==5&&(int)data["LSSLD1"]==300&&(int)data["LSSLD2"]==30,"Native first six constructor integers");
            Check((int)data["LSSEDGC"]==2&&(int)data["LSSR1"]==7&&(int)data["LSSR2"]==4&&(bool)data["LSSNTY"]&&!(bool)data["LSS260820"]&&(int)data["LSSSHSLV"]==10&&(int)data["LSSLSMAC"]==12&&!(bool)data["LSSAB"]&&(string)data["ksCountry"]=="ID-BR","Native remaining scalar defaults");
            Check(JToken.DeepEquals(data["LSSLDR"],JArray.Parse("[80,0,0,0,7,3,7,1,7,3]"))&&JToken.DeepEquals(data["LSSUPT"],JArray.Parse("[90,120]"))&&JToken.DeepEquals(data["LSSGPUL"],JArray.Parse("[8,21,61,111]")),"Native metadata/ELF defaults");
            var other=OriginalServerConfigJson.Read("{}",defaults);((JArray)data["LSSGPUL"])[0]=99;Check((int)other["LSSGPUL"][0]==8&&defaults.LSSGPUL[0]==8,"Per-object default arrays independent of asset");
            data=OriginalServerConfigJson.Read("{\"lsslr1\":6,\"LSSLR1\":7,\"LSSGPUL\":null,\"ksCountry\":null,\"unknown\":5}",defaults);
            Check((int)data["LSSLR1"]==7&&data["LSSGPUL"].Type==JTokenType.Null&&data["ksCountry"].Type==JTokenType.Null&&data["unknown"]==null,"Case-insensitive field assignment, explicit null arrays/string, unknown fields ignored");
            Check(OriginalServerConfigJson.Read("null",defaults)==null&&OriginalServerConfigJson.Read("  ",defaults)==null,"Null and empty Json.NET document");
            Expect<JsonReaderException>(()=>OriginalServerConfigJson.Read("{} {}",defaults));Expect<JsonSerializationException>(()=>OriginalServerConfigJson.Read("{\"LSSLR1\":null,\"LSSLR1\":4}",defaults));
            var saved=OriginalUserDataJson.Read("{\"ServerConfigData\":{\"lsslr1\":9}}",userDefaults);Check((int)saved.ServerConfigData["LSSLR1"]==9&&(int)saved.ServerConfigData["LSSGPUL"][0]==8,"Actual saved-user config applies same constructor defaults");
            Expect<JsonSerializationException>(()=>OriginalUserDataJson.Read("{\"ServerConfigData\":{\"LSSLR1\":null,\"LSSLR1\":4}}",userDefaults));
            var ordered=OriginalUserDataJson.Read("{\"ServerConfigData\":{\"lsslr1\":6,\"LSSLR1\":8},\"Level\":9}",userDefaults);
            Check((int)ordered.ServerConfigData["LSSLR1"]==8&&ordered.Level==9,"Saved nested config retains duplicate order and resumes outer user fields");
            var prior=OriginalServerConfigRequests.Cached;
            try
            {
                OriginalServerConfigRequests.Cached=null;var pending=new List<Action<string,byte[]>>();int reads=0,calls=0,randoms=0;
                var user=new OriginalUserLocalData(userDefaults){LoginTime=0,TodayChallengeTimes=0};
                var init=new OriginalServerConfigInitialization(()=>{Check(OriginalServerConfigRequests.Cached!=null,"Cache published before Init");return user;},()=>new DateTime(1970,1,2),()=>86400,(a,b)=>{randoms++;return 5;});
                var requests=new OriginalServerConfigRequests(defaults,()=>{reads++;return new OriginalCountryInfo{Area="USA"};},init,(area,done)=>{Check(area=="USA","scrap_mark is current country Area");pending.Add(done);});
                requests.Config(value=>{Check(!value,"Empty transport maps to false");calls++;});pending[0](null,new byte[]{1});Check(calls==1&&OriginalServerConfigRequests.Cached==null&&user.ServerConfigData==null,"Empty receive ignores bytes/cache/user");
                requests.Config(value=>{Check(value&&ReferenceEquals(user.ServerConfigData,OriginalServerConfigRequests.Cached)&&user.LoginTime==86400,"Config Init before bool callback");calls++;});pending[1]("{\"LSSLR1\":11,\"kinetic_gap\":\"NO-1\"}",null);
                var first=OriginalServerConfigRequests.Cached;Check(calls==2&&randoms==1&&(int)first["LSSLR1"]==11&&first["kinetic_gap"]==null,"Config has no success-envelope gate");
                var second=new OriginalServerConfigRequests(null,()=>throw new Exception("Cached country read"),null,(area,done)=>throw new Exception("Cached transport"));second.Request(value=>Check(ReferenceEquals(value,first),"Shared synchronous cache"));Check(reads==2&&randoms==1,"Cache skips country lookup and Init");
                OriginalServerConfigRequests.Cached=null;requests.Request(value=>calls++);var late=pending[2];Expect<JsonSerializationException>(()=>late("[]",null));Check(OriginalServerConfigRequests.Cached==null&&ReferenceEquals(user.ServerConfigData,first),"Parse failure before publication/user update");
                late("{}",null);var valid=OriginalServerConfigRequests.Cached;Expect<JsonSerializationException>(()=>late("[]",null));Check(ReferenceEquals(valid,OriginalServerConfigRequests.Cached),"Late parse failure preserves current cache");
                Expect<NullReferenceException>(()=>late("null",null));Check(OriginalServerConfigRequests.Cached==null&&ReferenceEquals(user.ServerConfigData,valid),"Literal null replaces cache then fails before Init");
                var failing=new OriginalServerConfigRequests(defaults,()=>new OriginalCountryInfo{Area="USA"},new OriginalServerConfigInitialization(()=>user,()=>throw new InvalidOperationException()),(area,done)=>done("{\"LSSLR1\":15}",null));
                Expect<InvalidOperationException>(()=>failing.Request());Check(ReferenceEquals(user.ServerConfigData,OriginalServerConfigRequests.Cached)&&(int)user.ServerConfigData["LSSLR1"]==15,"Init failure retains published cache and first user assignment");
                OriginalServerConfigRequests.Cached=null;requests.Request(value=>throw new InvalidOperationException());Expect<InvalidOperationException>(()=>pending[pending.Count-1]("{}",null));Check(OriginalServerConfigRequests.Cached!=null,"Callback failure doesn't roll back data");
            }
            finally{OriginalServerConfigRequests.Cached=prior;}
            Debug.Log("NUT_SERVER_CONFIG_REQUESTS_VALIDATION_PASS all native constructor defaults, independent arrays, explicit Json.NET projection and saved-user config defaults, current Area request, shared cache-before-Init, null/parse/init/callback failure order and bool mapping; transport remains explicit.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected exception");}
    }
}
