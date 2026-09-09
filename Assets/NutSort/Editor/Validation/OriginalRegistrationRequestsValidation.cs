using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalRegistrationRequestsValidation
    {
        public const string Success="{\"message_status\":\"fixture\",\"kinetic_gap\":\"NO-0\",\"Time\":9007199254740993,\"kinetic_data\":{\"scrap_mark\":\"IGNORED\",\"copp_uuid\":\"fixture-server-user\"}}";
        public static void Validate()
        {
            var parsed=OriginalRegistrationResponse.Read(Success);Check(parsed.IsSuccess&&parsed.Time==9007199254740993L&&parsed.kinetic_data.copp_uuid=="fixture-server-user","Exact envelope and 64-bit time");
            parsed=OriginalRegistrationResponse.Read("{\"KINETIC_GAP\":\"NO-0\",\"kinetic_data\":{\"copp_uuid\":\"first\"},\"KINETIC_DATA\":{\"scrap_mark\":\"USA\"}}");
            Check(parsed.IsSuccess&&parsed.kinetic_data.copp_uuid=="first"&&parsed.kinetic_data.scrap_mark=="USA","Case-insensitive projection and nested object reuse");
            parsed=OriginalRegistrationResponse.Read("{\"kinetic_data\":{\"copp_uuid\":\"first\"},\"kinetic_data\":null,\"kinetic_data\":{\"scrap_mark\":\"USA\"}}");Check(parsed.kinetic_data.copp_uuid==null,"Explicit null clears existing nested object");
            Check(OriginalRegistrationResponse.Read("null")==null&&OriginalRegistrationResponse.Read(" ")==null,"Json.NET null documents");
            Expect<JsonSerializationException>(()=>OriginalRegistrationResponse.Read("{\"Time\":null,\"Time\":4}"));Expect<JsonReaderException>(()=>OriginalRegistrationResponse.Read("{} {}"));
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");var first=new OriginalUserLocalData(defaults){UserId="device"};var current=first;
            var country=new OriginalCountryInfo{Area="USA",Code="US"};var pending=new List<Action<string,byte[]>>();int calls=0,reads=0;
            var requests=new OriginalRegistrationRequests(()=>{reads++;return current;},()=>country,(area,done)=>{Check(area==country.Area,"Live Area request");pending.Add(done);});
            requests.Register(value=>{Check(!value,"Empty response false");calls++;});pending[0](null,new byte[]{1});Check(calls==1&&reads==0&&first.UserId=="device","Empty ignores byte data and does not access user");
            requests.Register(value=>{Check(!value,"Failed status false");calls++;});pending[1](Success.Replace("NO-0","NO-1"),null);Check(calls==2&&reads==0,"Failure doesn't read or mutate user");
            requests.Register(value=>{Check(value&&current.UserId=="fixture-server-user","ID changed before completion");calls++;});
            current=new OriginalUserLocalData(defaults){UserId="replacement"};pending[2](Success,null);Check(calls==3&&reads==1&&first.UserId=="device"&&country.Area=="USA","Delayed response uses live user; response Area ignored");
            requests.Register(null);pending[3](Success.Replace("fixture-server-user",""),null);Check(current.UserId=="","Empty server ID accepted with null callback");
            requests.Register(null);pending[4]("{\"kinetic_gap\":\"NO-0\",\"kinetic_data\":{}}",null);Check(current.UserId==null,"Missing server ID overwrites with null");
            current.UserId="held";requests.Register(value=>calls++);Expect<NullReferenceException>(()=>pending[5]("{\"kinetic_gap\":\"NO-0\",\"kinetic_data\":null}",null));Check(current.UserId=="held"&&calls==3,"Missing success data throws before callback and ID update");
            Expect<NullReferenceException>(()=>pending[5]("null",null));Check(calls==3,"Literal null parsed response throws before manager callback");
            Expect<JsonSerializationException>(()=>pending[5]("{",null));Check(current.UserId=="held"&&calls==3,"Parse error leaves user and callback untouched");
            requests.Register(value=>throw new InvalidOperationException());Expect<InvalidOperationException>(()=>pending[6](Success,null));Check(current.UserId=="fixture-server-user","Callback exception preserves assigned ID");
            int count=pending.Count;requests.Register(null);requests.Register(null);Check(pending.Count==count+2,"No response cache or in-flight coalescing");
            OriginalRegistrationResponse raw=null;requests.Request(value=>raw=value);pending[pending.Count-1](Success.Replace("NO-0","NO-1"),null);Check(raw!=null&&!raw.IsSuccess,"Low-level callback receives failed envelope after Init");
            Debug.Log("NUT_REGISTRATION_REQUESTS_VALIDATION_PASS explicit Json.NET envelope/data fields, 64-bit time and nested reuse, current Area/no cache, empty/failure/success order, live user ID replacement before callback including empty/null ID, exception propagation and untouched country; SDK probes remain excluded.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected exception");}
    }
}
