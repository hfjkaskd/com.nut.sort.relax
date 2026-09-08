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
        [SerializeField] private GameObject Mask;
        [SerializeField] private GameObject DontMove;
        [SerializeField] private GameObject DontMoveSpine;
        [SerializeField] private GameObject Hidden;
        [SerializeField] private GameObject HiddenSpine;
        [SerializeField] private GameObject Locked;
        [SerializeField] private GameObject Locked1;
        private readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        private float hiddenBreakDelay, hiddenHideRemaining;
        private bool pendingHiddenHide;
        // The three source skeletal effects still require native animation assets.
        // Keep their original targets and clip requests explicit until converted.
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
                if (!screw.IsDone) AnimationRequested?.Invoke(DontMoveSpine, "animation", true);
            }
            else if (mask.Type == ScrewType.Hidden)
            {
                Hidden.SetActive(mask.IsShow);
                if (mask.IsShow) AnimationRequested?.Invoke(HiddenSpine, "wanzheng", true);
            }
        }

        public void PlayHiddenBreak()
        {
            // Original PlayHiddenTween requests non-looping "posui" then
            // SetGameObjectLSSActive(false, 1). Its DelayedCall ignores timeScale.
            AnimationRequested?.Invoke(HiddenSpine, "posui", false);
            if (!Hidden.activeSelf) return;
            if (hiddenBreakDelay <= 0f) { Hidden.SetActive(false); return; }
            if (!pendingHiddenHide || hiddenBreakDelay < hiddenHideRemaining)
                hiddenHideRemaining = hiddenBreakDelay;
            pendingHiddenHide = true;
        }

        private void Update() { AdvanceHiddenBreak(Time.unscaledDeltaTime); }
        public void AdvanceHiddenBreak(float unscaledDeltaTime)
        {
            if (unscaledDeltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(unscaledDeltaTime));
            if (!pendingHiddenHide) return;
            hiddenHideRemaining -= unscaledDeltaTime;
            if (hiddenHideRemaining > 0f) return;
            pendingHiddenHide = false;
            Hidden.SetActive(false);
        }

        public void Release() { pendingHiddenHide = false; AnimationRequested = null; }
    }
}
