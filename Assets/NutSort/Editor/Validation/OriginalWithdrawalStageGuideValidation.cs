using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalStageGuideValidation
    {
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            try
            {
                foreach(int index in new[]{11,13,15})foreach(bool banner in new[]{false,true})
                {
                    var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
                    try
                    {
                        var view=root.GetComponent<OriginalNewbieGuideView>();
                        var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){GuideIndex=index};
                        var trace=new List<string>();bool gate=false;
                        view.InitializeInteractions(user,banner,()=>gate,()=>trace.Add("audio"));
                        view.BindTeaching(tables,"en",v=>{},()=>{Check(user.GuideIndex==19,"Set exactly 19 before close");trace.Add("close");});
                        view.BindMask((a,b)=>throw new Exception("Unexpected mask request"));
                        view.ShowWithdrawalStageGuide(user,view.FullButton.GetComponent<RectTransform>(),
                            ()=>{Check(user.GuideIndex==index,"GoldGet before guide mutation");trace.Add("get");},()=>trace.Add("save"),
                            ()=>trace.Add("daily"),()=>trace.Add("refresh"),
                            completion=>{Check(completion!=null,"Native non-null empty banner callback");trace.Add("banner");completion();});
                        Check(!view.Tip.transform.parent.gameObject.activeSelf && !view.HollowMask.gameObject.activeSelf,"Tip hidden; branch does not reveal mask");
                        view.ContinueButton.onClick.Invoke();Check(trace.Count==0,"Click gate");gate=true;view.ContinueButton.onClick.Invoke();
                        Check(string.Join(",",trace)=="get,close,save,audio","Native click ordering");
                        UnityEngine.Object.DestroyImmediate(root);OriginalNewbieGuideView.CallbackActionInvoke(new object());
                        Check(string.Join(",",trace)==(banner?"get,close,save,audio,daily,refresh,banner":"get,close,save,audio,daily,refresh"),"Completion ignores payload and survives guide destruction");
                    }
                    finally{if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
                }
                var extra=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
                try
                {
                    var view=extra.GetComponent<OriginalNewbieGuideView>();var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    view.InitializeInteractions(user,false,()=>true,()=>{});view.BindTeaching(tables,"en",v=>{},()=>{});
                    int banners=0;
                    view.ShowWithdrawalStageGuide(user,view.FullButton.GetComponent<RectTransform>(),()=>{},()=>{},()=>{},
                        ()=>view.InitializeInteractions(user,true,()=>true,()=>{}),completion=>banners++);
                    OriginalNewbieGuideView.CallbackActionInvoke(null);Check(banners==1,"Banner flag is read after refresh re-entry, not captured at binding");
                    view.ShowWithdrawalStageGuide(user,view.FullButton.GetComponent<RectTransform>(),()=>throw new InvalidOperationException("get"),()=>throw new Exception("unexpected save"),()=>{},()=>{},completion=>{});
                    user.GuideIndex=13;var click=view.ClickAction;bool failed=false;
                    try{view.ContinueButton.onClick.Invoke();}catch(InvalidOperationException){failed=true;}
                    Check(failed && user.GuideIndex==13 && view.ClickAction==click,"Get failure prevents state change and preserves click callback");
                }
                finally{UnityEngine.Object.DestroyImmediate(extra);}
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;}
            Debug.Log("NUT_WITHDRAWAL_STAGE_GUIDE_VALIDATION_PASS three source indices, gate/get/19/close/save order, daily/main/banner continuation, live flag after refresh and failure boundary; production TXPanel binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
