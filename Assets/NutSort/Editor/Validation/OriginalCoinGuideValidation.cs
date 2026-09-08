using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalCoinGuideValidation
    {
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            try
            {
                var view=root.GetComponent<OriginalNewbieGuideView>();
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){IsShowCoin=false,GuideIndex=3};
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var trace=new List<string>();bool gate=false;Action reveal=null;int saves=0;
                view.BindTeaching(tables,"en",v=>{},()=>{Check(user.GuideIndex==4,"Increment before close");trace.Add("close");});
                view.InitializeInteractions(user,null,()=>gate,()=>trace.Add("audio"));
                view.BindMask((delay,callback)=>{Check(!user.IsShowCoin && delay==.2f,"Mask scheduled before coin flag");reveal=callback;trace.Add("mask");});
                var target=view.FullButton.GetComponent<RectTransform>();
                view.ShowCoinGuide(user,target,()=>{Check(user.IsShowCoin,"Flag before coin refresh");trace.Add("refresh");},
                    ()=>{saves++;trace.Add("save");},
                    flag=>{Check(!flag && user.GuideIndex==3,"Request false precedes increment");trace.Add("request");},
                    banner=>{Check(!banner,"Unlock without banner");trace.Add("unlock");});
                Check(string.Join(",",trace)=="mask,refresh,save" && view.Tip.text==tables.Text.GetText(127,"en"),"Original setup order and localized tip");
                Check(!view.HollowMask.gameObject.activeSelf,"Mask remains hidden until delay");reveal();
                Check(view.HollowMask.gameObject.activeSelf,"Original delayed reveal");
                int before=trace.Count;view.ContinueButton.onClick.Invoke();Check(trace.Count==before,"Rejected click has no effects");
                gate=true;view.ContinueButton.onClick.Invoke();
                Check(string.Join(",",trace)=="mask,refresh,save,request,close,audio" && saves==1,"Click sequence adds no save");
                UnityEngine.Object.DestroyImmediate(root);OriginalNewbieGuideView.CallbackActionInvoke(new object());
                Check(trace[trace.Count-1]=="unlock" && OriginalNewbieGuideView.CallbackAction==null,"Global callback ignores payload, survives guide destruction and unlocks");
                root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));view=root.GetComponent<OriginalNewbieGuideView>();
                view.BindTeaching(tables,"en",v=>{},()=>{});view.InitializeInteractions(user,null,()=>true,()=>{});view.BindMask((a,b)=>{});
                Action oldClick=()=>{};Action<object> oldGlobal=p=>{};view.ClickAction=oldClick;OriginalNewbieGuideView.CallbackAction=oldGlobal;
                bool failed=false;
                try{view.ShowCoinGuide(user,view.FullButton.GetComponent<RectTransform>(),()=>{},()=>throw new InvalidOperationException("save"),flag=>{},flag=>{});}
                catch(InvalidOperationException){failed=true;}
                Check(failed && user.IsShowCoin && view.ClickAction==oldClick && OriginalNewbieGuideView.CallbackAction==oldGlobal,"Save failure occurs after flag but before replacing callbacks");
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_COIN_GUIDE_VALIDATION_PASS actual prefab tip/mask, flag/refresh/save ordering, gated request/increment/close, retained callbacks on failure and global unlock(false); production coin binding pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
