using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalInitializationFlowValidation
    {
        private sealed class Probe : IOriginalInitializationActions
        {
            public OriginalUserLocalData User { get; set; }
            public int DisplayLevel = 6;
            public int ShowLevel { get { Trace.Add("show-level"); return DisplayLevel; } }
            private bool done;
            public bool IsInitDone { get => done; set { done=value; Trace.Add("done"); } }
            public bool Deadlocked, StageGuide, Unlock;
            public OriginalGameplayUnlock ActualUnlock;
            public Action BannerCallback;
            public readonly List<string> Trace = new List<string>();
            public bool IsCannotMove() { Trace.Add("deadlock"); return Deadlocked; }
            public bool IsGuidePassStage2Level() { Trace.Add("stage-guide"); return StageGuide; }
            public bool NewGameplayUnlock(bool showBanner) { Trace.Add(showBanner ? "unlock:banner" : "unlock:no-banner"); return ActualUnlock==null ? Unlock : ActualUnlock.Run(showBanner); }
            public void Fail() { Trace.Add("fail"); }
            public void ShowPanel(OriginalInitializationPanel panel) { Trace.Add("panel:"+(int)panel+":"+User.GuideIndex); }
            public void CloseAllPanels() { Trace.Add("close-all"); }
            public void ShowTargetRewardBanner(Action completed) { Trace.Add("banner"); BannerCallback=completed; }
            public void HideLevelHint() { Trace.Add("hide-hint"); }
            public void ShowEveryDayGift() { Trace.Add("daily-gift"); }
            public void PushPlayerGoldHint() { Trace.Add("gold-hint"); }
            public void CloseRecordGuide() { Trace.Add(User.IsCompleteRecordGuide ? "close-record:complete" : "close-record:incomplete"); }
        }

        public static void Validate()
        {
            var defaults=ScriptableObject.CreateInstance<OriginalUserDefaults>();
            try
            {
                var p=New(defaults); p.User.IsCompleteRecordGuide=false; p.Deadlocked=true;
                new OriginalInitializationFlow(p).Run(false,true);
                Expect(p,"done,panel:34:77");

                p=New(defaults); p.User.Level=2; p.Deadlocked=true;
                new OriginalInitializationFlow(p).Run();
                Expect(p,"show-level,panel:7:1,done");

                p=New(defaults); p.User.Level=3; p.Deadlocked=true;
                var flow=new OriginalInitializationFlow(p); flow.Run();
                Expect(p,"show-level,panel:7:1,done");
                p.User.IsGoldReduceLevel2=true; flow.Run();
                Expect(p,"show-level,panel:37:1,done");
                p.User.IsCompleteExtraGoldGuide=true; flow.Run();
                Expect(p,"show-level,deadlock,fail,done");

                p=New(defaults); p.StageGuide=true; p.User.ComeOnGold="10"; p.User.IsGuideGold=true;
                flow=new OriginalInitializationFlow(p); flow.Run();
                Expect(p,"show-level,deadlock,stage-guide,panel:7:10,done");
                p.StageGuide=false; p.User.ComeOnGold=" "; flow.Run();
                Expect(p,"show-level,deadlock,stage-guide,panel:7:12,done");
                p.User.ComeOnGold=""; flow.Run();
                Expect(p,"show-level,deadlock,stage-guide,panel:7:14,done");
                p.User.IsGuideGold=false; p.DisplayLevel=4; flow.Run();
                Expect(p,"show-level,deadlock,stage-guide,close-all,panel:42:14,done");
                flow.Run(false);
                Expect(p,"deadlock,stage-guide,unlock:no-banner,done");

                p=New(defaults);
                p.ActualUnlock=new OriginalGameplayUnlock(p.User,()=>new[]{p.User.Level+4},
                    (id,index,banner)=>p.Trace.Add("unlock-panel:"+id+":"+index+":"+(banner?1:0)));
                flow=new OriginalInitializationFlow(p);flow.Run();
                Expect(p,"show-level,deadlock,stage-guide,unlock:banner,unlock-panel:13:0:1,done");
                flow.Run(false);
                Expect(p,"deadlock,stage-guide,unlock:no-banner,unlock-panel:13:0:0,done");

                p=New(defaults); p.Unlock=true; flow=new OriginalInitializationFlow(p); flow.Run();
                Expect(p,"show-level,deadlock,stage-guide,unlock:banner,done");
                p=New(defaults); flow=new OriginalInitializationFlow(p); flow.Run(true,true);
                Expect(p,"show-level,deadlock,stage-guide,unlock:banner,banner");
                Require(!p.IsInitDone,"Banner must retain initialization gate until callback.");
                // Change state while the banner is open: callback must use live state.
                p.User.Level=1; p.BannerCallback();
                Expect(p,"done,panel:7:77,gold-hint");
                Require(p.User.GuideIndex==77,"First-level callback must not overwrite GuideIndex.");

                p=New(defaults); flow=new OriginalInitializationFlow(p); flow.Run(true,false); p.Trace.Clear();
                p.BannerCallback(); Expect(p,"done,show-level,panel:43:77");
                p=New(defaults); p.DisplayLevel=4; flow=new OriginalInitializationFlow(p);
                // Start at display 6, then cross below the callback's threshold.
                p.DisplayLevel=6; flow.Run(true,true); p.Trace.Clear(); p.DisplayLevel=4;
                p.BannerCallback(); Expect(p,"done,show-level,hide-hint,daily-gift,gold-hint");
                p=New(defaults); p.User.IsCompleteCoinNewPeopleReward=true;
                flow=new OriginalInitializationFlow(p); flow.Run(); p.Trace.Clear();
                p.BannerCallback(); Expect(p,"done,hide-hint,daily-gift");

                p=New(defaults); p.User.Level=2; p.User.IsCompleteRecordGuide=false;
                flow=new OriginalInitializationFlow(p); flow.CompleteRecordGuide();
                Expect(p,"close-record:incomplete,show-level,panel:7:1,done");
                Require(p.User.IsCompleteRecordGuide,"Record completion must update the shared user state before re-entry.");
                Debug.Log("NUT_INITIALIZATION_FLOW_VALIDATION_PASS original branch priority, lazy checks, guide writes, banner callback and record re-entry.");
            }
            finally { UnityEngine.Object.DestroyImmediate(defaults); }
        }

        private static Probe New(OriginalUserDefaults defaults)
        {
            return new Probe { User=new OriginalUserLocalData(defaults) { Level=7, GuideIndex=77, IsCompleteRecordGuide=true } };
        }
        private static void Expect(Probe p,string expected)
        {
            string actual=string.Join(",",p.Trace);
            Require(actual==expected,"Initialization trace: expected "+expected+"; actual "+actual);
            p.Trace.Clear();
        }
        private static void Require(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
