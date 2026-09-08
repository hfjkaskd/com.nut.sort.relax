using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalSuccessResponseValidation
    {
        public static void Validate()
        {
            int reads=0,restarts=0,panels=0,settled=0;bool complete=false;Action pending=null;
            var payload=new JObject{{"fixture",1}};JObject observed=null;
            var flow=new OriginalSuccessResponseFlow(()=>{reads++;return complete;},()=>restarts++,
                (id,callback)=>{Check(id==43,"Native special panel ID");panels++;pending=callback;},response=>{observed=response;settled++;});
            flow.Run(new OriginalLevelInfo{Level=6,SubTotalRound=4,SubRound=3},null);
            Check(restarts==1&&reads==0&&panels==0&&settled==0,"Intermediate subround skips flag and payload reads");
            flow.Run(new OriginalLevelInfo{Level=5,SubTotalRound=4,SubRound=4},payload);
            Check(reads==0&&settled==1&&observed==payload,"Other level immediately forwards same payload");
            flow.Run(new OriginalLevelInfo{Level=6,SubTotalRound=4,SubRound=4},payload);
            Check(reads==1&&panels==1&&settled==1,"Level-six incomplete flag defers settlement");
            complete=true;payload["fixture"]=2;pending();pending();
            Check(reads==1&&settled==3&&observed==payload&&(int)observed["fixture"]==2,"Deferred callback retains reference, no flag reread or invented once-only guard");
            flow.Run(new OriginalLevelInfo{Level=6,SubTotalRound=1,SubRound=2},null);
            Check(reads==2&&settled==4&&observed==null,"Over-complete round reaches normal branch; true flag bypasses panel; null payload remains untouched");
            bool failed=false;try{flow.Run(null,payload);}catch(NullReferenceException){failed=true;}
            Check(failed&&reads==2&&settled==4,"Null captured level fails before flag/panel/settlement");
            Debug.Log("NUT_SUCCESS_RESPONSE_VALIDATION_PASS captured subround priority, lazy level-six flag, panel 43 continuation, direct other/completed paths, reference-preserved payload, null boundaries and repeated callback semantics; no grants.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
