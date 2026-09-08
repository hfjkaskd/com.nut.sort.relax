using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalSuccessPanelFlowValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var trace=new List<string>();long time=123;int reads=0;
            var flow=new OriginalSuccessPanelFlow(user,()=>{reads++;trace.Add("time");return time;},()=>trace.Add("sound"),id=>{Check(id==7&&user.GuideIndex==0,"Guide index resets before panel 7");trace.Add("guide");});
            user.Level=1;flow.Init(()=>{trace.Add("init");user.Level=2;});
            Check(flow.IsGuide&&user.Level1TXTime==123&&user.Level2TXTime==0&&string.Join(",",trace)=="init,time,sound","Native init reads level after base, stores level-two timestamp before audio");
            user.Level=20;user.GuideIndex=88;trace.Clear();flow.TweenEndRefresh(()=>trace.Add("tween"));
            Check(string.Join(",",trace)=="tween,guide","Animation end uses init-captured guide flag after live level changes");
            user.GuideIndex=5;trace.Clear();flow.TweenEndRefresh(()=>trace.Add("tween"));Check(string.Join(",",trace)=="tween,guide","No invented one-shot guide guard");
            user.Level=3;time=456;flow.Init(()=>{});Check(user.Level2TXTime==456&&user.Level1TXTime==123&&reads==2,"Level three only changes its own timestamp");
            user.Level=4;flow.Init(()=>{});Check(!flow.IsGuide&&reads==2,"Later levels do not read clock");
            user.Level=2;user.GuideIndex=9;trace.Clear();flow.TweenEndRefresh(()=>trace.Add("tween"));Check(trace.Count==1&&user.GuideIndex==9,"Late level change cannot create a captured guide");
            foreach(int level in new[]{int.MinValue,-1,0,1,2,3,4,int.MaxValue})
            {
                user.Level=level;trace.Clear();
                flow.Refresh(()=>trace.Add("refresh"),()=>trace.Add("center"),id=>{Check(id==1,"Original continue text ID");trace.Add("text");},()=>trace.Add("ad"),()=>trace.Add("get"));
                bool early=level!=int.MinValue&&level<=3;
                Check(string.Join(",",trace)==(early?"refresh,center,text,ad,get":"refresh"),"Original signed subtract/compare and ordered UI edits");
                trace.Clear();flow.GetCallback(()=>trace.Add("close"),()=>trace.Add("baseGet"));
                Check(string.Join(",",trace)==(early?"close":"baseGet"),"Live level selects close versus original base callback");
            }
            user.Level=10;trace.Clear();flow.Refresh(()=>{trace.Add("refresh");user.Level=3;},()=>trace.Add("center"),id=>trace.Add("text"),()=>trace.Add("ad"),()=>trace.Add("get"));
            Check(trace.Count==5,"Refresh evaluates level after base reward generation");
            user.Level=2;flow.Init(()=>{});user.GuideIndex=33;bool failed=false;
            try{flow.TweenEndRefresh(()=>throw new InvalidOperationException("fixture"));}catch(InvalidOperationException){failed=true;}
            Check(failed&&user.GuideIndex==33,"Base tween failure preserves guide index");
            bool sound=false;var failing=new OriginalSuccessPanelFlow(user,()=>throw new InvalidOperationException("clock"),()=>sound=true,id=>{});
            long before=user.Level1TXTime;failed=false;
            try{failing.Init(()=>{});}catch(InvalidOperationException){failed=true;}
            Check(failed&&failing.IsGuide&&!sound&&user.Level1TXTime==before,"Guide flag is written before clock failure; audio and time assignment do not run");
            user.Level=2;flow.Init(()=>{});user.TXTargetGold=null;trace.Clear();
            flow.Hide(()=>trace.Add("hide"),()=>{trace.Add("mode");return true;},()=>throw new InvalidOperationException("Unused progress"),()=>trace.Add("sync"),()=>trace.Add("restart"));
            Check(string.Join(",",trace)=="hide,mode,restart","New mode restarts even during guide and bypasses legacy progression");
            foreach(string target in new string[]{null,"","0"," "})
            foreach(bool complete in new[]{false,true})
            foreach(bool guide in new[]{false,true})
            {
                user.Level=guide?2:4;flow.Init(()=>{});user.TXTargetGold=target;trace.Clear();
                flow.Hide(()=>trace.Add("hide"),()=>{trace.Add("mode");return false;},()=>{trace.Add("progress");return complete;},()=>trace.Add("sync"),()=>trace.Add("restart"));
                bool empty=string.IsNullOrEmpty(target);
                string expected="hide,mode"+(empty?",progress":"")+(empty&&complete?",sync":guide?"":",restart");
                Check(string.Join(",",trace)==expected,"Legacy short circuit, exact null/empty target and captured guide restart suppression");
            }
            user.Level=4;flow.Init(()=>{});trace.Clear();bool hideFailed=false;
            try{flow.Hide(()=>throw new InvalidOperationException("hide"),()=>{trace.Add("mode");return true;},()=>true,()=>{},()=>{});}catch(InvalidOperationException){hideFailed=true;}
            Check(hideFailed&&trace.Count==0,"Base hide failure prevents branch reads");
            var settings=Resources.Load<OriginalSceneSession>("Configuration/OriginalSceneSession");
            Check(settings.SuccessPanelSound=="Pass_level"&&Resources.Load<AudioClip>("Audio/"+settings.SuccessPanelSound)!=null,"Original panel audio asset loads separately from StageComplete");
            Debug.Log("NUT_SUCCESS_PANEL_FLOW_VALIDATION_PASS base-call ordering, level-two/three timestamps, captured guide flag, guide reset/panel 7, early-level text/ad/get presentation actions and live GetCallback dispatch and mode/target/guide-dependent Hide routing; complete panel prefab/reward response and production host pending.");
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    }
}
