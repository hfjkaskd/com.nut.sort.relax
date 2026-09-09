using System;
namespace NutSort.Gameplay
{
    // LuoSiSortMgr.RefreshScrewTypeObj 0x9FECAC, called immediately on a
    // completed rod when the board is not yet successful (0xA0A9A4).
    public sealed class OriginalScrewTypeRefresh
    {
        private readonly Action<int> maskBreak, dontMoveBreak, hiddenBreak;
        private readonly Action save;
        public OriginalScrewTypeRefresh(Action<int> maskBreak,Action<int> dontMoveBreak,Action<int> hiddenBreak,Action save)
        {
            this.maskBreak=maskBreak??throw new ArgumentNullException(nameof(maskBreak));
            this.dontMoveBreak=dontMoveBreak??throw new ArgumentNullException(nameof(dontMoveBreak));
            this.hiddenBreak=hiddenBreak??throw new ArgumentNullException(nameof(hiddenBreak));
            this.save=save??throw new ArgumentNullException(nameof(save));
        }
        public void Run(OriginalBoardState board,ScrewState completed)
        {
            foreach(var screw in board.Screws)
            {
                // Only the FIRST type determines admission, but once admitted
                // every type entry is visited, even one already marked hidden.
                if(!screw.IsColorMask&&!screw.IsHidden&&!screw.IsDontMove)continue;
                foreach(var mask in screw.Masks)
                {
                    if(mask.Type==ScrewType.Mask)
                    {
                        int color=mask.Object.Color;
                        NutSlot top=null;
                        for(int i=completed.Slots.Length-1;i>=0;i--)
                            if(completed.Slots[i].Nut!=null){top=completed.Slots[i];break;}
                        if(top!=null&&color==top.Nut.Color)
                        {
                            mask.Object.IsShow=false;maskBreak(screw.Index);save();continue;
                        }
                    }
                    if(ReferenceEquals(screw,completed)&&mask.Type==ScrewType.DontMove)
                    {dontMoveBreak(screw.Index);continue;}
                    if(mask.Type!=ScrewType.Hidden)continue;
                    int row=unchecked(completed.Coordinate.x-screw.Coordinate.x);
                    int column=unchecked(completed.Coordinate.y-screw.Coordinate.y);
                    // Native inlined integer abs does not throw on int.MinValue.
                    int ar=row<0?unchecked(-row):row,ac=column<0?unchecked(-column):column;
                    if(!((ac<=1&&row==0)||(ar==1&&column==0)))continue;
                    mask.IsShow=false;hiddenBreak(screw.Index);save();
                }
            }
        }
    }
}
