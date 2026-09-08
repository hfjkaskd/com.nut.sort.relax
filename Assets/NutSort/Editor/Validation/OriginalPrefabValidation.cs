using System;
using System.IO;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalPrefabValidation
    {
        public static void Validate()
        {
            var settings = Resources.Load<OriginalWorldSettings>("Configuration/OriginalWorld");
            Check(settings != null, "Original world configuration loads");
            for (int color = 0; color < 12; color++)
            {
                var prefab = Resources.Load<GameObject>(settings.NutPath(color));
                ValidatePrefab(prefab);
                var view = prefab.GetComponent<OriginalNutView>();
                Check(view != null && view.Renderer != null && view.ColorRoot != null, "Original nut component references");
                Check(view.Renderer.sharedMaterial.name == "NutMaterial" + (color + 1), "Original color-to-prefab mapping");
            }
            ValidatePrefab(Resources.Load<GameObject>(settings.HiddenNutPath));
            ValidatePrefab(Resources.Load<GameObject>(settings.ScrewTilePath));

            var holder = new GameObject("Original prefab validation");
            try
            {
                var pool = holder.AddComponent<OriginalPrefabPool>();
                GameObject nut = pool.Rent(settings.NutPath(11), holder.transform);
                var view = nut.GetComponent<OriginalNutView>();
                var slot = new NutSlot(Vector3Int.zero) { Nut = new NutState { Type = NutType.Hidden, Color = 11, V = true } };
                view.BindVisual(slot, settings, pool);
                GameObject hidden = view.HiddenInstance;
                Check(hidden != null && !view.ColorRoot.gameObject.activeSelf, "Hidden prefab replaces visible color root");
                Check(hidden.transform.parent == nut.transform && hidden.transform.localPosition == Vector3.zero &&
                    hidden.transform.localScale == Vector3.one, "Original hidden attachment coordinates");
                slot.Nut.Type = NutType.Normal;
                view.RefreshVisual();
                Check(view.HiddenInstance == null && view.ColorRoot.gameObject.activeSelf && !hidden.activeSelf,
                    "Revealing restores color and returns hidden view");
                slot.Nut.Type = NutType.Hidden;
                view.RefreshVisual();
                Check(view.HiddenInstance == hidden, "Hidden view is pooled and reused");
                view.ReleaseVisual();
                pool.Return(nut);
                Check(pool.Rent(settings.NutPath(11), holder.transform) == nut, "Original colored prefab is reused");

                var tile = pool.Rent(settings.ScrewTilePath, holder.transform).GetComponent<OriginalScrewTileView>();
                tile.Configure(3, 4, settings);
                Check(Math.Abs(tile.transform.localPosition.y - 1.08f) < 0.00001f && !tile.Shaft.activeSelf &&
                    tile.Tip.activeSelf && !tile.AddHint.activeSelf, "Four-slot top geometry and original 0.36 spacing");
                tile.Configure(1, 2, settings);
                Check(tile.Tip.activeSelf && tile.AddHint.activeSelf, "Short screw top exposes add hint");
                tile.Configure(0, 4, settings);
                Check(tile.Shaft.activeSelf && !tile.Tip.activeSelf && !tile.AddHint.activeSelf, "Lower screw uses shaft geometry");
                tile.Configure(4, 4, settings);
                Check(!tile.Shaft.activeSelf && !tile.Tip.activeSelf && !tile.AddHint.activeSelf, "Excess pooled tile geometry is hidden");
            }
            finally { UnityEngine.Object.DestroyImmediate(holder); }
            Debug.Log("NUT_PREFAB_VALIDATION_PASS 13 original nut prefabs and screw tile, complete native references, hidden reveal/reuse, exact tile geometry states verified.");
        }

        public static void RunGpu()
        {
            try
            {
                Validate();
                VerifyGrayPixels();
                EditorApplication.Exit(0);
            }
            catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
        }

        private static void ValidatePrefab(GameObject prefab)
        {
            Check(prefab != null, "Original prefab loads");
            foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true))
                Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject) == 0, "No missing prefab scripts");
            foreach (MeshFilter mesh in prefab.GetComponentsInChildren<MeshFilter>(true)) Check(mesh.sharedMesh != null, "Mesh dependency loads");
            foreach (SpriteRenderer sprite in prefab.GetComponentsInChildren<SpriteRenderer>(true)) Check(sprite.sprite != null, "Sprite dependency loads");
            foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>(true))
                foreach (Material material in renderer.sharedMaterials)
                    Check(material != null && material.shader != null && !ShaderUtil.ShaderHasError(material.shader), "Material shader compiles");
        }

        private static void VerifyGrayPixels()
        {
            Shader shader = Shader.Find("UI/UI Grey");
            Check(shader != null && shader.isSupported, "Recovered shader is supported on GPU");
            var material = new Material(shader);
            var source = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
            var readback = new Texture2D(4, 4, TextureFormat.RGBAFloat, false, true);
            var target = new RenderTexture(4, 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            RenderTexture previous = RenderTexture.active;
            try
            {
                target.Create();
                Color[] samples = { Color.red, Color.green, Color.blue, new Color(0.8f, 0.2f, 0.4f, 0.5f) };
                foreach (Color color in samples)
                    foreach (float grayness in new[] { 0f, 0.5f, 1f })
                    {
                        source.SetPixel(0, 0, color);
                        source.Apply();
                        material.SetFloat("_Grayness", grayness);
                        material.SetFloat("_Alpha", 0.6f);
                        RenderTexture.active = target;
                        GL.Clear(false, true, Color.clear);
                        Graphics.Blit(source, target, material);
                        readback.ReadPixels(new Rect(0, 0, 4, 4), 0, 0);
                        readback.Apply();
                        Color actual = readback.GetPixel(2, 2);
                        float gray = color.r * 0.298999995f + color.g * 0.587000012f + color.b * 0.114f;
                        float alpha = color.a * 0.6f;
                        Color expected = Color.LerpUnclamped(color, new Color(gray, gray, gray, color.a), grayness) * alpha;
                        expected.a = alpha * alpha; // original SrcAlpha blend applies to alpha as well
                        Check(Math.Abs(actual.r - expected.r) < 0.003f && Math.Abs(actual.g - expected.g) < 0.003f &&
                            Math.Abs(actual.b - expected.b) < 0.003f && Math.Abs(actual.a - expected.a) < 0.003f,
                            "GPU grayscale/alpha blend mismatch: " + actual + " expected " + expected);
                    }
            }
            finally
            {
                RenderTexture.active = previous;
                target.Release();
                UnityEngine.Object.DestroyImmediate(target);
                UnityEngine.Object.DestroyImmediate(readback);
                UnityEngine.Object.DestroyImmediate(source);
                UnityEngine.Object.DestroyImmediate(material);
            }
            Debug.Log("NUT_GRAY_GPU_VALIDATION_PASS 12 sampled colors/grayness combinations match original GLES formula and pass alpha blend.");
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }
    }
}
