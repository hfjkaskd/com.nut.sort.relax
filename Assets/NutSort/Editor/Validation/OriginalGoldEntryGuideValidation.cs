using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGoldEntryGuideValidation
    {
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            try
            {
                foreach(int index in new[]{10,12,14})
                {
                    var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
                    try
                    {
                        var view=root.GetComponent<OriginalNewbieGuideView>();
                        var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"))
                            {GuideIndex=index,IsGuideGold=true,IsGuideGoldComplete=false,IsGuideGoldTargetComplete=false};
                        var trace=new List<string>();bool gate=false;Action reveal=null;Action<object> retained=value=>{};
                        OriginalNewbieGuideView.CallbackAction=retained;
                        view.InitializeInteractions(user,null,()=>gate,()=>trace.Add("audio"));
                        view.BindTeaching(tables,"en",v=>{},()=>{Check(user.GuideIndex==100,"Live increment precedes close");trace.Add("close");});
                        view.BindMask((delay,callback)=>{Check(delay==.2f && user.IsGuideGold,"Setup does not mutate guide flags");reveal=callback;});
                        view.ShowGoldEntryGuide(user,view.FullButton.GetComponent<RectTransform>(),flag=>
                        {
                            Check(flag && !user.IsGuideGold && user.GuideIndex==99,"Request true follows flag changes and precedes increment");
                            Check(user.IsGuideGoldComplete==(index==12) && user.IsGuideGoldTargetComplete==(index==14),"Captured-index completion flags");trace.Add("request");
                        });
                        Check(view.Tip.text==tables.Text.GetText(index==14?187:126,"en"),"Original index-specific tip");
                        Check(OriginalNewbieGuideView.CallbackAction==retained && !view.HollowMask.gameObject.activeSelf,"Global callback untouched and reveal delayed");
                        reveal();Check(view.HollowMask.gameObject.activeSelf,"Actual Graphic reveal");
                        user.GuideIndex=99;view.ContinueButton.onClick.Invoke();Check(trace.Count==0 && user.IsGuideGold,"Gate rejection");
                        gate=true;view.ContinueButton.onClick.Invoke();Check(string.Join(",",trace)=="request,close,audio","Native request/increment/close/audio sequence");
                        user.IsGuideGold=true;user.GuideIndex=index;view.InitializeInteractions(user,null,()=>true,()=>throw new Exception("unexpected audio"));
                        view.ShowGoldEntryGuide(user,view.FullButton.GetComponent<RectTransform>(),flag=>throw new InvalidOperationException("request"));
                        var old=view.ClickAction;bool failed=false;
                        try{view.ContinueButton.onClick.Invoke();}catch(InvalidOperationException){failed=true;}
                        Check(failed && !user.IsGuideGold && user.GuideIndex==index && view.ClickAction==old,"Request failure preserves prior flag writes but blocks increment/close/clear");
                    }
                    finally{UnityEngine.Object.DestroyImmediate(root);}
                }
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;}
            Debug.Log("NUT_GOLD_ENTRY_GUIDE_VALIDATION_PASS actual tips 126/187 and delayed mask, cached-index flags, gate, request(true)/increment/close order and failure boundary; SDK analytics excluded, production binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
