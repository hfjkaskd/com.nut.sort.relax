using System;
using System.IO;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalGameplayUnlockPanelValidation
    {
        private const string PathName="prefabs/panels/UnlockGameplayPanel",Key="NutSort.UnlockPanelPlay";
        private static OriginalGameScene game;
        private static OriginalGameplayUnlockPanel panel;
        private static OriginalUserLocalData user;
        private static int phase,index,banners;
        private static double timeout,deadline;
        private static bool gate;
        static OriginalGameplayUnlockPanelValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>(PathName);Check(prefab!=null,"Unlock prefab exists");
            Check(prefab.GetComponentsInChildren<RectTransform>(true).Length==12,"Original twelve-object hierarchy");
            foreach(var t in prefab.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
            var view=prefab.GetComponent<OriginalGameplayUnlockPanel>();
            Check(view.Icons.Length==4 && view.ContinueButton!=null && view.Tip.font!=null,"Four original icons, Button and font");
            foreach(var icon in view.Icons)Check(icon.GetComponent<UnityEngine.UI.Image>().sprite!=null,"Original icon sprite resolved");
            Check(prefab.GetComponentInChildren<OriginalLoopRotation>(true)!=null,"Original glow animation restored");
            Debug.Log("NUT_GAMEPLAY_UNLOCK_PANEL_VALIDATION_PASS full hierarchy, references, four original sprites, standard Button, font and glow component.");
        }
        public static void RunPlay()
        {
            OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Unlock panel validation");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Unlock play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null || game.Level==null || game.IsRestarting || game.InputBlocked)return;
                if(phase==0)
                {
                    Validate();var startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>(PathName),startup.MainLevel.transform.parent,false).GetComponent<OriginalGameplayUnlockPanel>();
                    user=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;user.NewGameplayUnlockIndex=index;
                    gate=false;game.ModalInputBlocked=true;
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                    panel.Initialize(user,index,true,game.Tables,"en",()=>gate,s=>audio.PlaySound(s),callback=>{Check(callback==null && user.NewGameplayUnlockIndex==index,"Banner called before index mutation without completion callback");banners++;},()=>{UnityEngine.Object.Destroy(panel.gameObject);game.ModalInputBlocked=false;});
                    panel.Refresh();
                    for(int i=0;i<4;i++)Check(panel.Icons[i].activeSelf==(i==index),"Actual icon visibility");
                    phase=1;deadline=now+.7;return;
                }
                if(now<deadline)return;
                if(phase==1)
                {
                    string path=System.IO.Path.GetFullPath("Library/ValidationCaptures/unlock-"+index+".png");Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);
                    phase=2;deadline=now+.3;return;
                }
                if(phase==2)
                {
                    panel.ContinueButton.onClick.Invoke();Check(!panel.Closing && user.NewGameplayUnlockIndex==index,"Gate denies actual Continue Button");
                    gate=true;panel.ContinueButton.onClick.Invoke();Check(panel.Closing && user.NewGameplayUnlockIndex==index+1,"Continue updates index before delayed close");
                    panel.gameObject.SetActive(false);phase=3;deadline=now+.4;return;
                }
                Check(panel==null && !game.ModalInputBlocked,"Global close completes for hidden panel");
                if(++index<4){phase=0;return;}
                Check(banners==4,"Every variant invokes banner once");
                Debug.Log("NUT_GAMEPLAY_UNLOCK_PANEL_PLAY_PASS four current rendered variants, actual Continue Button gate/banner/index/close ordering and hidden close; banner callback is a fixture boundary.");Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
