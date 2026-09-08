using System;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalPunchValidation
    {
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>("Prefabs/Effects/WithdrawalProgressPunch");Check(prefab!=null,"Configured punch prefab loads");
            var obj=UnityEngine.Object.Instantiate(prefab);var track=obj.GetComponent<OriginalPunchRotation>();
            try
            {
                var path=new OriginalPunchPath(new Vector3(0,0,10),1,10,1);
                float total=0;for(int i=0;i<path.Count;i++)total+=path.DurationAt(i);
                float[] peaks={10,-9,8,-7,6,-5,4,-3,2,0};
                Check(path.Count==10&&Mathf.Abs(total-1)<.000001f,"Ten segments and original one-second total");
                for(int i=0;i<peaks.Length;i++)
                {
                    Check(path.OffsetAt(i)==new Vector3(0,0,peaks[i]),"Native alternating decaying offsets");
                    Check(Mathf.Abs(path.DurationAt(i)-(i+1)/55f)<.000001f,"Normalized increasing segment durations");
                }
                track.Target.localEulerAngles=new Vector3(0,0,17);track.Advance(0);
                track.Target.localEulerAngles=new Vector3(0,0,37);
                float first=path.DurationAt(0);track.Advance(first*.5f);
                Check(Angle(track,44.5f),"First half segment reaches 75% via forced OutQuad, capturing rotation on first update");
                track.Advance(first*.5f+path.DurationAt(1)*.5f);
                Check(Angle(track,32.75f),"Second half segment interpolates +10 to -9 with OutQuad");
                UnityEngine.Object.DestroyImmediate(obj);obj=UnityEngine.Object.Instantiate(prefab);track=obj.GetComponent<OriginalPunchRotation>();
                track.Target.localEulerAngles=new Vector3(0,0,37);
                for(int i=0;i<peaks.Length;i++){track.Advance(path.DurationAt(i));Check(Angle(track,37+peaks[i]),"Native segment endpoint "+i);}
                track.Advance(total);Check(Angle(track,37),"Complete yoyo returns to startup rotation");
                UnityEngine.Object.DestroyImmediate(obj);obj=UnityEngine.Object.Instantiate(prefab);track=obj.GetComponent<OriginalPunchRotation>();
                float t=path.DurationAt(0)*.25f;track.Advance(t);Quaternion forward=track.Target.localRotation;
                obj.SetActive(false);track.enabled=false;
                track.Advance(total*2-t*2);
                Check(Quaternion.Angle(track.Target.localRotation,forward)<.005f,"Reverse half retraces segment curve while hidden/disabled; no local activity gate");
                track.Advance(t+total*6);
                Check(Angle(track,0),"Multiple skipped yoyo periods do not accumulate angular drift");
                Debug.Log("NUT_WITHDRAWAL_PUNCH_VALIDATION_PASS original ten decay points, increasing durations, forced segment OutQuad, lazy Euler capture, endpoints and infinite yoyo phase; actual TXPanel ProgressTip binding and Play capture pending.");
            }
            finally{if(obj!=null)UnityEngine.Object.DestroyImmediate(obj);}
        }
        private static bool Angle(OriginalPunchRotation track,float z)=>Quaternion.Angle(track.Target.localRotation,Quaternion.Euler(0,0,z))<.02f;
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
