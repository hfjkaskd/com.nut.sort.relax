using UnityEngine;
namespace NutSort.World
{
    public sealed partial class OriginalScrewView
    {
        private GameObject guide;
        public GameObject GuideInstance=>guide;
        public void ClearGuide()
        {
            if(guide==null)return;
            Destroy(guide);guide=null;
        }
        public void SetGuide(int type)
        {
            if(type<0||type>2)return;
            var prefab=Resources.Load<GameObject>(settings.GuidePaths[type]);
            if(prefab==null)throw new System.InvalidOperationException("Missing original world guide prefab");
            guide=Instantiate(prefab,transform,false);
            if(type!=0)guide.transform.localPosition=new Vector3(0,ReadyPos.localPosition.y+settings.GuideHeightOffset,0);
        }
    }
}
