using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace NutSort.UI
{
    // Visual feedback only. Selection and activation remain on the standard Button.
    [RequireComponent(typeof(Button))]
    public sealed class OriginalButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private OriginalPanelSettings settings;
        private Vector3 initial;
        private float begin, end, elapsed, duration;
        private bool playing;
        private int startFrame;
        public void OnEnable() { initial=target.localScale; }
        public void OnPointerDown(PointerEventData data) { Begin(initial.x, initial.x*settings.ButtonDownScale, settings.ButtonDownDuration); }
        public void OnPointerUp(PointerEventData data) { Begin(target.localScale.x, initial.x, settings.ButtonUpDuration); }
        private void Begin(float from,float to,float seconds)
        {
            begin=from;end=to;duration=seconds;elapsed=0;playing=true;startFrame=Time.frameCount;
            AdvanceFrame(Time.fixedDeltaTime);
        }
        private void Update() { if(playing && Time.frameCount!=startFrame) AdvanceFrame(Time.fixedDeltaTime); }
        public void AdvanceFrame(float fixedStep)
        {
            if(!playing)return;
            if(elapsed>=duration) { target.localScale=Vector3.one*end;playing=false;return; }
            target.localScale=Vector3.one*Mathf.Lerp(begin,end,elapsed/duration);
            // Original coroutine yields each rendered frame but advances fixedDeltaTime.
            elapsed+=fixedStep;
        }
        private void OnDisable() { playing=false;target.localScale=initial; }
    }
}
