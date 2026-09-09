using System;
using System.Collections.Generic;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalBootstrapValidation
    {
        private sealed class Actions:IOriginalBootstrapActions
        {
            public string Code;public bool UserDone,LevelInfo;public string Fail;
            public readonly List<string> Trace=new List<string>();public int CountryReads,UserReads,LevelReads;
            public string CountryCode{get{CountryReads++;return Code;}}
            public bool IsUserInitDone{get{UserReads++;return UserDone;}}
            public bool HasLevelInfo{get{LevelReads++;return LevelInfo;}}
            private void Call(string name){Trace.Add(name);if(Fail==name)throw new InvalidOperationException(name);}
            public void SetLoading(bool visible)=>Call(visible?"loadingOn":"loadingOff");
            public void InitializeUI()=>Call("ui");public void InitializeTables()=>Call("tables");
            public void InitializeSdkBoundary()=>Call("sdkBoundary");public void InitializePool()=>Call("pool");
            public void InitializeUser()=>Call("user");public void InitializeAudio()=>Call("audio");
            public void InitializeScene()=>Call("scene");public void ShowAndAssignMainPanel()=>Call("main");
            public void InitializeRequests()=>Call("requests");public void PlayBgm()=>Call("bgm");
        }
        public static void Validate()
        {
            var a=new Actions();var run=OriginalBootstrap.Run(a,.5f);Check(a.Trace.Count==0,"Coroutine is lazy");
            string[] first={"loadingOn","ui","tables","sdkBoundary"};foreach(var name in first){Check(run.MoveNext()&&run.Current==null&&a.Trace[a.Trace.Count-1]==name,"Source one-frame initialization boundary "+name);}
            Check(run.MoveNext()&&run.Current is WaitUntil&&a.Trace[4]=="pool"&&a.CountryReads==0,"Country predicate not eagerly evaluated");
            var wait=(WaitUntil)run.Current;Check(wait.keepWaiting,"Null country waits");a.Code="";Check(wait.keepWaiting,"Empty country waits");a.Code=" ";Check(!wait.keepWaiting,"Native whitespace is nonempty, not trimmed");
            Check(run.MoveNext()&&run.Current==null&&a.Trace[5]=="user","User after country and then one frame");
            Check(run.MoveNext()&&run.Current is WaitUntil&&a.Trace[6]=="audio"&&a.UserReads==0,"Audio before user-ready wait");wait=(WaitUntil)run.Current;Check(wait.keepWaiting,"User remains pending");a.UserDone=true;Check(!wait.keepWaiting,"Live user readiness");
            int count=a.Trace.Count;Check(run.MoveNext()&&run.Current==null&&a.Trace.Count==count,"Extra empty frame after user readiness");
            Check(run.MoveNext()&&run.Current is WaitUntil&&a.Trace[7]=="scene"&&a.LevelReads==0,"Scene before level-info wait");wait=(WaitUntil)run.Current;Check(wait.keepWaiting,"No LevelInfo waits");a.LevelInfo=true;Check(!wait.keepWaiting,"Actual predicate is LevelInfo presence");
            Check(run.MoveNext()&&run.Current is WaitForSeconds&&a.Trace[8]=="main","Main assignment before scaled delay");
            Check(run.MoveNext()&&run.Current==null&&a.Trace[9]=="loadingOff","Hide loading then another frame");
            Check(!run.MoveNext()&&string.Join(",",a.Trace)=="loadingOn,ui,tables,sdkBoundary,pool,user,audio,scene,main,loadingOff,requests,bgm","Full native ordered lifecycle");Check(!run.MoveNext(),"Completed coroutine has no repeat work");
            foreach(string failure in new[]{"ui","user","main","requests"})
            {
                a=new Actions{Code="US",UserDone=true,LevelInfo=true,Fail=failure};run=OriginalBootstrap.Run(a,.5f);bool threw=false;
                try{while(run.MoveNext()){} }catch(InvalidOperationException){threw=true;}
                Check(threw&&a.Trace[a.Trace.Count-1]==failure&&!run.MoveNext(),"Failure stops later stages and does not auto-clean loading: "+failure);
            }
            Debug.Log("NUT_BOOTSTRAP_VALIDATION_PASS decoded source coroutine frame boundaries, live country/user/LevelInfo predicates, audio-before-user-wait, main-scaled-delay/loading/request/BGM order and failures; explicit ports, no SDK or automatic scene bootstrap claim.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
