using System;
using NutSort.Gameplay;
using NutSort.World;
using NutSort.UI;
using System.IO;
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
        private static bool loadingCaptured;
        private static int sparkCount, doneCount;
        private static bool effectsCaptured;
        private static OriginalGameplayEffects effects;
        static OriginalScenePlayValidation()
        {
            if(SessionState.GetBool(ActiveKey,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}
        }
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,854,"Nut Sort portrait validation");
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
                if(!loadingCaptured)
                {
                    var flow=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    if(flow!=null && flow.Loading.IsVisible && flow.Loading.Value>=.25f)
                    {
                        string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures/loading-frame.png"));
                        Directory.CreateDirectory(Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);
                        loadingCaptured=true;Debug.Log("NUT_LOADING_CAPTURE "+path);
                    }
                }
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||!game.Level.AreNutsInitialized)return;
                if(phase==0)
                {
                    if(game.Tables==null || game.Tables.LevelCount!=51 || game.ShowLevel!=1)throw new Exception("Startup did not bind the original stage table.");
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    if(startup.MainLevel==null || startup.MainLevel.Group.gameObject.activeSelf || startup.MainLevel.transform.parent.name!="UICanvas")throw new Exception("Main level prefab startup or first-level visibility differs.");
                    effects=game.GetComponent<OriginalGameplayEffects>();
                    if(effects==null)throw new Exception("Gameplay effects component missing.");
                    game.Level.SparkRequested += nut => sparkCount++;
                    game.Level.DoneEffectRequested += screw => doneCount++;
                    phase=1;deadline=EditorApplication.timeSinceStartup+2;return;
                }
                if(phase==3 && doneCount>0 && !effectsCaptured)
                {
                    OriginalSceneValidation.Capture(game.WorldCamera,"play-completion-effects.png");
                    effectsCaptured=true;
                }
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
                else if(phase==3)
                {
                    if(!game.Level.Board.IsSuccess||!game.Level.Board.Screws[0].IsCanOperator)throw new Exception("Runtime animation callbacks did not finish the board.");
                    if(sparkCount!=1 || doneCount!=1 || effects.ActiveCount==0)throw new Exception("Original landing/completion effect chain failed: sparks="+sparkCount+", done="+doneCount+", active="+effects.ActiveCount);
                    OriginalSceneValidation.Capture(game.WorldCamera,"play-first-board-complete.png");
                    // Component preview only: the underlying completed first
                    // board is unchanged; this is not a level-five playthrough.
                    var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    startup.MainLevel.Refresh(game.Tables,5,"en",false,false);
                    string preview=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures/main-level-component-preview.png"));
                    ScreenCapture.CaptureScreenshot(preview);
                    Debug.Log("NUT_MAIN_LEVEL_COMPONENT_CAPTURE "+preview);
                    phase=4;deadline=EditorApplication.timeSinceStartup+2.5;
                }
                else
                {
                    if(effects.ActiveCount!=0)throw new Exception("Effects did not expire.");
                    Transform parent=game.Level.GetScrew(0).transform;
                    GameObject first=effects.PlaySpark(parent);
                    first.transform.localRotation=Quaternion.Euler(15f,30f,45f);
                    effects.Advance(0f);
                    if(!first.activeSelf)throw new Exception("Paused time expired a spark.");
                    effects.Advance(1.49f);
                    if(!first.activeSelf)throw new Exception("Spark expired early.");
                    effects.Advance(.011f);
                    if(first.activeSelf)throw new Exception("Spark failed to return to pool.");
                    GameObject reused=effects.PlaySpark(parent);
                    if(reused!=first)throw new Exception("Spark was not reused.");
                    if(Quaternion.Angle(reused.transform.localRotation,Quaternion.identity)>.001f)throw new Exception("Pooled spark rotation was not reset.");
                    game.Level.Clear();
                    if(reused.activeSelf || effects.ActiveCount!=0)throw new Exception("Effect cleanup failed.");
                    Debug.Log("NUT_EFFECTS_PLAY_VALIDATION_PASS original first-board landing spark and colored completion, timed cleanup, pause and spark reuse with rotation reset.");
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
