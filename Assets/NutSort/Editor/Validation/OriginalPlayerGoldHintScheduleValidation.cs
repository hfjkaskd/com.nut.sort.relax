using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalPlayerGoldHintScheduleValidation
    {
        public static void Validate()
        {
            long now=100;bool panel=false,missing=false;
            var trace=new List<string>();
            JObject config=JObject.Parse("{\"LSSUPT\":[6,10,999],\"keep\":true}");string before=config.ToString();
            var data=new OriginalMarqueeItem();OriginalPlayerGoldHintSchedule schedule=null;
            schedule=new OriginalPlayerGoldHintSchedule(()=>{trace.Add("config");return config;},()=>{trace.Add("panel");return panel;},()=>{trace.Add("data");return missing?null:data;},item=>{trace.Add("present");Check(ReferenceEquals(item,data) && schedule.LastShowTime==now+7,"Schedule updates before presentation");},()=>{trace.Add("clock");return now;},(a,b)=>{trace.Add("range");Check(a==6 && b==10,"Original first two interval values");return 7;});
            schedule.Update();Check(trace.Count==0 && schedule.LastShowTime==-1,"Disabled sentinel avoids all queries");
            schedule.Push();Check(schedule.LastShowTime==100 && string.Join(",",trace)=="clock","Push records current second only");
            trace.Clear();now=99;schedule.Update();Check(string.Join(",",trace)=="clock","No early trigger");
            now=100;panel=true;trace.Clear();schedule.Update();
            Check(schedule.LastShowTime==103 && string.Join(",",trace)=="clock,panel","Panel blocks data and adds three to deadline");
            now=1000;trace.Clear();schedule.Update();Check(schedule.LastShowTime==106,"Overdue retry advances old deadline, not now plus three");
            panel=false;missing=true;trace.Clear();schedule.Update();Check(schedule.LastShowTime==109 && string.Join(",",trace)=="clock,panel,data","Missing data defers without reading interval");
            missing=false;trace.Clear();schedule.Update();
            Check(string.Join(",",trace)=="clock,panel,data,config,clock,range,present" && schedule.LastShowTime==1007,"Successful show native order");
            Check(before==config.ToString(),"Configuration unchanged");
            schedule.Init();trace.Clear();schedule.Update();Check(schedule.LastShowTime==-1 && trace.Count==0,"Init disables scheduling");
            config=JObject.Parse("{\"LSSUPT\":[6]}");bool failed=false;trace.Clear();
            try { schedule.NextPush(); } catch(ArgumentOutOfRangeException) { failed=true; }
            Check(failed && string.Join(",",trace)=="config,clock" && schedule.LastShowTime==-1,"Short interval reads clock before failing and retains deadline");
            DateTime epoch=new DateTime(1970,1,1);
            Check(OriginalPlayerGoldHintSchedule.SecondsFromUtc(epoch.AddTicks(-9999999))==0 && OriginalPlayerGoldHintSchedule.SecondsFromUtc(epoch.AddTicks(-10000000))==-1,"Negative timestamps truncate toward zero");
            Check(OriginalPlayerGoldHintSchedule.SecondsFromUtc(epoch.AddMilliseconds(1999))==1,"Integer seconds discard subsecond remainder");
            Debug.Log("NUT_PLAYER_GOLD_HINT_SCHEDULE_VALIDATION_PASS UTC integer clock, disabled sentinel, overdue retries, lazy panel/data gates, interval order and unchanged configuration.");
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
