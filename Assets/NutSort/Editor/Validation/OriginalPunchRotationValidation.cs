using System;
using NutSort.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalPunchRotationValidation
    {
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/FailureCoinTip"));
            try
            {
                var motion=instance.GetComponent<OriginalPunchRotation>();
                Check(motion!=null && motion.Target==instance.transform,"Source CoinTip targets its own transform");
                Check(instance.GetComponentsInChildren<Transform>(true).Length==4,"Original four-object subtree");
                foreach(var image in instance.GetComponentsInChildren<Image>(true))Check(image.sprite!=null,"Sprite dependencies resolve");
                foreach(var text in instance.GetComponentsInChildren<TextMeshProUGUI>(true))Check(text.font!=null && text.fontSharedMaterial!=null,"Original font/material resolve");
                var path=new OriginalPunchPath(new Vector3(0,0,10),2,6,1);
                float first=path.DurationAt(0),second=path.DurationAt(1),period=0;
                for(int i=0;i<path.Count;i++)period+=path.DurationAt(i);
                motion.Target.localRotation=Quaternion.Euler(0,0,23);
                motion.Advance(0);Angle(motion.Target,23,"Zero delta retains pose before startup");
                motion.Advance(first*.5f);Angle(motion.Target,30.5f,"Lazy Euler start plus per-segment OutQuad midpoint");
                motion.Advance(first*.5f);Angle(motion.Target,33,"First peak");
                motion.Advance(second*.5f);Angle(motion.Target,18.625f,"Next segment restarts OutQuad easing");
                motion.Advance(period-first-second*.5f);Angle(motion.Target,23,"Closed punch returns to captured angle");
                motion.Advance(period*20f+first*.5f);Angle(motion.Target,30.5f,"Incremental cycles have zero displacement");
                OriginalUIAnimationDriver.Register(motion,motion.Advance);
                instance.SetActive(false);motion.enabled=false;
                OriginalUIAnimationDriver.Advance(first*.5f);Angle(motion.Target,33,"Global driver advances inactive target");
            }
            finally {UnityEngine.Object.DestroyImmediate(instance);}
            OriginalUIAnimationDriver.Advance(.1f);
            Debug.Log("NUT_PUNCH_ROTATION_VALIDATION_PASS original CoinTip prefab dependencies, local Euler startup, per-segment OutQuad, closed incremental loops and global hidden target playback.");
        }
        private static void Angle(Transform target,float z,string message) {Check(Quaternion.Angle(target.localRotation,Quaternion.Euler(0,0,z))<.02f,message);}
        private static void Check(bool value,string message) {if(!value)throw new InvalidOperationException(message);}
    }
}
