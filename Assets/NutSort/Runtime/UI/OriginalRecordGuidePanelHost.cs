using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalRecordGuidePanelHost
    {
        private readonly OriginalPanelRegistry<OriginalRecordGuidePanel> registry;
        public bool IsOpen => registry.Contains(34);
        public OriginalRecordGuidePanel Panel => IsOpen ? registry.Get(34) : null;
        public OriginalRecordGuidePanelHost(Transform parent,string path,OriginalTables tables,
            string language,string country,Func<bool> gate,Action started,Action click,
            OriginalPanelActionQueue queue,Action<float,Action> schedule,Action hidden)
        {
            registry=new OriginalPanelRegistry<OriginalRecordGuidePanel>(id=>"TXRecordGuidePanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path),parent,false).GetComponent<OriginalRecordGuidePanel>(),
                (id,p)=>
                {
                    p.BindHide(queue,schedule,hidden);
                    p.Initialize(tables,language,country,gate,started,()=>registry.Hide(34),click,false);
                },p=>p.Refresh(tables,language,country),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
        }
        public OriginalRecordGuidePanel Show()=>registry.Show(34);
        public void Close()=>Panel.Close();
        public void Hide()=>registry.Hide(34);
    }
}
