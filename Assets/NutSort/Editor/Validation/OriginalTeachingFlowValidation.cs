using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalTeachingFlowValidation
    {
        private sealed class View : IOriginalTeachingView
        {
            public readonly List<string> Trace=new List<string>();
            public int Count=4;
            public Action<int> Changed;
            public int MarkerCount=>Count;
            public void SetCanOperateScrew(bool v){Trace.Add(v?"operate":"blocked");}
            public void SetTeachingVisible(bool v){Trace.Add(v?"teach":"hide-teach");}
            public void SetHandVisible(bool v){Trace.Add(v?"hand":"hide-hand");}
            public void SetHollowMaskVisible(bool v){Trace.Add(v?"mask":"hide-mask");}
            public void SetMarker(int i,bool v){Trace.Add(i+":"+(v?1:0));Changed?.Invoke(i);}
            public void SetTip(int id,int pos){Trace.Add("tip:"+id+":"+pos);}
            public void Close(){Trace.Add("close");}
        }
        public static void Validate()
        {
            var defaults=ScriptableObject.CreateInstance<OriginalUserDefaults>();
            try
            {
                var user=new OriginalUserLocalData(defaults){Level=2};var v=new View();
                var flow=new OriginalTeachingFlow(user,v,()=>throw new InvalidOperationException("Unexpected config read"));
                Check(!flow.Run() && string.Join(",",v.Trace)=="blocked,hide-teach","Later levels leave hand, mask, markers, tip and config untouched");
                user.Level=1;user.LevelSeed=0;v.Trace.Clear();
                flow=new OriginalTeachingFlow(user,v,()=>{v.Trace.Add("config");return false;});
                Check(flow.Run(),"First-level teaching handled");
                Check(string.Join(",",v.Trace)=="operate,teach,hide-hand,hide-mask,0:1,1:0,2:0,3:0,tip:70:0,config","Initial marker inclusive boundary and native action order");
                user.LevelSeed=2;v.Trace.Clear();flow.Run();Check(v.Trace.Contains("2:1") && v.Trace.Contains("3:0") && v.Trace.Contains("tip:72:0"),"Seed two uses third tutorial message");
                user.LevelSeed=3;v.Trace.Clear();
                flow=new OriginalTeachingFlow(user,v,()=>{v.Trace.Add("config");return true;});flow.Run();
                Check(v.Trace.Contains("3:1") && string.Join(",",v.Trace).EndsWith("tip:151:0,config,close"),"Higher seed uses special message and closes only after rendering");
                user.Level=-1;user.LevelSeed=-2;v.Count=0;v.Trace.Clear();flow.Run();
                Check(v.Trace.Contains("tip:68:0"),"No marker or negative seed clamp and no positive-level guard");
                user.Level=1;user.LevelSeed=0;v.Count=4;v.Trace.Clear();
                v.Changed=i=>{if(i==0){user.LevelSeed=2;v.Count=2;}};flow.Run();
                Check(v.Trace.Contains("1:1") && !v.Trace.Contains("2:1") && v.Trace.Contains("tip:72:0"),"Loop rereads marker count and user seed after view callbacks");
                v.Changed=null;v.Trace.Clear();
                flow=new OriginalTeachingFlow(user,v,()=>throw new InvalidOperationException("Missing config fixture"));
                bool failed=false;try{flow.Run();}catch(InvalidOperationException){failed=true;}
                Check(failed && v.Trace.Contains("tip:72:0") && !v.Trace.Contains("close"),"Config failure propagates after prior display writes");
            }
            finally{UnityEngine.Object.DestroyImmediate(defaults);}
            Debug.Log("NUT_TEACHING_FLOW_VALIDATION_PASS native level/seed boundaries, operation and visual order, live marker reads, special tip and late skip decision; full guide prefab binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
