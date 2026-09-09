using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalUserRegistrationFlowValidation
    {
        public static void Validate()
        {
            var trace=new List<string>();Action<bool> registration=null,configured=null;Action<JObject> reward=null;Action retry=null;bool ready=false,synchronous=false,failReward=false,failSave=false;
            var flow=new OriginalUserRegistrationFlow(done=>{trace.Add("register");registration=done;},(done,popup)=>{Check(popup,"Registration config enables failure popup");trace.Add("config");configured=done;},
                (done,refresh)=>{Check(refresh,"Registration requests refreshed reward info");trace.Add("reward");if(failReward)throw new InvalidOperationException();reward=done;if(synchronous)done(null);},
                (id,again,close)=>{Check(id==75&&!close,"Original retry message");trace.Add("message");retry=again;},value=>{Check(value,"Only true initialization signal");ready=value;trace.Add("ready");},()=>{trace.Add("save");if(failSave)throw new InvalidOperationException();});
            flow.Register();registration(false);Check(string.Join(",",trace)=="register,message"&&!ready,"Failed registration waits for confirmation without saving");
            retry();registration(true);Check(string.Join(",",trace)=="register,message,register,config"&&!ready,"Retry success reaches held config");
            configured(false);Check(trace[trace.Count-1]=="config"&&!ready,"False config callback does nothing");
            configured(true);Check(trace[trace.Count-2]=="reward"&&trace[trace.Count-1]=="save"&&!ready,"Reward request precedes save, readiness still held");
            reward(null);Check(ready&&trace[trace.Count-1]=="ready","Null reward callback also releases initialization");
            trace.Clear();flow.Register();Check(ready&&trace.Count==1,"Repeated Register does not reset initialized flag");
            ready=false;synchronous=true;registration(true);configured(true);Check(string.Join(",",trace)=="register,config,reward,ready,save","Synchronous reward readiness precedes save");
            trace.Clear();ready=false;synchronous=false;failReward=true;Expect<InvalidOperationException>(()=>configured(true));Check(string.Join(",",trace)=="reward"&&!ready,"Reward exception prevents save");failReward=false;
            trace.Clear();failSave=true;Expect<InvalidOperationException>(()=>configured(true));Check(string.Join(",",trace)=="reward,save"&&!ready,"Save exception doesn't fabricate ready");reward(new JObject());Check(ready,"Already requested callback remains valid after save error");
            Expect<InvalidOperationException>(()=>new OriginalUserRegistrationFlow(done=>done(false),null,null,(id,again,close)=>throw new InvalidOperationException(),null,null).Register());
            var panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel")).GetComponent<OriginalMessagePanel>();
            try
            {
                panel.Bind(id=>"localized "+id);panel.Init();panel.gameObject.SetActive(false);var held=new List<Action<bool>>();int requests=0,saves=0;
                var config=new OriginalUserConfigFlow(done=>held.Add(done),panel.Show);
                flow=new OriginalUserRegistrationFlow(done=>{Check(!panel.gameObject.activeSelf,"Hide before registration retry");requests++;registration=done;},config.Config,
                    (done,refresh)=>reward=done,panel.Show,value=>ready=value,()=>saves++);
                ready=false;flow.Register();registration(false);Check(panel.gameObject.activeSelf&&requests==1,"Actual registration failure panel");panel.ConfirmButton.onClick.Invoke();registration(true);
                held[0](false);Check(panel.gameObject.activeSelf&&!ready&&saves==0,"Config failure transitions to same original message view");
                panel.ConfirmButton.onClick.Invoke();held[1](true);Check(!panel.gameObject.activeSelf&&saves==1&&!ready,"Config retry saves but waits for actual reward callback");reward(null);Check(ready,"Complete register/config/message/reward composition");
            }
            finally{UnityEngine.Object.DestroyImmediate(panel.gameObject);}
            Debug.Log("NUT_USER_REGISTRATION_FLOW_VALIDATION_PASS source registration retry, popup config, refreshed reward request then save, readiness only on reward callback including null, synchronous/repeated/failure order and actual MessagePanel Button integration; request results remain explicit ports.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Expect<T>(Action action)where T:Exception{try{action();}catch(T){return;}throw new Exception("Expected exception");}
    }
}
