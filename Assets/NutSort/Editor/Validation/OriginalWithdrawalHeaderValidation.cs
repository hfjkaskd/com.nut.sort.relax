using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalHeaderValidation
    {
        private sealed class View:IOriginalWithdrawalHeaderUI
        {
            public readonly List<string> Trace=new List<string>();public Action OnStage,OnGold;
            public void SetLevelTitle(int slot,int id,int value){Check(id==30,"Original stage text ID");Trace.Add("stage"+slot+":"+value);OnStage?.Invoke();}
            public void SetGold(string value){Trace.Add("gold:"+value);OnGold?.Invoke();}
            public void SetTitle(int id,int value){Check(id==30,"Original main title ID");Trace.Add("title:"+value);}
        }
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.Level1Gold=5;user.Level2Gold=10;user.Gold=20;
            var view=new View();int shown=50;var formatter=new OriginalGoldFormatter(()=>"en-US");
            var flow=new OriginalWithdrawalHeader(user,()=>shown,v=>formatter.Format(v),view);
            flow.Refresh(1,Array.Empty<object>());Equal(view,"gold:$5,title:50");
            view.Trace.Clear();flow.Refresh(2,new object[]{1});Equal(view,"stage2:1,gold:$10,title:49");
            foreach(int level in new[]{int.MinValue,-1,0,3,int.MaxValue})
            {
                view.Trace.Clear();flow.Refresh(level,new object[]{null,"ignored"});Equal(view,"stage1:1,stage2:2,gold:$20,title:49");
            }
            view.OnStage=()=>user.Level2Gold=17;view.OnGold=()=>shown=99;view.Trace.Clear();
            flow.Refresh(2,Array.Empty<object>());Equal(view,"stage2:1,gold:$17,title:99");
            view.OnStage=null;view.OnGold=null;shown=int.MinValue;view.Trace.Clear();flow.Refresh(1,new object[]{0});Equal(view,"gold:$5,title:"+int.MaxValue);
            bool failed=false;view.Trace.Clear();try{flow.Refresh(1,null);}catch(NullReferenceException){failed=true;}
            Check(failed&&string.Join(",",view.Trace)=="gold:$5","Argument failure occurs after gold display, before live title read");
            view.OnStage=()=>throw new InvalidOperationException("label");view.Trace.Clear();failed=false;
            try{flow.Refresh(2,Array.Empty<object>());}catch(InvalidOperationException){failed=true;}
            Check(failed&&string.Join(",",view.Trace)=="stage2:1","Stage label failure prevents later gold/title updates");
            Debug.Log("NUT_WITHDRAWAL_HEADER_VALIDATION_PASS original stage label variants, saved/live gold selection, formatter, argument-dependent live show level, unchecked subtraction and ordered side effects; remaining refresh and prefab presentation pending.");
        }
        private static void Equal(View view,string expected){Check(string.Join(",",view.Trace)==expected,"Expected "+expected+" got "+string.Join(",",view.Trace));}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
