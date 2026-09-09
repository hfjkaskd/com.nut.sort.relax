using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalBoardGuideValidation
    {
        private static ScrewData Rod(params int[] colors)
        {var cells=new CData[4];for(int i=0;i<4;i++)cells[i]=new CData{LP=new LPData{y=i},BIM=i<colors.Length?new BIMData{Id=1,CI=colors[i]}:null};return new ScrewData{Id=1,C=cells};}
        public static void Validate()
        {
            var layout=Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            var board=new OriginalBoardState(new LevelData{B=new[]{Rod(1,2),Rod(),Rod(3),Rod(2)}},layout);
            var rules=new OriginalBoardGuide();var trace=new List<string>();Action<int> clear=i=>trace.Add("clear"+i);Action<int,int> show=(i,t)=>trace.Add("show"+i+":"+t);
            rules.Refresh(board,null,()=>false,()=>1,()=>0,()=>false,clear,show);
            Equal(trace,"clear0,clear1,clear2,clear3,show0:0","First movable rod hand");
            board.Screws[2].Slots[0].IsReady=true;trace.Clear();rules.Refresh(board,board.Screws[2],()=>false,()=>1,()=>0,()=>false,clear,show);
            Equal(trace,"clear0,clear1,clear2,clear3,show0:2,show1:1,show3:2","Ready rod overrides earlier movable; targets compare selected top");
            board.Screws[2].Slots[0].IsReady=false;board.Screws[0].Slots[1].IsReady=true;trace.Clear();rules.Refresh(board,board.Screws[0],()=>false,()=>1,()=>2,()=>false,clear,show);
            Equal(trace,"clear0,clear1,clear2,clear3,show1:1,show2:2,show3:1","Empty and matching color correct");
            trace.Clear();rules.Refresh(null,null,()=>true,()=>throw new Exception(),()=>throw new Exception(),()=>throw new Exception(),clear,show);Check(trace.Count==0,"New mode returns before clearing/querying level");
            rules.Refresh(null,null,()=>false,()=>2,()=>throw new Exception(),()=>throw new Exception(),clear,show);
            rules.Refresh(null,null,()=>false,()=>1,()=>3,()=>throw new Exception(),clear,show);
            rules.Refresh(null,null,()=>false,()=>1,()=>2,()=>true,clear,show);Check(trace.Count==0,"Level/seed/success return before clearing");
            var tight=new OriginalBoardState(new LevelData{B=new[]{Rod(1,2,2),Rod(2,2,2),Rod(3,3,3,3)}},layout);
            Check(!rules.CanMove(tight,tight.Screws[0]),"Guide requires capacity for entire top group");
            tight.Screws[1].Slots[2].Nut=null;tight.Screws[1].IsLocked=true;Check(rules.CanMove(tight,tight.Screws[0]),"Source guide predicate ignores lock state");
            var hand=Resources.Load<GameObject>("Game/GuideHand");Check(hand!=null&&hand.GetComponent<Animation>()!=null,"Native hand prefab");var clip=hand.GetComponent<Animation>().clip;Check(Mathf.Abs(clip.length-2)<.0001f,"One second out and one second back");
            Check(Resources.Load<GameObject>("Game/GuideCorrect").GetComponent<SpriteRenderer>().sprite!=null&&Resources.Load<GameObject>("Game/GuideError").GetComponent<SpriteRenderer>().sprite!=null,"Original correct/error sprite references");
            Debug.Log("NUT_BOARD_GUIDE_VALIDATION_PASS source clear/order, first ready preference, selected-top targets, empty/matching/mismatched destinations, entire-group capacity, lock-independent guide predicate, lazy level/mode/seed/success gates, native marker resources.");
        }
        private static void Equal(List<string> trace,string expected,string message)=>Check(string.Join(",",trace)==expected,message+": "+string.Join(",",trace));
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
