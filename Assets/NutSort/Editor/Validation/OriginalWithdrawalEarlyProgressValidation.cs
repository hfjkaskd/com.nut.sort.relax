using System;
using System.Collections.Generic;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalEarlyProgressValidation
    {
        private sealed class View:IOriginalWithdrawalProgressUI
        {
            public readonly List<string> Trace=new List<string>();public float Raw,Value,X;public string Progress;
            public float Fill{get=>Value;set{Raw=value;Value=Mathf.Clamp01(value);Trace.Add("fill");}}
            public void SetTip(int id,params object[] args){Trace.Add("tip:"+id);}
            public void SetProgress(string text){Progress=text;Trace.Add("progress");}
            public void SetProgressTip(int id,params object[] args){Trace.Add("progressTip:"+id);if(id==25)Check(args.Length==1&&args[0] is string,"Remaining count is a formatted string argument");}
            public void SetMarkerX(float x){X=x;Trace.Add("marker");}
        }
        public static void Validate()
        {
            var settings=Resources.Load<OriginalPanelSettings>("Configuration/OriginalPanels");
            Check(settings.WithdrawalEarlyTotal==5&&settings.WithdrawalProgressTravel==800&&settings.WithdrawalProgressOffset==-400&&settings.WithdrawalCompleteThreshold==.999f,"Original serialized constants");
            var view=new View();int reads=0,shown=6;
            var flow=new OriginalWithdrawalEarlyProgress(()=>{reads++;return shown;},view,settings);
            flow.Refresh(1,2,Array.Empty<object>());Check(view.Progress=="1/2"&&view.Value==.5f&&view.X==0&&!flow.IsDoneTask&&reads==0,"Stage one ordinary entry");
            view.Trace.Clear();flow.Refresh(1,2,new object[]{null});Check(view.Progress=="2/2"&&view.X==400&&flow.IsDoneTask,"Stage one argument contents ignored");
            Check(string.Join(",",view.Trace)=="tip:22,fill,progress,progressTip:25,marker,progressTip:23,tip:159","Completion text follows numeric progress and marker");
            flow.Refresh(2,99,Array.Empty<object>());Check(view.Progress=="4/5"&&view.Value==.8f&&reads==2&&flow.IsDoneTask,"Ordinary stage two caps at four; does not clear completed flag");
            flow.Refresh(2,99,new object[]{0});Check(view.Progress=="5/5"&&reads==3,"Settlement stage two caps at five with one live show-level read");
            shown=0;flow.Refresh(2,99,Array.Empty<object>());Check(view.Progress=="-1/5"&&view.Raw==-.2f&&view.Value==0&&view.X==-400,"No lower clamp on numeric count; Image clamps fill");
            reads=0;var live=new OriginalWithdrawalEarlyProgress(()=>++reads==1?99:3,view,settings);live.Refresh(2,9,Array.Empty<object>());Check(view.Progress=="2/5"&&reads==2,"Ordinary path re-reads current show level");
            bool failed=false;view.Trace.Clear();try{flow.Refresh(1,1,null);}catch(NullReferenceException){failed=true;}
            Check(failed&&string.Join(",",view.Trace)=="tip:22","Null input failure occurs after initial tip before progress");
            Debug.Log("NUT_WITHDRAWAL_EARLY_PROGRESS_VALIDATION_PASS original stage-one count, stage-two four/five cap, repeated live read, unclamped labels/Image fill, marker, completion threshold and retained done state; remaining stages/full view pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
