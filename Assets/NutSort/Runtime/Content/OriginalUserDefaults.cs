using UnityEngine;
namespace NutSort.Content
{
    [CreateAssetMenu(menuName="Nut Sort/Original user defaults")]
    public sealed class OriginalUserDefaults : ScriptableObject
    {
        public int Level, LevelId, RevokeCount;
        public bool IsAudio, IsVibrate;
    }
}
