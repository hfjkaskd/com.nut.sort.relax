using System;
using TMPro;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalDynamicFontAtlasBuilder
    {
        public static void Run()
        {
            try
            {
                var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Resources/fonts & materials/LiberationSans SDF - Fallback.asset");
                if(font==null||font.sourceFontFile==null||font.atlasPopulationMode!=AtlasPopulationMode.Dynamic)
                    throw new InvalidOperationException("Original dynamic fallback font/source is missing.");
                if(font.atlasTextures.Length==0||font.atlasTextures[0]==null)
                {
                    const string path="Assets/Resources/fonts & materials/LiberationSans Fallback Dynamic Atlas.asset";
                    var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if(texture==null)
                    {
                        // Matches TMP_FontAsset.CreateFontAsset's empty dynamic atlas.
                        texture=new Texture2D(0,0,TextureFormat.Alpha8,false){name="LiberationSans Fallback Dynamic Atlas"};
                        AssetDatabase.CreateAsset(texture,path);
                    }
                    font.atlasTextures=new[]{texture};font.material.mainTexture=texture;
                    EditorUtility.SetDirty(font);EditorUtility.SetDirty(font.material);AssetDatabase.SaveAssets();
                }
                Validate();Debug.Log("NUT_DYNAMIC_FONT_ATLAS_BUILD_PASS missing dynamic fallback atlas repaired using native TMP empty-atlas initialization; source font, packing and dynamic mode retained.");EditorApplication.Exit(0);
            }
            catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
        }
        public static void Validate()
        {
            foreach(string guid in AssetDatabase.FindAssets("t:TMP_FontAsset",new[]{"Assets"}))
            {
                var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guid));
                if(font.atlasTextures.Length==0||font.atlasTextures[0]==null)
                    throw new InvalidOperationException("Missing atlas: "+AssetDatabase.GetAssetPath(font));
            }
            var original=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Resources/fonts & materials/LiberationSans SDF - Fallback.asset");
            var copy=UnityEngine.Object.Instantiate(original);
            var atlas=new Texture2D(0,0,TextureFormat.Alpha8,false);var material=new Material(original.material);
            try
            {
                copy.atlasTextures=new[]{atlas};copy.material=material;material.mainTexture=atlas;
                if(!copy.TryAddCharacters("AΩЖ",out string missing)||missing.Length!=0||atlas.width!=512||atlas.height!=512)
                    throw new InvalidOperationException("Native dynamic fallback glyph packing failed.");
            }
            finally{UnityEngine.Object.DestroyImmediate(copy);UnityEngine.Object.DestroyImmediate(material);UnityEngine.Object.DestroyImmediate(atlas);}
            Debug.Log("NUT_DYNAMIC_FONT_ATLAS_VALIDATION_PASS all font atlas references and native dynamic Latin/Greek/Cyrillic glyph packing.");
        }
    }
}
