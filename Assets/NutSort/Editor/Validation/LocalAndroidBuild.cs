using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace NutSort.Validation
{
    // Official Unity Android toolchain; SDK/ads/account plugins remain excluded.
    public static class LocalAndroidBuild
    {
        public static void Run()
        {
            string editorProductName=PlayerSettings.productName;
            int exitCode=0;
            try
            {
                // APK label matches the source; restore the Editor product name
                // afterwards to preserve its existing PlayerPrefs namespace.
                PlayerSettings.productName="Nut Sort Relax";
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"com.nut.sort.relax");
                PlayerSettings.bundleVersion="1.0.4";
                PlayerSettings.Android.bundleVersionCode=4;
                PlayerSettings.Android.minSdkVersion=(AndroidSdkVersions)23;
                PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)36;
                PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
                EditorUserBuildSettings.buildAppBundle=false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject=false;
                AssetDatabase.SaveAssets();
                Directory.CreateDirectory("Builds/Android");
                Debug.Log("NUT_ANDROID_BUILD_START package=com.nut.sort.relax version=1.0.4 code=4 minSdk=23 targetSdk=36 ARM64 IL2CPP development; default scene and local SDK-skip composition.");
                var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes=new[]{"Assets/Scenes/LuoSiSortGame.unity"},
                    locationPathName="Builds/Android/NutSortRelax-local.apk",
                    target=BuildTarget.Android,
                    options=BuildOptions.Development
                });
                if(report.summary.result!=BuildResult.Succeeded||report.summary.totalErrors!=0)
                    throw new InvalidOperationException("Android build did not succeed; errors="+report.summary.totalErrors);
                Debug.Log("NUT_ANDROID_BUILD_PASS bytes="+report.summary.totalSize+" warnings="+report.summary.totalWarnings+" errors="+report.summary.totalErrors);
            }
            catch(Exception e){Debug.LogException(e);exitCode=1;}
            finally{PlayerSettings.productName=editorProductName;AssetDatabase.SaveAssets();}
            EditorApplication.Exit(exitCode);
        }
    }
}
