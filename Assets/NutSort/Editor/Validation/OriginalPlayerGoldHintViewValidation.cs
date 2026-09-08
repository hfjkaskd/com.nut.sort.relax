using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalPlayerGoldHintViewValidation
    {
        public static void Validate()
        {
            GameObject instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PlayerGoldGetHint"));
            try
            {
                var view=instance.GetComponent<OriginalPlayerGoldHintView>();
                Check(view!=null && instance.GetComponentsInChildren<Transform>(true).Length==6,"Original six-object hierarchy");
                foreach(var node in instance.GetComponentsInChildren<Transform>(true))
                    Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject)==0,"No missing scripts");
                var serialized=new SerializedObject(view);
                var head=(Image)serialized.FindProperty("head").objectReferenceValue;
                var name=(TMP_Text)serialized.FindProperty("playerName").objectReferenceValue;
                var info=(TMP_Text)serialized.FindProperty("info").objectReferenceValue;
                var effect=(ParticleSystem)serialized.FindProperty("effect").objectReferenceValue;
                Check(head.sprite!=null && name.font!=null && info.font!=null && effect!=null,"Original image, fonts and particle references");
                Check(!effect.main.playOnAwake && effect.GetComponent<ParticleSystemRenderer>().sortingOrder==1100,"Particle startup and sorting");
                Check(!ShaderUtil.ShaderHasError(effect.GetComponent<ParticleSystemRenderer>().sharedMaterial.shader),"Particle shader compiles");
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.Level=3;
                long now=100;bool blocked=false;var trace=new List<string>();
                var item=new OriginalMarqueeItem { IsGold=true,Minimum=5,Maximum=9 };
                var schedule=new OriginalPlayerGoldHintSchedule(()=>JObject.Parse("{\"LSSUPT\":[20,30]}"),()=>blocked,()=>item,view.PresentPush,()=>now,(a,b)=>20);
                var text=new OriginalPlayerGoldHintText(user,tables,n=>{trace.Add("amount");return "$7";},()=>{trace.Add("name");return "Player_AI29";},(a,b)=>7);
                var icons=new OriginalPlayerGoldHintIcons(tables.PayChannels,()=>"US",path=>{trace.Add("icon");return Resources.Load<Sprite>(path);},(a,b)=>2);
                view.Bind(schedule,text,icons,()=>"en",()=>{trace.Add("selected");return tables.ChannelInfos.GetChannelInfo("PayPal");});
                view.Init();Check(view.LastShowTime==-1 && instance.transform.localPosition==new Vector3(0,500,0),"Init position and sentinel");
                view.Push();blocked=true;view.Show();Check(view.LastShowTime==103 && trace.Count==0,"Panel guard before visual changes");
                blocked=false;view.Show();Check(view.LastShowTime==120 && string.Join(",",trace)=="icon,name,amount","Schedule before push presentation");
                Check(name.text==tables.Text.GetText(95,"en","Player_AI29") && info.text==tables.Text.GetText(96,"en","$7"),"Push labels wired");
                OriginalUIAnimationDriver.Advance(.25f);Near(instance.transform.localPosition.y,250,"Linear entrance halfway");
                OriginalUIAnimationDriver.Advance(.25f);Near(instance.transform.localPosition.y,0,"Entrance completion");Check(effect.isPlaying,"Particles begin on completion");
                instance.SetActive(false);OriginalUIAnimationDriver.Advance(3);Near(instance.transform.localPosition.y,0,"Hidden owner hold advances");
                OriginalUIAnimationDriver.Advance(.25f);Near(instance.transform.localPosition.y,375,"Default OutQuad exit");
                OriginalUIAnimationDriver.Advance(.25f);Near(instance.transform.localPosition.y,500,"Exit completes without changing schedule");
                Check(view.LastShowTime==120,"Animation does not reschedule");instance.SetActive(true);effect.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
                int calls=0;string oldName=name.text,oldInfo=info.text;trace.Clear();
                instance.transform.localPosition=new Vector3(17,500,23);view.Show(()=>calls++);
                Check(name.text==oldName && info.text==oldInfo && string.Join(",",trace)=="icon","Callback overload only refreshes icon");
                OriginalUIAnimationDriver.Advance(.25f);Check(instance.transform.localPosition.y<0 && instance.transform.localPosition.x==17 && instance.transform.localPosition.z==23,"OutBack overshoot and Y-only entry");
                OriginalUIAnimationDriver.Advance(.25f);Check(!effect.isPlaying && calls==0,"Callback entry has no particles or early callback");
                OriginalUIAnimationDriver.Advance(.5f);Near(instance.transform.localPosition.y,0,"Callback half-second hold");
                instance.transform.localPosition=new Vector3(41,0,43);OriginalUIAnimationDriver.Advance(.25f);
                Check(instance.transform.localPosition==new Vector3(41,250,43) && calls==0,"Linear Y-only return retains current X/Z");
                OriginalUIAnimationDriver.Advance(.25f);Check(calls==1,"Callback only after return completion");
                trace.Clear();view.ShowSelf(7);Check(string.Join(",",trace)=="selected,icon,amount" && name.text==tables.Text.GetText(94,"en"),"Self saved channel and labels");
                OriginalUIAnimationDriver.Advance(.5f);Check(instance.transform.localPosition==Vector3.zero && effect.isPlaying,"Self moves full vector and starts particles");
                OriginalUIAnimationDriver.Advance(3.5f);Near(instance.transform.localPosition.y,500,"Self return");
                calls=0;view.Show(()=>calls++);view.Show(()=>calls++);
                OriginalUIAnimationDriver.Advance(.5f);OriginalUIAnimationDriver.Advance(1);Check(calls==2,"Overlapping entries retain both callbacks");
                Debug.Log("NUT_PLAYER_GOLD_HINT_VIEW_VALIDATION_PASS prefab dependencies, schedule/text/icon integration, push/self particles, linear/OutQuad/OutBack timings, Y-only callback and overlapping tracks.");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
        private static void Near(float value,float expected,string message) { Check(Mathf.Abs(value-expected)<.01f,message); }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
