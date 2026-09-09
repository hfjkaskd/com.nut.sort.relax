using System;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalBackgroundTextureValidation
    {
        private const string PathName="Assets/Resources/game/spirte/bg.png";
        public static void Repair()
        {
            try
            {
                Measure("before");
                var importer=(TextureImporter)AssetImporter.GetAtPath(PathName);
                var android=importer.GetPlatformTextureSettings("Android");
                android.overridden=true;android.format=TextureImporterFormat.ASTC_10x10;
                android.maxTextureSize=2048;android.compressionQuality=50;
                importer.SetPlatformTextureSettings(android);importer.SaveAndReimport();
                Validate();Measure("after");EditorApplication.Exit(0);
            }
            catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
        }
        public static void Validate()
        {
            using(var sha=SHA256.Create())
            {
                string hash=BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(PathName))).Replace("-","").ToLowerInvariant();
                Require(hash=="9b02daa80d98a75a07b2b19362639387e850fd808443eb10eac64eef231c6073","Source background PNG hash");
            }
            var importer=(TextureImporter)AssetImporter.GetAtPath(PathName);
            var android=importer.GetPlatformTextureSettings("Android");
            Require(android.overridden&&android.format==TextureImporterFormat.ASTC_10x10&&android.maxTextureSize==2048,"Source Android ASTC 10x10 format and dimensions");
            Require(!importer.mipmapEnabled&&!importer.isReadable&&importer.sRGBTexture&&importer.filterMode==FilterMode.Bilinear&&importer.wrapMode==TextureWrapMode.Repeat,"Source background sampling and residency");
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(PathName);
            Require(texture.width==1024&&texture.height==2048&&texture.mipmapCount==1,"Source full-resolution background");
            if(EditorUserBuildSettings.activeBuildTarget==BuildTarget.Android)
                Require((int)texture.format==52,"Original Android compressed format");
            Debug.Log("NUT_BACKGROUND_TEXTURE_VALIDATION_PASS original decoded PNG unchanged; explicit Android ASTC 10x10, 1024x2048, single mip, sRGB, bilinear/repeat and unreadable CPU copy.");
        }
        private static void Measure(string label)
        {
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(PathName);
            var source=new Texture2D(2,2,TextureFormat.RGBA32,false);
            try
            {
                source.LoadImage(File.ReadAllBytes(PathName));
                var actual=ReadPixels(texture);var expected=ReadPixels(source);
                long absolute=0;int worst=0;
                for(int i=0;i<actual.Length;i++)
                {
                    int r=Math.Abs(actual[i].r-expected[i].r),g=Math.Abs(actual[i].g-expected[i].g),b=Math.Abs(actual[i].b-expected[i].b);
                    absolute+=r+g+b;worst=Math.Max(worst,Math.Max(r,Math.Max(g,b)));
                }
                Debug.Log("NUT_BACKGROUND_MEASURE "+label+" format="+texture.format+" meanRGB="+(absolute/(double)(actual.Length*3)).ToString("F6",System.Globalization.CultureInfo.InvariantCulture)+" worstChannel="+worst);
            }
            finally{UnityEngine.Object.DestroyImmediate(source);}
        }
        private static Color32[] ReadPixels(Texture2D source)
        {
            var previous=RenderTexture.active;bool previousWrite=GL.sRGBWrite;
            var target=RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);
            var copy=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false,true);
            try
            {
                GL.sRGBWrite=false;Graphics.Blit(source,target);RenderTexture.active=target;
                copy.ReadPixels(new Rect(0,0,source.width,source.height),0,0);copy.Apply();return copy.GetPixels32();
            }
            finally{RenderTexture.active=previous;GL.sRGBWrite=previousWrite;RenderTexture.ReleaseTemporary(target);UnityEngine.Object.DestroyImmediate(copy);}
        }
        private static void Require(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
