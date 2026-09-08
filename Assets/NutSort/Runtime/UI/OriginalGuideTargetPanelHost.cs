using System;
using NutSort.Content;
using NutSort.World;
using UnityEngine;
namespace NutSort.UI
{
    // Typed adapter for the original panel registration/Init/Refresh/Hide sequence.
    public sealed class OriginalGuideTargetPanelHost
    {
        private readonly OriginalPanelRegistry<OriginalGuideTargetPanel> registry;
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly string language;
        private readonly Func<bool> gate;
        private readonly Action<string> audio;
        private readonly Func<string> country;
        private readonly Func<float,string> formatGold;
        private readonly Action<bool,bool> initializeDone;
        private readonly OriginalPanelActionQueue queue;
        private readonly Action<float,Action> schedule;
        private readonly Action hidden;
        public bool IsOpen=>registry.Contains(35);
        public OriginalGuideTargetPanel Panel=>IsOpen ? registry.Get(35) : null;
        public OriginalGuideTargetPanelHost(Transform parent,string prefabPath,OriginalUserLocalData user,
            OriginalTables tables,string language,Func<bool> gate,Action<string> audio,Func<string> country,Func<float,string> formatGold,Action<bool,bool> initializeDone,
            OriginalPanelActionQueue queue,Action<float,Action> schedule,Action hidden=null)
        {
            this.user=user;this.tables=tables;this.language=language;this.gate=gate;this.audio=audio;
            this.country=country;this.formatGold=formatGold;this.initializeDone=initializeDone;this.queue=queue;this.schedule=schedule;this.hidden=hidden;
            registry=new OriginalPanelRegistry<OriginalGuideTargetPanel>(id=>"TXGuideTargetCompletePanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(prefabPath),parent,false).GetComponent<OriginalGuideTargetPanel>(),
                (id,p)=>{throw new InvalidOperationException("Target completion panel requires a level argument.");},
                p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
        }
        public OriginalGuideTargetPanel Show(int level)
        {
            return registry.Show(35,p=>
            {
                p.BindHide(queue,schedule,hidden);
                p.Initialize(user,level,tables,language,country,formatGold,gate,audio,initializeDone,()=>registry.Hide(35));
            });
        }
        public void Refresh(){registry.Refresh(35);}
        public void Hide(){registry.Hide(35);}
    }
}
