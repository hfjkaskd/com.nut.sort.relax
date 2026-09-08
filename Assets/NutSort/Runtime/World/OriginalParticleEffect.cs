using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalParticleEffect : MonoBehaviour
    {
        // Explicit prefab references preserve source hierarchy and avoid a
        // hierarchy scan and array allocation on each landing.
        [SerializeField] private ParticleSystem[] systems;
        [SerializeField] private ParticleSystem[] coloredSystems;

        public void PlayDone(Color color)
        {
            for (int i = 0; i < systems.Length; i++) systems[i].Play();
            for (int i = 0; i < coloredSystems.Length; i++)
            {
                var main = coloredSystems[i].main;
                main.startColor = color;
            }
        }
    }
}
