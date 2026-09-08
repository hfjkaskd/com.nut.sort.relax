using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalLevelValidation
    {
        private sealed class UI:IOriginalWithdrawalLevelUI
        {
            public readonly List<string> Trace=new List<string>();public int TipLevel;public string Gold;public float Hint;
            public bool Open=true,FailRefresh;
            public void SetTip(int id,int level){Check(id==89,"Original tip id");TipLevel=level;Trace.Add("tip");}
            public void SetGold(string value){Gold=value;Trace.Add("gold");}
            public void PlaySteps()=>Trace.Add("steps");
            public void Close()=>Trace.Add("close");
            public bool HasPanel(int id){Check(id==5,"Original auxiliary panel id");Trace.Add("has");return Open;}
            public void HidePanel(int id){Check(id==5,"Hide same panel");Open=false;Trace.Add("hide");}
            public void RefreshGoldItem(){Trace.Add("item");if(FailRefresh)throw new InvalidOperationException("fixture refresh");}
            public void ShowSelf(float amount){Hint=amount;Trace.Add("self");}
            public void RefreshGold(bool showTip,float addGold){Check(!showTip&&addGold==0,"Source display refresh flags");Trace.Add("refresh");}
        }
        public static void Validate()
        {
            bool old=OriginalWithdrawalLevelFlow.IsRecordIn;
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.Gold=12;user.Level1Gold=5;user.Level2Gold=10;
                var ui=new UI();Action<JObject> held=null,previous=null;int saves=0,requests=0;bool failSave=false,failGuide=false;object payload=null;
                var flow=new OriginalWithdrawalLevelFlow(user,ui,v=>v.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    (level,cb)=>{Check(level==flowLevel,"Captured request level");previous=held;held=cb;requests++;ui.Trace.Add("request");},
                    ()=>{saves++;ui.Trace.Add("save");if(failSave)throw new InvalidOperationException("fixture save");},
                    value=>{payload=value;ui.Trace.Add("guide");if(failGuide)throw new InvalidOperationException("fixture guide");return null;},
                    (seconds,cb)=>{Check(seconds==1&&cb==null,"Immediate guide invoke supplies its null return to delayed callback");ui.Trace.Add("delay");});
                flowLevel=1;OriginalWithdrawalLevelFlow.IsRecordIn=false;
                flow.Init(new object[]{1},()=>ui.Trace.Add("base"),()=>ui.Trace.Add("bindClose"),()=>ui.Trace.Add("bindGet"));
                Check(string.Join(",",ui.Trace)=="base,bindClose,bindGet"&&!flow.RecordEntry,"Native initialization order");
                ui.Trace.Clear();flow.Refresh();Check(ui.TipLevel==1&&ui.Gold=="5"&&string.Join(",",ui.Trace)=="tip,gold,steps","Original amount and refresh sequence");
                ui.Trace.Clear();flow.Get();Check(requests==1&&user.Gold==12&&saves==0&&string.Join(",",ui.Trace)=="close,request,has,hide","Request remains held; close before request and panel check after");
                ui.Trace.Clear();held(null);Check(user.IsTXLevel1&&user.IsGoldReduceLevel1&&user.Gold==7&&saves==1&&(int)payload==1&&ui.Hint==5&&string.Join(",",ui.Trace)=="item,save,self,refresh,guide,delay","Native callback ignores null response and orders actual local mutations");
                ui.Trace.Clear();held(JObject.Parse(@"{""kinetic_gap"":""failure""}"));Check(user.Gold==7&&saves==2&&string.Join(",",ui.Trace)=="save,self,refresh,guide,delay","Repeated callback does not deduct again but still saves/displays/calls guide");
                flow.Get();Check(!ReferenceEquals(previous,held),"Fresh instance callback per Get");
                OriginalWithdrawalLevelFlow.IsRecordIn=true;flowLevel=2;flow.Init(new object[]{2},()=>{},()=>{},()=>{});
                Check(flow.RecordEntry&&!OriginalWithdrawalLevelFlow.IsRecordIn,"Consume shared record-entry flag once");flow.Get();ui.Trace.Clear();held(null);
                Check(user.IsTXLevel2&&!user.IsGoldReduceLevel2&&user.Gold==7&&ui.Hint==10&&string.Join(",",ui.Trace)=="save,self,refresh","Record entry updates status but skips deduction and guide");
                flow.Init(new object[]{2},()=>{},()=>{},()=>{});flow.Get();ui.Trace.Clear();held(null);Check(user.Gold==0&&user.IsGoldReduceLevel2&&ui.Trace[0]=="item","Second level subtracts own amount and clamps zero");
                flowLevel=9;flow.Init(new object[]{9},()=>{},()=>{},()=>{});flow.Refresh();Check(ui.TipLevel==3&&ui.Gold=="10","Tip has upper clamp and non-one uses level-two amount");
                flowLevel=-2;flow.Init(new object[]{-2},()=>{},()=>{},()=>{});flow.Refresh();Check(ui.TipLevel==-2,"No lower clamp");
                ui.Trace.Clear();failGuide=true;Expect<InvalidOperationException>(flow.Close);Check(payload==null&&string.Join(",",ui.Trace)=="guide","Close invokes null guide before initiating close, and preserves errors");failGuide=false;
                flowLevel=1;flow.Init(new object[]{1},()=>{},()=>{},()=>{});flow.Get();ui.Trace.Clear();failSave=true;Expect<InvalidOperationException>(()=>held(null));Check(string.Join(",",ui.Trace)=="save","Save failure prevents subsequent display/callback");failSave=false;
                user.IsGoldReduceLevel1=false;user.Gold=12;ui.FailRefresh=true;ui.Trace.Clear();Expect<InvalidOperationException>(()=>held(null));Check(user.Gold==7&&user.IsGoldReduceLevel1&&string.Join(",",ui.Trace)=="item","Failed item refresh retains prior mutation but prevents save");
                OriginalWithdrawalLevelFlow.IsRecordIn=true;Expect<IndexOutOfRangeException>(()=>flow.Init(Array.Empty<object>(),()=>{},()=>{},()=>{}));Check(OriginalWithdrawalLevelFlow.IsRecordIn,"Invalid init does not consume shared entry flag");
            }
            finally{OriginalWithdrawalLevelFlow.IsRecordIn=old;}
            Debug.Log("NUT_WITHDRAWAL_LEVEL_VALIDATION_PASS required captured level, one-shot record entry, amount/tip refresh ordering, close/request/hide ordering, held response, native payload-independent state mutation and save/display/guide branches; view/steps and transport remain pending.");
        }
        private static int flowLevel;
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new InvalidOperationException("Expected "+typeof(T).Name);}
    }
}
