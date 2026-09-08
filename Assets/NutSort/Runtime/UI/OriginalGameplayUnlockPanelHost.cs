using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    // Typed adapter for the original panel registration/Init/Refresh/Hide sequence.
    public sealed class OriginalGameplayUnlockPanelHost
    {
        private readonly OriginalPanelRegistry<OriginalGameplayUnlockPanel> registry;
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly string language;
        private readonly Func<bool> gate;
        private readonly Action<string> audio;
        private readonly Action<Action> banner;
        private readonly OriginalPanelActionQueue queue;
        private readonly Action<float,Action> schedule;
        private readonly Action hidden;
        public bool IsOpen=>registry.Contains(13);
        public OriginalGameplayUnlockPanelHost(Transform parent,string prefabPath,OriginalUserLocalData user,
            OriginalTables tables,string language,Func<bool> gate,Action<string> audio,Action<Action> banner,
            OriginalPanelActionQueue queue,Action<float,Action> schedule,Action hidden=null)
        {
            this.user=user;this.tables=tables;this.language=language;this.gate=gate;this.audio=audio;
            this.banner=banner;this.queue=queue;this.schedule=schedule;this.hidden=hidden;
            registry=new OriginalPanelRegistry<OriginalGameplayUnlockPanel>(id=>"UnlockGameplayPanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(prefabPath),parent,false).GetComponent<OriginalGameplayUnlockPanel>(),
                (id,p)=>{throw new InvalidOperationException("Unlock panel requires index and banner arguments.");},
                p=>p.Refresh(),p=>p.Hide(),p=>UnityEngine.Object.Destroy(p.gameObject));
        }
        public OriginalGameplayUnlockPanel Show(int index,bool showBanner)
        {
            return registry.Show(13,p=>
            {
                p.BindHide(queue,schedule,hidden);
                p.Initialize(user,index,showBanner,tables,language,gate,audio,banner,()=>registry.Hide(13));
            });
        }
        public void Refresh(){registry.Refresh(13);}
        public void Hide(){registry.Hide(13);}
    }
}
