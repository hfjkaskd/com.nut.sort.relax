using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalLoadingHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalLoadingPanel> registry;
        public bool IsOpen=>registry.Contains(36);
        public OriginalWithdrawalLoadingPanel Panel=>IsOpen?registry.Get(36):null;
        public OriginalWithdrawalLoadingHost(Transform parent,string path,OriginalTables tables,string language,
            Func<OriginalWithdrawalPanel> withdrawal,Action hidden,Action<float,Action> schedule,Action next)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalLoadingPanel>(id=>"TXGuideLoadingPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalLoadingPanel>(),
                (id,p)=>{p.Bind(tables,language,()=>registry.Hide(36),()=>withdrawal().GetCallback(),hidden,schedule,next);p.Init();},
                p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
        }
        public OriginalWithdrawalLoadingPanel Show()=>registry.Show(36);
        public void Refresh()=>registry.Refresh(36);
        public void Hide()=>registry.Hide(36);
    }
}
