using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalSuccessValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var trace=new List<string>();float added=0;bool failRefresh=false,failSync=false,failCount=false,failFormat=false;
            var setter=new OriginalSetGoldFlow(user,s=>{Check(s.StartsWith("targetGold < UserLocalData.Gold:",StringComparison.Ordinal),"Source error prefix");trace.Add("error");},
                (tip,delta)=>{Check(tip,"Source HUD flag");added=delta;trace.Add("refresh");if(failRefresh)throw new InvalidOperationException("fixture refresh");},
                ()=>{trace.Add("sync");if(failSync)throw new InvalidOperationException("fixture sync");},()=>trace.Add("save"));
            user.Gold=5;setter.Set(3);Check(user.Gold==5&&string.Join(",",trace)=="error","Lower target logs and returns without save/sync/HUD");
            trace.Clear();setter.Set(8);Check(user.Gold==8&&added==3&&string.Join(",",trace)=="refresh,sync,save","Assignment before positive-delta refresh, sync and save");
            trace.Clear();setter.Set(8);Check(string.Join(",",trace)=="sync,save","Equal target still syncs and saves, no HUD refresh");
            trace.Clear();setter.Set(9,false,false);Check(user.Gold==9&&string.Join(",",trace)=="save","Flags skip HUD/sync but not save");
            trace.Clear();setter.Set(10,false,true);Check(string.Join(",",trace)=="sync,save","Sync independent from refresh flag");
            trace.Clear();setter.Set(11,true,false);Check(string.Join(",",trace)=="refresh,save","Refresh independent from sync flag");
            trace.Clear();failRefresh=true;Expect(()=>setter.Set(12));Check(user.Gold==12&&string.Join(",",trace)=="refresh","HUD failure retains assignment and prevents sync/save");failRefresh=false;
            trace.Clear();failSync=true;Expect(()=>setter.Set(13));Check(user.Gold==13&&string.Join(",",trace)=="refresh,sync","Sync failure prevents save");failSync=false;
            trace.Clear();setter.Set(float.NaN);Check(float.IsNaN(user.Gold)&&string.Join(",",trace)=="sync,save","Native comparisons do not add a nonfinite rejection");
            string shown=null;float formatted=0;
            var success=new OriginalWithdrawalSuccessFlow(user,v=>{formatted=v;trace.Add("format");if(failFormat)throw new InvalidOperationException("fixture format");return v.ToString(System.Globalization.CultureInfo.InvariantCulture);},
                text=>{shown=text;trace.Add("count");if(failCount)throw new InvalidOperationException("fixture count");},setter.Set,()=>trace.Add("close"));
            trace.Clear();success.Init(()=>trace.Add("base"),()=>trace.Add("bind"));Check(string.Join(",",trace)=="base,bind","Base before single Sure binding; no required level args");
            user.Gold=5;trace.Clear();success.Refresh();Check(formatted==5&&shown=="5"&&user.Gold==5&&string.Join(",",trace)=="format,count,error","Success displays pre-call current balance and native setter refuses zero debit");
            user.Gold=0;trace.Clear();success.Refresh();Check(shown=="0"&&string.Join(",",trace)=="format,count,sync,save","Zero balance still reaches setter sync/save");
            user.Gold=-2;trace.Clear();success.Refresh();Check(shown=="-2"&&user.Gold==0&&added==2&&string.Join(",",trace)=="format,count,refresh,sync,save","Negative value displays first then setter raises to zero");
            user.Gold=7;trace.Clear();failFormat=true;Expect(success.Refresh);Check(string.Join(",",trace)=="format"&&user.Gold==7,"Formatter failure stops assignment");failFormat=false;
            trace.Clear();failCount=true;Expect(success.Refresh);Check(string.Join(",",trace)=="format,count"&&user.Gold==7,"Text setter failure stops SetGold");failCount=false;
            trace.Clear();success.Sure();Check(string.Join(",",trace)=="close","Sure only closes, does not save or call guide");
            Action<JObject> held=null;
            var goldGet=new OriginalGoldGetFlow(cb=>held=cb,()=>throw new InvalidOperationException(),v=>throw new InvalidOperationException(),id=>{Check(id==2,"Failure tip");trace.Add("tip");},
                (id,args)=>{Check(id==29&&args.Length==0,"Original success routing");trace.Add("show29");success.Refresh();},
                (tip,delta)=>{Check(tip&&delta==0,"Pre-panel refresh flags");trace.Add("preRefresh");});
            trace.Clear();goldGet.Run(3,false);Check(held!=null&&trace.Count==0&&user.Gold==7,"Transport held without automatic response");
            held(JObject.Parse(@"{""kinetic_gap"":""failure""}"));Check(string.Join(",",trace)=="tip","Failure does not enter success controller");
            trace.Clear();held(JObject.Parse(@"{""kinetic_gap"":""NO-0""}"));Check(shown=="7"&&user.Gold==7&&string.Join(",",trace)=="preRefresh,show29,format,count,error","Explicit synthetic response composes original callback and controller without fabricating debit");
            Debug.Log("NUT_WITHDRAWAL_SUCCESS_VALIDATION_PASS native success display-before-SetGold, target setter refusal/equality/increase/flags/failure order, Sure close, explicit held GoldGet response composition; visual prefab and production host unavailable, no SDK implementation.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect(Action action){try{action();}catch(InvalidOperationException){return;}throw new Exception("Expected fixture failure");}
    }
}
