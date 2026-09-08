using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGoldGetValidation
    {
        public static void Validate()
        {
            int requests=0,reads=0,panel=0,tip=0;string raw=null;object[] args=null;
            Action<JObject> held=null,first=null;bool failRefresh=false;
            var trace=new List<string>();
            var flow=new OriginalGoldGetFlow(cb=>{requests++;first=held;held=cb;trace.Add("request");},
                ()=>{reads++;return "First\r\n<color=#AABBCC>value</color> <color=red>red</color>";},
                value=>{raw=value;trace.Add("raw");},id=>{tip=id;trace.Add("tip");},
                (id,value)=>{panel=id;args=value;trace.Add("panel");},
                (show,add)=>{Check(show&&add==0,"Native refresh flags, not a balance write");trace.Add("refresh");if(failRefresh)throw new InvalidOperationException("fixture refresh");});
            foreach(int level in new[]{int.MinValue,-1,0,1,2})
            {
                flow.Run(level,true);Check(panel==28&&args.Length==1&&(int)args[0]==level&&requests==0&&reads==0,"Every <=2 confirmation goes directly to level panel");
            }
            var previous=args;flow.Run(2,true);Check(!ReferenceEquals(previous,args),"Early dispatch uses a new boxed argument array");
            trace.Clear();flow.Run(2,false);Check(raw=="First\r <color=#FFFFFF>value</color> <color=red>red</color>"&&reads==1&&string.Join(",",trace)=="raw","False entry reads live HUD hint and applies original LF/color conversion");
            trace.Clear();flow.Run(3,false);Check(requests==1&&held!=null&&reads==1&&string.Join(",",trace)=="request","Higher level requests regardless of entry flag without early UI");
            flow.Run(int.MaxValue,true);Check(requests==2&&ReferenceEquals(first,held),"Request callback reused for repeated high-level calls");
            foreach(var value in new JObject[]{null,new JObject(),JObject.Parse(@"{""kinetic_gap"":null}"),JObject.Parse(@"{""kinetic_gap"":""no-0""}"),JObject.Parse(@"{""kinetic_gap"":""NO-0 ""}"),JObject.Parse(@"{""message_status"":""NO-0""}")})
            {
                trace.Clear();held(value);Check(tip==2&&string.Join(",",trace)=="tip","Null/missing/wrong status reports localized failure only");
            }
            trace.Clear();held(JObject.Parse(@"{""kinetic_gap"":""NO-0""}"));Check(panel==29&&args.Length==0&&string.Join(",",trace)=="refresh,panel","Successful callback refreshes HUD before showing success panel");
            var empty=args;held(JObject.Parse(@"{""KINETIC_GAP"":""NO-0""}"));Check(ReferenceEquals(empty,args),"Typed field binding is case insensitive and empty args are shared");
            trace.Clear();failRefresh=true;Expect<InvalidOperationException>(()=>held(JObject.Parse(@"{""kinetic_gap"":""NO-0""}")));Check(string.Join(",",trace)=="refresh","HUD failure prevents later panel display");
            trace.Clear();failRefresh=false;held(null);Check(string.Join(",",trace)=="tip","Repeated response remains actionable, no once-only guard");
            var nullHint=new OriginalGoldGetFlow(cb=>{},()=>null,value=>{},id=>{},(id,value)=>{},(show,add)=>{});
            Expect<NullReferenceException>(()=>nullHint.Run(1,false));
            Debug.Log("NUT_GOLD_GET_VALIDATION_PASS native <=2 direct level-panel/HUD-tip branches, higher-level held request and reused callback, exact NO-0 success status, ordered display refresh, failure handling and exception/repeated-response behavior; no transport/payment implementation.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new InvalidOperationException("Expected "+typeof(T).Name);}
    }
}
