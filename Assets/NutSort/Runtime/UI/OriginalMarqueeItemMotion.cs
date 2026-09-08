using System;
using System.Collections.Generic;
using UnityEngine;

namespace NutSort.UI
{
    public sealed partial class OriginalMarqueeItemView
    {
        private struct Travel { public float Elapsed,Duration,Start,End; public bool Started; }
        private readonly List<Travel> travel=new List<Travel>(2);
        private Action<float> travelStep;

        public void StartTravel(float endX,float duration)
        {
            travel.Add(new Travel { End=endX,Duration=duration });
            if(travelStep==null)travelStep=AdvanceTravel;
            OriginalUIAnimationDriver.Register(this,travelStep);
        }

        public void AdvanceTravel(float delta)
        {
            int count=travel.Count,alive=0;
            for(int i=0;i<count;i++)
            {
                Travel track=travel[i];
                if(!track.Started) { track.Started=true;track.Start=transform.localPosition.x; }
                track.Elapsed+=delta;
                Vector3 position=transform.localPosition;
                position.x=Mathf.LerpUnclamped(track.Start,track.End,Mathf.Min(1,track.Elapsed/track.Duration));
                transform.localPosition=position;
                if(track.Elapsed>=track.Duration)IsReady=true;
                else travel[alive++]=track;
            }
            if(alive<count)travel.RemoveRange(alive,count-alive);
            if(travel.Count==0)OriginalUIAnimationDriver.Unregister(this);
        }

        private void OnDestroy() => OriginalUIAnimationDriver.Unregister(this);
    }
}
