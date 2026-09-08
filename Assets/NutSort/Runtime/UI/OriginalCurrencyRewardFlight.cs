using System;
using System.Collections.Generic;
using NutSort.World;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    // World-position branches 9BDF08/9BEB10 and ItemMgr.FlyItem dispatch 9F649C.
    public sealed class OriginalCurrencyRewardFlight : MonoBehaviour
    {
        [SerializeField] private string goldPath,coinPath,starPath,sound;
        [SerializeField] private float goldRadius,coinRadius,goldScale,coinScale,delay,stagger,duration,starTail,starScale;
        [SerializeField] private int rewardCount;
        private sealed class Track
        {
            public GameObject Item,Star;
            public Transform Transform;
            public Vector3 Start,End;
            public float Elapsed,Delay;
            public bool Started,Returned;
        }
        private readonly List<Track> tracks=new List<Track>();
        private OriginalPrefabPool pool;
        private Transform top;
        private Func<int,Transform> target;
        private Func<string> country;
        private Func<Vector2> randomCircle;
        private Action<float,Action> schedule;
        private Action<string,float> audio;
        private OriginalToolRewardFlight tools;
        public int ActiveCount { get {int count=0;foreach(var track in tracks)if(!track.Returned)count++;return count;} }
        public int TrailCount=>tracks.Count;
        public void Bind(OriginalPrefabPool pool,Transform top,Func<int,Transform> target,Func<string> country,
            Action<float,Action> schedule,Action<string,float> audio,OriginalToolRewardFlight tools,Func<Vector2> randomCircle=null)
        {
            this.pool=pool;this.top=top;this.target=target;this.country=country;this.schedule=schedule;this.audio=audio;this.tools=tools;
            this.randomCircle=randomCircle ?? (()=>UnityEngine.Random.insideUnitCircle);
        }
        public void Fly(int type,Image source)=>Fly(type,source,null);
        public void Fly(int type,Image source,Action callback)
        {
            if(type>=2&&type<=4){tools.Fly(type,source,callback);return;}
            if(type!=0&&type!=1)return;
            FlyWorld(type,source.transform.position,type==0?goldScale:coinScale,rewardCount,true,0,callback);
        }
        public void FlyWorld(int type,Vector3 origin,float scale,int count,bool playAudio,float audioDelay,Action callback)
        {
            for(int i=0;i<count;i++)
            {
                Vector2 direction=randomCircle().normalized;
                var item=pool.Rent(type==0?goldPath:coinPath,top);
                item.transform.localRotation=Quaternion.identity;
                item.transform.localScale=Vector3.one*scale;
                if(type==0)item.GetComponent<OriginalGoldImage>().Bind(country);
                float radius=type==0?goldRadius:coinRadius;
                item.transform.position=new Vector3(origin.x+direction.x*radius*scale,origin.y+direction.y*radius*scale,origin.z);
                var star=pool.Rent(starPath,item.transform);
                star.transform.localPosition=Vector3.zero;star.transform.localRotation=Quaternion.identity;star.transform.localScale=Vector3.one*starScale;
                tracks.Add(new Track{Item=item,Star=star,Transform=item.transform,End=target(type).position,Delay=delay+i*stagger});
            }
            schedule(delay,callback);
            if(playAudio)audio(sound,audioDelay);
        }
        private void Awake(){OriginalUIAnimationDriver.Register(this,Advance);}
        private void OnDestroy()
        {
            OriginalUIAnimationDriver.Unregister(this);
            if(pool!=null)foreach(var track in tracks)
            {pool.Return(track.Star);if(!track.Returned)pool.Return(track.Item);}
            tracks.Clear();
        }
        public void Advance(float delta)
        {
            if(delta<=0)return;
            for(int i=0;i<tracks.Count;)
            {
                var track=tracks[i];track.Elapsed+=delta;
                if(!track.Returned&&track.Elapsed>track.Delay)
                {
                    if(!track.Started){track.Started=true;track.Start=track.Transform.position;}
                    float t=Mathf.Clamp01((track.Elapsed-track.Delay)/duration);
                    track.Transform.position=Vector3.LerpUnclamped(track.Start,track.End,1-(1-t)*(1-t));
                    if(t>=1){pool.Return(track.Item);track.Returned=true;}
                }
                if(track.Elapsed>=track.Delay+starTail){pool.Return(track.Star);tracks.RemoveAt(i);}else i++;
            }
        }
    }
}
