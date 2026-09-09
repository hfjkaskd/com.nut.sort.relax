using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalTooFastHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalTooFastPanel> registry;
        private readonly Action<OriginalWithdrawalTooFastPanel> bind;
        public bool IsOpen=>registry.Contains(33);
        public OriginalWithdrawalTooFastPanel Panel=>IsOpen?registry.Get(33):null;
        public OriginalWithdrawalTooFastHost(Transform parent,string path,OriginalUserLocalData user,OriginalTables tables,string language,
            Func<bool> canClick,Action<string> sound,Func<int> showLevel,Func<Action> findStageProgress,
            Action hidden,Action<float,Action> schedule,Action next)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalTooFastPanel>(id=>"TXTooFastHintPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalTooFastPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            bind=p=>p.Bind(user,tables,language,canClick,sound,showLevel,findStageProgress,()=>registry.Hide(33),hidden,schedule,next);
        }
        public OriginalWithdrawalTooFastPanel Show(object[] args)=>registry.Show(33,p=>{bind(p);p.Init();});
        public void Refresh()=>registry.Refresh(33);
        public void Hide()=>registry.Hide(33);
    }
}
