using System;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    // One-time authoring only. Runtime loads this saved native prefab.
    public static class LocalGameplayBuilder
    {
        private static RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 position,Vector2 size)
        {
            var go=new GameObject(name,typeof(RectTransform));go.layer=5;
            var t=go.GetComponent<RectTransform>();t.SetParent(parent,false);t.anchorMin=t.anchorMax=anchor;
            t.anchoredPosition=position;t.sizeDelta=size;return t;
        }
        private static TMP_Text Label(string name,Transform parent,TMP_FontAsset font,string text,Vector2 position,Vector2 size,float fontSize)
        {
            var t=Rect(name,parent,new Vector2(.5f,.5f),position,size);
            var label=t.gameObject.AddComponent<TextMeshProUGUI>();label.font=font;label.text=text;label.fontSize=fontSize;
            label.alignment=TextAlignmentOptions.Center;label.raycastTarget=false;return label;
        }
        private static Button Button(string name,Transform parent,TMP_FontAsset font,string text)
        {
            var t=Rect(name,parent,new Vector2(.5f,.5f),new Vector2(0,-85),new Vector2(320,82));
            var img=t.gameObject.AddComponent<Image>();img.color=new Color(.16f,.53f,.29f,1);
            var b=t.gameObject.AddComponent<Button>();b.targetGraphic=img;
            Label("Label",t,font,text,Vector2.zero,new Vector2(300,74),32);return b;
        }
        public static void Run()
        {
            GameObject root=null;
            try
            {
                var main=Resources.Load<GameObject>("Prefabs/Panels/MainPanel");
                var font=main.GetComponentInChildren<TextMeshProUGUI>(true).font;
                root=Rect("LocalGameplay",null,new Vector2(.5f,.5f),Vector2.zero,Vector2.zero).gameObject;
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
                var owner=root.AddComponent<LocalGameplayController>();
                var tag=Rect("LocalMode",rect,new Vector2(.5f,1),new Vector2(0,-175),new Vector2(560,46));
                var tagBg=tag.gameObject.AddComponent<Image>();tagBg.color=new Color(0,0,0,.5f);tagBg.raycastTarget=false;
                var mode=Label("Mode",tag,font,"LOCAL MODE - SDK skipped",Vector2.zero,new Vector2(550,44),25);
                var backdrop=Rect("Result",rect,new Vector2(.5f,.5f),Vector2.zero,Vector2.zero);
                backdrop.anchorMin=Vector2.zero;backdrop.anchorMax=Vector2.one;backdrop.offsetMin=backdrop.offsetMax=Vector2.zero;
                backdrop.gameObject.AddComponent<Image>().color=new Color(0,0,0,.68f);
                var card=Rect("Card",backdrop,new Vector2(.5f,.5f),Vector2.zero,new Vector2(620,330));
                card.gameObject.AddComponent<Image>().color=new Color(.10f,.18f,.24f,1);
                var message=Label("Message",card,font,"Level complete",new Vector2(0,45),new Vector2(560,150),38);
                var next=Button("Next",card,font,"Next level");var retry=Button("Retry",card,font,"Try again");var dismiss=Button("Dismiss",card,font,"Continue");
                var so=new SerializedObject(owner);
                so.FindProperty("modeLabel").objectReferenceValue=mode;so.FindProperty("message").objectReferenceValue=message;
                so.FindProperty("resultPanel").objectReferenceValue=backdrop.gameObject;so.FindProperty("next").objectReferenceValue=next;so.FindProperty("retry").objectReferenceValue=retry;
                so.FindProperty("dismiss").objectReferenceValue=dismiss;
                so.FindProperty("maximumAddedTiles").intValue=12;
                so.FindProperty("unavailableText").stringValue="Tool unavailable\nSDK rewards are skipped";
                so.FindProperty("gameplaySession").objectReferenceValue=Resources.Load<NutSort.World.OriginalSceneSession>("Configuration/OriginalSceneSession");
                so.FindProperty("unlockPrefabPath").stringValue="Prefabs/Panels/UnlockGameplayPanel";
                so.FindProperty("modeText").stringValue="LOCAL MODE - SDK skipped";
                so.FindProperty("successText").stringValue="Level complete\nProgress saved locally";
                so.FindProperty("failureText").stringValue="No moves left\nTry another board";
                var bottom=((GameObject)PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Prefabs/Panels/MainPanelBottom"),rect)).GetComponent<OriginalMainBottomView>();
                // Settings remain outside this core-only local composition.
                bottom.Setting.transform.parent.gameObject.SetActive(false);
                var original=Resources.Load<GameObject>("Prefabs/Panels/MainPanelComplete").GetComponent<OriginalMainPanelView>();
                var mask=UnityEngine.Object.Instantiate(original.ExchangeMask.gameObject,rect,false).GetComponent<Button>();
                mask.name="ExchangeMask";mask.gameObject.SetActive(false);
                so.FindProperty("bottom").objectReferenceValue=bottom;
                so.FindProperty("exchangeMask").objectReferenceValue=mask;
                so.FindProperty("exchangeCancelDelay").floatValue=new SerializedObject(original).FindProperty("exchangeCancelDelay").floatValue;
                so.FindProperty("noHistoryText").stringValue="No moves to undo";
                so.FindProperty("limitText").stringValue="No more rods can be added";
                bottom.transform.SetAsFirstSibling();mask.transform.SetSiblingIndex(1);backdrop.SetAsLastSibling();
                so.ApplyModifiedPropertiesWithoutUndo();backdrop.gameObject.SetActive(false);
                PrefabUtility.SaveAsPrefabAsset(root,"Assets/Resources/prefabs/panels/LocalGameplay.prefab");
                var settings=Resources.Load<OriginalMainLevelSettings>("Configuration/OriginalMainLevel");
                settings.LocalGameplayPath="Prefabs/Panels/LocalGameplay";EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();
                Debug.Log("NUT_LOCAL_GAMEPLAY_BUILD_PASS native prefab and explicit local-mode configuration");EditorApplication.Exit(0);
            }
            catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
            finally{if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
        }
    }
}
