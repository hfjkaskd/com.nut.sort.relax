using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalScrewDoneValidation
    {
        private sealed class Effects:IOriginalScrewDoneEffects
        {
            public readonly List<string> Trace=new List<string>();
            public readonly Dictionary<string,int> Values=new Dictionary<string,int>{{"LSSLR2",10},{"LSSLR1",100},{"LSSLD2",20},{"LSSLD1",200},{"LSSR1",9}};
            public float Coin=2;public bool Panel;public long Now=10,Reward,Draw;public Action OnGold,OnCoinFlight;public int ClockReads;
            public float RemoveGold{get{Trace.Add("goldAmount");return 1.5f;}}
            public float RemoveCoin{get{Trace.Add("coinAmount");return Coin;}}
            public void FlyGold(ScrewState s)=>Trace.Add("flyGold");
            public void FlyCoin(ScrewState s){Trace.Add("flyCoin");OnCoinFlight?.Invoke();}
            public void AddGold(float n,bool refresh,bool sync){Check(n==1.5f&&refresh,"Gold arguments");Trace.Add(sync?"goldSync":"goldNoSync");OnGold?.Invoke();}
            public void AddCoin(float n,bool refresh){Check(n==Coin&&refresh,"Fresh coin amount");Trace.Add("addCoin");}
            public bool HasPanel{get{Trace.Add("panel");return Panel;}}
            public int Config(string key){Trace.Add(key);return Values[key];}
            public long TimeSeconds{get{Trace.Add("clock");ClockReads++;return Now++;}}
            public long LuckyRewardTime{get=>Reward;set{Reward=value;Trace.Add("rewardTime");}}
            public long LuckyDrawTime{get=>Draw;set{Draw=value;Trace.Add("drawTime");}}
            public void RequestLuckyReward()=>Trace.Add("requestLucky");
            public void ShowPanel(int id)=>Trace.Add("show"+id);
        }
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var user=new OriginalUserLocalData(defaults){Level=1};var e=new Effects();int shown=1;
            Func<OriginalScrewDoneFlow> make=()=>new OriginalScrewDoneFlow(()=>user,()=>shown,()=>e.Trace.Add("success"),e);
            make().Run(null,true);Equal(e.Trace,"success","Early victory does not touch reward/config/panel dependencies");
            e=new Effects();shown=4;e.OnCoinFlight=()=>e.Coin=7;make().Run(null,true);
            Equal(e.Trace,"flyGold,goldAmount,goldNoSync,coinAmount,flyCoin,coinAmount,addCoin,success","Reward order and two coin reads before victory");
            e=new Effects();shown=3;e.OnGold=()=>shown=4;make().Run(null,true);Check(e.Trace.Contains("flyCoin"),"Display level is re-read after gold side effects");
            e=new Effects{Coin=float.NaN};shown=4;make().Run(null,true);Check(!e.Trace.Contains("flyCoin"),"NaN does not satisfy positive coin comparison");
            e=new Effects();shown=2;user.Level=3;user.LuckyScrewDoneCount=10;user.LuckyDrawScrewDoneTimes=20;make().Run(null,false);
            Equal(e.Trace,"panel,LSSLR2,clock,rewardTime,requestLucky","Lucky reward precedes draw; count threshold skips elapsed clock");Check(user.LuckyScrewDoneCount==0&&user.LuckyDrawScrewDoneTimes==20,"Only selected counter resets");
            e=new Effects{Now=100};user.LuckyScrewDoneCount=0;make().Run(null,false);Check(e.ClockReads==2&&e.Reward==101&&e.Trace[e.Trace.Count-1]=="requestLucky","Inclusive time threshold and separate reset clock");
            e=new Effects();user.LuckyScrewDoneCount=0;user.LuckyDrawScrewDoneTimes=20;make().Run(null,false);Check(e.Trace[e.Trace.Count-1]=="show12"&&user.LuckyDrawScrewDoneTimes==0,"Draw branch after unmet reward branch");
            e=new Effects{Panel=true};user.IsCompleteAppRatingPanel=false;user.Level=9;user.LuckyScrewDoneCount=10;make().Run(null,false);Check(!e.Trace.Contains("requestLucky")&&!e.Trace.Contains("show12")&&!user.IsCompleteAppRatingPanel&&e.ClockReads==0,"Open modal suppresses optional completions and clock reads");
            e=new Effects();user.Level=2;e.Values["LSSR1"]=2;make().Run(null,false);Equal(e.Trace,"LSSR1,panel,show2","Rating threshold independent of level-three lucky gate");Check(user.IsCompleteAppRatingPanel,"Rating flag set before panel dispatch");
            e=new Effects();make().Run(null,false);Check(e.Trace.Count==0,"Completed rating is lazy and silent below lucky gate");
            e=new Effects{Now=long.MaxValue,Reward=-1,Draw=long.MaxValue};user.Level=3;user.LuckyScrewDoneCount=0;user.LuckyDrawScrewDoneTimes=0;make().Run(null,false);Check(!e.Trace.Contains("requestLucky"),"Native signed elapsed overflow does not trigger reward");
            Debug.Log("NUT_SCREW_DONE_VALIDATION_PASS captured victory handoff, live display-level queries, flight/account ordering and sync flags, coin re-read, optional branch priority, modal gates, inclusive counts/times, separate clocks, rating flag and elapsed overflow; no synthetic request responses.");
        }
        private static void Equal(List<string> trace,string expected,string message)=>Check(string.Join(",",trace)==expected,message+": "+string.Join(",",trace));
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
