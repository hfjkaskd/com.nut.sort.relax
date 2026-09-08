using System;
using System.IO;
using NutSort.Gameplay;
using NutSort.World;
using NutSort.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NutSort.Validation
{
    public static class OriginalSceneValidation
    {
        public static void Validate(bool capture = false)
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            OriginalGameScene game = null;
            OriginalStartupFlow startup = null;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.TryGetComponent(out OriginalGameScene candidate)) game = candidate;
                if (root.TryGetComponent(out OriginalStartupFlow flow)) startup = flow;
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                    Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject)==0,"Missing scene script: "+child.name);
            }
            Check(game != null,"Native startup component exists in source scene");
            Check(startup != null,"Source UI startup root exists");
            OriginalLoadingValidation.Validate(startup.Loading);
            game.gameObject.SetActive(true);
            Check(Mathf.Abs(RenderSettings.ambientIntensity-5f)<.0001f,"Original ambient intensity");
            Check(LightmapSettings.lightmaps.Length==0,"Source has no baked lightmap textures");
            game.Initialize();
            game.ResizeCameras(480,854);
            var level=game.Level;
            level.AdvanceInitialization(1.501f);
            Check(level.NutViewCount==4,"Configured original first level loads through scene startup");
            foreach (ScrewState screw in level.Board.Screws)
                foreach (NutSlot slot in screw.Slots)
                    if (slot.Nut!=null) level.GetNut(slot.Nut).AdvanceMotion(2f);
            Check(game.WorldCamera.orthographic && game.WorldCamera.cullingMask==64,"Original camera mode and world layer");
            var stack=game.WorldCamera.GetUniversalAdditionalCameraData().cameraStack;
            Check(stack.Count==2 && stack[0]!=null && stack[1]!=null,"Original camera stack references");
            foreach(Renderer renderer in game.GetComponentsInChildren<Renderer>(true))
                foreach(Material material in renderer.sharedMaterials)
                    Check(material!=null && material.shader!=null && !ShaderUtil.ShaderHasError(material.shader),"Scene material shader references");
            Physics.SyncTransforms();
            if(capture) Capture(game.WorldCamera,"scene-first-board.png");
            Vector3 sourcePoint=level.GetScrew(1).Bounds.bounds.center;
            Vector2 sourceScreen=game.WorldCamera.WorldToScreenPoint(sourcePoint);
            game.InputBlocked=true;
            Check(game.TryOperateAtScreenPoint(sourceScreen).Kind==ScrewOperationKind.Ignored,"Panel gate blocks world input");
            game.InputBlocked=false;
            Check(game.TryOperateAtScreenPoint(sourceScreen).Kind==ScrewOperationKind.Ready,"Camera ray selects source collider");
            var nut=level.GetNut(level.Board.Screws[1].Slots[0].Nut);
            nut.AdvanceMotion(.2f);
            Vector2 targetScreen=game.WorldCamera.WorldToScreenPoint(level.GetScrew(0).Bounds.bounds.center);
            Check(game.TryOperateAtScreenPoint(targetScreen).Kind==ScrewOperationKind.Moved,"Second camera ray transfers to destination");
            nut.AdvanceMotion(0f);nut.AdvanceMotion(.2f);nut.AdvanceMotion(.2f);
            level.GetScrew(0).AdvanceDone(.3f);level.GetScrew(0).AdvanceDone(.201f);
            Check(level.Board.IsSuccess && level.Board.Screws[0].IsCanOperator,"Scene input completes actual first-board sort and animation gate");
            if(capture) Capture(game.WorldCamera,"scene-first-board-complete.png");
            Debug.Log("NUT_SCENE_VALIDATION_PASS original scene cameras/light/background, startup through selector, screen-point collider operations and first-board completion; full panels, saves, region routing and visual comparison remain pending.");
        }
        public static void Capture(Camera camera,string name)
        {
            var target=new RenderTexture(480,854,24,RenderTextureFormat.ARGB32);
            target.Create();
            var previous=RenderTexture.active;
            float aspect=camera.aspect;
            try
            {
                camera.aspect=480f/854f;
                RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest {destination=target});
                RenderTexture.active=target;
                var image=new Texture2D(480,854,TextureFormat.RGB24,false);
                try
                {
                    image.ReadPixels(new Rect(0,0,480,854),0,0);image.Apply();
                    string directory=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures"));
                    Directory.CreateDirectory(directory);File.WriteAllBytes(Path.Combine(directory,name),image.EncodeToPNG());
                    Debug.Log("NUT_SCENE_CAPTURE "+Path.Combine(directory,name));
                }
                finally{UnityEngine.Object.DestroyImmediate(image);}
            }
            finally {camera.aspect=aspect;RenderTexture.active=previous;target.Release();UnityEngine.Object.DestroyImmediate(target);}
        }
        public static void RunGpu()
        {
            try{OriginalPreferenceFixture.Begin();Validate(true);OriginalPreferenceFixture.Restore();EditorApplication.Exit(0);}
            catch(Exception exception){Debug.LogException(exception);OriginalPreferenceFixture.Restore();EditorApplication.Exit(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidDataException(message);}
    }
}
