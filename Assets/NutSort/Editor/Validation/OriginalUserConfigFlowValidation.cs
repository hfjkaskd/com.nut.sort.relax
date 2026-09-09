using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUserConfigFlowValidation
    {
        public static void Validate()
        {
            var pending=new List<Action<bool>>();var results=new List<bool>();var messages=new List<Action>();
            var flow=new OriginalUserConfigFlow(done=>pending.Add(done),(id,retry,close)=>{Check(id==75&&!close,"Native message 75 and false close flag");messages.Add(retry);});
            flow.Config(results.Add,false);pending[0](false);Check(results.Count==1&&!results[0]&&messages.Count==0,"Nonpopup false reaches caller");
            flow.Config(results.Add,true);pending[1](false);Check(results.Count==1&&messages.Count==1&&pending.Count==2,"Popup failure holds caller and doesn't auto retry");
            pending[1](false);Check(ReferenceEquals(messages[0],messages[1]),"Same response closure caches retry action");
            messages[0]();Check(pending.Count==3,"Confirmation starts new request");pending[2](false);Check(messages.Count==3&&results.Count==1,"Retry retains popup branch");
            messages[2]();pending[3](true);Check(results.Count==2&&results[1],"Retry success invokes original callback");
            flow.Config(null,true);pending[4](true);pending[4](false);Check(messages.Count==4,"Null callback still allows failure dialog");
            var synchronous=new OriginalUserConfigFlow(done=>done(false),(id,retry,close)=>Check(id==75,"Synchronous failure dialog"));synchronous.Config(null,true);
            Expect<InvalidOperationException>(()=>new OriginalUserConfigFlow(done=>throw new InvalidOperationException(),(id,retry,close)=>throw new Exception()).Config(null,true));
            Expect<InvalidOperationException>(()=>new OriginalUserConfigFlow(done=>done(false),(id,retry,close)=>throw new InvalidOperationException()).Config(value=>throw new Exception(),true));
            Expect<InvalidOperationException>(()=>new OriginalUserConfigFlow(done=>done(true),(id,retry,close)=>{}).Config(value=>throw new InvalidOperationException(),true));
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel")).GetComponent<OriginalMessagePanel>();
            try
            {
                panel.Bind(id=>"localized "+id);panel.Init();panel.gameObject.SetActive(false);pending.Clear();results.Clear();
                flow=new OriginalUserConfigFlow(done=>{Check(!panel.gameObject.activeSelf,"Panel hides before confirm initiates retry");pending.Add(done);},panel.Show);
                flow.Config(results.Add,true);pending[0](false);Check(panel.gameObject.activeSelf&&panel.ContentText.text=="localized 75"&&results.Count==0,"Actual localized retry panel");
                panel.ConfirmButton.onClick.Invoke();Check(pending.Count==2&&!panel.gameObject.activeSelf,"Actual Button starts held retry after hide");pending[1](false);Check(panel.gameObject.activeSelf,"Repeated failure reopens same panel");
                panel.ConfirmButton.onClick.Invoke();pending[2](true);Check(results.Count==1&&results[0]&&!panel.gameObject.activeSelf,"Actual confirm then successful completion");
                // A synchronous second failure may reopen inside the first confirm;
                // MessagePanel must not hide or clear the replacement afterward.
                int attempts=0;flow=new OriginalUserConfigFlow(done=>{attempts++;done(false);},panel.Show);flow.Config(null,true);panel.ConfirmButton.onClick.Invoke();Check(attempts==2&&panel.gameObject.activeSelf,"Synchronous retry failure remains visible");
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);}
            Debug.Log("NUT_USER_CONFIG_FLOW_VALIDATION_PASS native popup/nonpopup failure, held callback, cached retry action, repeated and synchronous retries, null/exception semantics and actual MessagePanel Button composition; server result remains an explicit port.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected exception");}
    }
}
