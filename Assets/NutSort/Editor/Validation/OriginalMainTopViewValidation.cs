using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalMainTopViewValidation
    {
        public static OriginalUserLocalData MakeUser()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.TXTargetGold="100";user.Level=52;user.Gold=10;user.Coin=20;user.IsGuideGoldComplete=true;user.IsGuideGoldTargetComplete=true;
            user.UserLssInfo=JObject.Parse("{\"GetType\":2}");user.ServerConfigData=JObject.Parse("{\"LSSUPT\":[20,30]}");
            user.GoldRewardTargetS2CData=JObject.Parse("{\"cal_cfg\":5,\"bear_rates\":\"100\",\"bear_list\":[{\"psi_value\":\"10\"},{\"psi_value\":\"20\"},{\"StartLevel\":7,\"RealLevel\":22,\"Stage2StartShowLevel\":30,\"Stage2StartRealLevel\":47,\"Stage2RealLevel\":51,\"psi_value\":\"100\",\"caliper_logs\":3,\"caliper_rank\":20}],\"bear_zs_list\":[{\"caliper_psi\":\"1000\",\"psi_value\":\"10\",\"caliper_logs\":3,\"caliper_rank\":20}]}");
            return user;
        }
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelTop"));
            try
            {
                var top=instance.GetComponent<OriginalMainTopView>();
                Check(top!=null && instance.GetComponentsInChildren<Transform>(true).Length==77,"Full original Top hierarchy");
                string[] children={"GM","GoldItem","CoinItem","HideLevel","TX_JD","PMDBullet","Level","PlayerGoldGetHint"};
                Check(instance.transform.childCount==children.Length,"Eight original children");
                for(int i=0;i<children.Length;i++)Check(instance.transform.GetChild(i).name==children[i],"Source sibling order");
                foreach(var node in instance.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject)==0,"No missing scripts");
                foreach(var button in instance.GetComponentsInChildren<Button>(true))Check(button.onClick.GetPersistentEventCount()==0,"No dragged events");
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var user=MakeUser();var formatter=new OriginalGoldFormatter(()=>"en-US");
                var trace=new List<string>();Action goldDone=null,coinDone=null;int selected=0;bool blocked=true,allow=false;
                var item=new OriginalMarqueeItem { IsGold=true,Minimum=7,Maximum=7 };
                top.Bind(user,tables,"en",formatter,()=>"US",()=>allow,done=>{trace.Add("gold request");goldDone=done;},done=>{trace.Add("coin request");coinDone=done;},id=>trace.Add("panel:"+id),()=>trace.Add("audio"),()=>true,()=>{selected++;return item;},()=>blocked,(seconds,done)=>{});
                top.PlayerHint.Push();long pushed=top.PlayerHint.LastShowTime;
                top.Init();Check(top.PlayerHint.LastShowTime==pushed,"Top Init does not reset MainPanel-owned hint");
                Check(top.GM.gameObject.activeSelf && top.Progress.IsShow && !top.Progress.gameObject.activeSelf,"Bound reentrant Gold refresh precedes progress hide");
                top.PlayerHint.Init();Check(top.PlayerHint.LastShowTime==-1,"Separate MainPanel hint initialization");
                top.PlayerHint.Push();top.PlayerHint.Show();Check(selected==0 && top.PlayerHint.LastShowTime>=pushed+3,"Hint shares modal guard");
                blocked=false;top.PlayerHint.Show();Check(selected==1,"Hint uses shared live selector");
                top.Refresh();Check(top.Gold.Value.text==formatter.Format(10) && top.Coin.Value.text==formatter.FormatCoin(20),"Bound currency formatting");
                top.Gold.Click.onClick.Invoke();top.Coin.Click.onClick.Invoke();top.GM.Button.onClick.Invoke();Check(trace.Count==0,"Shared click gate");
                allow=true;top.Gold.Click.onClick.Invoke();Check(string.Join(",",trace)=="gold request,audio","Gold defers panel until request callback");
                goldDone();Check(trace[2]=="panel:18","Gold callback routes panel 18");
                trace.Clear();top.Coin.Click.onClick.Invoke();Check(string.Join(",",trace)=="coin request,audio","Coin defers panel until request callback");coinDone();Check(trace[2]=="panel:19","Coin callback routes panel 19");
                trace.Clear();top.GM.Button.onClick.Invoke();Check(string.Join(",",trace)=="panel:3,audio","GM shared route");
                top.PlayerHint.ShowSelf(7);
                var serialized=new SerializedObject(top.PlayerHint);var head=(Image)serialized.FindProperty("head").objectReferenceValue;
                Check(head.sprite==Resources.Load<Sprite>("Atlas/PaySimple/PayPal"),"Self reads saved channel through Top binding");
                user.UserLssInfo["GetType"]=0;top.PlayerHint.ShowSelf(7);Check(head.sprite==Resources.Load<Sprite>("Atlas/PaySimple/Venmo"),"Self reads changed saved index live");
                user.Level=2;top.Refresh();Check(top.Hidden.gameObject.activeSelf && !top.Level.Group.gameObject.activeSelf && !top.Coin.gameObject.activeSelf,"Live user stage refresh");
                Debug.Log("NUT_MAIN_TOP_VIEW_VALIDATION_PASS full original Top hierarchy, one live provider binding, MainPanel-owned hint init, request callbacks, shared click/panel gates and saved channel changes.");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
