using System;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalPunchPathValidation
    {
        public static void Validate()
        {
            var path=new OriginalPunchPath(new Vector3(0,0,10),2f,6,1f);
            Check(path.Count==12,"Source CoinTip has twelve generated segments");
            float total=0f;
            for(int i=0;i<12;i++)
            {
                total+=path.DurationAt(i);
                Near(path.DurationAt(i),2f*(i+1)/78f,"Normalized duration weights");
                float expected=i==11?0f:10f*(1f-i/12f)*((i&1)==0?1f:-1f);
                Near(path.OffsetAt(i).z,expected,"Alternating diminishing source offsets");
                Check(path.OffsetAt(i).x==0f && path.OffsetAt(i).y==0f,"Original axis preserved");
            }
            Near(total,2f,"Durations sum to configured period");
            var elastic=new OriginalPunchPath(new Vector3(3,4,0),1f,4,.5f);
            VectorNear(elastic.OffsetAt(0),new Vector3(3,4,0),"Full first displacement");
            VectorNear(elastic.OffsetAt(1),new Vector3(-1.125f,-1.5f,0),"Elasticity affects negative swing");
            VectorNear(elastic.OffsetAt(2),new Vector3(1.5f,2,0),"Positive swing uses unscaled remaining magnitude");
            VectorNear(elastic.OffsetAt(3),Vector3.zero,"Final offset returns to origin");
            var clamped=new OriginalPunchPath(Vector3.up,1f,4,-5f);
            VectorNear(clamped.OffsetAt(1),Vector3.zero,"Negative elasticity clamps to zero");
            clamped=new OriginalPunchPath(Vector3.up,1f,4,5f);
            Near(clamped.OffsetAt(1).y,-.75f,"High elasticity clamps to one");
            var minimum=new OriginalPunchPath(Vector3.right,.1f,1,1f);
            Check(minimum.Count==2,"At least two points");Near(minimum.DurationAt(0),.1f/3,"Minimum first duration");
            VectorNear(minimum.OffsetAt(1),Vector3.zero,"Minimum path ends at zero");
            var zero=new OriginalPunchPath(Vector3.zero,2f,6,1f);
            for(int i=0;i<zero.Count;i++)VectorNear(zero.OffsetAt(i),Vector3.zero,"Zero direction remains finite");
            Debug.Log("NUT_PUNCH_PATH_VALIDATION_PASS source twelve-point rotation trajectory, weighted durations, alternating decay, elasticity bounds, minimum segment count and zero direction.");
        }
        private static void VectorNear(Vector3 value,Vector3 expected,string message) {Check((value-expected).sqrMagnitude<.0000001f,message);}
        private static void Near(float value,float expected,string message) {Check(Mathf.Abs(value-expected)<.00001f,message);}
        private static void Check(bool value,string message) {if(!value)throw new InvalidOperationException(message);}
    }
}
