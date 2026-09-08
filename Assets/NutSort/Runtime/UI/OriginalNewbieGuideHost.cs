using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalNewbieGuideHost
    {
        private readonly OriginalPanelRegistry<OriginalNewbieGuideView> registry;
        private readonly OriginalUserLocalData user;
        private readonly OriginalTables tables;
        private readonly string language;
        private readonly Func<bool> gate,skipTeaching;
        private readonly Action clickSound;
        private readonly Action<bool> canOperate;
        private readonly Action<float,Action> schedule;
        private readonly IOriginalGuideUI ui;
        public bool IsOpen=>registry.Contains(7);
        public OriginalNewbieGuideView Panel=>IsOpen?registry.Get(7):null;
        public OriginalNewbieGuideHost(Transform parent,string prefabPath,OriginalUserLocalData user,
            OriginalTables tables,string language,Func<bool> gate,Action clickSound,Action<bool> canOperate,
            Func<bool> skipTeaching,Action<float,Action> schedule,IOriginalGuideUI ui)
        {
            this.user=user;this.tables=tables;this.language=language;this.gate=gate;this.clickSound=clickSound;
            this.canOperate=canOperate;this.skipTeaching=skipTeaching;this.schedule=schedule;this.ui=ui;
            registry=new OriginalPanelRegistry<OriginalNewbieGuideView>(id=>"NewbieGuidePanel",
                id=>UnityEngine.Object.Instantiate(Resources.Load<GameObject>(prefabPath),parent,false).GetComponent<OriginalNewbieGuideView>(),
                (id,view)=>Initialize(view,null),view=>view.ShowGuide(),
                view=>{}, // Native Hide only kills its optional, unassigned tweener; no base hide queue.
                view=>UnityEngine.Object.Destroy(view.gameObject));
        }
        private void Initialize(OriginalNewbieGuideView view,bool? showBanner)
        {
            view.BindTeaching(tables,language,canOperate,()=>registry.Hide(7));
            view.InitializeLabels(tables,language);
            view.InitializeInteractions(user,showBanner,gate,clickSound);
            view.BindMask(schedule);
            view.BindGuide(user,skipTeaching,new OriginalGuideBranchAdapter(view,user,ui));
        }
        public OriginalNewbieGuideView Show(bool? showBanner=null)=>registry.Show(7,view=>Initialize(view,showBanner));
        public void Refresh()=>registry.Refresh(7);
        public void Hide()=>registry.Hide(7);
    }
}
