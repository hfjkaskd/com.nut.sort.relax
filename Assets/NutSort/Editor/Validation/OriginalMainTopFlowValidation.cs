using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalMainTopFlowValidation
    {
        public static void Validate()
        {
            var instances=new List<GameObject>();
            try
            {
                Func<string,GameObject> make=path=>{var go=UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path));instances.Add(go);return go;};
                var main=make("Prefabs/Panels/MainPanel");
                var level=main.GetComponentInChildren<OriginalMainLevelView>(true);
                var progress=main.GetComponentInChildren<OriginalRewardProgressView>(true);
                var gold=make("Prefabs/Panels/GoldItem").GetComponent<OriginalGoldItem>();
                var coin=make("Prefabs/Panels/CoinItem").GetComponent<OriginalCoinItem>();
                var hidden=make("Prefabs/Panels/HiddenLevel").GetComponent<OriginalHiddenLevelView>();
                var marquee=make("Prefabs/Panels/PMDBullet").GetComponent<OriginalMarqueeLauncher>();
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                user.TXTargetGold="100";user.Level=52;user.Gold=10;user.Coin=20;user.IsGuideGoldComplete=true;user.IsGuideGoldTargetComplete=true;
                user.GoldRewardTargetS2CData=JObject.Parse("{\"cal_cfg\":5,\"bear_rates\":\"100\",\"bear_list\":[null,null,{\"StartLevel\":7,\"RealLevel\":22,\"Stage2StartShowLevel\":30,\"Stage2StartRealLevel\":47,\"Stage2RealLevel\":51,\"psi_value\":\"100\",\"caliper_logs\":3,\"caliper_rank\":20}],\"bear_zs_list\":[{\"caliper_psi\":\"1000\",\"psi_value\":\"10\",\"caliper_logs\":3,\"caliper_rank\":20}]}");
                var formatter=new OriginalGoldFormatter(()=>"en-US");
                int gm=0,reentrant=0;
                var flow=new OriginalMainTopFlow(gold,coin,hidden,progress,marquee,level,user,tables,"en",()=>{gm++;Check(gold.Value.text=="sentinel","GM initialization precedes currency initialization");});
                progress.Bind(user,tables,"en",n=>formatter.Format(n));hidden.Bind(user,tables,"en");
                gold.Bind(user,tables,"en",n=>formatter.Format(n),()=>"US",()=>true,done=>{},id=>{},()=>{},()=>{reentrant++;Check(gm==1,"Reentrant refresh follows GM setup");flow.RefreshRewardAndLevel();});
                coin.Bind(user,tables,"en",formatter,()=>true,done=>{},id=>{},()=>{});
                var text=new OriginalMarqueeText(user,tables,n=>formatter.Format(n));
                marquee.Bind(()=>user.Level,()=>null,item=>item.Bind(text,tables.PayChannels,"en",()=>"US",(delay,done)=>{}));
                gold.Value.text="sentinel";coin.Value.text="coin sentinel";hidden.gameObject.SetActive(false);
                flow.Init();
                Check(reentrant==1 && progress.IsShow && !progress.gameObject.activeSelf,"Gold Init refreshes progress before Top Init hides it without resetting IsShow");
                Check(level.Group.anchoredPosition.x==374 && level.Group.gameObject.activeSelf,"Reentrant label uses progress eligibility");
                Check(gold.Value.text=="sentinel" && coin.Value.text=="coin sentinel" && !hidden.SubRoundTemplate.activeSelf,"Init retains amounts and hides sub-round template");
                flow.Refresh();
                Check(gold.Value.text==formatter.Format(user.Gold) && coin.Value.text==formatter.FormatCoin(user.Coin),"Full refresh updates both amounts");
                Check(!hidden.gameObject.activeSelf && !progress.gameObject.activeSelf && progress.IsShow && level.Group.gameObject.activeSelf && level.Group.anchoredPosition.x==374,"Late-level eligibility updates without showing progress");
                progress.Show();Check(progress.gameObject.activeSelf,"Separate lifecycle Show reveals eligible progress");
                user.Level=2;flow.Refresh();
                Check(hidden.gameObject.activeSelf && !coin.gameObject.activeSelf && !progress.IsShow && !level.Group.gameObject.activeSelf,"Early hidden progress supersedes ordinary level label");
                user.Level=1;flow.Refresh();
                Check(!hidden.gameObject.activeSelf && !level.Group.gameObject.activeSelf,"First-level title stays hidden");
                Debug.Log("NUT_MAIN_TOP_FLOW_VALIDATION_PASS actual prefab consumers, reentrant Gold Init/progress hide ordering, amount refresh and early/late level visibility.");
            }
            finally { foreach(var instance in instances)UnityEngine.Object.DestroyImmediate(instance); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
