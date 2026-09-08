using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NutSort.Validation
{
    public static class OriginalRenderingValidation
    {
        [Serializable] private sealed class MeshEntry { public string path; public int vertices; }
        [Serializable] private sealed class MaterialEntry
        {
            public string path;
            public Color baseColor;
            public float smoothness, metallic, surface;
        }
        [Serializable] private sealed class Expected { public MeshEntry[] meshes; public MaterialEntry[] materials; }

        public static void Validate()
        {
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/MonoBehaviour/URP-Balanced.asset");
            Check(pipeline != null && GraphicsSettings.defaultRenderPipeline == pipeline, "Original pipeline is selected");
            Check(!pipeline.supportsHDR && pipeline.msaaSampleCount == 1 && pipeline.renderScale == 1f,
                "Original HDR, MSAA and render scale");
            Check(!pipeline.supportsMainLightShadows && !pipeline.supportsAdditionalLightShadows && pipeline.useSRPBatcher,
                "Original shadows and batching");
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/MonoBehaviour/URP-Balanced-Renderer.asset");
            Check(renderer != null && renderer.opaqueLayerMask.value == 64 && renderer.transparentLayerMask.value == -1,
                "Original world rendering layers");
            Check(renderer.rendererFeatures.Count == 1 && renderer.rendererFeatures[0] != null &&
                !renderer.rendererFeatures[0].isActive, "Original SSAO remains inactive");
            var expected = JsonUtility.FromJson<Expected>(File.ReadAllText(
                Path.Combine(Application.dataPath, "NutSort/Editor/Validation/rendering-expectations.json")));
            Check(expected.meshes.Length == 5 && expected.materials.Length == 19, "Original world resource coverage");
            foreach (MeshEntry entry in expected.meshes)
            {
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(entry.path);
                Check(mesh != null && mesh.vertexCount == entry.vertices, "Original mesh vertices: " + entry.path);
            }
            foreach (MaterialEntry entry in expected.materials)
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(entry.path);
                Check(material != null && material.shader != null && material.shader.name == "Universal Render Pipeline/Lit",
                    "Official Lit shader: " + entry.path);
                Check(AssetDatabase.GetAssetPath(material.shader).StartsWith("Packages/com.unity.render-pipelines.universal/", StringComparison.Ordinal),
                    "Material must use official shader, not exported placeholder");
                Color actual = material.GetColor("_BaseColor");
                Check(Math.Abs(actual.r - entry.baseColor.r) < 0.00001f && Math.Abs(actual.g - entry.baseColor.g) < 0.00001f &&
                    Math.Abs(actual.b - entry.baseColor.b) < 0.00001f && Math.Abs(actual.a - entry.baseColor.a) < 0.00001f,
                    "Original material base color: " + entry.path);
                Check(Math.Abs(material.GetFloat("_Smoothness") - entry.smoothness) < 0.00001f &&
                    Math.Abs(material.GetFloat("_Metallic") - entry.metallic) < 0.00001f &&
                    material.GetFloat("_Surface") == entry.surface, "Original surface parameters: " + entry.path);
            }
            foreach (string name in new[] { "Nut_Stand_Color", "Nut_Stand_Normal_03", "Hide" })
                Check(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Texture2D/" + name + ".png") != null, "Original texture loads");
            Debug.Log("NUT_RENDER_ASSET_VALIDATION_PASS pipeline/layers/disabled SSAO, 5 original meshes, 19 original materials and 3 textures verified; rendered scene fidelity remains pending.");
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }
    }
}
