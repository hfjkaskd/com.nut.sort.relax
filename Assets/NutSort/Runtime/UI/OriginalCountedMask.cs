using System;

namespace NutSort.UI
{
    // UIMgr.SetMask (0x9F8694) and delayed release (0x9F89D8).
    public sealed class OriginalCountedMask
    {
        private readonly Action<bool> setActive;
        private readonly Action<float,Action> schedule;
        private int count;
        public int Count => count;
        public OriginalCountedMask(Action<bool> setActive,Action<float,Action> schedule)
        {
            this.setActive=setActive ?? throw new ArgumentNullException(nameof(setActive));
            this.schedule=schedule ?? throw new ArgumentNullException(nameof(schedule));
        }
        public void SetMask(bool active,float hideTime=-1f,string tip="")
        {
            // Source never reads tip; it does not create/update a message here.
            if(active)
            {
                count=unchecked(count+1);
                setActive(true);
                if(hideTime>0f)schedule(hideTime,Release);
            }
            else Release();
        }
        private void Release()
        {
            count=unchecked(count-1);
            if(count>0)return;
            count=0;
            setActive(false);
        }
    }
}
