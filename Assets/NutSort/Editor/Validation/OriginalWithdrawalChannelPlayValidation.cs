using System;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalWithdrawalChannelPlayValidation
    {
        private const string Key="NutSort.WithdrawalChannelPlay";
        private const string Fixture="Fixture channel\nfixture@example.test";
        private static double timeout;private static float start;private static int phase,tips,channelHides,formHides,confirmationHides;
        private static OriginalGameScene game;private static OriginalWithdrawalUserInfoHost form;
        private static OriginalWithdrawalChannelHost channel;private static OriginalWithdrawalConfirmationHost confirmation;
        static OriginalWithdrawalChannelPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+120;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");PlayModeWindow.SetCustomRenderingResolution(480,1040,"Other withdrawal channel");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Channel Play timeout phase="+phase);
                if(phase==0)
                {
                    game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();if(game==null||game.Level==null||game.IsRestarting)return;
                    Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;
                    Check(parent!=null,"Actual canvas");game.User.UserLssInfo=null;
                    var audio=UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>();
                    form=new OriginalWithdrawalUserInfoHost(parent,"Prefabs/Panels/TXUserInfoPanel",game.User,game.Tables,"en",()=>"US",()=>"1",()=>true,s=>audio.PlaySound(s),
                        id=>throw new InvalidOperationException("Unexpected form tip"),
                        (id,args)=>{Check(id==22&&(int)args[0]==1&&form.IsOpen,"Channel button preserves underlying form");channel.Show(args);},
                        ()=>throw new InvalidOperationException("Channel branch must not save"),v=>throw new InvalidOperationException("Channel branch must not invoke guide"),()=>{},()=>formHides++,game.ScheduleDelay,()=>{});
                    channel=new OriginalWithdrawalChannelHost(parent,"Prefabs/Panels/TXChannelPanel",game.User,game.Tables,"en",()=>true,s=>audio.PlaySound(s),
                        id=>{Check(id==120,"Original channel tip");tips++;},
                        (id,args)=>{Check(id==21&&(int)args[0]==1&&!channel.Panel.Closing,"Confirmation opens before channel close");confirmation.Show(args);},
                        id=>{Check(id==20&&channel.Panel.Closing&&confirmation.IsOpen,"Underlying form hides after confirmation show and channel close");form.Hide();},
                        ()=>channelHides++,game.ScheduleDelay,()=>{});
                    confirmation=new OriginalWithdrawalConfirmationHost(parent,"Prefabs/Panels/TXUserInfoSurePanel",game.User,game.Tables,"en",
                        ()=>throw new InvalidOperationException("OtherInfo must bypass country lookup"),()=>throw new InvalidOperationException("OtherInfo must bypass area lookup"),()=>true,s=>audio.PlaySound(s),
                        (id,args)=>{Check(id==20&&(int)args[0]==1,"Reenter captured form");form.Show(args);},
                        (level,args)=>throw new InvalidOperationException("No payment request in channel fixture"),v=>throw new InvalidOperationException("No guide completion in channel fixture"),
                        ()=>confirmationHides++,game.ScheduleDelay,()=>{});
                    form.Show(new object[]{1});start=Time.time;phase=1;return;
                }
                if(Time.time-start<.7f)return;
                if(phase==1){form.Panel.ChannelInfoButton.onClick.Invoke();Check(channel.IsOpen&&form.IsOpen,"Actual other-channel Button");start=Time.time;phase=2;return;}
                if(phase==2){channel.Panel.CloseButton.onClick.Invoke();start=Time.time;phase=3;return;}
                if(phase==3){Check(!channel.IsOpen&&channelHides==1&&form.IsOpen&&game.User.UserLssInfo==null,"Close only removes channel panel");form.Panel.ChannelInfoButton.onClick.Invoke();start=Time.time;phase=4;return;}
                if(phase==4)
                {
                    channel.Panel.Info="1234";channel.Panel.SureButton.onClick.Invoke();Check(tips==1&&!channel.Panel.Closing&&game.User.UserLssInfo==null,"Actual short-input rejection");
                    channel.Panel.Info=Fixture;ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-channel-current.png");start=Time.time;phase=5;return;
                }
                if(phase==5){channel.Panel.SureButton.onClick.Invoke();Check(!form.IsOpen&&formHides==1&&channel.Panel.Closing&&confirmation.IsOpen,"Actual successful branch owners");start=Time.time;phase=6;return;}
                if(phase==6)
                {
                    Check(!channel.IsOpen&&channelHides==2&&confirmation.Panel.TipLabel.text==Fixture&&confirmation.Panel.ChannelImage.sprite!=null,"Actual OtherInfo confirmation and icon");
                    ScreenCapture.CaptureScreenshot("Library/ValidationCaptures/withdrawal-channel-confirmation-current.png");start=Time.time;phase=7;return;
                }
                if(phase==7){confirmation.Panel.ReenterButton.onClick.Invoke();start=Time.time;phase=8;return;}
                if(phase==8){Check(!confirmation.IsOpen&&confirmationHides==1&&form.IsOpen,"Actual reenter form after animated close");form.Panel.ChannelInfoButton.onClick.Invoke();start=Time.time;phase=9;return;}
                if(phase==9){Check(channel.Panel.Info==Fixture,"Reopened other-channel input restores live account value");channel.Panel.CloseButton.onClick.Invoke();start=Time.time;phase=10;return;}
                Check(!channel.IsOpen&&channelHides==3&&form.IsOpen&&(string)game.User.UserLssInfo["OtherInfo"]==Fixture,"Closing reopened channel preserves account form and value");
                Debug.Log("NUT_WITHDRAWAL_CHANNEL_PLAY_PASS actual form/channel/confirmation/reenter Buttons, short input, raw OtherInfo, retained underlying form on close, hidden form on success and restored reentry text; no save/payment/guide callback, production composition remains pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
