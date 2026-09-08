using System;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalNewbieInteractionValidation
    {
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            try
            {
                var v=instance.GetComponent<OriginalNewbieGuideView>();
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){GuideIndex=12};
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                int calls=0,audio=0,closed=0;bool allowed=false;
                v.BindTeaching(tables,"en",value=>{},()=>{Check(!v.IsOpen,"Close marker cleared before callback");closed++;});
                v.InitializeInteractions(user,null,()=>allowed,()=>audio++);
                Check(v.IsOpen && v.ShowBanner && v.InitialGuideIndex==12 && !v.HollowMask.gameObject.activeSelf && !v.CloseButton.gameObject.activeSelf,"Native initial state and hidden controls");
                Action first=()=>{Check(v.ClickAction!=null,"Action remains installed during invocation");calls++;};
                v.ClickAction=first;v.ContinueButton.onClick.Invoke();
                Check(calls==0 && audio==0 && v.ClickAction==first,"Denied gate preserves pending action");
                allowed=true;v.FullButton.onClick.Invoke();
                Check(calls==1 && audio==1 && v.ClickAction==null,"Full button dispatches then clears then plays sound");
                v.ContinueButton.onClick.Invoke();Check(calls==1 && audio==2,"Null action still permits common click sound");
                v.ClickAction=()=>{calls++;v.ClickAction=()=>calls+=100;};
                v.ContinueButton.onClick.Invoke();v.FullButton.onClick.Invoke();
                Check(calls==2 && audio==4 && v.ClickAction==null,"Replacement installed during callback is cleared after return");
                Action failure=()=>throw new InvalidOperationException("fixture callback failure");v.ClickAction=failure;
                bool failed=false;try{v.FullButton.onClick.Invoke();}catch(InvalidOperationException){failed=true;}
                Check(failed && v.ClickAction==failure && audio==4,"Callback failure preserves action and skips following sound");
                user.GuideIndex=14;v.InitializeInteractions(user,false,()=>allowed,()=>audio++);
                Check(!v.ShowBanner && v.InitialGuideIndex==14 && v.ClickAction==failure,"Reinitialization captures guide without clearing pending action");
                v.InitializeInteractions(user,null,()=>allowed,()=>audio++);
                Check(!v.ShowBanner,"Absent argument retains prior banner field");
                allowed=false;v.CloseButton.onClick.Invoke();Check(v.IsOpen && closed==0,"Denied close gate keeps guide open");
                allowed=true;v.CloseButton.onClick.Invoke();
                Check(!v.IsOpen && closed==1 && audio==5,"Rebinding does not accumulate listeners; close precedes sound");
            }
            finally{UnityEngine.Object.DestroyImmediate(instance);}
            Debug.Log("NUT_NEWBIE_INTERACTION_VALIDATION_PASS actual three Buttons, captured initialization state, hidden controls, common gate, invoke-before-clear, replacement/failure semantics and close ordering.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
