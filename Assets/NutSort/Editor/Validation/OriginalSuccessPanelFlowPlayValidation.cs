using System;
using NutSort.World;
using NutSort.Content;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalSuccessPanelFlowPlayValidation
    {
        private const string Key="NutSort.SuccessPanelFlowPlay";
        private static double timeout;
        static OriginalSuccessPanelFlowPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Success panel flow Play timeout");
                var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                var user=game.User;user.IsAudio=true;user.Level=2;user.GuideIndex=19;
                var board=game.Level.Board;float gold=user.Gold;double coin=user.Coin;
                int sounds=0,shown=0,closed=0;bool baseCalled=false;
                Action<string> onSound=name=>{if(name=="Pass_level"){Check(baseCalled&&user.Level1TXTime==123456,"Real audio follows base init and timestamp");sounds++;}};
                audio.SoundStarted+=onSound;
                var flow=game.CreateSuccessPanelFlow(()=>123456,id=>{Check(id==7&&user.GuideIndex==0,"Scene user guide reset precedes explicit panel consumer");shown++;});
                try
                {
                    flow.Init(()=>baseCalled=true);
                    Check(sounds==1&&flow.IsGuide,"Configured panel sound actually starts");
                    bool found=false;foreach(var source in audio.GetComponentsInChildren<AudioSource>())if(source.clip!=null&&source.clip.name=="Pass_level"&&source.isPlaying)found=true;
                    Check(found,"Actual original audio clip is playing");
                    user.Level=4;flow.TweenEndRefresh(()=>{});Check(shown==1,"Captured init guide survives live level change");
                    flow.GetCallback(()=>closed++,()=>{});Check(closed==0,"Later-level callback delegates base");
                    user.Level=3;flow.GetCallback(()=>closed++,()=>throw new InvalidOperationException("Unexpected base callback"));Check(closed==1,"Early-level callback closes");
                    user.IsAudio=false;flow.Init(()=>{});Check(sounds==1&&user.Level2TXTime==123456,"Muted panel retains state changes without new sound");
                    Check(user.Gold==gold&&user.Coin==coin&&game.Level.Board==board,"Presentation flow does not grant balance or advance/rebuild board");
                    user.GoldRewardTargetS2CData=null;
                    var progress=new OriginalRewardProgress(user,game.Tables,value=>value.ToString());
                    game.HideSuccessPanel(flow,()=>{},()=>true,progress,()=>throw new InvalidOperationException("Unexpected sync"));
                    Check(game.IsRestarting,"New mode hide enters actual scene InitLevel restart");
                }
                finally{audio.SoundStarted-=onSound;}
                Debug.Log("NUT_SUCCESS_PANEL_FLOW_PLAY_PASS scene composition, live user timestamps, actual Pass_level AudioSource and mute, captured guide dispatch and early close; explicit base callbacks/panel consumer, complete panel presentation pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
