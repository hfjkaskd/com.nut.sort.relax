using System;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class LocalSubroundViewBuilder
    {
        public static void Run()
        {
            try
            {
                var instance=(GameObject)PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Prefabs/Panels/HiddenLevel"));
                try
                {
                    instance.transform.Find("Progress").gameObject.SetActive(false);
                    instance.transform.Find("Levels").gameObject.SetActive(false);
                    PrefabUtility.SaveAsPrefabAsset(instance,"Assets/Resources/prefabs/panels/LocalSubrounds.prefab");
                }
                finally{UnityEngine.Object.DestroyImmediate(instance);}
                const string localPath="Assets/Resources/prefabs/panels/LocalGameplay.prefab";
                var local=PrefabUtility.LoadPrefabContents(localPath);
                try
                {
                    var serialized=new SerializedObject(local.GetComponent<NutSort.UI.LocalGameplayController>());
                    serialized.FindProperty("subroundPrefabPath").stringValue="Prefabs/Panels/LocalSubrounds";
                    serialized.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.SaveAsPrefabAsset(local,localPath);
                }
                finally{PrefabUtility.UnloadPrefabContents(local);}
                AssetDatabase.SaveAssets();Debug.Log("NUT_LOCAL_SUBROUND_VIEW_BUILD_PASS original HiddenLevel variant retains subround layout and hides reward-dependent milestones.");EditorApplication.Exit(0);
            }
            catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
        }
    }
}
