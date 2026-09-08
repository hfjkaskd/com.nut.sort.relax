using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalHollowMaskGraphic : Graphic
    {
        [SerializeField] private float Radius=10f;
        [SerializeField] private int TriangleNum=6;
        [SerializeField] private RectTransform inner_trans;
        [SerializeField] private bool realtimeRefresh;
        [SerializeField] private bool ShowHollowOut=true;
        private readonly OriginalHollowMaskGeometry geometry=new OriginalHollowMaskGeometry();
        protected override void Awake(){base.Awake();geometry.CalculateBounds(rectTransform,inner_trans);}
        private void Update(){if(realtimeRefresh)RefreshBounds();}
        public void RefreshBounds(){geometry.CalculateBounds(rectTransform,inner_trans);SetVerticesDirty();}
        protected override void OnPopulateMesh(VertexHelper helper)
        {
            geometry.PopulateMesh(helper,color,ref Radius,ref TriangleNum,ShowHollowOut);
        }
    }
}
