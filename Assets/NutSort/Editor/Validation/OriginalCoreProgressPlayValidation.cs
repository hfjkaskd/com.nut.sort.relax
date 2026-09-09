using System;
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
    public static class OriginalCoreProgressPlayValidation
    {
        // The real first tutorial path must not touch later reward consumers.
        private sealed class FirstTutorialEffects:IOriginalScrewDoneEffects
        {
            private static Exception Unexpected()=>new InvalidOperationException("First tutorial reached a later reward/config consumer");
            public float RemoveGold=>throw Unexpected();public float RemoveCoin=>throw Unexpected();
            public void FlyGold(ScrewState s)=>throw Unexpected();public void FlyCoin(ScrewState s)=>throw Unexpected();
            public void AddGold(float n,bool r,bool sync)=>throw Unexpected();public void AddCoin(float n,bool r)=>throw Unexpected();
            public bool HasPanel=>throw Unexpected();public int Config(string f)=>throw Unexpected();public long TimeSeconds=>throw Unexpected();
            public long LuckyRewardTime{get=>throw Unexpected();set=>throw Unexpected();}
            public long LuckyDrawTime{get=>throw Unexpected();set=>throw Unexpected();}
            public void RequestLuckyReward()=>throw Unexpected();public void ShowPanel(int id)=>throw Unexpected();
        }
        private const string Key="NutSort.CoreProgressPlay";
        private static double timeout;
        private static int phase,refreshes;
        private static OriginalGameScene game;
        private static OriginalStartupFlow startup;
        static OriginalCoreProgressPlayValidation()
        {
            if(SessionState.GetBool(Key,false))
            {timeout=EditorApplication.timeSinceStartup+80;EditorApplication.update+=Tick;}
        }
        public static void Run()
        {
            OriginalPreferenceFixture.Begin();
            EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Core progression Play timeout");
                if(game==null)game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                if(phase==0)
                {
                    startup=UnityEngine.Object.FindObjectOfType<OriginalStartupFlow>();
                    game.User.Level=1;game.User.LevelSeed=0;game.InitLevel(true,false,false);
                    phase=1;return;
                }
                if(phase==1)
                {
                    // Use the actual scene DoneEvent consumer rather than a test
                    // callback that calls Success on behalf of gameplay.
                    game.BindSuccess(()=>false,()=>refreshes++,()=>{},()=>{},startup.MainLevel.transform.parent,
                        captured=>throw new Exception("First tutorial completion must not request reward"));
                    game.BindScrewCompletion(new FirstTutorialEffects(),
                        ()=>throw new Exception("Early seed cannot hide guide"),()=>{});
                    Check(game.Level.Operate(0).Kind==ScrewOperationKind.Ready,"Real world nut selected");
                    Check(game.Level.Operate(1).Kind==ScrewOperationKind.Moved,"Real world transfer");
                    Check(game.Level.Board.IsSuccess,"Actual first board solved");phase=2;return;
                }
                if(phase==2)
                {
                    if(game.User.LevelSeed!=1||game.IsRestarting||!game.Level.AreNutsInitialized)return;
                    Check(game.User.Level==1&&refreshes==1&&!game.IsSucceed,"Native delayed completion rebuilds next tutorial seed once");
                    // Explicit server result fixture. Runtime does not fabricate it.
                    var progress=new OriginalClearanceProgress(()=>game.User,()=>
                        startup.MainLevel.Refresh(game.Tables,game.PlayerLevel,"en",false,false),
                        ()=>throw new Exception("Level two does not reset lucky clocks"),n=>{},n=>{});
                    progress.Apply(true,new OriginalClearanceProgressData{gap_all_gates=1,gap_day_gates=1});
                    Check(game.User.Level==2&&game.User.LevelSeed==0,"Apply actual response semantics before selecting next board");
                    game.InitLevel(true,false,false);phase=3;return;
                }
                if(!game.Level.AreNutsInitialized)return;
                var settings=Resources.Load<OriginalContentSettings>("Configuration/OriginalContent");
                var repository=new OriginalLevelRepository(settings);repository.Initialize();
                var selector=new OriginalLevelSelector(repository,settings,UnityLevelRandom.Instance);
                var expected=selector.Select(new LevelProgressState{Level=2,LevelSeed=0},false,1);
                Check(game.User.LevelId==expected.LevelId,"Next board uses original level-index offset");
                Check(game.Level.Board.LevelId==repository.LoadBoard(expected.Loop,expected.Seed).LId,"Expected original board loaded");
                game.SaveUserData();
                var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),
                    Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                Check(saved.Level==2&&saved.LevelSeed==0&&saved.TodayPassLevelCount==1,"Actual PlayerPrefs contains applied progress");
                var snapshot=OriginalBoardSnapshotJson.Read(saved.LevelInfo);
                Check(snapshot!=null,"Actual next board persists with progress");
                Debug.Log("NUT_CORE_PROGRESS_PLAY_PASS real first-board transfer through native scene DoneEvent consumer, delayed native tutorial success and seed rebuild, explicit clearance result application, original next-level index/board and PlayerPrefs persistence; production initialization and completion bindings remain pending.");
                Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code)
        {OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
