using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class LocalToolsPlayValidation
    {
        private const string Key="NutSort.LocalToolsPlay";
        private static double timeout;
        private static float waitUntil;
        private static int phase,source,destination,locked;
        private static OriginalGameScene game;
        private static OriginalStartupFlow startup;
        private static LocalGameplayController local;
        private static string originalBoard;
        private static NutSlot[] oldSlots;
        private static int firstExchangedSlot;
        static LocalToolsPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+100;EditorApplication.update+=Tick;}}
        public static void Run()
        {
            // Explicit owned-tool inventory fixture, no production grants/config.
            OriginalPreferenceFixture.Begin("{\"Level\":4,\"NewGameplayUnlockIndex\":1,\"RevokeCount\":3,\"ExchangeCount\":3,\"AddScrewCount\":3}");
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,1040,"Core tools");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static ScrewOperation Click(int index)
        {Physics.SyncTransforms();return game.TryOperateAtScreenPoint(game.WorldCamera.WorldToScreenPoint(game.Level.GetScrew(index).Bounds.bounds.center));}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Tools Play timeout phase "+phase);
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(startup==null)startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                if(game==null||startup==null||startup.LocalGameplay==null||game.IsRestarting||game.InputBlocked||!game.IsInitDone)return;
                local=startup.LocalGameplay;
                if(Time.time<waitUntil)return;
                Check(game.User.ServerConfigData==null&&game.User.GoldRewardTargetS2CData==null,"Tools require no fake server/reward documents");
                if(phase==0)
                {
                    Check(local.Bottom.gameObject.activeInHierarchy&&!startup.Replay.ReplayButton.gameObject.activeInHierarchy,"Actual original bottom replaces partial replay control");
                    source=destination=locked=-1;
                    for(int i=0;i<game.Level.Board.Screws.Length;i++)
                    {
                        var rod=game.Level.Board.Screws[i];if(rod.IsLocked){locked=i;continue;}
                        if(rod.IsNull){destination=i;continue;}
                        if(rod.IsDone||rod.IsHidden||rod.IsColorMask||rod.IsDontMove||rod.IsNutColorSame)continue;
                        bool normal=true;foreach(var slot in rod.Slots)if(slot.Nut!=null&&slot.Nut.Type!=NutType.Normal)normal=false;
                        if(normal)source=i;
                    }
                    Check(source>=0&&destination>=0&&locked>=0,"Original level four supports normal move and locked tool paths");
                    originalBoard=OriginalBoardSnapshotJson.Write(game.Level.CaptureSnapshot());
                    Check(Click(source).Kind==ScrewOperationKind.Ready&&Click(destination).Kind==ScrewOperationKind.Moved,"Real world move creates undo history");
                    waitUntil=Time.time+1.4f;phase=1;return;
                }
                if(phase==1)
                {
                    local.Bottom.GetOtherItem(2).Display.Click.onClick.Invoke();
                    Check(game.Level.MoveHistoryCount==0&&game.User.RevokeCount==2,"Actual undo Button removes history and consumes owned tool");
                    Check(local.Bottom.GetOtherItem(2).Display.Value.text=="2","Undo inventory display refreshed");
                    waitUntil=Time.time+1.4f;phase=2;return;
                }
                if(phase==2)
                {
                    Check(OriginalBoardSnapshotJson.Write(game.Level.CaptureSnapshot())==originalBoard,"Undo restores original board after real reverse animation");
                    var rod=game.Level.Board.Screws[source];oldSlots=(NutSlot[])rod.Slots.Clone();var top=new List<NutSlot>();rod.GetTopSame(top);int count=0;foreach(var slot in rod.Slots)if(slot.Nut!=null)count++;
                    firstExchangedSlot=count-top.Count;
                    local.Bottom.GetOtherItem(3).Display.Click.onClick.Invoke();
                    Check(game.IsExchanging&&local.ExchangeMask.gameObject.activeSelf&&game.User.ExchangeCount==3,"Exchange Button enters source mask without spending");
                    waitUntil=Time.time+.4f;phase=3;return;
                }
                if(phase==3)
                {
                    Check(Click(source).Kind==ScrewOperationKind.ExchangeRequested,"Actual world click routes exchange");
                    Check(game.Level.Board.Screws[source].Slots[0]==oldSlots[firstExchangedSlot]&&game.User.ExchangeCount==2,"Original slot rotation and inventory cost");
                    Check(local.Bottom.GetOtherItem(3).Display.Value.text=="2"&&game.IsExchanging,"Source remains in mode and refreshes inventory");
                    ScreenCapture.CaptureScreenshot("Library/local-tools-exchange-current.png");phase=4;return;
                }
                if(phase==4)
                {
                    local.ExchangeMask.onClick.Invoke();
                    Check(!local.ExchangeMask.gameObject.activeSelf&&game.IsExchanging,"Mask cancel restores visuals before original delayed flag");
                    waitUntil=Time.time+.7f;phase=5;return;
                }
                if(phase==5)
                {
                    Check(!game.IsExchanging&&!game.ModalInputBlocked,"Delayed cancel returns normal input");
                    local.Bottom.GetOtherItem(4).Display.Click.onClick.Invoke();
                    Check(!game.Level.Board.Screws[locked].IsLocked&&game.User.AddScrewCount==2&&game.User.CurrentLevelAddScrewCount==4,"Actual add Button unlocks original rod and charges tool");
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    Check(saved.AddScrewCount==2&&saved.ExchangeCount==2&&saved.RevokeCount==2&&!string.IsNullOrEmpty(saved.LevelInfo),"All three native tool mutations persisted");
                    waitUntil=Time.time+.4f;phase=6;return;
                }
                if(phase==6)
                {
                    local.Bottom.GetOtherItem(2).Display.Click.onClick.Invoke();
                    Check(local.ResultVisible&&game.User.RevokeCount==2,"No-history notice without consuming another tool");
                    waitUntil=Time.time+.4f;phase=7;return;
                }
                if(phase==7)
                {
                    Check(game.ModalInputBlocked,"Common click-mask expiry cannot unlock active local notice");
                    local.DismissButton.onClick.Invoke();waitUntil=Time.time+.3f;phase=8;return;
                }
                if(phase==8)
                {
                    Check(!game.ModalInputBlocked&&!local.ResultVisible,"Notice dismiss restores input");
                    game.User.AddScrewCount=0;local.Bottom.Refresh();local.Bottom.GetOtherItem(4).Display.Click.onClick.Invoke();
                    Check(local.ResultVisible&&game.User.AddScrewCount==0,"No inventory opens SDK skip notice without grant");
                    local.DismissButton.onClick.Invoke();waitUntil=Time.time+.05f;phase=13;return;
                }
                if(phase==13)
                {
                    Check(startup.Replay.IsClickMasked&&!game.ModalInputBlocked,"Dismissal preserves the UI shield without inventing a registered modal");
                    waitUntil=Time.time+.4f;phase=9;return;
                }
                if(phase==9)
                {
                    ScreenCapture.CaptureScreenshot("Library/local-tools-bottom-current.png");phase=12;return;
                }
                if(phase==12)
                {
                    local.Bottom.Replay.onClick.Invoke();Check(startup.Replay.Panel!=null,"Restored bottom opens original replay popup");
                    waitUntil=Time.time+.7f;phase=10;return;
                }
                if(phase==10)
                {
                    startup.Replay.Panel.ContinueButton.onClick.Invoke();waitUntil=Time.time+.7f;phase=11;return;
                }
                if(startup.Replay.Panel!=null)return;
                Check(!game.ModalInputBlocked&&game.PlayerLevel==4,"Replay continue returns to current board");
                Debug.Log("NUT_LOCAL_TOOLS_PLAY_PASS actual default original bottom; real ray move and Button undo; reverse animation restores board; exchange Button/world rotation/mask delayed cancel; add Button unlock; inventory display/save; no-history and SDK-skip notices; counted-mask modal guard; native bottom replay popup. No synthetic server documents.");Finish(0);
            }
            catch(Exception e){Debug.LogException(e);Finish(1);}
        }
        private static void Check(bool condition,string message){if(!condition)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
