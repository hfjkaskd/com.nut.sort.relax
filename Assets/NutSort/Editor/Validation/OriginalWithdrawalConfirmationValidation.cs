using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalConfirmationValidation
    {
        private sealed class UI:IOriginalWithdrawalConfirmationUI
        {
            public bool Visible,FailClose;public string Tip,Icon;public int Panel;public object[] Args;
            public Action TipChanged;
            public readonly List<string> Trace=new List<string>();
            public void SetConfirmVisible(bool value){Visible=value;Trace.Add("visible");}
            public void SetTip(string value){Tip=value;Trace.Add("tip");TipChanged?.Invoke();}
            public void SetIcon(string path){Icon=path;Trace.Add("icon");}
            public void ShowPanel(int id,object[] args){Panel=id;Args=args;Trace.Add("show");}
            public void Close(){Trace.Add("close");if(FailClose)throw new InvalidOperationException("fixture close");}
        }
        public static void Validate()
        {
            bool old=OriginalWithdrawalConfirmationFlow.IsHintGoldGet;
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.Level=8;
                user.UserLssInfo=JObject.Parse(@"{""Name"":""Ada"",""Email"":""ada@example.test"",""Number"":""12345"",""GetType"":2,""OtherInfo"":""""}");
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var ui=new UI();int calls=0,areas=0;bool guideThrows=false;
                var flow=new OriginalWithdrawalConfirmationFlow(user,tables.PayChannels,tables.ChannelInfos,()=>"US",()=>{areas++;return "1";},ui,
                    (level,hasArgs)=>{Check(level==7&&hasArgs,"Empty array remains non-null in GoldGet flag");calls++;ui.Trace.Add("get");},
                    value=>{Check((int)value==7,"Captured level in close callback");ui.Trace.Add("guide");if(guideThrows)throw new InvalidOperationException("fixture guide");});
                OriginalWithdrawalConfirmationFlow.IsHintGoldGet=true;
                flow.Init(Array.Empty<object>(),()=>ui.Trace.Add("base"),()=>ui.Trace.Add("bindGet"),()=>ui.Trace.Add("bindReenter"),()=>ui.Trace.Add("bindClose"));
                Check(flow.Level==7&&!ui.Visible&&string.Join(",",ui.Trace)=="base,bindGet,visible,bindReenter,bindClose","Native level fallback and visibility binding order");
                flow.Refresh();Check(ui.Tip=="Ada\nada@example.test"&&ui.Icon=="Atlas/Pay/PayPal"&&areas==0,"Email named display, no area lookup");
                var captured=user.UserLssInfo;user.UserLssInfo=new JObject();flow.Refresh();Check(ui.Tip=="Ada\nada@example.test","Init captures account reference, replacement does not retarget refresh");
                captured["Name"]="";flow.Refresh();Check(ui.Tip=="ada@example.test","Nameless email omits newline");
                captured["GetType"]=0;flow.Refresh();Check(ui.Tip=="1-12345"&&areas==1,"Nameless phone omits plus");
                captured["Name"]="Ada";flow.Refresh();Check(ui.Tip=="Ada\n+1-12345"&&areas==2,"Named phone contains newline plus area separator");
                captured["OtherInfo"]=" ";captured["GetType"]=-50;flow.Refresh();Check(ui.Tip==" "&&ui.Icon=="Atlas/Pay/Other"&&areas==2,"Nonempty other info bypasses channel and country formatting");
                captured["OtherInfo"]="";captured["GetType"]=2;ui.TipChanged=()=>captured["GetType"]=0;
                flow.Refresh();Check(ui.Tip=="Ada\nada@example.test"&&ui.Icon=="Atlas/Pay/Venmo","Icon re-reads selected index after text assignment");ui.TipChanged=null;
                ui.Trace.Clear();flow.Get();Check(calls==1&&string.Join(",",ui.Trace)=="close,get"&&OriginalWithdrawalConfirmationFlow.IsHintGoldGet,"Confirm closes before application boundary, retains hint flag");
                ui.Trace.Clear();flow.Reenter();Check(ui.Panel==20&&ui.Args.Length==1&&(int)ui.Args[0]==7&&string.Join(",",ui.Trace)=="show,close","Re-enter creates captured-level argument and opens before closing");
                ui.Trace.Clear();guideThrows=true;Expect<InvalidOperationException>(flow.Close);Check(OriginalWithdrawalConfirmationFlow.IsHintGoldGet&&string.Join(",",ui.Trace)=="close,guide","Guide error prevents hint clearing");
                guideThrows=false;flow.Close();Check(!OriginalWithdrawalConfirmationFlow.IsHintGoldGet,"Successful close clears hint after callback");
                ui.FailClose=true;Expect<InvalidOperationException>(flow.Get);Check(calls==1,"Close failure suppresses GoldGet");ui.FailClose=false;
                user.UserLssInfo=captured;flow.Init(new object[]{1},()=>{},()=>{},()=>{},()=>{});Check(flow.Level==1&&ui.Visible,"Explicit level and normal confirm visibility");
                user.Level=int.MinValue;flow.Init(Array.Empty<object>(),()=>{},()=>{},()=>{},()=>{});Check(flow.Level==int.MaxValue,"Native unchecked fallback subtraction");
                Expect<NullReferenceException>(()=>flow.Init(null,()=>{},()=>{},()=>{},()=>{}));
            }
            finally{OriginalWithdrawalConfirmationFlow.IsHintGoldGet=old;}
            Debug.Log("NUT_WITHDRAWAL_CONFIRMATION_VALIDATION_PASS native account reference, email/phone/other display, lazy area lookup, text-before-icon index reread, binding/flag order, empty-array GoldGet flag, reenter and close failure ordering; payment boundary not implemented.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new InvalidOperationException("Expected "+typeof(T).Name);}
    }
}
