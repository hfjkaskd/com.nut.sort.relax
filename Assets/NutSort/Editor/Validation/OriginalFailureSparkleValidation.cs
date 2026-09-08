using System;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalFailureSparkleValidation
    {
        public static void Validate()
        {
            var objects=new GameObject[5];
            try
            {
                for(int i=0;i<5;i++)
                {
                    var prefab=Resources.Load<GameObject>("prefabs/panels/FailureSparkle"+(i+1));
                    Check(prefab!=null,"Original sparkle variant exists");
                    objects[i]=UnityEngine.Object.Instantiate(prefab);
                    var fade=objects[i].GetComponent<OriginalLoopColor>();
                    Check(fade!=null && fade.Target.sprite!=null && objects[i].GetComponentsInChildren<Transform>().Length==1,
                        "Original single image topology with resolved sprite");
                    fade.Target.color=new Color(.2f,.3f,.4f,.8f);
                    fade.Advance(0f);Near(fade.Target.color.a,.8f,"Zero delta does not advance");
                    fade.Advance(.25f);Near(fade.Target.color.a,Mathf.Lerp(.8f,.627451f,.25f),"Lazy initial alpha and linear quarter fade");
                    fade.Advance(.75f);Near(fade.Target.color.a,.627451f,"First cycle end");
                    fade.Advance(.5f);Near(fade.Target.color.a,Mathf.Lerp(.8f,.627451f,.5f),"Reverse midpoint");
                    fade.Advance(.5f);Near(fade.Target.color.a,.8f,"Yoyo returns to captured alpha");
                    fade.Advance(4.5f);Near(fade.Target.color.a,Mathf.Lerp(.8f,.627451f,.5f),"Multiple loops retain phase");
                    Near(fade.Target.color.r,.6f,"Red interpolates");Near(fade.Target.color.g,.65f,"Green interpolates");Near(fade.Target.color.b,.7f,"Blue interpolates");
                    // Edit-mode fixture does not invoke runtime Awake.
                    OriginalUIAnimationDriver.Register(fade,fade.Advance);
                    objects[i].SetActive(false);fade.enabled=false;
                }
                OriginalUIAnimationDriver.Advance(.5f);
                foreach(var obj in objects)Near(obj.GetComponent<OriginalLoopColor>().Target.color.a,.627451f,"Global pump continues disabled/inactive targets");
            }
            finally { foreach(var obj in objects)if(obj!=null)UnityEngine.Object.DestroyImmediate(obj); }
            OriginalUIAnimationDriver.Advance(.1f);
            Debug.Log("NUT_FAILURE_SPARKLE_VALIDATION_PASS five source sprite prefab variants, lazy-color linear infinite yoyo, multi-cycle phase, RGBA interpolation, hidden/disabled pump and destroyed target cleanup.");
        }
        private static void Near(float value,float expected,string message) {Check(Mathf.Abs(value-expected)<.00001f,message);}
        private static void Check(bool value,string message) {if(!value)throw new InvalidOperationException(message);}
    }
}
