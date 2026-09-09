using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalHiddenCoverPlayValidation
    {
        private const string Key="NutSort.HiddenCoverPlay";
        private static double timeout,paused;
        private static float started;
        private static int phase;
        private static OriginalGameScene game;
        static OriginalHiddenCoverPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+80;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,1040,"Hidden cover");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static ScrewData Rod(int[] colors,params OBIMData[] masks)
        {var cells=new CData[4];for(int i=0;i<4;i++)cells[i]=new CData{LP=new LPData{y=i},BIM=i<colors.Length?new BIMData{Id=1,CI=colors[i]}:null};return new ScrewData{Id=1,C=cells,OBIM=masks};}
        private static OriginalNativeWorldEffect Hidden(int index)
        {
            foreach(var effect in game.Level.GetScrew(index).GetComponentsInChildren<OriginalNativeWorldEffect>(true))if(effect.Player!=null&&effect.Player["wanzheng"]!=null)return effect;
            throw new InvalidOperationException("Hidden native effect not loaded");
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Hidden Play timeout");if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    ValidateClipping();
                    game.Level.Bind(new LevelData{B=new[]{Rod(new[]{11}),Rod(new[]{11,11,11}),Rod(new[]{1,2,1,2}),Rod(Array.Empty<int>()),Rod(new[]{1,2,1,2},new OBIMData{Id=7}),Rod(new[]{1,2,1,2},new OBIMData{Id=7})}},UnityEngine.Object.FindObjectOfType<OriginalPrefabPool>(),false,true);
                    game.ResizeCameras(480,1040);phase=1;return;
                }
                if(!game.Level.AreNutsInitialized)return;
                if(phase==1)
                {
                    Check(Hidden(4).Player.IsPlaying("wanzheng")&&Hidden(5).Player.IsPlaying("wanzheng"),"Configure starts both native idle clips");
                    started=Time.time;phase=2;return;
                }
                if(phase==2)
                {
                    if(Time.time-started<.6f)return;
                    ScreenCapture.CaptureScreenshot("Library/core-hidden-idle-current.png");phase=5;return;
                }
                if(phase==5)
                {
                    game.BindMoveCompletion((s,success)=>Check(!success,"Fixture not whole-board victory"),()=>{},()=>{},null,id=>throw new Exception("Unexpected fail"));
                    Check(game.Level.Operate(0).Kind==ScrewOperationKind.Ready,"Selection");Check(game.Level.Operate(1).Kind==ScrewOperationKind.Moved,"Transfer");
                    Check(!game.Level.Board.Screws[4].IsHidden&&game.Level.Board.Screws[5].IsHidden,"Automatic adjacency reveal");
                    Check(Hidden(4).Player.IsPlaying("posui")&&Hidden(5).Player.IsPlaying("wanzheng"),"Actual completion switches only adjacent cover to break");
                    started=Time.time;phase=3;return;
                }
                if(phase==3)
                {
                    if(Time.time-started<.25f)return;
                    var driver=Hidden(4).Player.GetComponent<OriginalHiddenCoverMesh>();Check(driver.target.sharedMesh.vertexCount==72,"Runtime mesh");
                    bool visible=false;foreach(var slot in driver.slots)if(slot.color.a>.1f)visible=true;Check(visible&&game.Level.GetScrew(4).TypeView.IsHiddenVisible,"Break visible before source hide delay");
                    var points=driver.target.sharedMesh.vertices;foreach(var point in points)Check(!float.IsNaN(point.x)&&!float.IsInfinity(point.y),"Finite deformed fragment vertices");
                    Check(Mathf.Abs(driver.bones[3].shearX-driver.bones[3].shearY)>1,"Asymmetric animated shear is live");
                    ScreenCapture.CaptureScreenshot("Library/core-hidden-break-current.png");Time.timeScale=0;paused=EditorApplication.timeSinceStartup;phase=4;return;
                }
                if(EditorApplication.timeSinceStartup-paused<1.1)return;
                Check(!game.Level.GetScrew(4).TypeView.IsHiddenVisible&&game.Level.GetScrew(5).TypeView.IsHiddenVisible,"Source one-second unscaled hide while animation clock paused");
                Debug.Log("NUT_HIDDEN_COVER_PLAY_PASS actual board transfer automatically reveals adjacent cover, native idle/break and asymmetric shear mesh, unaffected diagonal cover, unscaled hide; GPU clipping validated independently; explicit fixture, default startup pending.");Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void ValidateClipping()
        {
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Game/NativeEffects/HiddenCover/HiddenCover"));var cameraObject=new GameObject("HiddenMaskValidationCamera");var rt=new RenderTexture(256,256,24);var pixels=new Texture2D(256,256,TextureFormat.RGB24,false);var old=RenderTexture.active;
            try
            {
                foreach(var t in root.GetComponentsInChildren<Transform>())t.gameObject.layer=30;
                root.GetComponentInChildren<MeshRenderer>().enabled=false;var player=root.GetComponent<Animation>();player["wanzheng"].clip.SampleAnimation(root,.8f);
                var glow=root.GetComponentInChildren<SpriteRenderer>();glow.color=Color.white;
                var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=2.5f;camera.aspect=1;camera.transform.position=new Vector3(0,.5f,-10);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.cullingMask=1<<30;camera.targetTexture=rt;camera.allowHDR=false;camera.allowMSAA=false;
                var mask=root.GetComponentInChildren<SpriteMask>();var polygon=mask.sprite.vertices;
                camera.Render();RenderTexture.active=rt;pixels.ReadPixels(new Rect(0,0,256,256),0,0);pixels.Apply();var clipped=pixels.GetPixels32();
                mask.enabled=false;glow.maskInteraction=SpriteMaskInteraction.None;camera.Render();pixels.ReadPixels(new Rect(0,0,256,256),0,0);pixels.Apply();var unclipped=pixels.GetPixels32();int removed=0,retained=0,leaked=0;
                for(int y=0;y<256;y++)for(int x=0;x<256;x++)
                {
                    int i=y*256+x;var point=new Vector2((x+.5f)/256*5-2.5f,(y+.5f)/256*5-2);
                    bool inside=false;float distance=float.MaxValue;
                    for(int a=0,b=polygon.Length-1;a<polygon.Length;b=a++)
                    {
                        var v=polygon[a];var w=polygon[b];if((v.y>point.y)!=(w.y>point.y)&&point.x<(w.x-v.x)*(point.y-v.y)/(w.y-v.y)+v.x)inside=!inside;
                        var edge=w-v;float t=Mathf.Clamp01(Vector2.Dot(point-v,edge)/edge.sqrMagnitude);distance=Mathf.Min(distance,(point-v-t*edge).magnitude);
                    }
                    if(distance<.04f)continue;int before=unclipped[i].r+unclipped[i].g+unclipped[i].b,after=clipped[i].r+clipped[i].g+clipped[i].b;
                    if(before>30){if(inside&&after>20)retained++;if(!inside&&after<5)removed++;if(!inside&&after>20)leaked++;}
                }
                Check(retained>100&&removed>100&&leaked==0,"GPU polygon mask: retained="+retained+" removed="+removed+" leaked="+leaked);
                Debug.Log("NUT_HIDDEN_COVER_GPU_MASK_PASS inside="+retained+" outsideRemoved="+removed+" outsideLeaks="+leaked);
            }
            finally{RenderTexture.active=old;UnityEngine.Object.Destroy(root);UnityEngine.Object.Destroy(cameraObject);rt.Release();UnityEngine.Object.Destroy(rt);UnityEngine.Object.Destroy(pixels);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){Time.timeScale=1;OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
