using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalMarqueeLauncherValidation
    {
        public static void Validate()
        {
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PMDBullet"));
            var defaults=ScriptableObject.CreateInstance<OriginalUserDefaults>();
            try
            {
                var launcher=instance.GetComponent<OriginalMarqueeLauncher>();
                Check(launcher!=null && instance.GetComponentsInChildren<Transform>(true).Length==4,"Original launcher hierarchy");
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var user=new OriginalUserLocalData(defaults);int level=3,reads=0,queries=0,binds=0,delays=0;
                bool missing=true,secondMissing=false;
                var data=new OriginalMarqueeItem { IsGold=true,Minimum=5,Maximum=9 };
                var trace=new List<Vector2>();
                var text=new OriginalMarqueeText(user,tables,()=>"Player_AI29",n=>"$7",(a,b)=>7);
                launcher.Bind(()=>{reads++;return level;},()=>{queries++;return missing || (secondMissing && queries%2==0)?null:data;},view=>
                {
                    binds++;view.Bind(text,tables.PayChannels,"en",()=>"US",(delay,callback)=>{Check(delay==.01f,"Item delay boundary");delays++;},path=>null,(a,b)=>2);
                },(a,b)=>{trace.Add(new Vector2(a,b));return a;});
                launcher.Init(2f);Check(launcher.PoolCount==0 && reads==0,"Aspect gate has no level/pool side effects");
                launcher.Init(2.1f);Check(launcher.PoolCount==1 && launcher.Template.IsReady && reads==1,"Inclusive aspect and template ready");
                launcher.AdvanceLaunch(100);Check(queries==0,"Below level four does not accumulate");
                level=4;launcher.AdvanceLaunch(4);Check(queries==0,"Initial five-second delay");
                launcher.AdvanceLaunch(1);Check(queries==1 && trace.Count==1 && trace[0]==new Vector2(6,8),"Missing data still consumes next interval");
                missing=false;trace.Clear();launcher.AdvanceLaunch(6);
                float halfHeight=launcher.Rect.sizeDelta.y*.5f,halfItem=launcher.Template.Rect.sizeDelta.y*.5f;
                Check(queries==3 && delays==1 && launcher.PoolCount==1,"Double selection and template reuse");
                Check(trace.Count==4 && trace[0]==new Vector2(9,11) && trace[1]==new Vector2(-halfHeight,halfHeight) && trace[2]==new Vector2(halfItem,halfHeight) && trace[3]==new Vector2(12,15),"Second attempt upper lane and all float draws");
                float end=-launcher.Rect.sizeDelta.x*.5f;
                Check(launcher.Template.transform.localPosition==new Vector3(-end,halfItem,0) && !launcher.Template.IsReady,"Entry coordinates");
                // The live level changes; text still uses the captured level-three branch.
                level=100;trace.Clear();launcher.AdvanceLaunch(9);
                Check(launcher.PoolCount==2 && binds==2 && queries==5 && delays==2,"Grow only while the first item is busy");
                Check(trace[0]==new Vector2(6,8) && trace[2]==new Vector2(-halfHeight,-halfItem),"Next attempt lower lane");
                instance.SetActive(false);OriginalUIAnimationDriver.Advance(6);
                Check(Mathf.Abs(launcher.Template.transform.localPosition.x)<.001f && !launcher.Template.IsReady,"Hidden item continues halfway linearly");
                OriginalUIAnimationDriver.Advance(0);Check(!launcher.Template.IsReady,"Paused animation remains pending");
                OriginalUIAnimationDriver.Advance(6);
                Check(launcher.Template.IsReady && launcher.Template.transform.localPosition.x==end && launcher.Template.gameObject.activeSelf,"Complete marks ready without hiding");
                instance.SetActive(true);launcher.Init(2f);trace.Clear();
                secondMissing=true;queries=0;launcher.AdvanceLaunch(6);
                Check(queries==2 && delays==2 && launcher.PoolCount==2 && !launcher.Template.IsReady,"Second missing selection launches old text; low-aspect reinit does not reset existing state");
                OriginalUIAnimationDriver.Advance(12);
                Debug.Log("NUT_MARQUEE_LAUNCHER_VALIDATION_PASS aspect/level gates, alternating delays/lanes, double selection, unused random sample, busy growth/ready reuse and hidden global travel.");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance);UnityEngine.Object.DestroyImmediate(defaults); }
        }
        private static void Check(bool condition,string message) { if(!condition)throw new InvalidOperationException(message); }
    }
}
