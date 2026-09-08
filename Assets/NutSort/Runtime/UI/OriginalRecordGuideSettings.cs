using UnityEngine;
namespace NutSort.UI
{
    [CreateAssetMenu(menuName="Nut Sort/Original record guide")]
    public sealed class OriginalRecordGuideSettings : ScriptableObject
    {
        public float StartDelay, StartDuration;
        public string GoldTexturePrefix;
    }
}
