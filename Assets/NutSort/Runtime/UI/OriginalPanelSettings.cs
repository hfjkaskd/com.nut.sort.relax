using UnityEngine;
namespace NutSort.UI
{
    [CreateAssetMenu(menuName="Nut Sort/Original panel motion")]
    public sealed class OriginalPanelSettings : ScriptableObject
    {
        public float PanelDurationTime;
        public AnimationCurve PanelOpenCurve;
        public float TitleDelayTime, TitleDurationTime;
        public AnimationCurve TitleCurve;
        public float CloseDuration, BackOvershoot, BackdropAlpha, ClickMaskDuration;
        public float ButtonDownScale, ButtonDownDuration, ButtonUpDuration;
        public string ClickSound, ReplayPath;
    }
}
