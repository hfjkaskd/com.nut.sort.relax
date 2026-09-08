using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalPanelRegistryValidation
    {
        public static void Validate()
        {
            var trace=new List<string>();var token=new object();bool initThrows=false,hideThrows=false,destroyThrows=false;
            OriginalPanelRegistry<object> registry=null;
            registry=new OriginalPanelRegistry<object>(id=>"FailPanel",id=>{trace.Add("create");return token;},(id,p)=>
            {Check(registry.Contains(id) && registry.Get(id)==p,"Registration precedes initialization");trace.Add("init");if(initThrows)throw new InvalidOperationException("fixture");},
            p=>trace.Add("refresh"),p=>{trace.Add("hide");if(hideThrows)throw new InvalidOperationException("fixture");},
            p=>{Check(registry.Contains(10),"Destroy precedes deregistration");trace.Add("destroy");if(destroyThrows)throw new InvalidOperationException("fixture");});
            Check(registry.Show(10)==token && string.Join(",",trace)=="create,init,refresh","Source show order");
            trace.Clear();registry.Hide(10);Check(registry.Count==0 && string.Join(",",trace)=="hide,destroy","Source close order");
            initThrows=true;trace.Clear();Expect(()=>registry.Show(10));Check(registry.Contains(10) && string.Join(",",trace)=="create,init","Failed Init retains registered panel without Refresh");
            hideThrows=true;trace.Clear();Expect(()=>registry.Hide(10));Check(registry.Contains(10) && string.Join(",",trace)=="hide","Failed Hide skips destroy/removal");
            hideThrows=false;destroyThrows=true;trace.Clear();Expect(()=>registry.Hide(10));Check(registry.Contains(10) && string.Join(",",trace)=="hide,destroy","Failed destroy retains registration");
            destroyThrows=false;registry.Hide(10);
            ValidateRefresh();
            var root=new GameObject("Panel registry prefab integration");
            try
            {
                var user=OriginalMainTopViewValidation.MakeUser();user.GoldRewardTargetS2CData=JObject.Parse("{\"bear_zs_show\":\"250\"}");user.ServerConfigData=new JObject();
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                OriginalPanelRegistry<OriginalFailurePanelView> live=null;
                var queue=new OriginalPanelActionQueue();Action pending=null;int events=0;
                queue.Add(()=>events++);
                live=new OriginalPanelRegistry<OriginalFailurePanelView>(id=>"FailPanel",
                    id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/FailPanel"),root.transform,false).GetComponent<OriginalFailurePanelView>(),
                    (id,p)=>{p.Bind(user,tables,"en",()=>true,s=>{},done=>done(),()=>{},()=>{},()=>live.Hide(id));p.BindHide(queue,(seconds,callback)=>{Check(seconds==2.5f,"Original hide delay");pending=callback;});p.Init();},
                    p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.DestroyImmediate(p.gameObject));
                var first=live.Show(10);Check(first.CoinValue.text=="250" && live.Count==1,"Actual prefab initialized/refreshed through registry");
                user.GoldRewardTargetS2CData["bear_zs_show"]="375";
                live.Refresh(10);
                Check(live.Get(10)==first && first.CoinValue.text=="375" && live.Count==1,
                    "Manager refresh updates actual prefab from current user data without replacement");
                first.Restart.onClick.Invoke();Check(first==null && live.Count==0,"Actual Restart Button closes through registry");
                Check(events==0 && pending!=null,"Closing schedules rather than executes next panel action");
                pending();Check(events==1,"Queued manager callback survives destruction of panel");
                var second=live.Show(10);Check(second!=null && live.Count==1,"Subsequent open instantiates new panel");live.Hide(10);
            }
            finally {UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_PANEL_REGISTRY_VALIDATION_PASS source show/hide registration order, retained state on lifecycle exceptions, explicit refresh name/lookup/error/exception semantics, actual FailPanel data refresh, Button close and fresh reopen.");
        }
        private static void ValidateRefresh()
        {
            var trace=new List<string>();var token=new object();bool throws=false;
            var registry=new OriginalPanelRegistry<object>(id=>{trace.Add("name");return "FailPanel";},
                id=>{trace.Add("create");return token;},(id,p)=>trace.Add("init"),
                p=>{Check(p==token,"Refresh receives registered instance");trace.Add("refresh");if(throws)throw new InvalidOperationException("fixture refresh");},
                p=>trace.Add("hide"),p=>trace.Add("destroy"));
            registry.Show(10);trace.Clear();registry.Refresh(10);
            Check(string.Join(",",trace)=="name,refresh" && registry.Get(10)==token,
                "Existing refresh resolves name then refreshes without Init/Create/Hide");
            throws=true;trace.Clear();Expect(()=>registry.Refresh(10));
            Check(registry.Get(10)==token && string.Join(",",trace)=="name,refresh","Refresh exception preserves registration");
            throws=false;registry.Hide(10);trace.Clear();
            string message=null;LogType? type=null;
            Application.LogCallback capture=(text,stack,kind)=>{message=text;type=kind;};
            Application.logMessageReceived+=capture;
            try{registry.Refresh(10);}
            finally{Application.logMessageReceived-=capture;}
            Check(registry.Count==0 && string.Join(",",trace)=="name" && message=="panel exist :FailPanel" && type==LogType.Error,
                "Missing refresh logs original diagnostic without creating or invoking view");
        }
        private static void Expect(Action action){bool caught=false;try{action();}catch(InvalidOperationException){caught=true;}Check(caught,"Expected fixture exception");}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
