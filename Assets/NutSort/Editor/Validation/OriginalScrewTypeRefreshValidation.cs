using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalScrewTypeRefreshValidation
    {
        private static ScrewData Rod(params OBIMData[] masks)
        {
            var cells=new CData[4];
            for(int i=0;i<4;i++)cells[i]=new CData{LP=new LPData{y=i},BIM=new BIMData{Id=1,CI=11}};
            return new ScrewData{Id=1,C=cells,OBIM=masks};
        }
        private static OBIMData Mask()=>new OBIMData{Id=4,Obj=new OBIMObjData{CI=11}};
        private static OBIMData Hidden()=>new OBIMData{Id=7};
        public static void Validate(OriginalLevelRepository repository)
        {
            var layout=Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
            var board=new OriginalBoardState(new LevelData{B=new[]{
                Rod(new OBIMData{Id=6}),Rod(Mask(),Mask(),Hidden()),Rod(Hidden()),Rod(Hidden()),Rod(Hidden()),Rod(new OBIMData{Id=1},Hidden())}},layout);
            board.Screws[0].Coordinate=new Vector2Int(0,0);board.Screws[1].Coordinate=new Vector2Int(0,1);
            board.Screws[2].Coordinate=new Vector2Int(1,0);board.Screws[3].Coordinate=new Vector2Int(1,1);
            board.Screws[4].Coordinate=new Vector2Int(0,2);board.Screws[5].Coordinate=new Vector2Int(0,1);
            var calls=new List<string>();
            var flow=new OriginalScrewTypeRefresh(i=>calls.Add("m"+i),i=>calls.Add("d"+i),i=>calls.Add("h"+i),()=>calls.Add("save"));
            flow.Run(board,board.Screws[0]);
            Check(string.Join(",",calls)=="d0,m1,save,m1,save,h1,save,h2,save","Ordered per-entry animation then save, independent duplicate masks");
            Check(!board.Screws[1].IsColorMask&&!board.Screws[2].IsHidden,"Actual input gates unlock");
            Check(board.Screws[3].IsHidden&&board.Screws[4].IsHidden&&board.Screws[5].Masks[1].IsShow,"Diagonal/distant/secondary-only entries stay untouched");
            calls.Clear();flow.Run(board,board.Screws[0]);
            Check(string.Join(",",calls)=="d0","First-type admission excludes already revealed rods on next invocation");
            board.Screws[1].Masks[0].Object.IsShow=true;board.Screws[1].Masks[0].Object.Color=3;
            calls.Clear();flow.Run(board,board.Screws[0]);
            Check(board.Screws[1].IsColorMask&&string.Join(",",calls)=="d0,m1,save,h1,save","Mismatch does not block later entries, already hidden entries still process within admitted rod");
            ValidateView(repository);
            Debug.Log("NUT_SCREW_TYPE_REFRESH_VALIDATION_PASS ordered mask/unmovable/adjacent-hidden transitions, first-type admission, multiple entries, per-entry saves, original mask scale and independent hide timings; skeletal clip conversion remains pending.");
        }
        private static void ValidateView(OriginalLevelRepository repository)
        {
            var holder=new GameObject("type transition validation");
            try
            {
                var pool=holder.AddComponent<OriginalPrefabPool>();
                var settings=Resources.Load<OriginalScrewSettings>("Configuration/OriginalScrew");
                var screw=pool.Rent(settings.PrefabPath,holder.transform).GetComponent<OriginalScrewView>();
                var state=new ScrewState(repository.LoadBoard(false,"4b56d_1_1-1").B[0],0);
                screw.Bind(state,pool,Resources.Load<OriginalWorldSettings>("Configuration/OriginalWorld"),settings,true);
                var view=screw.TypeView;var clips=new List<string>();
                view.AnimationRequested+=(go,clip,loop)=>clips.Add(clip+":"+(loop?1:0));
                view.Configure(new ScrewMaskState{Type=ScrewType.Mask,Object=new MaskObjectState{Color=11}},state,settings,true);
                view.PlayMaskBreak();view.AdvanceTransitions(0,2);
                Check(view.MaskDoneScale==Vector3.zero&&clips.Count==0&&view.IsMaskVisible,"Paused scale cannot trigger hide early");
                view.AdvanceTransitions(.25f,0);
                Check(view.MaskDoneScale.x>1&&view.IsMaskVisible,"Source OutBack overshoot");
                view.AdvanceTransitions(.25f,0);
                Check(view.MaskDoneScale==Vector3.one&&view.IsMaskBreakVisible&&clips[0]=="animation:0","Half-second mask completion starts nonloop clip");
                view.AdvanceTransitions(0,1.19f);Check(view.IsMaskVisible,"Mask remains through independent hide delay");
                view.AdvanceTransitions(0,.02f);Check(!view.IsMaskVisible,"Mask hides at source 1.2-second delay while paused");
                view.Configure(new ScrewMaskState{Type=ScrewType.DontMove},state,settings,true);
                view.PlayDontMoveBreak();Check(clips[clips.Count-1]=="animation2:0","Fixed rod requests native break clip");
                view.AdvanceTransitions(0,1.33f);Check(view.IsDontMoveVisible,"Fixed cover native delay");
                view.AdvanceTransitions(0,.02f);Check(!view.IsDontMoveVisible,"Fixed cover hides independently at 1.34 seconds");
                screw.Release();pool.Return(screw.gameObject);
            }
            finally{UnityEngine.Object.DestroyImmediate(holder);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
