using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalMoveCompletionValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var layout=Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            var session=Resources.Load<OriginalSceneSession>("Configuration/OriginalSceneSession");
            Check(session.ScrewDoneEventDelay==1&&session.SuccessDoneEventDelay==2,"Source delays configured");
            var board=new OriginalBoardState(repository.LoadBoard(false,"4b56d_1_1-1"),layout);
            var op=new OriginalScrewOperator();op.Operate(board.Screws[0],null);var batch=op.Operate(board.Screws[1],board.Screws[0]).Move;
            var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){Level=1,LevelSeed=3};var trace=new List<string>();Action pending=null;
            bool success=true,die=false;int reads=0;
            var flow=new OriginalMoveCompletionFlow(user,()=>{reads++;return success;},()=>{trace.Add("die");return die;},
                (seconds,callback)=>{trace.Add("delay"+seconds);pending=callback;},(s,b)=>{Check(s==batch.Destination,"Captured target");trace.Add(b?"doneTrue":"doneFalse");},
                ()=>trace.Add("hide"),()=>trace.Add("push"),s=>trace.Add("refresh"),()=>trace.Add("fail"),1,2);
            flow.Run(batch.Destination);
            Check(string.Join(",",trace)=="hide,push,delay2"&&user.ScrewDoneCount==1&&user.LuckyScrewDoneCount==1&&user.LuckyDrawScrewDoneTimes==1,"Counters before captured success, guide order and delay");
            success=false;die=true;trace.Clear();pending();Check(string.Join(",",trace)=="doneTrue"&&reads==1,"Delayed captured success skips fresh success and deadlock reads");
            trace.Clear();flow.Run(batch.Destination);Check(string.Join(",",trace)=="delay1,refresh","Non-success schedules before type refresh");
            trace.Clear();success=true;pending();Check(string.Join(",",trace)=="doneFalse,die,fail","Done precedes deadlock and failure using captured false");
            trace.Clear();int before=user.ScrewDoneCount;flow.Run(batch.Source);Check(string.Join(",",trace)=="die,fail"&&user.ScrewDoneCount==before,"Non-done target checks deadlock immediately without completion counters");
            user.ScrewDoneCount=int.MaxValue;user.LuckyScrewDoneCount=int.MaxValue;user.LuckyDrawScrewDoneTimes=int.MaxValue;
            flow.Run(batch.Destination);Check(user.ScrewDoneCount==int.MinValue&&user.LuckyScrewDoneCount==int.MinValue&&user.LuckyDrawScrewDoneTimes==int.MinValue,"Native unchecked increments");
            Debug.Log("NUT_MOVE_COMPLETION_VALIDATION_PASS three counter writes, captured success and target, guide-before-delay, one/two-second configuration, type refresh, delayed event-before-deadlock, immediate incomplete deadlock and overflow.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
