using System;
using System.IO;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalMarqueeItemCapture
    {
        public static void Run()
        {
            try
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                var camera=new GameObject("Validation Camera",typeof(Camera)).GetComponent<Camera>();
                camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.15f,.2f,.3f,1);
                camera.orthographic=true;camera.orthographicSize=150;camera.transform.position=new Vector3(0,0,-10);
                var rt=new RenderTexture(1200,300,24);camera.targetTexture=rt;
                var canvas=new GameObject("Validation Canvas",typeof(Canvas)).GetComponent<Canvas>();
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
                var item=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PMDItem"),canvas.transform).GetComponent<OriginalMarqueeItemView>();
                item.Rect.anchorMin=item.Rect.anchorMax=new Vector2(.5f,.5f);item.Rect.anchoredPosition=Vector2.zero;
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var defaults=ScriptableObject.CreateInstance<OriginalUserDefaults>();
                var user=new OriginalUserLocalData(defaults);
                var text=new OriginalMarqueeText(user,tables,()=>"Player_AI29",n=>"$7",(a,b)=>7);
                Action resize=null;
                item.Bind(text,tables.PayChannels,"en",()=>"US",(seconds,done)=>resize=done,null,(a,b)=>2);
                item.Init(new OriginalMarqueeItem { IsGold=true },3);
                item.Tip.ForceMeshUpdate(true);Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(item.Tip.rectTransform);resize();Canvas.ForceUpdateCanvases();
                camera.Render();RenderTexture.active=rt;
                var image=new Texture2D(1200,300,TextureFormat.RGB24,false);
                image.ReadPixels(new Rect(0,0,1200,300),0,0);image.Apply();
                Directory.CreateDirectory("Library/ValidationCaptures");
                File.WriteAllBytes("Library/ValidationCaptures/marquee-item.png",image.EncodeToPNG());
                Debug.Log("NUT_MARQUEE_ITEM_CAPTURE_PASS width="+item.Rect.sizeDelta.x);
                RenderTexture.active=null;camera.targetTexture=null;rt.Release();
                UnityEngine.Object.DestroyImmediate(image);UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(defaults);
                EditorApplication.Exit(0);
            }
            catch(Exception error) { Debug.LogException(error);EditorApplication.Exit(1); }
        }
    }
}
