using System;
using System.IO;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalMessagePanelPlayValidation
    {
        private const string Key="NutSort.MessagePanelPlay";private static double timeout;private static int phase,calls;private static float begin;
        private static OriginalMessagePanel panel;
        static OriginalMessagePanelPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Message panel timeout phase="+phase);
                if(phase==0)
                {
                    var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;Check(parent!=null,"Actual canvas");
                    panel=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MessagePanel"),parent,false).GetComponent<OriginalMessagePanel>();
                    panel.Bind(id=>game.Tables.Text.GetText(id,"en"));panel.Init();panel.Show(120,()=>{Check(!panel.gameObject.activeSelf,"Hide before real button callback");calls++;});begin=Time.time;phase=1;return;
                }
                if(Time.time-begin<1)return;
                if(phase==1){Check(panel.gameObject.activeSelf&&!string.IsNullOrEmpty(panel.ContentText.text),"Visible original localized message");Directory.CreateDirectory("Library/ValidationCaptures");ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/message-panel-current.png");begin=Time.time;phase=2;return;}
                if(phase==2){panel.ConfirmButton.onClick.Invoke();Check(calls==1&&!panel.gameObject.activeSelf,"Actual Button confirmation");panel.Show("Explicit validation message",null);Check(panel.gameObject.activeSelf,"Reuse same panel");panel.ConfirmButton.onClick.Invoke();Check(calls==1&&!panel.gameObject.activeSelf,"Null replacement");Debug.Log("NUT_MESSAGE_PANEL_PLAY_PASS actual scene canvas, original localized prefab, standard Button confirm, hide-before-callback and reuse; explicit fixture caller, production global UI initialization pending.");Finish(0);}
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
