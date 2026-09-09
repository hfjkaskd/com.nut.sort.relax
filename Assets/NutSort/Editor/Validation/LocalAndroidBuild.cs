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
        public static void Run()=>Build("com.nut.sort.relax","Nut Sort Relax","Builds/Android/NutSortRelax-local.apk",false);
        // Separate app sandbox protects the installed reference app and its saves.
        // Only package identity differs; scene, runtime, assets and build options are shared.
        public static void RunDeviceTest()=>Build("com.nut.sort.relax.localtest","Nut Sort Relax Local Test","Builds/Android/NutSortRelax-device-test.apk",true);
        private static void Build(string package,string label,string output,bool restorePackage)
        {
            string editorProductName=PlayerSettings.productName;
            string originalPackage=PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
            int exitCode=0;
            try
            {
                // Set this artifact label; restore the Editor product name
                // afterwards to preserve its existing PlayerPrefs namespace.
                PlayerSettings.productName=label;
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,package);
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
                Debug.Log("NUT_ANDROID_BUILD_START package="+package+" version=1.0.4 code=4 minSdk=23 targetSdk=36 ARM64 IL2CPP development; default scene and local SDK-skip composition.");
                var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes=new[]{"Assets/Scenes/LuoSiSortGame.unity"},
                    locationPathName=output,
                    target=BuildTarget.Android,
                    options=BuildOptions.Development
                });
                if(report.summary.result!=BuildResult.Succeeded||report.summary.totalErrors!=0)
                    throw new InvalidOperationException("Android build did not succeed; errors="+report.summary.totalErrors);
                Debug.Log("NUT_ANDROID_BUILD_PASS bytes="+report.summary.totalSize+" warnings="+report.summary.totalWarnings+" errors="+report.summary.totalErrors);
            }
            catch(Exception e){Debug.LogException(e);exitCode=1;}
            finally
            {
                PlayerSettings.productName=editorProductName;
                if(restorePackage)PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,originalPackage);
                AssetDatabase.SaveAssets();
            }
            EditorApplication.Exit(exitCode);
        }
    }
}
