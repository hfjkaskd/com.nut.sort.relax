using System;
using System.Collections.Generic;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalScrewTypeView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer MaskNut;
        [SerializeField] private SpriteRenderer MaskDone;
        [SerializeField] private GameObject MaskSpine;
        [SerializeField] private OriginalNativeWorldEffect maskEffect;
        [SerializeField] private GameObject Mask;
        [SerializeField] private GameObject DontMove;
        [SerializeField] private GameObject DontMoveSpine;
        [SerializeField] private OriginalNativeWorldEffect dontMoveEffect;
        [SerializeField] private GameObject Hidden;
        [SerializeField] private GameObject HiddenSpine;
        [SerializeField] private OriginalNativeWorldEffect hiddenEffect;
        [SerializeField] private GameObject Locked;
        [SerializeField] private GameObject Locked1;
        private readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        private float hiddenBreakDelay, hiddenHideRemaining;
        private OriginalScrewSettings settings;
        private struct MaskTrack { public float Elapsed; public Vector3 Start; }
        private readonly List<MaskTrack> maskTracks=new List<MaskTrack>();
        private readonly List<float> maskHides=new List<float>(),dontMoveHides=new List<float>();
        public Vector3 MaskDoneScale=>MaskDone.transform.localScale;
        public bool IsMaskBreakVisible=>MaskSpine.activeSelf;
        private bool pendingHiddenHide;
        // Native prefab consumers play the effects; this event is an observer.
        public event Action<GameObject, string, bool> AnimationRequested;
        public bool IsMaskVisible => Mask.activeSelf;
        public bool IsDontMoveVisible => DontMove.activeSelf;
        public bool IsHiddenVisible => Hidden.activeSelf;
        public bool IsShortLockVisible => Locked.activeSelf;
        public bool IsFullLockVisible => Locked1.activeSelf;
        public Sprite MaskSprite => MaskNut.sprite;

        public void Configure(ScrewMaskState mask, ScrewState screw, OriginalScrewSettings settings, bool lssab)
        {
            pendingHiddenHide = false;
            this.settings = settings;
            maskTracks.Clear();maskHides.Clear();dontMoveHides.Clear();
            hiddenBreakDelay = settings.HiddenBreakHideDelay;
            Mask.SetActive(false); DontMove.SetActive(false); Hidden.SetActive(false);
            Locked.SetActive(screw.IsLocked && lssab);
            Locked1.SetActive(screw.IsLocked && !lssab);
            if (mask == null) return;
            if (mask.Type == ScrewType.Mask && mask.Object != null)
            {
                Mask.SetActive(mask.Object.IsShow);
                MaskDone.transform.localScale = Vector3.zero;
                MaskSpine.SetActive(false);
                if (!mask.Object.IsShow) return;
                string path = settings.MaskNutPath(mask.Object.Color);
                if (!sprites.TryGetValue(path, out Sprite sprite))
                {
                    sprite = Resources.Load<Sprite>(path);
                    if (sprite == null) throw new InvalidOperationException("Original color mask sprite missing: " + path);
                    sprites.Add(path, sprite);
                }
                MaskNut.sprite = sprite;
            }
            else if (mask.Type == ScrewType.DontMove)
            {
                DontMove.SetActive(!screw.IsDone);
                if (!screw.IsDone)
                {
                    if(dontMoveEffect==null)throw new InvalidOperationException("Original fixed screw animation binding missing.");
                    dontMoveEffect.Play("animation",true);
                    AnimationRequested?.Invoke(DontMoveSpine,"animation",true);
                }
            }
            else if (mask.Type == ScrewType.Hidden)
            {
                Hidden.SetActive(mask.IsShow);
                if (mask.IsShow)
                {
                    if(hiddenEffect==null)throw new InvalidOperationException("Original hidden cover binding missing.");
                    hiddenEffect.Play("wanzheng",true);
                    AnimationRequested?.Invoke(HiddenSpine, "wanzheng", true);
                }
            }
        }

        public void RefreshLock(ScrewState screw,Func<bool> lssab)
        {
            Locked.SetActive(screw.IsLocked && lssab());
            Locked1.SetActive(screw.IsLocked && !lssab());
        }

        public void PlayHiddenBreak()
        {
            // Original PlayHiddenTween requests non-looping "posui" then
            // SetGameObjectLSSActive(false, 1). Its DelayedCall ignores timeScale.
            if(hiddenEffect==null)throw new InvalidOperationException("Original hidden cover binding missing.");
            hiddenEffect.Play("posui",false);
            AnimationRequested?.Invoke(HiddenSpine, "posui", false);
            if (!Hidden.activeSelf) return;
            if (hiddenBreakDelay <= 0f) { Hidden.SetActive(false); return; }
            if (!pendingHiddenHide || hiddenBreakDelay < hiddenHideRemaining)
                hiddenHideRemaining = hiddenBreakDelay;
            pendingHiddenHide = true;
        }

        public void PlayMaskBreak()
        {
            maskTracks.Add(new MaskTrack{Start=MaskDone.transform.localScale});
        }
        public void PlayDontMoveBreak()
        {
            if(dontMoveEffect==null)throw new InvalidOperationException("Original fixed screw animation binding missing.");
            dontMoveEffect.Play("animation2",false);
            AnimationRequested?.Invoke(DontMoveSpine,"animation2",false);
            if(DontMove.activeSelf)dontMoveHides.Add(settings.DontMoveBreakHideDelay);
        }
        // Host on the level, so disabling an individual rod does not pause its
        // scaled mask tween or the source's independent-time hide callbacks.
        public void AdvanceTransitions(float scaledDelta,float unscaledDelta)
        {
            if(scaledDelta<0||unscaledDelta<0)throw new ArgumentOutOfRangeException();
            AdvanceHides(maskHides,Mask,unscaledDelta);
            AdvanceHides(dontMoveHides,DontMove,unscaledDelta);
            for(int i=0;i<maskTracks.Count;)
            {
                var track=maskTracks[i];track.Elapsed+=scaledDelta;
                float t=Mathf.Clamp01(track.Elapsed/settings.MaskDoneScaleDuration)-1;
                float eased=1+t*t*((settings.MaskBackOvershoot+1)*t+settings.MaskBackOvershoot);
                MaskDone.transform.localScale=Vector3.LerpUnclamped(track.Start,Vector3.one,eased);
                if(track.Elapsed<settings.MaskDoneScaleDuration){maskTracks[i++]=track;continue;}
                maskTracks.RemoveAt(i);MaskSpine.SetActive(true);
                if(maskEffect==null)throw new InvalidOperationException("Original mask smoke prefab binding missing.");
                maskEffect.Play("animation",false);
                AnimationRequested?.Invoke(MaskSpine,"animation",false);
                if(Mask.activeSelf)maskHides.Add(settings.MaskBreakHideDelay);
            }
            AdvanceHiddenBreak(unscaledDelta);
        }
        private static void AdvanceHides(List<float> tracks,GameObject target,float delta)
        {
            for(int i=0;i<tracks.Count;)
            {
                float remaining=tracks[i]-delta;
                if(remaining>0){tracks[i++]=remaining;continue;}
                tracks.RemoveAt(i);target.SetActive(false);
            }
        }
        public void AdvanceHiddenBreak(float unscaledDeltaTime)
        {
            if (unscaledDeltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(unscaledDeltaTime));
            if (!pendingHiddenHide) return;
            hiddenHideRemaining -= unscaledDeltaTime;
            if (hiddenHideRemaining > 0f) return;
            pendingHiddenHide = false;
            Hidden.SetActive(false);
        }

        public void Release() { pendingHiddenHide = false; maskTracks.Clear();maskHides.Clear();dontMoveHides.Clear();AnimationRequested = null; }
    }
}
