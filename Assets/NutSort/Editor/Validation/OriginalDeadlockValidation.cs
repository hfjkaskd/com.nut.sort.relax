using System;
using System.Collections.Generic;
using System.IO;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalDeadlockValidation
    {
        public static void Validate()
        {
            var layout=Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            var trace=new List<string>();
            var rules=new OriginalDeadlockRules(index=>trace.Add("break:"+index),()=>trace.Add("save"));
            Check(!rules.IsCannotMove(null),"Absent level is not a deadlock");
            Check(rules.IsCannotMove(Board(layout)),"Empty screw list retains original exhaustive result");
            var board=Board(layout,S(1,2,1,1,1),S(1,2,1,1,-1));
            Check(rules.IsCannotMove(board),"Partial transfer alone does not pass the original whole-group deadlock check");
            var op=new OriginalScrewOperator();op.Operate(board.Screws[0],null);
            Check(op.Operate(board.Screws[1],board.Screws[0]).Move.Transfers.Length==1,"Same board permits one-nut player transfer");
            board=Board(layout,S(1,2,1,-1,-1),S(1,2,1,1,-1));
            Check(!rules.IsCannotMove(board),"Exact group fit is available");
            board=Board(layout,S(6,1,2,-1,-1),S(1,-1,-1,-1,-1));
            Check(rules.IsOnlyDontMove(board) && rules.IsCannotMove(board),"Only-DontMove takes priority over empty destination");
            board=Board(layout,S(6,2,1,1,-1),S(1,2,1,-1,-1));
            Check(!rules.IsCannotMove(board),"DontMove may receive from a normal source");
            var locked=S(1,-1,-1,-1,-1);locked.IsLocked=true;
            Check(rules.IsCannotMove(Board(layout,S(1,1,2,1,2),locked)),"Locked emptiness is unavailable");
            locked.IsLocked=false;locked.NutMaxCount=0;
            Check(rules.IsCannotMove(Board(layout,S(1,1,2,1,2),locked)),"Zero usable capacity is unavailable");
            Check(!rules.IsCannotMove(Board(layout,S(1,1,2,1,2),S(1,-1,-1,-1,-1))),"Accessible empty rod prevents ordinary deadlock");

            board=Board(layout,S(1,1,1,2,2),S(1,1,1,3,3),S(7,4,5,-1,-1));
            Check(!rules.CheckDie(board) && board.Screws[2].IsHidden,"Four occurrences across visible unfinished rods prevent reveal");
            Check(trace.Count==0,"Unchanged masks do not animate or save");
            board.Screws[0].Slots[0].Nut.Color=6;
            Check(rules.CheckDie(board) && !board.Screws[2].IsHidden,"Three occurrences permit reveal");
            Expect(trace,"break:2,save");
            Check(!rules.CheckDie(board),"Repeated check does not re-save revealed mask");

            board=Board(layout,S(1,1,1,1,1),S(4,2,2,2,2,1),S(7,3,4,-1,-1));
            Check(rules.CheckDie(board),"Completed and color-masked rods are excluded from color inventory");
            Expect(trace,"break:2,save");
            var contributing=S(6,1,1,1,1,2);contributing.IsLocked=true;
            board=Board(layout,contributing,S(7,3,4,-1,-1));
            board.Screws[0].Slots[0].Nut.Type=NutType.Hidden;
            Check(!rules.CheckDie(board),"Locked/DontMove rods and hidden nuts still contribute all colors");
            Check(trace.Count==0,"Four contributed nuts keep hidden rod intact");

            // Reveal the first rod, then recount from the start. Its fourth red
            // nut makes the second hidden rod ineligible during this same call.
            board=Board(layout,S(1,1,1,1,2),S(7,1,3,-1,-1),S(7,4,5,-1,-1));
            Check(rules.CheckDie(board) && !board.Screws[1].IsHidden && board.Screws[2].IsHidden,"Recount after each reveal can stop a chain");
            Expect(trace,"break:1,save");
            ValidateScene(layout);
            Debug.Log("NUT_DEADLOCK_VALIDATION_PASS original whole-group rules, hidden color threshold/recount, mask order, actual scene save envelope, native visibility delay and pooled cancellation; skeletal clip remains pending.");
        }

        private static void ValidateScene(OriginalLayoutSettings layout)
        {
            bool had=PlayerPrefs.HasKey(OriginalUserStore.Key);
            string previous=PlayerPrefs.GetString(OriginalUserStore.Key,string.Empty);
            try
            {
                var hidden=S(7,3,4,-1,-1);
                hidden.ScrewMaskDatas.Add(new SavedMask { ScrewType=7,IsShow=true });
                var snapshot=new OriginalBoardSnapshot { ScrewInfos=new List<SavedScrew> {S(1,1,2,-1,-1),hidden,S(7,5,6,-1,-1)} };
                var defaults=Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults");
                var user=new OriginalUserLocalData(defaults) {Level=7,IsAudio=false,ScrewMoveCount=42,LevelInfo=OriginalBoardSnapshotJson.Write(snapshot)};
                string json=OriginalUserDataJson.Write(user);PlayerPrefs.SetString(OriginalUserStore.Key,json);PlayerPrefs.Save();
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");
                OriginalGameScene game=null;
                foreach(var root in scene.GetRootGameObjects()) if(root.TryGetComponent(out OriginalGameScene found)){game=found;break;}
                Check(game!=null,"Actual scene contains game controller");
                game.gameObject.SetActive(true);game.Initialize();game.AdvanceInitialization(.301f);
                game.Level.AdvanceInitialization(1.501f);game.AdvanceInitialization(0);
                var type=game.Level.GetScrew(1).TypeView;
                var type2=game.Level.GetScrew(2).TypeView;
                Check(type.IsHiddenVisible && type2.IsHiddenVisible,"Saved hidden rods have active native roots");
                var events=new List<string>();
                type.AnimationRequested+=(target,clip,loop)=>events.Add("clip:"+clip+":"+loop);
                type2.AnimationRequested+=(target,clip,loop)=>events.Add("clip2:"+clip+":"+loop);
                game.Level.SaveRequested+=()=>
                {
                    // Scene subscribed first: inspect actual PlayerPrefs after
                    // each individual SaveData, including the intermediate mask.
                    var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),defaults);
                    var board=OriginalBoardSnapshotJson.Read(saved.LevelInfo);
                    Check(saved.ScrewMoveCount==42,"Hidden reveal must not count a move attempt");
                    events.Add("saved:"+board.ScrewInfos[1].ScrewMaskDatas[0].IsShow+":"+board.ScrewInfos[1].ScrewMaskDatas[1].IsShow+":"+board.ScrewInfos[2].ScrewMaskDatas[0].IsShow);
                };
                Check(game.Level.IsCannotMove(),"Revealed mixed-color fixture has no legal whole-group move");
                Expect(events,"clip:posui:False,saved:False:True:True,clip:posui:False,saved:False:False:True,clip2:posui:False,saved:False:False:False");
                Check(type.IsHiddenVisible,"Mask state is saved before visual deactivation");
                type.AdvanceHiddenBreak(.99f);Check(type.IsHiddenVisible,"Native root stays visible before one unscaled second");
                type.AdvanceHiddenBreak(.011f);Check(!type.IsHiddenVisible,"Native root deactivates at unscaled deadline");
                type2.Release();type2.AdvanceHiddenBreak(2f);
                Check(type2.IsHiddenVisible,"Pool release cancels pending work before reused roots can be hidden");
                string savedJson=PlayerPrefs.GetString(OriginalUserStore.Key);
                Check(game.Level.IsCannotMove() && PlayerPrefs.GetString(OriginalUserStore.Key)==savedJson && events.Count==0,"Repeated checker does not repeat saves or animation requests");
                game.Level.Clear();
            }
            finally
            {
                if(had)PlayerPrefs.SetString(OriginalUserStore.Key,previous);else PlayerPrefs.DeleteKey(OriginalUserStore.Key);
                PlayerPrefs.Save();
            }
        }

        private static OriginalBoardState Board(OriginalLayoutSettings layout,params SavedScrew[] screws)
        {
            return new OriginalBoardSnapshot { ScrewInfos=new List<SavedScrew>(screws) }.Restore(layout);
        }
        internal static SavedScrew S(int type,params int[] colors)
        {
            var screw=new SavedScrew { NutMaxCount=colors.Length };
            for(int i=0;i<colors.Length;i++)screw.NutInfos.Add(new SavedNut { NutPos=new SavedNutPosition { y=i },
                NutData=colors[i]<0?null:new SavedNutData {NutType=1,NutColor=colors[i],V=true} });
            if(type!=1)screw.ScrewMaskDatas.Add(new SavedMask {ScrewType=type,Obj=new SavedMaskObject {NutColor=2} });
            return screw;
        }
        private static void Expect(List<string> trace,string expected)
        {
            string actual=string.Join(",",trace);Check(actual==expected,"Expected "+expected+"; actual "+actual);trace.Clear();
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidDataException(message);}
    }
}
