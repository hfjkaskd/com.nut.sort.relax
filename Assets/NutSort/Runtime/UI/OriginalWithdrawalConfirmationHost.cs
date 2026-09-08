using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalConfirmationHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalConfirmationPanel> registry;
        private readonly Action<OriginalWithdrawalConfirmationPanel> bind;
        public bool IsOpen=>registry.Contains(21);
        public OriginalWithdrawalConfirmationPanel Panel=>IsOpen?registry.Get(21):null;
        public OriginalWithdrawalConfirmationHost(Transform parent,string path,OriginalUserLocalData user,OriginalTables tables,string language,
            Func<string> country,Func<string> area,Func<bool> canClick,Action<string> sound,Action<int,object[]> show,
            Action<int,bool> goldGet,Action<object> guide,Action hidden,Action<float,Action> schedule,Action next)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalConfirmationPanel>(id=>"TXUserInfoSurePanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalConfirmationPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            bind=p=>p.Bind(user,tables,language,country,area,canClick,sound,show,goldGet,guide,()=>registry.Hide(21),hidden,schedule,next);
        }
        public OriginalWithdrawalConfirmationPanel Show(object[] args)=>registry.Show(21,p=>{bind(p);p.Init(args);});
        public void Refresh()=>registry.Refresh(21);
        public void Hide()=>registry.Hide(21);
    }
}
