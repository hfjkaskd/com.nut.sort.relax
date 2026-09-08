using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalTargetGuideReturnValidation
    {
        public static void Validate()
        {
            var old=OriginalNewbieGuideView.CallbackAction;
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            try
            {
                var view=root.GetComponent<OriginalNewbieGuideView>();
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){GuideIndex=1,Level=2,IsCompleteRecordGuide=false};
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var trace=new List<string>();
                view.BindTeaching(tables,"en",v=>{},()=>trace.Add("guideClose"));
                view.ShowTargetCompletion(user,flag=>
                    {Check(!flag && user.IsCompleteRecordGuide && user.GuideIndex==1,"Request after target initialization, before index");trace.Add("request");user.GuideIndex=7;},
                    ()=>{Check(user.GuideIndex==8,"Increment reads post-request live index");trace.Add("save");},
                    (id,level)=>{Check(id==35 && level==1,"Target arguments");trace.Add("show");});
                Check(string.Join(",",trace)=="guideClose,show","Initial handoff must not request or save");
                UnityEngine.Object.DestroyImmediate(root);root=null;
                var target=new OriginalGuideTargetCompletion(user,1,value=>"",()=>trace.Add("targetClose"),
                    (banner,first)=>{Check(banner && !first && user.IsCompleteRecordGuide,"Native InitDone flags");trace.Add("init");});
                target.Continue();
                Check(string.Join(",",trace)=="guideClose,show,targetClose,init,request,save,guideClose" && OriginalNewbieGuideView.CallbackAction==null,"Target completion reaches original return sequence after guide destruction");
                root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));view=root.GetComponent<OriginalNewbieGuideView>();
                int closes=0;view.BindTeaching(tables,"en",v=>{},()=>closes++);
                view.ShowTargetCompletion(user,flag=>throw new InvalidOperationException("request"),()=>throw new Exception("unexpected save"),(a,b)=>{});
                var retained=OriginalNewbieGuideView.CallbackAction;bool failed=false;
                try{OriginalNewbieGuideView.CallbackActionInvoke(null);}catch(InvalidOperationException){failed=true;}
                Check(failed && user.GuideIndex==8 && closes==1 && OriginalNewbieGuideView.CallbackAction==retained,"Request failure leaves index/close and global callback unchanged");
                view.ShowTargetCompletion(user,flag=>{},()=>throw new InvalidOperationException("save"),(a,b)=>{});
                retained=OriginalNewbieGuideView.CallbackAction;failed=false;
                try{OriginalNewbieGuideView.CallbackActionInvoke("ignored");}catch(InvalidOperationException){failed=true;}
                Check(failed && user.GuideIndex==9 && closes==2 && OriginalNewbieGuideView.CallbackAction==retained,"Save failure preserves increment and blocks second close/clear");
            }
            finally{OriginalNewbieGuideView.CallbackAction=old;if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_TARGET_GUIDE_RETURN_VALIDATION_PASS guide/target continuation composition, request(false), live increment/save/close order and request/save exception boundaries; production request response remains external.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
