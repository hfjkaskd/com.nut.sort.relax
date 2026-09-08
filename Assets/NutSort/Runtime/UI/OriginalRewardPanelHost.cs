using System;
using NutSort.Content;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalRewardPanelHost
    {
        private readonly OriginalPanelRegistry<OriginalRewardPanel> registry;
        public bool IsOpen=>registry.Contains(8);
        public OriginalRewardPanel Panel=>IsOpen?registry.Get(8):null;
        public OriginalRewardPanelHost(Transform parent,string path,OriginalRewardItemFactory factory,OriginalItemManager items,
            Action<int,Image> flyItem,Action<float,Action> schedule,OriginalPanelActionQueue queue,Action hidden=null)
        {
            registry=new OriginalPanelRegistry<OriginalRewardPanel>(id=>"RewardPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalRewardPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            initialize=p=>p.Bind(factory,items,flyItem,schedule,()=>registry.Hide(8),queue,hidden);
        }
        private readonly Action<OriginalRewardPanel> initialize;
        public OriginalRewardPanel Show(OriginalItemGetInfo info)=>registry.Show(8,p=>{initialize(p);p.Init(info);});
        public void Refresh()=>registry.Refresh(8);
        public void Hide()=>registry.Hide(8);
    }
}
