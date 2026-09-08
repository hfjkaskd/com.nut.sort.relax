using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalNewbieSuccessGuideValidation
    {
        public static void Validate()
        {
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            var sourceRoot=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/SuccessPanel"));
            try
            {
                var view=root.GetComponent<OriginalNewbieGuideView>();
                // Use the actual restored SuccessPanel visual target.
                var target=sourceRoot.GetComponent<OriginalSuccessPanel>().MoreButton;
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                var trace=new List<string>();Action delayed=null;bool gate=false;
                view.InitializeInteractions(user,null,()=>gate,()=>trace.Add("audio"));
                // Hidden tip branch does not read a table; use current content only for binding.
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                view.BindTeaching(tables,"en",v=>{},()=>{Check(user.GuideIndex==8,"Increment before close");trace.Add("close");});
                target.transform.position=new Vector3(113,219,0);target.image.rectTransform.sizeDelta=new Vector2(321,87);
                view.ShowSuccessGuide(user,target,()=>{Check(user.GuideIndex==7,"Request before guide increment");trace.Add("more");},
                    (enabled,seconds,text)=>{Check(enabled && seconds==1.5f && text==string.Empty,"Native mask arguments");trace.Add("mask");},
                    (seconds,action)=>{Check(seconds==.5f,"Native reopen delay");trace.Add("schedule");delayed=action;},
                    id=>{Check(id==7,"Reopen guide ID");trace.Add("show");});
                var serialized=new SerializedObject(view);
                var pos=(RectTransform)serialized.FindProperty("Pos").objectReferenceValue;
                var hand=(RectTransform)serialized.FindProperty("Hand").objectReferenceValue;
                Check(pos.position==target.transform.position && view.ContinueButton.image.rectTransform.sizeDelta==target.image.rectTransform.sizeDelta,"Original target world position and image size");
                Check(hand.gameObject.activeSelf && view.ContinueButton.gameObject.activeSelf && !view.Tip.transform.parent.gameObject.activeSelf,"Hand/button visible and tip hidden");
                user.GuideIndex=7;view.ContinueButton.onClick.Invoke();Check(trace.Count==0,"Rejected gate prevents all side effects");
                gate=true;view.ContinueButton.onClick.Invoke();
                Check(string.Join(",",trace)=="more,close,mask,schedule,audio" && view.ClickAction==null,"Native continuation order and one-shot listener");
                view.ShowSuccessGuide(user,target,()=>throw new InvalidOperationException("request"),(a,b,c)=>throw new Exception("unreachable"),(a,b)=>throw new Exception("unreachable"),id=>{});
                var retained=view.ClickAction;int count=trace.Count;bool failed=false;
                try{view.ContinueButton.onClick.Invoke();}catch(InvalidOperationException){failed=true;}
                Check(failed && user.GuideIndex==8 && view.ClickAction==retained && trace.Count==count,"Request failure leaves index, click action and later side effects untouched");
                view.BindTeaching(tables,"en",v=>{},()=>{});user.GuideIndex=int.MaxValue;
                view.ShowSuccessGuide(user,target,()=>{},(a,b,c)=>{},(a,b)=>{},id=>{});
                view.ContinueButton.onClick.Invoke();Check(user.GuideIndex==int.MinValue,"Guide increment preserves unchecked native overflow");
                UnityEngine.Object.DestroyImmediate(root);delayed();Check(trace[trace.Count-1]=="show","Reopen callback survives guide destruction");
            }
            finally{if(root!=null)UnityEngine.Object.DestroyImmediate(root);UnityEngine.Object.DestroyImmediate(sourceRoot);}
            Debug.Log("NUT_NEWBIE_SUCCESS_GUIDE_VALIDATION_PASS configured Button placement and visibility, gated request/increment/close/mask/schedule/audio ordering and delayed reopen after destruction; actual SuccessPanel Button target; complete host lifecycle covered separately.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
