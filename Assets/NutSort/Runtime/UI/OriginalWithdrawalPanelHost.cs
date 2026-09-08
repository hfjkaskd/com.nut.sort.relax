using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalPanelHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalPanel> registry;
        private readonly Action<OriginalWithdrawalPanel> initialize;
        public bool IsOpen=>registry.Contains(18);
        public OriginalWithdrawalPanel Panel=>IsOpen?registry.Get(18):null;
        public OriginalWithdrawalPanelHost(Transform parent,string path,OriginalUserLocalData user,
            OriginalTables tables,string language,Func<float,string> format,
            Func<Action,IOriginalWithdrawalServices> createServices)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalPanel>(id=>"TXPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            initialize=p=>p.Bind(user,tables,language,format,createServices(()=>registry.Hide(18)));
        }
        public OriginalWithdrawalPanel Show(object[] arguments)=>registry.Show(18,p=>{initialize(p);p.Init(arguments);});
        public OriginalGuideButtonBinding GetWithdrawalGuideTarget()
        {
            var panel=registry.Get(18);
            return new OriginalGuideButtonBinding(panel.WithdrawalButton,panel.GetCallback);
        }
        public void Refresh()=>registry.Refresh(18);
        public void Hide()=>registry.Hide(18);
    }
}
