using System;
using System.Collections.Generic;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalNewbieMaskPlacementValidation
    {
        public static void Validate()
        {
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            var targetObject=new GameObject("isolated mask target",typeof(RectTransform));
            try
            {
                var view=root.GetComponent<OriginalNewbieGuideView>();
                var serialized=new SerializedObject(view);
                var mask=(RectTransform)serialized.FindProperty("MaskRect").objectReferenceValue;
                Check(serialized.FindProperty("maskRevealDelay").floatValue==.2f,"Recovered 0.2 second delay configured in prefab");
                var pending=new List<Action>();
                view.BindMask((delay,callback)=>{Check(delay==.2f,"Native delay");pending.Add(callback);});
                var target=(RectTransform)targetObject.transform;
                root.transform.position=new Vector3(12,33,0);root.transform.localScale=new Vector3(2,3,1);
                target.position=new Vector3(142,-63,0);target.sizeDelta=new Vector2(93,57);
                target.localScale=new Vector3(3,4,1);target.localRotation=Quaternion.Euler(0,0,35);
                var originalRotation=mask.localRotation;var originalScale=mask.localScale;
                view.SetHollowMaskVisible(false);view.ShowMask(target);
                Check(mask.sizeDelta==target.sizeDelta && Vector3.Distance(mask.position,target.position)<.001f,"Copies sizeDelta and world position, not transformed bounds/local position");
                Check(mask.localRotation==originalRotation && mask.localScale==originalScale,"Source does not copy rotation or scale");
                Check(!view.HollowMask.gameObject.activeSelf && pending.Count==1,"Refresh precedes delayed reveal");
                var position=mask.position;var size=mask.sizeDelta;
                view.ShowMask(null);
                Check(view.HollowMask.gameObject.activeSelf && pending.Count==1 && mask.position==position && mask.sizeDelta==size,"Null target reveals immediately and preserves pending callback and geometry");
                view.SetHollowMaskVisible(false);pending[0]();
                Check(view.HollowMask.gameObject.activeSelf,"Previously scheduled reveal remains live after later visibility change");
                target.position=new Vector3(-19,51,0);view.ShowMask(target);view.ShowMask(target);
                Check(view.HollowMask.gameObject.activeSelf && pending.Count==3,"Non-null request does not first hide or coalesce callbacks");
                UnityEngine.Object.DestroyImmediate(targetObject);view.SetHollowMaskVisible(false);view.ShowMask(target);
                Check(view.HollowMask.gameObject.activeSelf && pending.Count==3,"Destroyed Unity target follows null branch");
                view.BindMask((delay,callback)=>throw new InvalidOperationException("schedule"));
                view.SetHollowMaskVisible(false);
                var next=view.ContinueButton.GetComponent<RectTransform>();
                bool failed=false;try{view.ShowMask(next);}catch(InvalidOperationException){failed=true;}
                Check(failed && mask.sizeDelta==next.sizeDelta && !view.HollowMask.gameObject.activeSelf,"Scheduling failure occurs after geometry mutation, without reveal");
            }
            finally{if(targetObject!=null)UnityEngine.Object.DestroyImmediate(targetObject);UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_NEWBIE_MASK_PLACEMENT_VALIDATION_PASS source size/world position, preserved scale/rotation, null/destroyed target, independent delayed reveal and failure ordering.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
