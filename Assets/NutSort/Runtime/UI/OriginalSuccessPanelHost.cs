using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.UI
{
    // Owns UIName 9 with the same registration/init/refresh/hide ordering as UIMgr.
    public sealed class OriginalSuccessPanelHost
    {
        private readonly OriginalPanelRegistry<OriginalSuccessPanel> registry;
        private readonly Action<OriginalSuccessPanel> initialize;
        public bool IsOpen=>registry.Contains(9);
        public OriginalSuccessPanel Panel=>IsOpen?registry.Get(9):null;
        public OriginalSuccessPanelHost(Transform parent,string path,OriginalRewardItemFactory factory,
            OriginalTables tables,string language,Func<OriginalSuccessPanelFlow> createLifecycle,
            Func<Action<bool>,OriginalRewardGetFlow> createClaims,Action<bool,Action<JObject>> request,
            Action<int,OriginalItemGetInfo> showPanel,Action<OriginalSuccessPanelFlow,Action> hideFlow,
            Func<bool> gate,Action<string> audio,Action<float,Action> schedule,
            OriginalPanelActionQueue queue,Action hidden=null)
        {
            registry=new OriginalPanelRegistry<OriginalSuccessPanel>(id=>"SuccessPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalSuccessPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            initialize=p=>
            {
                var lifecycle=createLifecycle();
                var reward=new OriginalSuccessRewardFlow(request,showPanel,p.ClosePanel);
                p.Bind(lifecycle,createClaims(reward.GetReward),factory,tables,language,gate,audio,
                    ()=>registry.Hide(9),baseHide=>hideFlow(lifecycle,baseHide),schedule,queue,hidden);
            };
        }
        public OriginalSuccessPanel Show(OriginalItemGetInfo info)=>registry.Show(9,p=>{initialize(p);p.Init(info);});
        public void Refresh()=>registry.Refresh(9);
        public void Hide()=>registry.Hide(9);
    }
}
