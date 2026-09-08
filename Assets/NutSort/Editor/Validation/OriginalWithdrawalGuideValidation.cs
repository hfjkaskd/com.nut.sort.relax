using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalGuideValidation
    {
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"))
                    {GuideIndex=2,Gold=15,Level1Gold=10,Level2Gold=20};
                var view=root.GetComponent<OriginalNewbieGuideView>();var trace=new List<string>();bool gate=false;
                view.InitializeInteractions(user,null,()=>gate,()=>trace.Add("audio"));
                view.BindTeaching(new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables")),"en",v=>{},
                    ()=>{Check(user.GuideIndex==2,"Close precedes increment");trace.Add("close");});
                int refresh=0,init=0;
                view.ShowWithdrawalGuide(user,view.FullButton.GetComponent<RectTransform>(),
                    ()=>{Check(user.GuideIndex==3,"Save follows increment");trace.Add("save");},
                    id=>{Check(id==36,"Original panel ID");trace.Add("show");},()=>refresh++,
                    (reset,first,unused)=>{Check(reset && first && !unused,"Original InitLevel arguments");init++;});
                view.ContinueButton.onClick.Invoke();Check(trace.Count==0,"Gate rejection");
                gate=true;view.ContinueButton.onClick.Invoke();Check(string.Join(",",trace)=="close,save,show,audio","Click side-effect ordering");
                UnityEngine.Object.DestroyImmediate(root);
                OriginalNewbieGuideView.CallbackActionInvoke(1);
                Check(user.GuideIndex==3 && user.IsGoldReduceLevel1 && user.Gold==5 && refresh==1 && init==1,"Captured guide index survives live increment and panel destruction");
                Check(OriginalNewbieGuideView.CallbackAction==null,"Global dispatch clears after callback");
                var flow=new OriginalWithdrawalGuideCompletion(user,2,()=>refresh++,(a,b,c)=>init++);
                flow.Complete(1);Check(user.Gold==5 && refresh==1 && init==2,"Repeated stage does not deduct or refresh twice, but still initializes");
                flow.Complete(2);Check(user.IsGoldReduceLevel2 && user.Gold==0 && refresh==2 && init==3,"Second stage clamps below zero");
                flow.Complete(99);Check(refresh==2 && init==4,"Other integer still initializes");
                user.IsGoldReduceLevel1=false;user.Gold=17;
                new OriginalWithdrawalGuideCompletion(user,3,()=>throw new Exception("unexpected"),(a,b,c)=>init++).Complete(1);
                Check(!user.IsGoldReduceLevel1 && user.Gold==17 && init==5,"Other captured guide does not deduct");
                foreach(object invalid in new object[]{null,"1",1L})
                {
                    bool failed=false;try{flow.Complete(invalid);}catch(NullReferenceException){failed=true;}catch(InvalidCastException){failed=true;}
                    Check(failed && user.Gold==17 && init==5,"Strict unboxing before effects");
                }
                flow=new OriginalWithdrawalGuideCompletion(user,2,()=>throw new InvalidOperationException("refresh"),(a,b,c)=>init++);
                bool interrupted=false;try{flow.Complete(1);}catch(InvalidOperationException){interrupted=true;}
                Check(interrupted && user.IsGoldReduceLevel1 && user.Gold==7 && init==5,"Refresh failure preserves deduction but prevents initialization");
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_WITHDRAWAL_GUIDE_VALIDATION_PASS actual Button close/save/panel ordering; cached-index callback, strict payload, once-per-stage deduction, zero clamp, refresh and initialization; production TXPanel wiring pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
