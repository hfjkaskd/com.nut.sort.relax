using System;
using System.Collections.Generic;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalCurrencyRewardFlightValidation
    {
        public static void Validate()
        {
            var root=new GameObject("Currency flight validation");
            try
            {
                var pool=root.AddComponent<OriginalPrefabPool>();
                var top=new GameObject("Top").transform;top.SetParent(root.transform);
                var destination=new GameObject("Destination").transform;destination.SetParent(root.transform);destination.position=new Vector3(10,20,7);
                var flight=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/CurrencyRewardFlight"),root.transform).GetComponent<OriginalCurrencyRewardFlight>();
                var timers=new List<Action>();var times=new List<float>();int sounds=0,randoms=0,targets=0,callbacks=0;
                flight.Bind(pool,top,type=>{targets++;return destination;},()=>"US",(seconds,done)=>{times.Add(seconds);timers.Add(done);},
                    (name,delay)=>{Check(name=="Reward_fly"&&delay==.2f,"Source sound and passed delay");sounds++;},null,()=>{randoms++;return new Vector2(.3f,.4f);});
                flight.FlyWorld(0,new Vector3(5,6,7),2,2,true,.2f,()=>callbacks++);
                Check(flight.ActiveCount==2&&flight.TrailCount==2&&top.childCount==2&&randoms==2&&targets==2,"One normalized random sample and target capture per pooled image");
                var first=top.GetChild(0);var second=top.GetChild(1);Vector3 origin=new Vector3(5.96f,7.28f,7);
                Check(Vector3.Distance(first.position,origin)<.0001f&&first.localScale==Vector3.one*2,"Gold radius .8 times scale, unchanged world Z");
                Check(first.GetChild(0).localScale==Vector3.one*100&&first.GetComponentsInChildren<ParticleSystem>().Length==1,"Original pooled star child and scale");
                foreach(var t in first.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"Official prefab scripts resolve");
                Check(times[0]==.5f&&sounds==1&&callbacks==0,"Independent callback and audio dispatch");
                flight.Advance(.5f);timers[0]();Check(callbacks==1&&first.position==origin,"Callback precedes delayed flight");
                destination.position=new Vector3(999,999,999);
                flight.Advance(.25f);
                Check(Vector3.Distance(first.position,Vector3.Lerp(origin,new Vector3(10,20,7),.75f))<.001f,"Half-second duration and OutQuad destination snapshot");
                Check(Vector3.Distance(second.position,Vector3.Lerp(origin,new Vector3(10,20,7),.51f))<.001f,"Second icon starts .1 seconds later");
                flight.Advance(.26f);Check(flight.ActiveCount==1&&flight.TrailCount==2&&!first.gameObject.activeSelf,"First item returns before star lifetime");
                flight.Advance(.1f);Check(flight.ActiveCount==0&&flight.TrailCount==1,"Second item returns while final star tail remains");
                flight.Advance(.1f);Check(flight.TrailCount==0&&top.childCount==0,"All independent star leases return");
                destination.position=Vector3.zero;
                flight.FlyWorld(0,Vector3.zero,1,1,false,0,null);
                Check(top.GetChild(0)==second||top.GetChild(0)==first,"Cash prefab reused from pool");flight.Advance(2);
                int before=randoms;
                flight.FlyWorld(1,Vector3.zero,.7f,1,false,0,null);
                var coin=top.GetChild(0);Check(Mathf.Abs(coin.position.magnitude-.42f)<.0001f&&coin.GetComponent<Image>().sprite!=null,"Coin uses .6 radius with .7 reward scale");
                flight.Advance(2);Check(flight.ActiveCount==0&&flight.TrailCount==0,"Coin and trail recovered");
                flight.FlyWorld(0,Vector3.zero,1,0,false,0,()=>callbacks++);
                Check(randoms==before+1&&times[times.Count-1]==.5f,"Zero count still schedules callback without resource/target access");
                flight.Fly(99,null,()=>callbacks++);Check(flight.ActiveCount==0,"Unknown item type returns without dereferencing source");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_CURRENCY_REWARD_FLIGHT_VALIDATION_PASS pooled Gold/Coin/StarEffect, normalized world-space rings, independent callback/audio, staggered OutQuad travel, captured targets, separate star lifetime and reuse; production HUD composition pending.");
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    }
}
