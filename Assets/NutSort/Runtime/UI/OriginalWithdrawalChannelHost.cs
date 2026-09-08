using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalChannelHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalChannelPanel> registry;
        private readonly Action<OriginalWithdrawalChannelPanel> bind;
        public bool IsOpen=>registry.Contains(22);
        public OriginalWithdrawalChannelPanel Panel=>IsOpen?registry.Get(22):null;
        public OriginalWithdrawalChannelHost(Transform parent,string path,OriginalUserLocalData user,OriginalTables tables,string language,
            Func<bool> canClick,Action<string> sound,Action<int> tip,Action<int,object[]> show,Action<int> hide,
            Action hidden,Action<float,Action> schedule,Action next)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalChannelPanel>(id=>"TXChannelPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalChannelPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            bind=p=>p.Bind(user,tables,language,canClick,sound,tip,show,hide,()=>registry.Hide(22),hidden,schedule,next);
        }
        public OriginalWithdrawalChannelPanel Show(object[] args)=>registry.Show(22,p=>{bind(p);p.Init(args);});
        public void Refresh()=>registry.Refresh(22);
        public void Hide()=>registry.Hide(22);
    }
}
