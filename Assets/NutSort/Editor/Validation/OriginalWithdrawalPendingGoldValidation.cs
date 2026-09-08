using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalPendingGoldValidation
    {
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var trace=new List<string>();bool enabled=false;int reads=0;Action<float> update=null;float from=0,to=0;
            var formatter=new OriginalGoldFormatter(()=>"en-US");OriginalWithdrawalPendingGold flow=null;
            flow=new OriginalWithdrawalPendingGold(user,()=>{reads++;return enabled;},
                (value,a,b)=>{Check(flow.IsShowTargetHint&&a&&b,"Hint set before native gold setter flags");trace.Add("set");user.Gold=value;},
                ()=>{Check(user.ComeOnGold==string.Empty,"Pending value cleared before persistence");trace.Add("save");user.Gold+=1;},
                (a,b,seconds,cb)=>{Check(seconds==1.5f,"Serialized original duration");from=a;to=b;update=cb;trace.Add("animate");},
                value=>formatter.Format(value),text=>trace.Add(text),1.5f);
            foreach(string pending in new string[]{null,""}){user.ComeOnGold=pending;flow.Refresh();}
            Check(reads==0&&!flow.IsShowTargetHint&&trace.Count==0,"Empty pending amount skips flag read and all effects");
            user.Gold=4;user.ComeOnGold="20";flow.Refresh();Check(reads==1&&user.ComeOnGold=="20"&&user.Gold==4&&trace.Count==0,"Disabled tween preserves pending value");
            enabled=true;flow.Refresh();Check(string.Join(",",trace)=="set,save,animate"&&from==4&&to==21,"Setter, clear, save, then animate to post-save live total");
            trace.Clear();float gold=user.Gold;update(12.5f);Check(string.Join(",",trace)==formatter.Format(12.5f)&&user.Gold==gold,"Animation callback only formats displayed value");
            trace.Clear();int oldReads=reads;flow.Refresh();Check(trace.Count==0&&reads==oldReads&&flow.IsShowTargetHint,"Consumed refresh does not repeat or reset existing hint");
            user.ComeOnGold="0";trace.Clear();flow.Refresh();Check(from==21&&to==1&&string.Join(",",trace)=="set,save,animate","Literal zero is processed rather than skipped");
            bool failed=false;user.ComeOnGold="8";
            var badSet=new OriginalWithdrawalPendingGold(user,()=>true,(v,a,b)=>throw new InvalidOperationException("set"),()=>throw new Exception("save"),(a,b,c,d)=>throw new Exception("animate"),v=>formatter.Format(v),s=>{},1.5f);
            try{badSet.Refresh();}catch(InvalidOperationException){failed=true;}
            Check(failed&&badSet.IsShowTargetHint&&user.ComeOnGold=="8","Setter failure keeps pending amount and marked hint");
            var badSave=new OriginalWithdrawalPendingGold(user,()=>true,(v,a,b)=>user.Gold=v,()=>throw new InvalidOperationException("save"),(a,b,c,d)=>throw new Exception("animate"),v=>formatter.Format(v),s=>{},1.5f);
            failed=false;try{badSave.Refresh();}catch(InvalidOperationException){failed=true;}
            Check(failed&&user.Gold==8&&user.ComeOnGold==string.Empty,"Save failure leaves applied/cleared state and does not animate");
            Debug.Log("NUT_WITHDRAWAL_PENDING_GOLD_VALIDATION_PASS pending/tween gates, hint-set order, actual formatter, set/clear/save/post-save animation target, display-only updates, zero and failure boundaries; production balance setter and tween consumer pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
