using System;
using System.Collections.Generic;
using System.IO;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalReplayPlayValidation
    {
        private const string Key="NutSort.ReplayPlayValidation";
        private static OriginalGameScene game;
        private static OriginalReplayController replay;
        private static object board;
        private static int phase, stageStarts, startsBeforeReplay;
        private static bool maskChecked;
        private static double deadline,timeout;
        static OriginalReplayPlayValidation()
        {
            if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}
        }
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            PlayModeWindow.SetCustomRenderingResolution(480,854,"Nut Sort replay validation");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Click(Button button)
        {
            Canvas.ForceUpdateCanvases();
            var canvas=button.GetComponentInParent<Canvas>();
            Vector2 point=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,button.transform.position);
            var data=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);
            Check(hits.Count>0 && hits[0].gameObject.GetComponentInParent<Button>()==button,"Standard graphic raycast reaches intended button: "+button.name);
            data.pointerCurrentRaycast=hits[0];data.pointerPressRaycast=hits[0];
            ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                double now=EditorApplication.timeSinceStartup;Check(now<timeout,"Replay test timeout phase="+phase);
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null)return;
                if(phase==0)
                {
                    if(!game.Level.AreNutsInitialized||game.InputBlocked)return;
                    replay=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>().Replay;
                    Check(replay!=null&&EventSystem.current!=null,"Native replay entry and EventSystem");
                    UnityEngine.Object.FindObjectOfType<OriginalAudioPlayer>().SoundStarted += name => { if(name=="StageStart")stageStarts++; };
                    board=game.Level.Board;game.Level.Operate(1);Click(replay.ReplayButton);
                    Check(replay.Panel!=null&&game.ModalInputBlocked,"Button opens real blocking replay panel");
                    phase=1;deadline=now+.55;return;
                }
                if(phase==1&&!maskChecked&&now>=deadline-.48)
                {
                    var maskHits=new List<RaycastResult>();
                    EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=new Vector2(240,427)},maskHits);
                    string first=maskHits.Count==0?"none":maskHits[0].gameObject.name;
                    Check(first=="Mask","Original transparent top-canvas click mask blocks graphic raycasts; first="+first);
                    maskChecked=true;
                }
                if(now<deadline)return;
                switch(phase)
                {
                    case 1:
                        Check(replay.Panel.Main.localScale==Vector3.one,"Original opening curve completes");
                        var point=game.WorldCamera.WorldToScreenPoint(game.Level.GetScrew(0).Bounds.bounds.center);
                        Check(!game.TryOperateAtScreenPoint(point).OriginalReturnValue,"Modal prevents gameplay click-through");
                        string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Library/ValidationCaptures/replay-panel.png"));
                        Directory.CreateDirectory(Path.GetDirectoryName(path));ScreenCapture.CaptureScreenshot(path);Debug.Log("NUT_REPLAY_CAPTURE "+path);
                        phase=2;deadline=now+.15;break;
                    case 2:
                        Click(replay.Panel.ContinueButton);Check(replay.Panel.Closing,"Continue starts original close animation");
                        phase=3;deadline=now+.4;break;
                    case 3:
                        Check(replay.Panel==null&&!game.ModalInputBlocked&&ReferenceEquals(board,game.Level.Board)&&game.Level.Board.Screws[1].IsReadyMove,"Cancel preserves board and selection");
                        game.Level.Operate(0);Click(replay.ReplayButton);phase=4;deadline=now+.55;break;
                    case 4:
                        startsBeforeReplay=stageStarts;
                        var user=UnityEngine.Object.FindObjectOfType<OriginalUserSession>().Data;
                        user.CurrentLevelAddScrewCount=2;user.PassLevelTime=23;user.LuckyScrewDoneCount=7;
                        Click(replay.Panel.ReplayButton);
                        Check(user.CurrentLevelAddScrewCount==0&&user.PassLevelTime==0&&user.LuckyScrewDoneCount==0,"Original reset clears current-level added screws, time and lucky count");
                        Check(game.IsRestarting&&game.Level.Board==null,"Replay alias dispatch clears board immediately");
                        phase=5;deadline=now+.12;break;
                    case 5:
                        Check(game.Level.Board==null,"Original 0.3 second reconstruction delay");
                        phase=6;deadline=now+.85;break;
                    case 6:
                        Check(stageStarts==startsBeforeReplay+1,"Restart emits the original delayed StageStart once");
                        Check(!game.IsRestarting&&game.Level.AreNutsInitialized&&replay.Panel==null,"Replay finishes native reconstruction");
                        Check(!ReferenceEquals(board,game.Level.Board)&&game.PlayerLevel==1&&!game.Level.Board.IsSuccess,"Replay keeps current level with new unsolved state");
                        Check(game.Level.Board.Screws[1].Slots[0].Nut!=null&&game.Level.Board.Screws[0].Slots[3].Nut==null&&!game.Level.Board.Screws[1].IsReadyMove,"Original three-plus-one occupancy and cleared selection");
                        Check(game.GetComponent<OriginalGameplayEffects>().ActiveCount==0,"Old transient effects do not survive restart");
                        Click(replay.ReplayButton);phase=7;deadline=now+.5;break;
                    case 7:
                        Click(replay.Panel.ContinueButton);phase=8;deadline=now+.4;break;
                    case 8:
                        Check(replay.Panel==null&&!game.ModalInputBlocked,"Repeated open/cancel releases modal gate");
                        Debug.Log("NUT_REPLAY_PLAY_VALIDATION_PASS standard UI raycast/pointer dispatch, alias binding, native open/close, modal input, cancel preserves selection, immediate clear, delayed rebuild, level/seed occupancy and effect cleanup, repeated open/cancel.");Finish(0);break;
                }
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new Exception(message);}
        private static void Finish(int code){SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
