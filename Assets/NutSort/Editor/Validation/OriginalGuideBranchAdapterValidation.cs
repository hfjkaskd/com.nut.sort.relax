using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalGuideBranchAdapterValidation
    {
        private sealed class UI:IOriginalGuideUI
        {
            public Button Target;
            public readonly List<string> Calls=new List<string>();
            public Action Activation;
            public OriginalGuideSuccessBinding GetSuccessGuideTarget(){Calls.Add("successTarget");return new OriginalGuideSuccessBinding(Target,Activation);}
            public OriginalGuideButtonBinding GetWithdrawal(){Calls.Add("withdrawalTarget");return new OriginalGuideButtonBinding(Target,Activation);}
            public RectTransform GoldTarget {get{Calls.Add("goldTarget");return Target.GetComponent<RectTransform>();}}
            public RectTransform CoinTarget {get{Calls.Add("coinTarget");return Target.GetComponent<RectTransform>();}}
            public void Save()=>Calls.Add("save");
            public void ShowPanel(int id)=>Calls.Add("panel"+id);
            public void ShowTargetPanel(int id,int level){Check(id==35 && level==1,"Target panel arguments");Calls.Add("targetPanel");}
            public void SetMask(bool enabled,float duration,string text)=>Calls.Add("mask");
            public void Schedule(float delay,Action action)=>Calls.Add("schedule");
            public void RefreshGold()=>Calls.Add("refreshGold");
            public void RefreshCoin()=>Calls.Add("refreshCoin");
            public void InitializeLevel(bool a,bool b,bool c)=>Calls.Add("init");
            public void RequestTargetGoldInfo(bool flag){Check(!flag,"Target request false");Calls.Add("targetRequest");}
            public void RequestCoinGoldInfo(bool flag){Check(!flag,"Coin request false");Calls.Add("coinRequest");}
            public void RequestEntryGoldInfo(bool flag){Check(flag,"Entry request true");Calls.Add("entryRequest");}
            public void NewGameplayUnlock(bool flag)=>Calls.Add("unlock");
            public void DailyGift()=>Calls.Add("daily");
            public void RefreshMain()=>Calls.Add("refreshMain");
            public void ShowTargetBanner(Action action)=>Calls.Add("banner");
        }
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            try
            {
                foreach(int index in new[]{0,1,2,3,10,11,12,13,14,15})
                {
                    var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
                    try
                    {
                        var view=root.GetComponent<OriginalNewbieGuideView>();
                        var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){GuideIndex=index,Level=2};
                        var ui=new UI{Target=view.FullButton};ui.Activation=()=>ui.Calls.Add("capturedActivate");
                        view.BindTeaching(tables,"en",v=>{},()=>ui.Calls.Add("close"));view.BindMask(ui.Schedule);
                        view.InitializeInteractions(user,false,()=>true,()=>ui.Calls.Add("audio"));
                        view.BindGuide(user,()=>false,new OriginalGuideBranchAdapter(view,user,ui));
                        view.ShowGuide();ui.Activation=()=>throw new Exception("Late target lookup");
                        if(index==1)OriginalNewbieGuideView.CallbackActionInvoke(null);else view.ContinueButton.onClick.Invoke();
                        string expected=index==0?"successTarget,capturedActivate,close,mask,schedule,audio":
                            index==1?"close,targetPanel,targetRequest,save,close":
                            index==2?"withdrawalTarget,close,save,panel36,audio":
                            index==3?"coinTarget,schedule,refreshCoin,save,coinRequest,close,audio":
                            index%2==0?"goldTarget,schedule,entryRequest,close,audio":
                            "withdrawalTarget,capturedActivate,close,save,audio";
                        Check(string.Join(",",ui.Calls)==expected,"Real ShowGuide dispatch reaches the matching recovered branch with separate request handlers");
                    }
                    finally{UnityEngine.Object.DestroyImmediate(root);}
                }
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;}
            Debug.Log("NUT_GUIDE_BRANCH_ADAPTER_VALIDATION_PASS all active indices through actual guide dispatch and prefab Buttons; distinct request handlers and captured target callbacks; production UI implementation remains pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
