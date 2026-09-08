using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.World;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGameplayUnlockConfigValidation
    {
        public static void Validate()
        {
            var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
            var user=new OriginalUserLocalData(defaults);
            var session=Resources.Load<OriginalSceneSession>("Configuration/OriginalSceneSession");
            Check(string.Join(",",session.GameplayUnlockLevels)=="8,21,61,111","Native metadata defaults");
            var reader=new OriginalGameplayUnlockConfig(user,session.GameplayUnlockLevels);
            bool caught=false;try{reader.Read();}catch(NullReferenceException){caught=true;}Check(caught,"Missing config object fails");
            user.ServerConfigData=new JObject();Check(string.Join(",",reader.Read())=="8,21,61,111","Missing field retains constructor default");
            user.ServerConfigData=JObject.Parse("{\"LSSGPUL\":[10],\"lssgpul\":[12,30]}");
            Check(string.Join(",",reader.Read())=="12,30","Last case-insensitive field and current values");
            user.ServerConfigData["lssgpul"]=new JArray();Check(reader.Read().Length==0,"Explicit empty remains empty");
            user.ServerConfigData["lssgpul"]=JValue.CreateNull();Check(reader.Read()==null,"Explicit null remains null");
            try
            {
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                OriginalGameScene game=null;
                foreach(var root in scene.GetRootGameObjects())if(root.TryGetComponent(out OriginalGameScene found)){game=found;break;}
                game.gameObject.SetActive(true);game.Initialize();
                var actual=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;
                actual.ServerConfigData=new JObject();actual.Level=4;actual.NewGameplayUnlockIndex=0;
                int calls=0;
                Check(game.NewGameplayUnlock(false,(id,index,banner)=>{Check(id==13 && index==0 && !banner,"Actual scene defaults route panel args");calls++;}),"Actual scene default unlock");
                actual.ServerConfigData=JObject.Parse("{\"LSSGPUL\":[20]}");
                Check(!game.NewGameplayUnlock(true,(id,index,banner)=>calls++),"Actual scene reads replacement server config");
                actual.Level=16;Check(game.NewGameplayUnlock(true,(id,index,banner)=>calls++ ) && calls==2,"Actual scene uses live real player level");
            }
            finally{EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);}
            Debug.Log("NUT_GAMEPLAY_UNLOCK_CONFIG_VALIDATION_PASS original serialized defaults, missing/null/empty distinction, field case/order and actual scene live configuration to unlock panel dispatch.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
