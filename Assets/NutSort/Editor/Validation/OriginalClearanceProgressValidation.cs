using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalClearanceProgressValidation
    {
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var user=new OriginalUserLocalData(defaults){Level=8,LevelSeed=7,Gold=123,Coin=456,IsRandomLevelSeed=true,LuckyScrewDoneCount=9};
            var calls=new List<string>();int reads=0;long clock=100;
            var flow=new OriginalClearanceProgress(()=>{reads++;return user;},()=>
            {
                Check(user.UserLevel==6&&user.Level==3&&user.LevelSeed==0,"Progress is applied before refresh");
                Check(user.LoginDay==4&&user.LoginDayCoin==5&&user.TodayPassLevelCount==2,"All server progress fields");
                Check(user.LuckyScrewDoneCount==9,"Lucky reset follows refresh");calls.Add("refresh");
            },()=>++clock,n=>calls.Add("reward:"+n),n=>calls.Add("draw:"+n));
            flow.Apply(false,null);Check(reads==0&&user.Level==8,"Failed response must not read or mutate user");
            var data=new OriginalClearanceProgressData{gap_rank=6,gap_all_gates=2,ext_gap_logs=4,gap_logs=5,gap_day_gates=2};
            flow.Apply(true,data);
            Check(string.Join(",",calls)=="refresh,reward:101,draw:102"&&user.LuckyScrewDoneCount==0,"Level three uses separate ordered clock reads");
            Check(user.Gold==123&&user.Coin==456&&user.IsRandomLevelSeed,"No fabricated grant or random-seed reset");
            var replacement=new OriginalUserLocalData(defaults){Level=4,LuckyScrewDoneCount=8};
            var reentrant=new OriginalClearanceProgress(()=>user,()=>user=replacement,
                ()=>throw new Exception("Clock must not run after refresh changes level"),n=>{},n=>{});
            reentrant.Apply(true,data);Check(user==replacement&&user.LuckyScrewDoneCount==8,"Post-refresh branch uses current user");
            var overflow=new OriginalClearanceProgress(()=>user,()=>{},()=>0,n=>{},n=>{});
            overflow.Apply(true,new OriginalClearanceProgressData{gap_all_gates=int.MaxValue});
            Check(user.Level==int.MinValue,"Native integer overflow, no invented clamp");
            bool threw=false;try{overflow.Apply(true,null);}catch(NullReferenceException){threw=true;}
            Check(threw,"Successful missing data is an error");
            Debug.Log("NUT_CLEARANCE_PROGRESS_VALIDATION_PASS server level plus one, seed reset, ordered progress refresh, reentrant user, separate level-three clocks, failure and overflow; no currency grant or synthetic response.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
