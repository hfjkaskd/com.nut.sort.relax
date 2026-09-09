using System;
using NutSort.World;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalBoardGuideBuilder
    {
        public static void Run()
        {
            try
            {
                var clip=new AnimationClip{name="GuideHand",legacy=true,frameRate=30,wrapMode=WrapMode.Loop};
                clip.SetCurve("Hand",typeof(Transform),"m_LocalPosition.x",new AnimationCurve(new Keyframe(0,-.4f,.1f,.1f),new Keyframe(1,-.3f,.1f,-.1f),new Keyframe(2,-.4f,-.1f,-.1f)));
                clip.SetCurve("Hand",typeof(Transform),"m_LocalPosition.y",new AnimationCurve(new Keyframe(0,.4f,.1f,.1f),new Keyframe(1,.5f,.1f,-.1f),new Keyframe(2,.4f,-.1f,-.1f)));
                clip=OriginalMaskSmokeBuilder.Store(clip,"Assets/Resources/game/GuideHand.anim");
                var root=PrefabUtility.LoadPrefabContents("Assets/Resources/game/GuideHand.prefab");
                try{var player=root.GetComponent<Animation>();if(player==null)player=root.AddComponent<Animation>();player.AddClip(clip,"GuideHand");player.clip=clip;player.playAutomatically=true;player.cullingType=AnimationCullingType.AlwaysAnimate;PrefabUtility.SaveAsPrefabAsset(root,"Assets/Resources/game/GuideHand.prefab");}
                finally{PrefabUtility.UnloadPrefabContents(root);}
                var settings=Resources.Load<OriginalScrewSettings>("Configuration/OriginalScrew");settings.GuidePaths=new[]{"Game/GuideHand","Game/GuideCorrect","Game/GuideError"};settings.GuideHeightOffset=1;EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();
                Debug.Log("NUT_BOARD_GUIDE_BUILD_PASS original world sprites and native one-second linear yoyo hand animation");EditorApplication.Exit(0);
            }
            catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
        }
    }
}
