using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalRefreshValidation
    {
        private sealed class Host:IOriginalWithdrawalRefreshHost,IOriginalWithdrawalHeaderUI,IOriginalWithdrawalProgressUI
        {
            public readonly List<string> Trace=new List<string>();public bool Tween=true;public int Claim,Tip;public string Progress,Gold;
            public Action<int> Player;
            private float fill;
            public float Fill{get=>fill;set{fill=Mathf.Clamp01(value);Trace.Add("fill");}}
            public string MainGoldHint{get{Trace.Add("hud");return "Live HUD";}}
            public bool PlayGoldTween{get=>Tween;set{Tween=value;Trace.Add("reset");}}
            public void SetRawTip(string text){Trace.Add("raw");}
            public void SetTip(int id){Tip=id;Trace.Add("tip:"+id);}
            public void SetTip(int id,params object[] args){SetTip(id);}
            public void SetClaimText(int id){Claim=id;Trace.Add("claim:"+id);}
            public void InitializePlayerInfo(int level){Trace.Add("player");Player(level);}
            public void SetLevelTitle(int slot,int id,int value){Trace.Add("level:"+slot);}
            public void SetGold(string value){Gold=value;Trace.Add("gold");}
            public void SetTitle(int id,int value){Trace.Add("title");}
            public void SetProgress(string text){Progress=text;Trace.Add("progress");}
            public void SetProgressTip(int id,params object[] args){Trace.Add("hint:"+id);}
            public void SetMarkerX(float x){Trace.Add("marker");}
        }
        public static void Validate()
        {
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var settings=Resources.Load<OriginalPanelSettings>("Configuration/OriginalPanels");
            var host=new Host();var format=new OriginalGoldFormatter(()=>"en-US");Func<float,string> money=v=>format.Format(v);
            user.Level=20;user.Gold=5;user.TXTargetGold="10";user.ComeOnGold="10";user.LoginDay=1;user.IsGuideGold=false;
            user.GoldRewardTargetS2CData=JObject.Parse(@"{""cal_cfg"":5,""bear_list"":[null,null,{""RealLevel"":5,""Stage2RealLevel"":10,""caliper_logs"":3,""caliper_rank"":8}]}");
            user.ServerConfigData=JObject.Parse(@"{""LSS260820"":true}");
            GameObject player=null;var tween=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/WithdrawalGoldTween")).GetComponent<OriginalWithdrawalGoldTween>();
            try
            {
                host.Player=level=>
                {
                    Check(level==20&&host.Tween,"Captured level and tween flag reach actual PlayerInfo before reset");
                    player=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerInfo/PlayerInfo"));
                    player.GetComponent<OriginalPlayerInfo>().Init(level,user,tables,"en",()=>throw new Exception("New mode skips PMD"),()=>"",()=>0,()=>0,money,(p,l)=>{});
                    Check(!player.activeSelf,"New-mode actual PlayerInfo remains hidden");user.IsGuideGold=true;
                };
                var lifecycle=new OriginalWithdrawalPanelFlow(user,()=>false,b=>{},()=>false,id=>{},()=>{});
                lifecycle.Init(Array.Empty<object>(),()=>{},()=>{},()=>{});
                var header=new OriginalWithdrawalHeader(user,()=>tables.GetShowLevel(user.Level),money,host);
                var progress=new OriginalWithdrawalEarlyProgress(()=>tables.GetShowLevel(user.Level),host,settings);
                var stages=new OriginalRewardProgress(user,tables,money);
                bool failSave=false;
                var pending=new OriginalWithdrawalPendingGold(user,()=>host.PlayGoldTween,(v,a,b)=>{Check(a&&b,"Native setter flags");host.Trace.Add("apply");user.Gold=v;},
                    ()=>{host.Trace.Add("save");if(failSave)throw new InvalidOperationException("Save fixture");},
                    (a,b,seconds,cb)=>{host.Trace.Add("animate");tween.Play(a,b,seconds,cb);},money,host.SetGold,tween.Duration);
                var flow=new OriginalWithdrawalRefreshFlow(user,lifecycle,header,pending,progress,stages,host,money,()=>throw new Exception("Wrong stage table"),()=>throw new Exception("Wrong stage table"));
                flow.Refresh(Array.Empty<object>());
                Check(user.Gold==10&&user.ComeOnGold==""&&host.Progress=="1/3","Stage selection uses applied pending gold: login-days branch rather than amount branch");
                Check(host.Gold==money(5)&&pending.IsShowTargetHint&&!host.Tween&&host.Claim==36&&host.Tip==159,"Header starts before pending tween; later caption and final live guide override");
                Check(string.Join(",",host.Trace)=="level:1,level:2,gold,title,apply,save,animate,claim:1,hud,raw,fill,progress,hint:28,marker,claim:36,player,reset,tip:159","Complete recovered refresh operation order");
                tween.Advance(.75f);Check(host.Gold==money(8.75f)&&user.Gold==10&&!host.Tween,"Actual pending gold tween continues after shared flag reset and changes display only");
                UnityEngine.Object.DestroyImmediate(player);player=null;
                user.GoldRewardTargetS2CData=null;user.ComeOnGold="";user.Level1Gold=7;user.IsGuideGold=false;
                lifecycle.Init(new object[]{1},()=>{},()=>{},()=>{});host.Trace.Clear();host.Player=level=>Check(level==1,"Captured early level retained");
                flow.Refresh(new object[]{1});Check(host.Progress=="1/1"&&host.Gold==money(7)&&host.Claim==1&&progress.IsDoneTask,"Early path needs no server target and retains default claim caption");
                Check(!host.Trace.Contains("hud")&&host.Trace[host.Trace.Count-1]=="reset","Early branches skip later HUD/caption path but still run shared tail");
                host.Trace.Clear();failSave=true;host.Tween=true;user.ComeOnGold="11";bool failed=false;
                try{flow.Refresh(new object[]{1});}catch(InvalidOperationException){failed=true;}
                Check(failed&&host.Tween&&user.Gold==11&&user.ComeOnGold==""&&!host.Trace.Contains("claim:1")&&!host.Trace.Contains("player"),"Save failure aborts stage, caption and tail while preserving native partial mutations");
                Debug.Log("NUT_WITHDRAWAL_REFRESH_VALIDATION_PASS ordered header/pending/claim/stage/progress/actual PlayerInfo/tail composition, post-credit stage selection, actual tween after reset, early server-independent branch and save failure boundary; full TXPanel view/owner pending.");
            }
            finally{UnityEngine.Object.DestroyImmediate(tween.gameObject);if(player!=null)UnityEngine.Object.DestroyImmediate(player);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
