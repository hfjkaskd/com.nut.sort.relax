using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalUserInfoHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalUserInfoPanel> registry;
        private readonly Action<OriginalWithdrawalUserInfoPanel> bind;
        public bool IsOpen=>registry.Contains(20);
        public OriginalWithdrawalUserInfoPanel Panel=>IsOpen?registry.Get(20):null;
        public OriginalWithdrawalUserInfoHost(Transform parent,string path,OriginalUserLocalData user,OriginalTables tables,string language,
            Func<string> country,Func<string> area,Func<bool> canClick,Action<string> sound,Action<int> tip,Action<int,object[]> show,
            Action save,Action<object> guide,Action clearHint,Action hidden,Action<float,Action> schedule,Action next)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalUserInfoPanel>(id=>"TXUserInfoPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalUserInfoPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            bind=p=>p.Bind(user,tables,language,country,area,canClick,sound,tip,show,save,guide,clearHint,()=>registry.Hide(20),hidden,schedule,next);
        }
        public OriginalWithdrawalUserInfoPanel Show(object[] args)=>registry.Show(20,p=>{bind(p);p.Init(args);});
        public void Refresh()=>registry.Refresh(20);
        public void Hide()=>registry.Hide(20);
    }
}
