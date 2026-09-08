using UnityEngine;

namespace NutSort.Content
{
    [CreateAssetMenu(menuName = "Nut Sort/Original table archive")]
    public sealed class OriginalTableSettings : ScriptableObject
    {
        public string ResourcePath;
        public string Password;
        public string Salt;
        public string IV;
    }
}
