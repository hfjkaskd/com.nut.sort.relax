using System;
using TMPro;
using UnityEngine;
namespace NutSort.UI
{
    // Tip.Update 0x9B6F74 / SetText 0x9B703C and UIMgr.ShowLssPopTip 0x9F8450.
    public sealed class OriginalPopTip : MonoBehaviour
    {
        [SerializeField] private TMP_Text content;
        [SerializeField] private float speed,lifetime;
        private Transform cached;
        public TMP_Text Content=>content;
        private void Awake(){cached=transform;}
        private void Update(){cached.localPosition+=Vector3.up*(Time.deltaTime*speed);}
        public void Show(string text,Action<float,Action> schedule)
        {
            content.text=text;
            schedule(lifetime,()=>Destroy(gameObject));
        }
    }
}
