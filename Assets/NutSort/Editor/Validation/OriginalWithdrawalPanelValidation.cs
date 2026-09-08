using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalPanelValidation
    {
        public sealed class Services:IOriginalWithdrawalServices
        {
            public OriginalUserLocalData User;public OriginalTables Tables;public bool Allowed;
            public readonly List<int> Panels=new List<int>();public readonly List<string> Tips=new List<string>();
            public int Sounds,Closes,Saves,HiddenCount,Dequeues;public Action Queued;
            public bool CanClick=>Allowed;
            public bool OnlineTimeHint{get;set;}
            public bool Progress2Guide{get;set;}
            public string MainGoldHint=>"Fixture HUD hint";
            public string Country=>"US";
            public void SetGold(float value,bool refresh,bool showHint){Check(refresh&&showHint,"Native gold flags");User.Gold=value;}
            public void Save()=>Saves++;
            public void ShowPanel(int id,object[] args)=>Panels.Add(id);
            public void ShowTip(string text)=>Tips.Add(text);
            public void PlaySound(string sound){Check(sound=="Click","Native click audio");Sounds++;}
            public void Closed()=>Closes++;
            public void Hidden()=>HiddenCount++;
            public void Schedule(float delay,Action action){Check(delay==2.5f,"Native queue delay");Queued=action;}
            public void NextPanel()=>Dequeues++;
            public void InitializePlayerInfo(OriginalPlayerInfo player,int level)
            {player.Init(level,User,Tables,"en",()=>false,()=>"",()=>0,()=>0,v=>"",(p,l)=>throw new Exception("No PMD in fixture"));}
        }
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>("Prefabs/Panels/TXPanel");Check(prefab!=null,"TXPanel loads");
            Check(prefab.GetComponentsInChildren<RectTransform>(true).Length==28,"Original 28-object hierarchy");
            foreach(var t in prefab.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts: "+t.name);
            foreach(var t in prefab.GetComponentsInChildren<TMP_Text>(true))Check(t.font!=null&&t.fontSharedMaterial!=null,"Font/material resolved");
            foreach(var b in prefab.GetComponentsInChildren<Button>(true))Check(b.onClick.GetPersistentEventCount()==0&&b.GetComponent<OriginalButtonFeedback>()!=null,"Standard buttons with code binding and configured feedback");
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            user.Level=2;user.Level1Gold=5;user.GuideIndex=2;
            user.ServerConfigData=JObject.Parse(@"{""LSS260820"":true}");
            user.GoldRewardTargetS2CData=JObject.Parse(@"{""cal_cfg"":5,""bear_list"":[{""psi_value"":""5""},{""psi_value"":""10""},{""RealLevel"":5,""Stage2RealLevel"":10,""caliper_logs"":3,""caliper_rank"":8}]}");
            var host=new Services{User=user,Tables=tables};var formatter=new OriginalGoldFormatter(()=>"en-US");
            var obj=UnityEngine.Object.Instantiate(prefab);var panel=obj.GetComponent<OriginalWithdrawalPanel>();
            try
            {
                panel.Bind(user,tables,"en",v=>formatter.Format(v),host);panel.Init(new object[]{1});panel.Refresh();
                Check(panel.Main.localScale==Vector3.zero&&panel.Main.Find("Title")==null,"Only original main scales; Title1 is not a title-tween target");
                Check(panel.GoldLabel.text==formatter.Format(5)&&panel.ProgressLabel.text=="1/1"&&panel.IsDoneTask,"Actual header/progress/TMP binding");
                Check(panel.PlayerParent.childCount==1&&!panel.PlayerParent.GetChild(0).gameObject.activeSelf,"Actual PlayerInfo prefab instantiated and hidden by new mode");
                panel.GetComponentInChildren<OriginalGoldImage>(true).Apply();
                panel.Advance(.251f);Check(host.Panels.Count==1&&host.Panels[0]==7,"Actual opening completion invokes guide branch");
                panel.WithdrawalButton.onClick.Invoke();Check(host.Sounds==0&&host.Closes==0,"Gate suppresses click and audio");
                host.Allowed=true;panel.WithdrawalButton.onClick.Invoke();Check(host.Panels[1]==20&&host.Sounds==1&&host.Closes==0,"Actual claim button dispatches native early route before closing tween");
                panel.Advance(.251f);Check(host.Closes==1,"Close callback follows original duration");panel.Hide();Check(host.HiddenCount==1&&host.Dequeues==0,"Hide schedules queue");host.Queued();Check(host.Dequeues==1,"Queue callback retained");
                UnityEngine.Object.DestroyImmediate(obj);obj=UnityEngine.Object.Instantiate(prefab);panel=obj.GetComponent<OriginalWithdrawalPanel>();
                user.Level=20;user.GuideIndex=0;user.Gold=5;user.TXTargetGold="10";user.ComeOnGold="10";user.LoginDay=1;user.IsGuideGold=false;OriginalWithdrawalPanel.IsPlayGoldTween=true;
                panel.Bind(user,tables,"en",v=>formatter.Format(v),host);panel.Init(Array.Empty<object>());panel.Refresh();
                Check(user.Gold==10&&host.Saves==1&&panel.ProgressLabel.text=="1/3"&&panel.Fill==1f/3&&!OriginalWithdrawalPanel.IsPlayGoldTween,"Actual pending credit selects login-days UI and shared tail resets flag");
                panel.GoldTween.Advance(.75f);Check(panel.GoldLabel.text==formatter.Format(8.75f)&&user.Gold==10,"Actual serialized gold tween updates only the bound TMP");
                panel.Refresh();Check(panel.PlayerParent.childCount==2&&host.Saves==1,"Repeated refresh appends PlayerInfo without paying twice");
                int count=host.Panels.Count;panel.GetCallback();Check(host.Panels.Count==count+1&&host.Panels[count]==31,"Retained pending-target hint routes actual claim callback");
                Debug.Log("NUT_WITHDRAWAL_PANEL_VALIDATION_PASS original prefab/resources/buttons, actual ordered refresh, gold tween, PlayerInfo, guide/open/close and claim routes; production registry host and current Play capture pending.");
            }
            finally{OriginalWithdrawalPanel.IsPlayGoldTween=false;UnityEngine.Object.DestroyImmediate(obj);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
