using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalRewardPanelValidation
    {
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/RewardPanel"));
            try
            {
                var panel=instance.GetComponent<OriginalRewardPanel>();
                Check(panel!=null&&instance.GetComponentsInChildren<Transform>(true).Length==3,"Original three-object RewardPanel hierarchy");
                Check(instance.transform.Find("main")==null&&Mathf.Approximately(instance.GetComponent<Image>().color.a,1f/255),"Source no main tween and custom alpha backdrop");
                foreach(var t in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"Official scripts resolve");
                var layout=panel.Items.GetComponent<HorizontalLayoutGroup>();
                Check(layout!=null&&layout.spacing==155&&layout.childAlignment==TextAnchor.MiddleCenter&&!layout.childControlWidth&&!layout.childForceExpandWidth,"Original horizontal layout");
                Check(instance.transform.Find("Image").GetComponent<Image>().sprite!=null,"Original backdrop sprite resolves");
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){RevokeCount=0};
                var trace=new List<string>();var timers=new List<Action>();var delays=new List<float>();
                var manager=new OriginalItemManager(user,()=>trace.Add("save"),()=>trace.Add("bottom"),(v,a,b)=>{user.Gold=v;trace.Add("gold");},(v,a)=>{user.Coin=v;trace.Add("coin");});
                var queue=new OriginalPanelActionQueue();queue.Add(()=>trace.Add("queue"));
                var factory=new OriginalRewardItemFactory(Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem"),new OriginalGoldFormatter(()=>"en-US"),()=>"US");
                panel.Bind(factory,manager,(type,icon)=>{Check(icon!=null,"Fly receives actual icon Image");trace.Add("fly"+type);},(seconds,done)=>{delays.Add(seconds);timers.Add(done);},()=>trace.Add("close"),queue,()=>trace.Add("hidden"));
                var info=new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=0,Count=5,MoreCount=9,CurrentCount=42},new OriginalItemInfo{ItemType=1,Count=0,DoubleCurrentCount=16777217.25}}};
                panel.Init(info);Check(panel.Views==null&&timers.Count==0,"Init only retains payload");panel.Refresh();
                Check(info.IsMore&&!info.ItemInfos[0].IsMore&&panel.Views.Count==2&&panel.Views[0].Count.text=="$5","Forced aggregate More does not change item counts");
                Check(delays.Count==1&&delays[0]==1&&trace.Count==0,"No inventory mutation until one-second timer");
                timers[0]();Check(string.Join(",",trace)=="gold,fly0,coin,fly1"&&user.Gold==42&&user.Coin==16777217.25,"Each actual balance assignment precedes its fly call");
                Check(delays.Count==2&&delays[1]==.5f,"Close delay starts after all items process");timers[1]();Check(trace[4]=="close","Delayed close");
                panel.Hide();Check(trace[5]=="hidden"&&delays[2]==2.5f,"Base hidden callback precedes delayed queue");timers[2]();Check(trace[6]=="queue","Queue continues after hide delay");
                trace.Clear();timers.Clear();delays.Clear();
                panel.Init(new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=2,Count=2}}});panel.Refresh();
                panel.Init(new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=2,Count=4}}});panel.Refresh();
                Check(panel.Items.childCount==4,"Refresh appends without clearing old items");timers[0]();timers[1]();
                Check(user.RevokeCount==8,"Both delayed callbacks use latest view list, without invented cancellation/deduplication");
                trace.Clear();timers.Clear();delays.Clear();
                panel.Bind(factory,manager,(type,icon)=>throw new InvalidOperationException("fly fixture"),(seconds,done)=>{delays.Add(seconds);timers.Add(done);},()=>{},queue,null);
                panel.Init(new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>{new OriginalItemInfo{ItemType=2,Count=1}}});panel.Refresh();
                bool failed=false;try{timers[0]();}catch(InvalidOperationException){failed=true;}
                Check(failed&&user.RevokeCount==9&&timers.Count==1,"Fly failure retains prior item addition and prevents close scheduling");
            }
            finally{UnityEngine.Object.DestroyImmediate(instance);}
            Debug.Log("NUT_REWARD_PANEL_VALIDATION_PASS source prefab/custom alpha/layout, small entries, aggregate More, delayed actual item-manager apply then fly, delayed close/queue, repeat refresh and failure ordering; flight implementation and default host composition pending.");
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    }
}
