using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalProgressFlowValidation
    {
        private sealed class UI:IOriginalWithdrawalStageUI
        {
            public readonly List<string> Trace=new List<string>();public readonly Dictionary<int,string> Times=new Dictionary<int,string>();
            public bool HasGold{get;set;}=true;public int StepCount{get;set;}=4;public bool MissingTwo,FailGold;public Action<int> OnTime;
            public void SetGold(string value){Trace.Add("gold:"+value);if(FailGold)throw new InvalidOperationException("fixture gold");}
            public bool HasStepTime(int number){Trace.Add("find:"+number);return !(MissingTwo&&number==2);}
            public void SetStepTime(int number,string value){Times[number]=value;Trace.Add("time:"+number);OnTime?.Invoke(number);}
            public void Close()=>Trace.Add("close");
        }
        private sealed class Stage:OriginalWithdrawalProgressFlow
        {
            public int SureCalls,StepCalls;
            public Stage(OriginalUserLocalData u,UI ui):base(u,ui,()=>1,v=>"",(t,f)=>""){}
            public override void Sure(){SureCalls++;}
            public override void RefreshSteps(){StepCalls++;}
        }
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.UserLssInfo=null;user.Gold=5;
            var ui=new UI();int clocks=0;bool failSecond=false;Action onFirst=null;long clockBase=100;
            var flow=new OriginalWithdrawalProgressFlow(user,ui,()=>{clocks++;ui.Trace.Add("clock");if(clocks==1)onFirst?.Invoke();if(failSecond&&clocks==2)throw new InvalidOperationException("fixture time");return unchecked(clockBase+clocks);},
                v=>v.ToString(System.Globalization.CultureInfo.InvariantCulture),(t,f)=>{Check(f=="g","Source general short format");return t.ToString(System.Globalization.CultureInfo.InvariantCulture);});
            Action sure=null,close=null;flow.Init(()=>ui.Trace.Add("base"),a=>{sure=a;ui.Trace.Add("bindSure");},a=>{close=a;ui.Trace.Add("bindClose");});
            Check(string.Join(",",ui.Trace)=="base,bindSure,bindClose","Base initialization and listener order");
            ui.Trace.Clear();flow.Refresh();Check(clocks==2&&(long)user.UserLssInfo["ApplicationTime"]==101&&(long)user.UserLssInfo["AccountSureTime"]==105,"Independent clock reads, second plus three");
            Check(ui.Times[1]=="101"&&ui.Times[2]=="105"&&ui.Times[3]=="0"&&!ui.Times.ContainsKey(4)&&ui.Trace[2]=="gold:5","Gold before original three time fields; later steps untouched");
            var account=user.UserLssInfo;user.Gold=6;ui.Trace.Clear();flow.Refresh();Check(clocks==2&&ReferenceEquals(account,user.UserLssInfo)&&ui.Trace[0]=="gold:6","Live gold and retained initialized timestamps");
            account["ApplicationTime"]=-1;account["AccountSureTime"]=999;clocks=0;flow.Refresh();Check((long)account["ApplicationTime"]==101&&(long)account["AccountSureTime"]==105,"Nonpositive application replaces both times regardless of existing confirmation");
            ui.HasGold=false;ui.MissingTwo=true;ui.Trace.Clear();ui.Times.Clear();flow.Refresh();Check(!ui.Trace.Exists(x=>x.StartsWith("gold:",StringComparison.Ordinal))&&!ui.Times.ContainsKey(2)&&ui.Times[3]=="0","Optional missing gold/content/time nodes skip their updates");ui.HasGold=true;ui.MissingTwo=false;
            account["ApplicationTime"]=0;clocks=0;failSecond=true;ui.Trace.Clear();Expect(flow.Refresh);Check((long)account["ApplicationTime"]==101&&(long)account["AccountSureTime"]==105&&ui.Trace.Count==2,"Second clock failure keeps first mutation and prevents labels");failSecond=false;
            user.UserLssInfo=new JObject();var first=user.UserLssInfo;clocks=0;onFirst=()=>user.UserLssInfo=new JObject();flow.Refresh();Check((long)first["ApplicationTime"]==101&&user.UserLssInfo["ApplicationTime"]==null&&(long)user.UserLssInfo["AccountSureTime"]==105,"Each assignment captures account before its own clock read");onFirst=null;
            clocks=0;clockBase=long.MaxValue-2;user.UserLssInfo=new JObject();flow.Refresh();Check((long)user.UserLssInfo["AccountSureTime"]==unchecked(long.MaxValue+3L),"Native unchecked confirmation-time addition");
            user.UserLssInfo=JObject.Parse(@"{""ApplicationTime"":10,""APPLICATIONTIME"":20,""AccountSureTime"":30}");ui.OnTime=n=>{if(n==1)user.UserLssInfo["AccountSureTime"]=40;};flow.Refresh();Check(ui.Times[1]=="20"&&ui.Times[2]=="40","Current JSON projection and live time reads between label writes");ui.OnTime=null;
            ui.FailGold=true;ui.Trace.Clear();Expect(flow.Refresh);Check(ui.Trace.Count==1,"Failed gold assignment prevents step refresh");ui.FailGold=false;
            ui.Trace.Clear();sure();close();Check(string.Join(",",ui.Trace)=="close,close","Default Sure and close both close only");
            var stage=new Stage(user,ui);stage.Init(()=>{},a=>sure=a,a=>close=a);sure();stage.Refresh();Check(stage.SureCalls==1&&stage.StepCalls==1,"Bound Sure and RefreshSteps retain virtual dispatch");
            Debug.Log("NUT_WITHDRAWAL_PROGRESS_FLOW_VALIDATION_PASS native shared initialization, two independent timestamp reads, nonpositive application gate, optional gold/time labels, live fields, failure order and virtual stage dispatch; concrete stage views and production composition remain pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect(Action action){try{action();}catch(InvalidOperationException){return;}throw new Exception("Expected fixture failure");}
    }
}
