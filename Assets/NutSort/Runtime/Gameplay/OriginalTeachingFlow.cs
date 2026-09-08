using System;
using NutSort.Content;

namespace NutSort.Gameplay
{
    public interface IOriginalTeachingView
    {
        void SetCanOperateScrew(bool enabled);
        void SetTeachingVisible(bool visible);
        void SetHandVisible(bool visible);
        void SetHollowMaskVisible(bool visible);
        int MarkerCount { get; }
        void SetMarker(int index, bool filled);
        void SetTip(int textId, int positionIndex);
        void Close();
    }

    // NewbieGuidePanel.SetTeach 0x9E2F70. Markers represent LevelSeed, not
    // GuideIndex or LevelId. The source reads current state during the loop.
    public sealed class OriginalTeachingFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly IOriginalTeachingView view;
        private readonly Func<bool> skipTeaching;
        public OriginalTeachingFlow(OriginalUserLocalData user,IOriginalTeachingView view,Func<bool> skipTeaching)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.view=view ?? throw new ArgumentNullException(nameof(view));
            this.skipTeaching=skipTeaching ?? throw new ArgumentNullException(nameof(skipTeaching));
        }
        public bool Run()
        {
            if(user.Level>=2)
            {
                view.SetCanOperateScrew(false);
                view.SetTeachingVisible(false);
                return false;
            }
            view.SetCanOperateScrew(true);
            view.SetTeachingVisible(true);
            view.SetHandVisible(false);
            view.SetHollowMaskVisible(false);
            int index=0,seed;
            while(true)
            {
                int count=view.MarkerCount;
                seed=user.LevelSeed;
                if(index>=count)break;
                view.SetMarker(index,index<=seed);
                index++;
            }
            view.SetTip(seed>2 ? 151 : unchecked(user.LevelSeed+70),0);
            if(skipTeaching())view.Close();
            return true;
        }
    }
}
