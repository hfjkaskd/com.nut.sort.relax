using System;
using System.Collections.Generic;
using UnityEngine;
namespace NutSort.Gameplay
{
    // LevelInfo.RefreshGuide 0x9F94F4 and manager IsCanMove 0x9FB4E0.
    public sealed class OriginalBoardGuide
    {
        private readonly List<NutSlot> sourceTop=new List<NutSlot>(),targetTop=new List<NutSlot>();
        public bool CanMove(OriginalBoardState board,ScrewState source)
        {
            foreach(var target in board.Screws)
            {
                if(ReferenceEquals(source,target)||target.IsColorMask||target.Capacity<1)continue;
                if(target.IsNull)return true;
                source.GetTopSame(sourceTop);target.GetTopSame(targetTop);
                if(sourceTop.Count==0||targetTop.Count==0){Debug.LogError("top.NutMaxCount <= 0 || top1.NutMaxCount <= 0");continue;}
                int count=0;foreach(var slot in target.Slots)if(slot.Nut!=null)count++;
                if(sourceTop[0].Nut.Color==targetTop[0].Nut.Color&&sourceTop.Count<=unchecked(target.Capacity-count))return true;
            }
            return false;
        }
        public void Refresh(OriginalBoardState board,ScrewState selected,Func<bool> newLevelMode,Func<int> level,Func<int> seed,
            Func<bool> success,Action<int> clear,Action<int,int> show)
        {
            // Early returns deliberately do not clear existing markers.
            if(newLevelMode()||level()>1||seed()>2||success())return;
            ScrewState ready=null,movable=null;
            foreach(var screw in board.Screws)
            {
                clear(screw.Index);
                if(screw.IsDone||screw.IsNull)continue;
                bool can=CanMove(board,screw),isReady=screw.IsReadyMove;
                if(ready==null&&isReady){ready=screw;if(can)movable=screw;}
                if(movable==null&&can)movable=screw;
            }
            if(movable==null){Debug.LogError("canMoveScrewInfo == null");return;}
            if(ready==null){show(movable.Index,0);return;}
            var selectedTop=Top(selected);
            foreach(var screw in board.Screws)
            {
                if(ReferenceEquals(screw,selected))continue;
                var top=Top(screw);
                bool correct=top==null||(!screw.IsFull&&selectedTop.Nut.Color==top.Nut.Color);
                show(screw.Index,correct?1:2);
            }
        }
        private static NutSlot Top(ScrewState screw)
        {for(int i=screw.Slots.Length-1;i>=0;i--)if(screw.Slots[i].Nut!=null)return screw.Slots[i];return null;}
    }
}
