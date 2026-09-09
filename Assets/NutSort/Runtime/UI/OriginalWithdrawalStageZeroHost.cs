using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalStageZeroHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalStageZeroPanel> registry;
        private readonly Action<OriginalWithdrawalStageZeroPanel,object[]> bind;
        public bool IsOpen=>registry.Contains(23);
        public OriginalWithdrawalStageZeroPanel Panel=>IsOpen?registry.Get(23):null;
        public OriginalWithdrawalStageZeroHost(Transform parent,string path,OriginalUserLocalData user,OriginalTables tables,string language,
            Func<long> clock,Func<float,string> goldFormat,Func<long,string,string> timeFormat,Func<int,int,int> random,Func<bool> complete,Func<int> showLevel,
            Action save,Action<object> guide,Func<bool> canClick,Action<string> sound,Action<int,object[]> show,
            Action hidden,Action<float,Action> schedule,Action next)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalStageZeroPanel>(id=>"TXProgress0Panel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalStageZeroPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            bind=(p,args)=>p.Bind(user,tables,language,args,clock,goldFormat,timeFormat,random,complete,showLevel,save,guide,canClick,sound,show,()=>registry.Hide(23),hidden,schedule,next);
        }
        public OriginalWithdrawalStageZeroPanel Show(object[] args)=>registry.Show(23,p=>{bind(p,args);p.Init();});
        public void Refresh()=>registry.Refresh(23);
        public void Hide()=>registry.Hide(23);
    }
}
