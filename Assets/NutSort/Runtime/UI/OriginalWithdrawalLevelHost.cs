using System;
using NutSort.Content;
using Newtonsoft.Json.Linq;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalWithdrawalLevelHost
    {
        private readonly OriginalPanelRegistry<OriginalWithdrawalLevelPanel> registry;
        private readonly Action<OriginalWithdrawalLevelPanel> bind;
        public bool IsOpen=>registry.Contains(28);
        public OriginalWithdrawalLevelPanel Panel=>IsOpen?registry.Get(28):null;
        public OriginalWithdrawalLevelHost(Transform parent,string path,OriginalUserLocalData user,OriginalTables tables,string language,
            Func<float,string> format,Func<bool> canClick,Action<string> sound,IOriginalWithdrawalLevelUI services,
            Action<int,Action<JObject>> request,Action save,Func<object,Action> guide,Action hidden,Action<float,Action> schedule,Action next)
        {
            registry=new OriginalPanelRegistry<OriginalWithdrawalLevelPanel>(id=>"TXLevelPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalWithdrawalLevelPanel>(),
                (id,p)=>{},p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
            bind=p=>p.Bind(user,tables,language,format,canClick,sound,services,request,save,guide,()=>registry.Hide(28),hidden,schedule,next);
        }
        public OriginalWithdrawalLevelPanel Show(object[] args)=>registry.Show(28,p=>{bind(p);p.Init(args);});
        public void Refresh()=>registry.Refresh(28);
        public void Hide()=>registry.Hide(28);
    }
}
