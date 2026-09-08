using System;
using System.IO;
using NutSort.World;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalEffectsValidation
    {
        public static void Validate()
        {
            var settings = Resources.Load<OriginalEffectSettings>("Configuration/OriginalEffects");
            Require(settings != null && settings.SparkLifetime == 1.5f && settings.DoneLifetime == 2f, "Original effect lifetimes");
            string[] expected = { "#68A013", "#1E7BB8", "#B23D20", "#A87507", "#C53C69", "#827A8C", "#A86859", "#373434", "#B31A1F", "#247062", "#6036C8", "#600E62" };
            Require(settings.NutColors.Length == expected.Length, "All original effect colors");
            for (int i = 0; i < expected.Length; i++)
            {
                ColorUtility.TryParseHtmlString(expected[i], out Color color);
                Require(settings.GetColor(i) == color, "Effect palette index " + i);
            }
            Require(settings.GetColor(-1) == settings.GetColor(0) && settings.GetColor(12) == settings.GetColor(0), "Original fallback color zero");
            var spark = Resources.Load<GameObject>(settings.SparkPath);
            var done = Resources.Load<GameObject>(settings.DonePath);
            Require(spark != null && done != null, "Both original Resources paths");
            Require(spark.GetComponentsInChildren<ParticleSystem>().Length == 2, "Two landing systems");
            var instance = UnityEngine.Object.Instantiate(done);
            try
            {
                var systems = instance.GetComponentsInChildren<ParticleSystem>();
                Require(systems.Length == 4, "Four completion systems");
                var original = new ParticleSystem.MinMaxGradient[systems.Length];
                for (int i = 0; i < systems.Length; i++) original[i] = systems[i].main.startColor;
                instance.GetComponent<OriginalParticleEffect>().PlayDone(settings.GetColor(11));
                for (int i = 0; i < systems.Length; i++)
                {
                    bool excluded = systems[i].name == "CenterGlow" || systems[i].name == "MagicDust";
                    Require(systems[i].main.startColor.color == (excluded ? original[i].color : settings.GetColor(11)), "Tint exclusion " + systems[i].name);
                    var renderer = systems[i].GetComponent<ParticleSystemRenderer>();
                    Require(renderer.sharedMaterial != null && renderer.sharedMaterial.shader != null, "Particle material resolves");
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("NUT_EFFECTS_VALIDATION_PASS original 2/4 particle systems, palette, tint exclusions, paths and durations.");
        }
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
    }
}
