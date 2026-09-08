using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalClaimValidation
    {
        private sealed class View:IOriginalWithdrawalClaimUI
        {
            public readonly List<string> Trace=new List<string>();public bool Online;public object[] Arguments;public bool FailShow;
            public bool OnlineTimeHint{get{Trace.Add("online");return Online;}set{Online=value;Trace.Add("online="+value);}}
            public bool Progress2Guide{set{Trace.Add("guide="+value);}}
            public string Tip=>"<color=#abcDEF>Need\nmore</color>";
            public string Text(int id){Check(id==6,"Native table text id");return "<color=12345678>Stage\ncomplete</color>";}
            public void ShowPanel(int id,object[] arguments){Trace.Add("panel:"+id);Arguments=arguments;if(FailShow)throw new InvalidOperationException("show");}
            public void ShowTip(int id){Trace.Add("tip:"+id);}
            public void ShowTip(string text){Trace.Add("tip:"+text);}
            public void Close(){Trace.Add("close");}
        }
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.GoldRewardTargetS2CData=JObject.Parse(@"{""cal_cfg"":3,""bear_list"":[{},{},{""RealLevel"":5,""Stage2RealLevel"":10,""caliper_logs"":4}]}");
            user.ServerConfigData=JObject.Parse(@"{""LSS260820"":true}");
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var view=new View();var flow=new OriginalWithdrawalClaimFlow(user,new OriginalRewardProgress(user,tables,v=>v.ToString()),view);
            user.Level=5;user.UserLssInfo=null;flow.Run(1,Array.Empty<object>(),false,false);Equal(view,"tip:13");
            view.Trace.Clear();flow.Run(1,Array.Empty<object>(),true,false);Equal(view,"panel:20,close");Check((int)view.Arguments[0]==1,"Captured panel level forwarded");
            user.ServerConfigData["LSS260820"]=false;view.Trace.Clear();flow.Run(2,null,false,true);Equal(view,"panel:20,close");
            user.UserLssInfo=new JObject();user.ServerConfigData=null;
            var arguments=new object[]{4,"retained"};
            for(int stage=1;stage<=7;stage++)
            {
                user.Level=20;user.TXTargetGold="10";user.Gold=10;user.IsGuideGold=false;user.LoginDay=0;user.TodayPassLevelCount=0;
                int level=stage==1?1:stage==2?4:stage==3?8:11;
                if(stage==4)user.Gold=0;
                if(stage==6)user.TodayPassLevelCount=3;
                if(stage==7)user.LoginDay=4;
                foreach(bool hint in new[]{false,true})
                {
                    view.Trace.Clear();view.Online=true;flow.Run(level,arguments,true,hint);Equal(view,"online,online=False,panel:30,close");Check(!view.Online&&view.Arguments.Length==0,"Online hint priority over stage and target");
                    view.Trace.Clear();view.Online=false;flow.Run(level,arguments,true,hint);
                    int id=stage<=2?28:stage==3?0:hint?31:stage==4?24:stage<=6?25:26;
                    string expected=stage==3?"online,tip:<color=#FFFFFF>Stage complete</color>":"online,"+(!hint&&(stage==5||stage==6)?"guide=True,":"")+"panel:"+id+",close";
                    Equal(view,expected);
                    if(stage<=2)Check(view.Arguments.Length==1&&(int)view.Arguments[0]==level,"Stages one/two use captured level only");
                }
            }
            view.Trace.Clear();user.Level=8;flow.Run(8,arguments,false,true);Equal(view,"online,panel:23,close");Check(ReferenceEquals(view.Arguments,arguments),"Incomplete stage three forwards original argument array identity before target hint");
            view.Trace.Clear();flow.Run(1,Array.Empty<object>(),false,false);Equal(view,"online,tip:<color=#FFFFFF>Need more</color>");
            user.Level=20;user.LoginDay=0;user.TodayPassLevelCount=0;view.Trace.Clear();flow.Run(11,Array.Empty<object>(),false,false);Equal(view,"online,guide=False,panel:25,close");
            user.TXTargetGold=null;view.Online=true;view.Trace.Clear();flow.Run(11,arguments,false,false);Equal(view,"close");Check(view.Online,"Stage zero skips online-hint consumption");
            user.TXTargetGold="10";view.FailShow=true;view.Trace.Clear();bool failed=false;
            try{flow.Run(11,arguments,false,false);}catch(InvalidOperationException){failed=true;}
            Check(failed&&!view.Online&&string.Join(",",view.Trace)=="online,online=False,panel:30","Dispatch failure preserves flag reset but prevents close");
            Check(OriginalWithdrawalClaimFlow.WhiteTip("<color = #12345678 >A\r\nB</color><COLOR=#abcdef>C</COLOR><color=red>D</color>")=="<color=#FFFFFF>A\r B</color><COLOR=#abcdef>C</COLOR><color=red>D</color>","Exact LF-only replacement, case-sensitive six/eight-digit color pattern, other tags preserved");
            Debug.Log("NUT_WITHDRAWAL_CLAIM_VALIDATION_PASS actual reward-stage evaluator across stages 0-7, first-user task gate, hint priority, argument identity, nonclosing tips, guide-state mutation and exception ordering; actual TXPanel view and SDK excluded.");
        }
        private static void Equal(View view,string expected){Check(string.Join(",",view.Trace)==expected,"Expected "+expected+" got "+string.Join(",",view.Trace));}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
