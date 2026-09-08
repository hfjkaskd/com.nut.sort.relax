using System;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalScenePlayValidation
    {
        private const string ActiveKey="NutSort.ScenePlayValidation";
        private static OriginalGameScene game;
        private static int phase;
        private static double deadline, timeout;
        static OriginalScenePlayValidation()
        {
            if(SessionState.GetBool(ActiveKey,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}
        }
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(ActiveKey,true);
            EditorApplication.EnterPlaymode();
        }
        public static void MigrateBackgroundAndRun()
        {
            var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/game/Scene.mat");
            Texture texture=material.GetTexture("_MainTex");
            Vector2 scale=material.GetTextureScale("_MainTex"),offset=material.GetTextureOffset("_MainTex");
            material.shader=Shader.Find("Universal Render Pipeline/Unlit");
            material.SetTexture("_BaseMap",texture);
            material.SetTextureScale("_BaseMap",scale);material.SetTextureOffset("_BaseMap",offset);
            material.SetColor("_BaseColor",Color.white);
            EditorUtility.SetDirty(material);AssetDatabase.SaveAssets();
            Run();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                if(EditorApplication.timeSinceStartup>timeout)throw new Exception("Play mode scene validation timeout.");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||!game.Level.AreNutsInitialized)return;
                if(phase==0){phase=1;deadline=EditorApplication.timeSinceStartup+2;return;}
                if(EditorApplication.timeSinceStartup<deadline)return;
                if(phase==1)
                {
                    game.ResizeCameras(480,854);
                    foreach(Renderer renderer in game.GetComponentsInChildren<Renderer>())
                        if(renderer.name=="BG")Debug.Log("NUT_BG_DIAGNOSTIC shader="+renderer.sharedMaterial.shader.name+" supported="+renderer.sharedMaterial.shader.isSupported+" bounds="+renderer.bounds+" texture="+renderer.sharedMaterial.mainTexture);
                    OriginalSceneValidation.Capture(game.WorldCamera,"play-first-board.png");
                    Vector2 point=game.WorldCamera.WorldToScreenPoint(game.Level.GetScrew(1).Bounds.bounds.center);
                    if(game.TryOperateAtScreenPoint(point).Kind!=ScrewOperationKind.Ready)throw new Exception("Play mode source ray failed.");
                    phase=2;deadline=EditorApplication.timeSinceStartup+.3;
                }
                else if(phase==2)
                {
                    Vector2 point=game.WorldCamera.WorldToScreenPoint(game.Level.GetScrew(0).Bounds.bounds.center);
                    if(game.TryOperateAtScreenPoint(point).Kind!=ScrewOperationKind.Moved)throw new Exception("Play mode target ray failed.");
                    phase=3;deadline=EditorApplication.timeSinceStartup+1.5;
                }
                else
                {
                    if(!game.Level.Board.IsSuccess||!game.Level.Board.Screws[0].IsCanOperator)throw new Exception("Runtime animation callbacks did not finish the board.");
                    OriginalSceneValidation.Capture(game.WorldCamera,"play-first-board-complete.png");
                    Debug.Log("NUT_SCENE_PLAY_VALIDATION_PASS automatic Start, scaled runtime updates, camera-ray selection/transfer and actual animation completion verified in Play mode.");
                    Finish(0);
                }
            }
            catch(Exception exception){Debug.LogException(exception);Finish(1);}
        }
        private static void Finish(int code)
        {
            SessionState.SetBool(ActiveKey,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);
        }
    }
}
